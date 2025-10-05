// File: Ogur.Capabilities.Fishing/FishingCapability.cs
// Project: Ogur.Capabilities.Fishing
// Namespace: Ogur.Capabilities.Fishing

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ogur.Abstractions;
using Ogur.Abstractions.Primitives;
using Stateless;

namespace Ogur.Capabilities.Fishing
{
    /// <summary>
    /// Fishing capability implementing finite state machine for the fishing flow.
    /// Sequence: bait -> space -> wait for bite signal -> hook -> loot.
    /// </summary>
    public sealed partial class FishingCapability : IBotCapability
    {
        private readonly ILogger<FishingCapability> _logger;
        private readonly IInput _input;
        private readonly IFishingSignalSource _signal;
        private readonly FishingOptions _options;

        private readonly Channel<BotEvent> _eventChannel = Channel.CreateUnbounded<BotEvent>();

        private State _state = State.Idle;
        private readonly StateMachine<State, Trigger> _fsm;
        private CancellationTokenSource? _loopCts;

        /// <summary>
        /// Capability FSM states.
        /// </summary>
        public enum State { Idle, Casting, WaitingBite, Hooking, Looting, Error }

        /// <summary>
        /// Capability FSM triggers.
        /// </summary>
        public enum Trigger { Start, Tick, BiteDetected, Timeout, LootDone, Fault }

        /// <summary>
        /// Initializes a new instance of the <see cref="FishingCapability"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="input">Input abstraction.</param>
        /// <param name="signal">Bite signal source.</param>
        /// <param name="options">Capability options.</param>
        public FishingCapability(
            ILogger<FishingCapability> logger,
            IInput input,
            IFishingSignalSource signal,
            IOptions<FishingOptions> options)
        {
            _logger = logger;
            _input = input;
            _signal = signal;
            _options = options.Value;

            _fsm = new StateMachine<State, Trigger>(() => _state, s => _state = s);

            _fsm.Configure(State.Idle)
                .Permit(Trigger.Start, State.Casting);

            _fsm.Configure(State.Casting)
                .OnEntryAsync(OnCastingEnterAsync)
                .Permit(Trigger.Tick, State.WaitingBite)
                .Permit(Trigger.Fault, State.Error);

            _fsm.Configure(State.WaitingBite)
                .OnEntryAsync(OnWaitingEnterAsync)
                .Permit(Trigger.BiteDetected, State.Hooking)
                .Permit(Trigger.Timeout, State.Casting)
                .Permit(Trigger.Fault, State.Error)
                .Ignore(Trigger.Tick);

            _fsm.Configure(State.Hooking)
                .OnEntryAsync(OnHookingEnterAsync)
                .Permit(Trigger.Tick, State.Looting)
                .Permit(Trigger.Fault, State.Error);

            _fsm.Configure(State.Looting)
                .OnEntryAsync(OnLootingEnterAsync)
                .Permit(Trigger.LootDone, State.Casting)
                .Permit(Trigger.Fault, State.Error);

            _fsm.Configure(State.Error)
                .OnEntryAsync(async _ => await EmitAsync(NewEvent("fishing.error", "Unhandled error")))
                .Permit(Trigger.Start, State.Casting)
                .Ignore(Trigger.Tick);
        }

        /// <summary>
        /// Gets the capability identifier.
        /// </summary>
        public string CapabilityId => "fishing";

        /// <summary>
        /// Gets the current status of the capability.
        /// </summary>
        public CapabilityStatus Status { get; private set; } = CapabilityStatus.Stopped;

        /// <summary>
        /// Provides an event stream emitted by the capability.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>An async stream of bot events.</returns>
        public async IAsyncEnumerable<BotEvent> Events([EnumeratorCancellation] CancellationToken ct)
        {
            while (await _eventChannel.Reader.WaitToReadAsync(ct).ConfigureAwait(false))
            {
                while (_eventChannel.Reader.TryRead(out var e))
                {
                    yield return e;
                }
            }
        }

        /// <summary>
        /// Starts the capability.
        /// </summary>
        /// <param name="ctx">Context with required services.</param>
        /// <param name="ct">Cancellation token.</param>
        public async Task StartAsync(CapabilityStartContext ctx, CancellationToken ct)
        {
            if (Status is CapabilityStatus.Running)
            {
                return;
            }

            if (Status is CapabilityStatus.Stopped)
            {
                _state = State.Idle;
            }

            Status = CapabilityStatus.Running;

            await EmitAsync(NewEvent("fishing.start", "Starting fishing")).ConfigureAwait(false);

            _loopCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _ = Task.Run(() => LoopAsync(_loopCts.Token), _loopCts.Token);

            await _fsm.FireAsync(Trigger.Start).ConfigureAwait(false);
        }

