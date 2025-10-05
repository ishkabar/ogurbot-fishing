using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ogur.Infrastructure.Windows
{
    /// <summary>
    /// Brings a given window handle (HWND) to the foreground using Win32 APIs.
    /// </summary>
    public sealed class WindowActivator : Ogur.Abstractions.Windows.IWindowActivator
    {
        private readonly ILogger<WindowActivator> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowActivator"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public WindowActivator(ILogger<WindowActivator> logger)
        {
            _logger = logger;
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        /// <summary>
        /// Activates the specified window handle (HWND).
        /// </summary>
        /// <param name="hWnd">Window handle.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if activation succeeded.</returns>
        public Task<bool> ActivateAsync(nint hWnd, CancellationToken ct)
        {
            if (hWnd == 0)
            {
                _logger.LogWarning("Invalid HWND (0) passed to ActivateAsync.");
                return Task.FromResult(false);
            }

            try
            {
                ShowWindow((IntPtr)hWnd, SW_RESTORE);
                var ok = SetForegroundWindow((IntPtr)hWnd);
                if (!ok)
                {
                    _logger.LogWarning("SetForegroundWindow failed for {Hwnd}", hWnd);
                }
                return Task.FromResult(ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to activate window {Hwnd}", hWnd);
                return Task.FromResult(false);
            }
        }
    }
}
