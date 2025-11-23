// File: Ogur.Capabilities.Fishing/FishingCapability.cs
// Project: Ogur.Capabilities.Fishing
// Namespace: Ogur.Capabilities.Fishing

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ogur.Abstractions;
using Ogur.Abstractions.Events;
using Ogur.Abstractions.Metin;
using Ogur.Abstractions.Primitives;
using Ogur.Infrastructure.configuration;

namespace Ogur.Capabilities.Fishing;

/// <summary>
/// Core fishing capability - manages fishing loop and emits events via EventBus.
/// </summary>
public sealed class FishingCapability : IApplicationCapability
{
    private readonly ILogger<FishingCapability> _logger;
    private readonly IFishingSignalSource _signal;
    private readonly FishingOptions _options;
    private readonly IEventBus _eventBus;

    private CancellationTokenSource? _loopCts;

    /// <summary>
    /// Initializes a new instance of the <see cref="FishingCapability"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="signal">Fishing signal source.</param>
    /// <param name="options">Fishing options.</param>
    /// <param name="eventBus">Event bus for broadcasting events.</param>
    public FishingCapability(
        ILogger<FishingCapability> logger,
        IFishingSignalSource signal,
        IOptions<FishingOptions> options,
        IEventBus eventBus)
    {
        _logger = logger;
        _signal = signal;
        _options = options.Value;
        _eventBus = eventBus;
    }

    /// <summary>
    /// Gets the capability identifier.
    /// </summary>
    public string CapabilityId => "fishing";

    /// <summary>
    /// Gets the current capability status.
    /// </summary>
    public CapabilityStatus Status { get; private set; } = CapabilityStatus.Stopped;

    /// <summary>
    /// Gets the event stream for this capability.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Async enumerable of fishing events.</returns>
    public IAsyncEnumerable<ApplicationEvent> Events(CancellationToken ct)
    {
        return _eventBus.Subscribe("fishing.*", ct);
    }

    /// <summary>
    /// Starts the fishing capability.
    /// </summary>
    /// <param name="ctx">Capability start context.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task representing the async operation.</returns>
    public async Task StartAsync(CapabilityStartContext ctx, CancellationToken ct)
    {
        if (Status is CapabilityStatus.Running)
        {
            _logger.LogWarning("Already running, ignoring StartAsync");
            return;
        }

        _logger.LogInformation("FishingCapability.StartAsync() called");

        Status = CapabilityStatus.Running;
        _eventBus.Publish("fishing.start", "Fishing started");

        _loopCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        _logger.LogInformation("Starting fishing loop in background task");
        _ = Task.Run(() => FishingLoopAsync(_loopCts.Token), _loopCts.Token);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Pauses the fishing capability.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task representing the async operation.</returns>
    public Task PauseAsync(CancellationToken ct)
    {
        _logger.LogInformation("FishingCapability.PauseAsync() called");
        Status = CapabilityStatus.Paused;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the fishing capability.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task representing the async operation.</returns>
    public async Task StopAsync(CancellationToken ct)
    {
        _logger.LogInformation("FishingCapability.StopAsync() called");

        if (_loopCts is not null)
        {
            _loopCts.Cancel();
            _loopCts.Dispose();
            _loopCts = null;
        }

        Status = CapabilityStatus.Stopped;
        _eventBus.Publish("fishing.stop", "Fishing stopped");
        await Task.CompletedTask;
    }

    private async Task FishingLoopAsync(CancellationToken ct)
    {
        _logger.LogInformation("FishingLoopAsync() STARTED");

        try
        {
            while (!ct.IsCancellationRequested && Status == CapabilityStatus.Running)
            {
                // ✅ JEDEN Random na początku pętli
                var random = new Random();

                // 1. CAST (przynęta + space)
                _logger.LogInformation("[LOOP] Publishing fishing.cast.request");
                _eventBus.Publish("fishing.cast.request", "Casting rod");

                int castAnimation = random.Next(1200, 2000);
                _logger.LogInformation("[LOOP] Waiting for cast + animation: {Delay}ms", castAnimation);
                await Task.Delay(castAnimation, ct);

                // 2. WAITING - 🔍 START skanowania pamięci
                _logger.LogInformation("[LOOP] Publishing fishing.waiting");
                _eventBus.Publish("fishing.waiting", "Waiting for bite");

                var timeout = TimeSpan.FromSeconds(13);
                _logger.LogInformation("[LOOP]  START WaitForBiteAsync (memory scan START)");

                int spaceCount = await _signal.WaitForBiteAsync(timeout, ct);

                _logger.LogInformation("[LOOP]  END WaitForBiteAsync (memory scan STOP) - returned: {SpaceCount}",
                    spaceCount);

                if (spaceCount > 0)
                {
                    _logger.LogInformation("[LOOP] BITE! Space count: {Count}", spaceCount);
                    _eventBus.Publish("fishing.bite", $"Bite detected (hooks: {spaceCount})");

                    _logger.LogInformation("[LOOP] Publishing fishing.hook.request (count={Count})", spaceCount);
                    _eventBus.Publish("fishing.hook.request", $"Hooking fish (count: {spaceCount})");

                    int hookToAnimation = random.Next(500, 801);
                    _logger.LogInformation("[LOOP]  Delay hook → animation: {Delay}ms", hookToAnimation);
                    await Task.Delay(hookToAnimation, ct);

                    int pullAnimation = random.Next(1500, 2201);
                    _logger.LogInformation("[LOOP]  Pull animation: {Delay}ms", pullAnimation);
                    await Task.Delay(pullAnimation, ct);

                    int safetyCooldown = random.Next(5450, 7001);
                    _logger.LogInformation("[LOOP]  Safety cooldown: {Delay}ms", safetyCooldown);
                    await Task.Delay(safetyCooldown, ct);

                    _logger.LogInformation("[LOOP] ✅ All post-hook delays DONE");
                }
                else
                {
                    _logger.LogWarning("[LOOP] TIMEOUT - no bite");
                    _eventBus.Publish("fishing.timeout", "No bite detected");

                    int timeoutCooldown = random.Next(1000, 1500);
                    _logger.LogInformation("[LOOP]  Timeout cooldown: {Delay}ms", timeoutCooldown);
                    await Task.Delay(timeoutCooldown, ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("FishingLoopAsync() CANCELLED");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FishingLoopAsync() ERROR");
            _eventBus.Publish("fishing.error", ex.Message);
        }
        finally
        {
            _logger.LogInformation("FishingLoopAsync() ENDED");
        }
    }
}