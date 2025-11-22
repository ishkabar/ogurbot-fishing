// File: Ogur.Fishing.Host.Wpf/Services/FishingActionExecutor.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Services

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions;
using Ogur.Abstractions.Input;
using Ogur.Abstractions.Windows;
using Ogur.Capabilities.Fishing;

namespace Ogur.Fishing.Host.Wpf.Services;

/// <summary>
/// Consumes fishing events and executes physical actions (window activation, key presses).
/// </summary>
public sealed class FishingActionExecutor : BackgroundService
{
    private readonly ILogger<FishingActionExecutor> _logger;
    private readonly FishingCapability _fishing;
    private readonly IInput _input;
    private readonly IWindowActivator _activator;
    private readonly ISessionState _session;

    /// <summary>
    /// Initializes a new instance of the <see cref="FishingActionExecutor"/> class.
    /// </summary>
    public FishingActionExecutor(
        ILogger<FishingActionExecutor> logger,
        FishingCapability fishing,
        IInput input,
        IWindowActivator activator,
        ISessionState session)
    {
        _logger = logger;
        _fishing = fishing;
        _input = input;
        _activator = activator;
        _session = session;
    }

    /// <summary>
    /// Executes the background service, consuming fishing events.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🔥 FishingActionExecutor STARTED - waiting for events...");
        
        try
        {
            await foreach (var e in _fishing.Events(stoppingToken))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("🔥 FishingActionExecutor stopping (cancellation requested)");
                    break;
                }

                _logger.LogInformation("🔥 FishingActionExecutor received event: {Type} - {Message}", e.Type, e.Message);

                _logger.LogWarning("🔥 DEBUG: About to switch on event type: '{Type}'", e.Type);
                
                try
                {
                    switch (e.Type)
                    {
                        case "fishing.cast.request":
                            _logger.LogInformation("🔥 Handling CAST request");
                            await HandleCastAsync(stoppingToken);
                            break;

                        case "fishing.hook.request":
                            _logger.LogInformation("🔥 Handling HOOK request");
                            await HandleHookAsync(e, stoppingToken);
                            break;
                            
                        default:
                            _logger.LogInformation("🔥 Ignoring event type: {Type}", e.Type);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "🔥 Failed to handle event {Type}", e.Type);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("🔥 FishingActionExecutor CANCELLED");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 FishingActionExecutor CRASHED");
        }
        finally
        {
            _logger.LogInformation("🔥 FishingActionExecutor STOPPED");
        }
    }

    private async Task<bool> ActivateWindowAsync(CancellationToken ct)
    {
        var hwnd = _session.SelectedProcess?.Hwnd ?? 0;
        
        _logger.LogInformation("🔥 ActivateWindowAsync: HWND=0x{Hwnd:X}", hwnd);
        
        if (hwnd == 0)
        {
            _logger.LogWarning("🔥 No window selected (HWND=0), cannot activate");
            return false;
        }
        
        bool result = await _activator.ActivateAsync(hwnd, ct);
        _logger.LogInformation("🔥 Window activation result: {Result}", result);
        
        return result;
    }

    private async Task HandleCastAsync(CancellationToken ct)
    {
        _logger.LogInformation("🔥 HandleCastAsync START");
        await Task.Delay(150, ct);
        
        _logger.LogInformation("🔥 Activating window...");
        if (!await ActivateWindowAsync(ct))
        {
            _logger.LogWarning("🔥 HandleCastAsync ABORTED - window activation failed");
            return;
        }
        _logger.LogInformation("🔥 Window activated");
        
        _logger.LogInformation("🔥 Waiting 100ms after window activation");
        await Task.Delay(150, ct);

        var bait = _session.SelectedBait;
        _logger.LogInformation("🔥 SelectedBait: {Bait}", bait?.DisplayName ?? "NULL");
    
        if (bait is not null)
        {
            var baitKey = InputKeyMapper.ToInputKey(bait.Key);
            _logger.LogInformation("🔥 Sending bait key: {Key}", baitKey);
        
            await _input.SendKeyAsync(baitKey, ct);
        
            _logger.LogInformation("🔥 Waiting 200ms after bait");
            await Task.Delay(200, ct);
        }

        _logger.LogInformation("🔥 Sending cast key: Space");
        await _input.SendKeyAsync(InputKey.Space, ct);
    
        _logger.LogInformation("🔥 HandleCastAsync DONE");
    }

    
    
    private async Task HandleHookAsync(ApplicationEvent e, CancellationToken ct)  // ✅ ZMIEŃ NA ApplicationEvent
    {
        _logger.LogInformation("🔥 HandleHookAsync START");
    
        int spaceCount = 1;
    
        // ✅ FIX: Najprostsza wersja TryParse
        if (!string.IsNullOrEmpty(e.Message) && int.TryParse(e.Message, out int parsed) && parsed >= 1 && parsed <= 3)
        {
            spaceCount = parsed;
        }
        else
        {
            _logger.LogWarning("🔥 Failed to parse space count from message: '{Message}', using default: 1", e.Message);
        }
    
        _logger.LogInformation("🔥 Space count: {Count}", spaceCount);

        var random = new Random();

        int firstDelay = random.Next(20, 61);
        _logger.LogDebug("🔥 Initial delay before hook: {Delay}ms", firstDelay);
        await Task.Delay(firstDelay, ct);
    
        for (int i = 0; i < spaceCount; i++)
        {
            _logger.LogInformation("🔥 Sending hook key: Space ({Current}/{Total})", i + 1, spaceCount);
            await _input.SendKeyAsync(InputKey.Space, ct);
        
            if (i < spaceCount - 1)
            {
                int delay = random.Next(20, 61);
                _logger.LogDebug("🔥 Delay between space: {Delay}ms", delay);
                await Task.Delay(delay, ct);
            }
        }

        _logger.LogInformation("🔥 HandleHookAsync DONE");
    }
}