using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Windows;

namespace Ogur.Infrastructure.Windows
{
    /// <summary>
    /// Attempts to reliably bring a window to the foreground using multiple Win32 techniques.
    /// </summary>
    public sealed class RobustWindowActivator : IWindowActivator
    {
        private readonly ILogger<RobustWindowActivator> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RobustWindowActivator"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public RobustWindowActivator(ILogger<RobustWindowActivator> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Activates the specified window handle (HWND) using ShowWindow/BringWindowToTop/SetForegroundWindow.
        /// Falls back to AttachThreadInput when needed. Verifies foreground and retries briefly.
        /// </summary>
        /// <param name="hwnd">Target window handle.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if the window is foreground after activation; otherwise false.</returns>
        public async Task<bool> ActivateAsync(nint hwnd, CancellationToken ct)
        {
            if (hwnd == 0)
            {
                _logger.LogWarning("ActivateAsync: invalid HWND=0");
                return false;
            }

            try
            {
                var target = (IntPtr)hwnd;

                // Step 1: basic restore + bring to top + foreground attempt
                ShowWindow(target, SW_RESTORE);
                BringWindowToTop(target);
                AllowSetForegroundWindow(ASFW_ANY);
                var ok = SetForegroundWindow(target);
                _logger.LogDebug("SetForegroundWindow({Hwnd}) -> {Ok}", hwnd, ok);

                if (!ok)
                {
                    // Step 2: AttachThreadInput fallback
                    ok = TryAttachAndFocus(target);
                    _logger.LogDebug("AttachThreadInput focus -> {Ok}", ok);
                }

                // Step 3: verify foreground with short retries
                var attempts = 0;
                while (ok && GetForegroundWindow() != target && attempts < 5 && !ct.IsCancellationRequested)
                {
                    attempts++;
                    BringWindowToTop(target);
                    SetActiveWindow(target);
                    SetForegroundWindow(target);
                    await Task.Delay(40, ct);
                }

                var success = GetForegroundWindow() == target;
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

        private static bool TryAttachAndFocus(IntPtr target)
        {
            var fg = GetForegroundWindow();
            var fgThread = GetWindowThreadProcessId(fg, out _);
            var targetThread = GetWindowThreadProcessId(target, out _);
            var curThread = GetCurrentThreadId();

            var attached1 = false;
            var attached2 = false;

            try
            {
                if (fgThread != 0 && targetThread != 0 && fgThread != targetThread)
                {
                    attached1 = AttachThreadInput(fgThread, targetThread, true);
                    attached2 = AttachThreadInput(curThread, targetThread, true);
                }

                ShowWindow(target, SW_RESTORE);
                BringWindowToTop(target);
                SetActiveWindow(target);
                SetFocus(target);
                var ok = SetForegroundWindow(target);
                return ok;
            }
            finally
            {
                if (attached1) AttachThreadInput(fgThread, targetThread, false);
                if (attached2) AttachThreadInput(curThread, targetThread, false);
            }
        }

        private const int SW_RESTORE = 9;
        private const int ASFW_ANY = -1;

        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] private static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")] private static extern bool AllowSetForegroundWindow(int dwProcessId);
    }
}
