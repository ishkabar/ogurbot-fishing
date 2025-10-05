using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Input;

namespace Ogur.Infrastructure.Input;


    /// <summary>
    /// Win32-based keyboard input using SendInput with scan codes.
    /// Supports: Space, digits '1'..'4'. Mouse methods are no-op for MVP.
    /// </summary>
    public sealed class Win32Input : IInput
    {
        private readonly ILogger<Win32Input> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Win32Input"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public Win32Input(ILogger<Win32Input> logger)
        {
            _logger = logger;
            _logger.LogInformation("Win32Input ready (scan-code mode).");
        }

        /// <summary>
        /// Sends textual input to the currently active window (' ', '1'..'4').
        /// </summary>
        /// <param name="text">Text to send.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task.</returns>
        public Task SendTextAsync(string text, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(text)) return Task.CompletedTask;

            foreach (var ch in text)
            {
                if (ct.IsCancellationRequested) break;
                if (!TryMapCharToVk(ch, out var vk))
                {
                    _logger.LogDebug("Unsupported char '{Ch}' skipped.", ch);
                    continue;
                }

                var scan = (ushort)MapVirtualKey(vk, 0);
                if (scan == 0)
                {
                    _logger.LogWarning("MapVirtualKey failed for VK=0x{VK:X}.", vk);
                    continue;
                }

                SendScan(scan, true);
                SendScan(scan, false);
            }

            _logger.LogInformation("SendTextAsync(\"{Text}\") sent.", text);
            return Task.CompletedTask;
        }

        /// <summary>
        /// No-op: mouse is not used in MVP.
        /// </summary>
        public Task LeftClickAsync(CancellationToken ct)
        {
            _logger.LogTrace("LeftClickAsync() no-op.");
            return Task.CompletedTask;
        }

        /// <summary>
        /// No-op: mouse is not used in MVP.
        /// </summary>
        public Task MoveCursorAsync(int x, int y, CancellationToken ct)
        {
            _logger.LogTrace("MoveCursorAsync({X},{Y}) no-op.", x, y);
            return Task.CompletedTask;
        }

        private static bool TryMapCharToVk(char ch, out ushort vk)
        {
            if (ch == ' ') { vk = 0x20; return true; }           // VK_SPACE
            if (ch is >= '1' and <= '4') { vk = ch; return true; } // '1'..'4'
            vk = 0; return false;
        }

        private static void SendScan(ushort scan, bool down)
        {
            var input = new INPUT
            {
                type = 1, // KEYBOARD
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = scan,
                        dwFlags = down ? 0x0008u : 0x0008u | 0x0002u, // SCANCODE | (KEYUP if release)
                        time = 0,
                        dwExtraInfo = UIntPtr.Zero
                    }
                }
            };
            _ = SendInput(1, new[] { input }, Marshal.SizeOf<INPUT>());
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT { public int type; public InputUnion U; }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion { [FieldOffset(0)] public KEYBDINPUT ki; }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")] private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        [DllImport("user32.dll")] private static extern uint MapVirtualKey(uint uCode, uint uMapType);
    }