        /// <summary>
        /// Pauses the capability.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        public Task PauseAsync(CancellationToken ct)
        {
            Status = CapabilityStatus.Paused;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the capability and resets the FSM to Idle for the next start.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        public async Task StopAsync(CancellationToken ct)
        {
            if (_loopCts is not null)
            {
                _loopCts.Cancel();
                _loopCts.Dispose();
                _loopCts = null;
            }

            Status = CapabilityStatus.Stopped;
            _state = State.Idle;

            await EmitAsync(NewEvent("fishing.stop", "Stopped fishing")).ConfigureAwait(false);
        }

        /// <summary>
        /// Main tick loop that drives the FSM via Tick trigger.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        private async Task LoopAsync(CancellationToken ct)
        {
            try
            {
                var interval = TimeSpan.FromMilliseconds(_options.PollIntervalMs);
                while (!ct.IsCancellationRequested && Status == CapabilityStatus.Running)
                {
                    await _fsm.FireAsync(Trigger.Tick).ConfigureAwait(false);
                    await Task.Delay(interval, ct).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                _logger.LogError(ex, "Fishing loop faulted");
                await EmitAsync(NewEvent("fishing.exception", ex.Message)).ConfigureAwait(false);
                await _fsm.FireAsync(Trigger.Fault).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// FSM: enter Casting.
        /// </summary>
        private async Task OnCastingEnterAsync()
        {
            await EmitAsync(NewEvent("fishing.state", "Casting")).ConfigureAwait(false);
            await EmitAsync(NewEvent("fishing.cast.request", "Request: perform cast")).ConfigureAwait(false);
        }

        /// <summary>
        /// FSM: enter WaitingBite. Waits for external bite signal or times out.
        /// </summary>
        private async Task OnWaitingEnterAsync()
        {
            await EmitAsync(NewEvent("fishing.state", "Waiting for bite")).ConfigureAwait(false);

            var timeout = TimeSpan.FromSeconds(_options.BiteTimeoutSeconds);
            bool bite = false;

            try
            {
                bite = await _signal.WaitForBiteAsync(timeout, CancellationToken.None).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                bite = false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "WaitForBiteAsync failed; will continue flow via timeout.");
                bite = false;
            }

            if (bite)
            {
                await EmitAsync(NewEvent("fishing.bite", "Bite detected")).ConfigureAwait(false);
                await _fsm.FireAsync(Trigger.BiteDetected).ConfigureAwait(false);
                return;
            }

            await EmitAsync(NewEvent("fishing.timeout", "Bite wait timeout")).ConfigureAwait(false);
            await _fsm.FireAsync(Trigger.Timeout).ConfigureAwait(false);
        }

        /// <summary>
        /// FSM: enter Hooking.
        /// </summary>
        private async Task OnHookingEnterAsync()
        {
            await EmitAsync(NewEvent("fishing.state", "Hooking")).ConfigureAwait(false);
            await EmitAsync(NewEvent("fishing.hook.request", "Request: hook the fish")).ConfigureAwait(false);
            await _fsm.FireAsync(Trigger.Tick).ConfigureAwait(false);
        }

        /// <summary>
        /// FSM: enter Looting.
        /// </summary>
        private async Task OnLootingEnterAsync()
        {
            await EmitAsync(NewEvent("fishing.state", "Looting")).ConfigureAwait(false);
            await EmitAsync(NewEvent("fishing.loot.request", "Request: loot items")).ConfigureAwait(false);
            await _fsm.FireAsync(Trigger.LootDone).ConfigureAwait(false);
        }

        /// <summary>
        /// Emits a bot event into the capability event channel.
        /// </summary>
        /// <param name="e">Event to emit.</param>
        /// <returns>Awaitable value task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ValueTask EmitAsync(BotEvent e)
        {
            _eventChannel.Writer.TryWrite(e);
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Creates a new <see cref="BotEvent"/> with the current timestamp.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="payload">Event payload.</param>
        /// <returns>Constructed event.</returns>
        private static BotEvent NewEvent(string name, string payload)
            => new BotEvent(name, payload, DateTimeOffset.UtcNow);
    }
}
