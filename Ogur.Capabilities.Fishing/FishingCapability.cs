using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ogur.abstractions;
using ogur.Infrastructure.Input;
using ogur.Infrastructure.Screen;
using ogur.Infrastructure.Ocr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stateless;


namespace Ogur.Capabilities.Fishing;


/// <summary>
/// Fishing capability implementing finite state machine for fishing flow.
/// </summary>
public sealed class FishingCapability : IBotCapability
{
    private readonly ILogger<FishingCapability> _logger;
    private readonly IInput _input;
    private readonly IScreenCapture _screen;
    private readonly IOcr _ocr;
    private readonly FishingOptions _options;

    private readonly Channel<BotEvent> _eventChannel = Channel.CreateUnbounded<BotEvent>();
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
    /// <param name="logger">Logger.</param>
    /// <param name="input">Input abstraction for simulated key/mouse.</param>
    /// <param name="screen">Screen capture provider.</param>
    /// <param name="ocr">OCR provider.</param>
    /// <param name="options">Options.</param>
    public FishingCapability(
        ILogger<FishingCapability> logger,
        IInput input,
        IScreenCapture screen,
        IOcr ocr,
        IOptions<FishingOptions> options)
    {
        _logger = logger;
        _input = input;
        _screen = screen;
        _ocr = ocr;
        _options = options.Value;

        _fsm = new StateMachine<State, Trigger>(State.Idle);

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
            .Permit(Trigger.Fault, State.Error);

        _fsm.Configure(State.Hooking)
            .OnEntryAsync(OnHookingEnterAsync)
            .Permit(Trigger.Tick, State.Looting)
            .Permit(Trigger.Fault, State.Error);

        _fsm.Configure(State.Looting)
            .OnEntryAsync(OnLootingEnterAsync)
            .Permit(Trigger.LootDone, State.Casting)
            .Permit(Trigger.Fault, State.Error);

        _fsm.Configure(State.Error)
            .OnEntryAsync(async _ => await EmitAsync(new BotEvent("fishing.error", "Unhandled error")))
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
    /// <returns>Async stream of bot events.</returns>
    public async IAsyncEnumerable<BotEvent> Events(CancellationToken ct)
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
        if (Status is CapabilityStatus.Running) return;
        Status = CapabilityStatus.Running;

        await EmitAsync(new BotEvent("fishing.start", "Starting fishing"));
        _loopCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _ = Task.Run(() => LoopAsync(_loopCts.Token));
        await _fsm.FireAsync(Trigger.Start);
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
    /// Stops the capability.
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
        await EmitAsync(new BotEvent("fishing.stop", "Stopped fishing"));
    }

    private async Task LoopAsync(CancellationToken ct)
    {
        try
        {
            var interval = TimeSpan.FromMilliseconds(_options.PollIntervalMs);
            while (!ct.IsCancellationRequested && Status == CapabilityStatus.Running)
            {
                await _fsm.FireAsync(Trigger.Tick);
                await Task.Delay(interval, ct);
            }
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogError(ex, "Fishing loop faulted");
            await EmitAsync(new BotEvent("fishing.exception", ex.Message));
            await _fsm.FireAsync(Trigger.Fault);
        }
    }

    private async Task OnCastingEnterAsync()
    {
        await EmitAsync(new BotEvent("fishing.state", "Casting"));
        await _input.KeyTapAsync(VirtualKey.Space);
    }

    private async Task OnWaitingEnterAsync()
    {
        await EmitAsync(new BotEvent("fishing.state", "Waiting for bite"));
        var timeout = TimeSpan.FromSeconds(_options.BiteTimeoutSeconds);
        var start = DateTimeOffset.UtcNow;

        while (DateTimeOffset.UtcNow - start < timeout)
        {
            var bite = await DetectBiteAsync();
            if (bite)
            {
                await _fsm.FireAsync(Trigger.BiteDetected);
                return;
            }
            await Task.Delay(_options.PollIntervalMs);
        }

        await _fsm.FireAsync(Trigger.Timeout);
    }

    private async Task OnHookingEnterAsync()
    {
        await EmitAsync(new BotEvent("fishing.state", "Hooking"));
        await _input.MouseRightClickAsync();
    }

    private async Task OnLootingEnterAsync()
    {
        await EmitAsync(new BotEvent("fishing.state", "Looting"));
        await _input.KeyTapAsync(VirtualKey.F);
        await _fsm.FireAsync(Trigger.LootDone);
    }

    private async Task<bool> DetectBiteAsync()
    {
        var bmp = await _screen.CaptureAsync();
        var text = await _ocr.ReadTextAsync(bmp);
        return text.Contains("Bite", StringComparison.OrdinalIgnoreCase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ValueTask EmitAsync(BotEvent e)
    {
        _eventChannel.Writer.TryWrite(e);
        return ValueTask.CompletedTask;
    }
}