// File: Ogur.Fishing.Host.Wpf/Services/FishingActionExecutor.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Services
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions;
using Ogur.Abstractions.Input;
using Ogur.Abstractions.Windows;
using Ogur.Capabilities.Fishing;
using Ogur.Capabilities.Fishing.Adapters;
using Ogur.Fishing.Host.Wpf.Services.Models;
using Ogur.Fishing.Host.Wpf.Adapters;

namespace Ogur.Fishing.Host.Wpf.Services
{
    /// <summary>
    /// Consumes fishing events and dispatches key presses to the selected game window.
    /// </summary>
    public sealed class FishingActionExecutor : BackgroundService
    {
        private readonly ILogger<FishingActionExecutor> _logger;
        private readonly FishingCapability _fishing;
        private readonly IInput _input;
        private readonly IWindowActivator _activator;
        private readonly ISessionState _session;
        private readonly IKeyboardSynthesizer _keys;
        private readonly FishingClickAdapter _clicks;
        private readonly IFishingRunGate _runGate;

        private readonly Stopwatch _castWatch = Stopwatch.StartNew();

        /// <summary>
        /// Initializes a new instance of the <see cref="FishingActionExecutor"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="fishing">Fishing capability that produces events.</param>
        /// <param name="input">Input simulation abstraction.</param>
        /// <param name="activator">Window activator.</param>
        /// <param name="session">Current session state.</param>
        /// <param name="keys">Keyboard synthesizer.</param>
        /// <param name="clicks">High-level click adapter.</param>
        /// <param name="runGate">Run gate (enabled/disabled by UI).</param>
        public FishingActionExecutor(
            ILogger<FishingActionExecutor> logger,
            FishingCapability fishing,
            IInput input,
            IWindowActivator activator,
            ISessionState session,
            IKeyboardSynthesizer keys,
            FishingClickAdapter clicks,
            IFishingRunGate runGate)
        {
            _logger = logger;
            _fishing = fishing;
            _input = input;
            _activator = activator;
            _session = session;
            _keys = keys;
            _clicks = clicks;
            _runGate = runGate;
        }

        /// <summary>
        /// Executes the background loop that handles fishing events.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var e in _fishing.Events(stoppingToken))
            {
                if (stoppingToken.IsCancellationRequested) break;

                // 🔒 TWARDY BEZPIECZNIK: ignoruj wszystko, dopóki gate off lub capability nie Running
                if (!_runGate.Enabled || _fishing.Status != CapabilityStatus.Running)
                {
                    // lekki oddech, jeśli jakiś emit spamuje
                    await Task.Delay(50, stoppingToken);
                    continue;
                }

                switch (e.Type)
                {
                    case "fishing.cast.request":
                        await HandleCastAsync(stoppingToken);
                        break;

                    case "fishing.hook.request":
                        await HandleHookAsync(stoppingToken);
                        break;

                    case "fishing.loot.request":
                        await HandleLootAsync(stoppingToken);
                        break;

                    default:
                        _logger.LogDebug("Unhandled event type: {Type}", e.Type);
                        break;
                }
            }
        }

        /// <summary>
        /// Brings the selected game window to foreground using HWND from session.
        /// </summary>
        private async Task<bool> ActivateWindowAsync(CancellationToken ct)
        {
            var proc = _session.SelectedProcess;
            if (proc is null)
            {
                _logger.LogWarning("No target process is selected.");
                return false;
            }

            nint hwnd = 0;
            if (proc is ProcessOption po) hwnd = po.Hwnd;
            else
            {
                var hwndProp = proc.GetType().GetProperty("Hwnd");
                if (hwndProp is not null && hwndProp.GetValue(proc) is nint v) hwnd = v;
            }

            if (hwnd == 0)
            {
                _logger.LogWarning("Selected process has invalid HWND (0).");
                return false;
            }

            var ok = await _activator.ActivateAsync(hwnd, ct);
            _logger.LogInformation("ActivateAsync(Hwnd={Hwnd}) -> {Ok}", hwnd, ok);
            return ok;
        }

        /// <summary>
        /// Handles the cast action with throttle and EButton presses.
        /// </summary>
        private async Task HandleCastAsync(CancellationToken ct)
        {
            // anty-spam
            if (_castWatch.ElapsedMilliseconds < 150) return;
            _castWatch.Restart();

            if (!_runGate.Enabled || _fishing.Status != CapabilityStatus.Running) return;

            if (!await ActivateWindowAsync(ct)) return;

            var bait = _session.SelectedBait;
            if (bait is not null)
            {
                var inputKey = WpfKeyMapper.ToInputKey(bait.Key);
                await _clicks.PressAsync(inputKey, ct);
                await Task.Delay(60, ct);
            }

            await _keys.PressKey2Async(ScanCode.Space, ct);
            _logger.LogInformation("CAST (bait={Bait})", bait?.DisplayName ?? "none");
        }

        /// <summary>
        /// Handles the hook action.
        /// </summary>
        private async Task HandleHookAsync(CancellationToken ct)
        {
            if (!_runGate.Enabled || _fishing.Status != CapabilityStatus.Running) return;

            if (!await ActivateWindowAsync(ct)) return;

            await _keys.PressKey2Async(ScanCode.Space, ct);
            _logger.LogInformation("HOOK");
        }

        /// <summary>
        /// Handles the loot action (stub for MVP).
        /// </summary>
        private async Task HandleLootAsync(CancellationToken ct)
        {
            if (!_runGate.Enabled || _fishing.Status != CapabilityStatus.Running) return;

            if (!await ActivateWindowAsync(ct)) return;

            _logger.LogInformation("LOOT (stub)");
            await Task.CompletedTask;
        }
    }
}
