using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Input;


namespace Ogur.Infrastructure.Input;

/// <summary>
/// Keyboard synthesizer backed by EButton.dll; translates <see cref="ScanCode"/> to EButton BT7 keys.
/// </summary>
public sealed class EButtonKeyboardSynthesizer : IKeyboardSynthesizer
{
    private readonly ILogger<EButtonKeyboardSynthesizer> _logger;
    private readonly EButton.Button _btn;

    /// <summary>
    /// Initializes a new instance of the <see cref="EButtonKeyboardSynthesizer"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    public EButtonKeyboardSynthesizer(ILogger<EButtonKeyboardSynthesizer> logger)
    {
        _logger = logger;
        _btn = new EButton.Button();
        _logger.LogInformation("EButtonKeyboardSynthesizer initialized (EButton backend).");
    }

    /// <summary>
    /// Presses a key (down+up) using the EButton backend with a small internal hold.
    /// </summary>
    /// <param name="scanCode">Scan code to press.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task PressKeyAsync(ScanCode scanCode, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var bt7 = ScanCodeToBt7.Convert(scanCode);
        _logger.LogDebug("PressKeyAsync -> EButton PressKey({BT7}) for sc=0x{SC:X}", bt7, (short)scanCode);
        // EButton API is synchronous; wrap to keep async signature.
        _btn.PressKey(bt7);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Presses a key using the EButton backend variant expected by legacy callers.
    /// </summary>
    /// <param name="scanCode">Scan code to press.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task PressKey2Async(ScanCode scanCode, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var bt7 = ScanCodeToBt7.Convert(scanCode);
        _logger.LogDebug("PressKey2Async -> EButton PressKey({BT7}) for sc=0x{SC:X}", bt7, (short)scanCode);
        _btn.PressKey(bt7);
        return Task.CompletedTask;
    }
}