// File: Ogur.Infrastructure/Input/Win32Input.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Core.Metin.Legacy;
using Ogur.Abstractions.Input;


namespace Ogur.Infrastructure.Input;

/// <summary>
/// Windows input implementation using HACK.Button for Metin2.
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
    }

    /// <summary>
    /// Sends a single key press using scan codes.
    /// </summary>
    /// <param name="key">Key to send.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task.</returns>
    public Task SendKeyAsync(InputKey key, CancellationToken ct)
    {
        var scanCode = MapToScanCode(key);
        
        _logger.LogDebug("SendKeyAsync({Key}) -> ScanCode {ScanCode}", key, scanCode);
        
        // Use PressKey2 for sensitive keys (F-keys, Space)
        Button.PressKey2(scanCode);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends a left mouse click.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task.</returns>
    public Task LeftClickAsync(CancellationToken ct)
    {
        _logger.LogDebug("LeftClickAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Moves the cursor to specified coordinates.
    /// </summary>
    /// <param name="x">X position.</param>
    /// <param name="y">Y position.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task.</returns>
    public Task MoveCursorAsync(int x, int y, CancellationToken ct)
    {
        _logger.LogDebug("MoveCursorAsync({X}, {Y})", x, y);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends text input to the active window.
    /// </summary>
    /// <param name="text">Text to send.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task.</returns>
    public Task SendTextAsync(string text, CancellationToken ct)
    {
        _logger.LogDebug("SendTextAsync(\"{Text}\")", text);
        return Task.CompletedTask;
    }

    private static Button.BT7 MapToScanCode(InputKey key)
    {
        return key switch
        {
            InputKey.D1 => Button.BT7.KEY_1,
            InputKey.D2 => Button.BT7.KEY_2,
            InputKey.D3 => Button.BT7.KEY_3,
            InputKey.D4 => Button.BT7.KEY_4,
            InputKey.F1 => Button.BT7.F1,
            InputKey.F2 => Button.BT7.F2,
            InputKey.F3 => Button.BT7.F3,
            InputKey.F4 => Button.BT7.F4,
            InputKey.Space => Button.BT7.SPACE,
            _ => throw new NotSupportedException($"InputKey {key} not mapped to scan code")
        };
    }
}