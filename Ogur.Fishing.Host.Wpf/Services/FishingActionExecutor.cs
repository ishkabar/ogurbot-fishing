using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions;
using Ogur.Abstractions.Windows;
using Ogur.Abstractions.Input;
using Ogur.Capabilities.Fishing;
using Ogur.Fishing.Host.Wpf.Services.Models;

namespace Ogur.Fishing.Host.Wpf.Services
{
    /// <summary>
    /// Consumes fishing events and dispatches key presses to the selected game window using input abstraction.
    /// </summary>
    public sealed class FishingActionExecutor : BackgroundService
    {
        private readonly ILogger<FishingActionExecutor> _logger;
        private readonly FishingCapability _fishing;
        private readonly IInput _input;
        private readonly Ogur.Abstractions.Windows.IWindowActivator _activator;
        private readonly ISessionState _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="FishingActionExecutor"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="fishing">Fishing capability that produces events.</param>
        /// <param name="input">Input simulation service.</param>
        /// <param name="activator">Window activator service.</param>
        /// <param name="session">Current session state.</param>
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
        /// Executes the background loop that handles fishing events.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var e in _fishing.Events(stoppingToken))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                switch (e.Type)
                {
                    case "fishing.cast.request":
                        await HandleCastAsync(stoppingToken).ConfigureAwait(false);
                        break;

                    case "fishing.hook.request":
                        await HandleHookAsync(stoppingToken).ConfigureAwait(false);
                        break;

                    case "fishing.loot.request":
                        await HandleLootAsync(stoppingToken).ConfigureAwait(false);
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
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if activation succeeded; otherwise false.</returns>
        private async Task<bool> ActivateWindowAsync(CancellationToken ct)
        {
            var proc = _session.SelectedProcess;
            if (proc is null)
            {
                _logger.LogWarning("No target process is selected.");
                return false;
            }

            // Expecting Hwnd (nint) on the SelectedProcess model.
            var hwndProp = proc.GetType().GetProperty("Hwnd");
            if (hwndProp is null)
            {
                _logger.LogWarning("Selected process model has no 'Hwnd' property.");
                return false;
            }

            var hwndObj = hwndProp.GetValue(proc);
            if (hwndObj is null || hwndObj is not nint hwnd || hwnd == 0)
            {
                _logger.LogWarning("Selected process has invalid HWND.");
                return false;
            }

            var ok = await _activator.ActivateAsync(hwnd, ct).ConfigureAwait(false);
            if (!ok)
            {
                _logger.LogWarning("Failed to activate window: {Hwnd}", hwnd);
            }
            else
            {
                _logger.LogDebug("Game window activated: {Hwnd}", hwnd);
            }

            return ok;
        }

        /// <summary>
        /// Handles the cast action: optional bait selection, then cast key.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task.</returns>
        private async Task HandleCastAsync(CancellationToken ct)
        {
            if (!await ActivateWindowAsync(ct).ConfigureAwait(false))
            {
                return;
            }

            var bait = _session.SelectedBait;
            if (bait is not null)
            {
                await SendWpfKeyAsync(bait.Key, ct).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromMilliseconds(150), ct).ConfigureAwait(false);
            }

            await SendKeyAsync(InputKey.Space, ct).ConfigureAwait(false);
            _logger.LogInformation("CAST (bait={Bait})", bait?.DisplayName ?? "none");
        }

        /// <summary>
        /// Handles the hook action.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task.</returns>
        private async Task HandleHookAsync(CancellationToken ct)
        {
            if (!await ActivateWindowAsync(ct).ConfigureAwait(false))
            {
                return;
            }

            await SendKeyAsync(InputKey.Space, ct).ConfigureAwait(false);
            _logger.LogInformation("HOOK");
        }

        /// <summary>
        /// Handles the loot action.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task.</returns>
        private async Task HandleLootAsync(CancellationToken ct)
        {
            if (!await ActivateWindowAsync(ct).ConfigureAwait(false))
            {
                return;
            }

            _logger.LogInformation("LOOT (stub)");
            await Task.CompletedTask;
        }

        /// <summary>
        /// Sends a WPF key using the input abstraction after mapping it to <see cref="InputKey"/>.
        /// </summary>
        /// <param name="wpfKey">WPF key value.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task.</returns>
        private Task SendWpfKeyAsync(System.Windows.Input.Key wpfKey, CancellationToken ct)
        {
            var mapped = InputKeyMapper.ToInputKey(wpfKey);
            return SendKeyAsync(mapped, ct);
        }

        /// <summary>
        /// Sends a key using the input abstraction. Falls back to text for simple keys.
        /// </summary>
        /// <param name="key">Key to send.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task.</returns>
        private async Task SendKeyAsync(InputKey key, CancellationToken ct)
        {
            switch (key)
            {
                case InputKey.D1:
                    await _input.SendTextAsync("1", ct).ConfigureAwait(false);
                    return;
                case InputKey.D2:
                    await _input.SendTextAsync("2", ct).ConfigureAwait(false);
                    return;
                case InputKey.D3:
                    await _input.SendTextAsync("3", ct).ConfigureAwait(false);
                    return;
                case InputKey.D4:
                    await _input.SendTextAsync("4", ct).ConfigureAwait(false);
                    return;
                case InputKey.Space:
                    await _input.SendTextAsync(" ", ct).ConfigureAwait(false);
                    return;
                default:
                    _logger.LogWarning("InputKey {Key} not supported by current IInput. Extend IInput or SendKeyAsync mapping.", key);
                    return;
            }
        }
    }
}
