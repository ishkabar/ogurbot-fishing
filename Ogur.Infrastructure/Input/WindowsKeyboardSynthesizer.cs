using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Input;

namespace Ogur.Infrastructure.Input;

/// <summary>
/// Windows implementation of <see cref="IKeyboardSynthesizer"/> using SendInput with scan codes.
/// </summary>
public sealed class WindowsKeyboardSynthesizer : IKeyboardSynthesizer
{
    private readonly ILogger<WindowsKeyboardSynthesizer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsKeyboardSynthesizer"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public WindowsKeyboardSynthesizer(ILogger<WindowsKeyboardSynthesizer> logger) => _logger = logger;

    /// <summary>
    /// Presses a key (down+up).
    /// </summary>
    /// <param name="code">Scan code.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task PressKeyAsync(ScanCode code, CancellationToken ct)
    {
        var down = CreateScan((ushort)code, false);
        var up = CreateScan((ushort)code, true);
        _ = SendInput(1, new[] { down }, Marshal.SizeOf<INPUT>());
        _ = SendInput(1, new[] { up }, Marshal.SizeOf<INPUT>());
        _logger.LogDebug("PressKey({Code})", code);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Presses a key using two discrete events.
    /// </summary>
    /// <param name="code">Scan code.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task PressKey2Async(ScanCode code, CancellationToken ct)
    {
        var down = CreateScan((ushort)code, false);
        var up = CreateScan((ushort)code, true);
        _ = SendInput(1, new[] { down }, Marshal.SizeOf<INPUT>());
        _ = SendInput(1, new[] { up }, Marshal.SizeOf<INPUT>());
        _logger.LogDebug("PressKey2({Code})", code);
        return Task.CompletedTask;
    }

    private static INPUT CreateScan(ushort scan, bool keyUp)
    {
        return new INPUT
        {
            type = 1,
            U = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0,
                    wScan = scan,
                    dwFlags = keyUp ? (0x0008u | 0x0002u) : 0x0008u,
                    time = 0,
                    dwExtraInfo = UIntPtr.Zero
                }
            }
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public int type;
        public InputUnion U;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
}
