using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Windows;
using Ogur.Core.Legacy.HACK;

namespace Ogur.Infrastructure.Windows
{
    /// <summary>
    /// Activates a window using legacy User32 wrappers present in Ogur.Core.Legacy.HACK.User.
    /// Adds minimal retry and foreground verification.
    /// </summary>
    public sealed class LegacyUserWindowActivator : IWindowActivator
    {
        private readonly ILogger<LegacyUserWindowActivator> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LegacyUserWindowActivator"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public LegacyUserWindowActivator(ILogger<LegacyUserWindowActivator> logger)
        {
            _logger = logger;
        }

        private const int SW_RESTORE = 9;

        /// <summary>
        /// Brings the given HWND to foreground using ShowWindow + SetForegroundWindow,
        /// verifying foreground with short retry.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if the window became foreground.</returns>
        public async Task<bool> ActivateAsync(nint hwnd, CancellationToken ct)
        {
            if (hwnd == 0)
            {
                _logger.LogWarning("ActivateAsync: invalid HWND=0");
                return false;
            }

            try
            {
                var h = (int)hwnd;

                User.ShowWindow(h, SW_RESTORE);
                var ok = User.SetForegroundWindow(h) != 0;
                _logger.LogDebug("SetForegroundWindow({Hwnd}) -> {Ok}", hwnd, ok);

                var attempts = 0;
                while (!ct.IsCancellationRequested && attempts < 5 && User.GetForegroundWindow() != h)
                {
                    attempts++;
                    ok = User.SetForegroundWindow(h) != 0;
                    await Task.Delay(40, ct);
                }

                var success = User.GetForegroundWindow() == h;
                _logger.LogInformation("ActivateAsync(Hwnd={Hwnd}) -> {Ok} (attempts={Attempts})", hwnd, success, attempts);
                return success;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActivateAsync failed for {Hwnd}", hwnd);
                return false;
            }
        }
    }
}