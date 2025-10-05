using System;
using Ogur.Abstractions.Input;

namespace Ogur.Infrastructure.Input;

/// <summary>
/// Converts <see cref="ScanCode"/> values to <c>EButton.Button.BT7</c> constants.
/// </summary>
internal static class ScanCodeToBt7
{
    /// <summary>
    /// Converts a scan code to the corresponding EButton BT7 key.
    /// </summary>
    /// <param name="scanCode">Scan code to convert.</param>
    /// <returns>EButton BT7 key value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when scan code is unsupported.</exception>
    public static EButton.Button.BT7 Convert(ScanCode scanCode) => scanCode switch
    {
        ScanCode.D1    => EButton.Button.BT7.KEY_1,
        ScanCode.D2    => EButton.Button.BT7.KEY_2,
        ScanCode.D3    => EButton.Button.BT7.KEY_3,
        ScanCode.D4    => EButton.Button.BT7.KEY_4,

        ScanCode.F1    => EButton.Button.BT7.F1,
        ScanCode.F2    => EButton.Button.BT7.F2,
        ScanCode.F3    => EButton.Button.BT7.F3,
        ScanCode.F4    => EButton.Button.BT7.F4,

        ScanCode.Space => EButton.Button.BT7.SPACE,

        // Add more mappings here if you use them (e.g., arrows, ESC, etc.)
        _ => throw new ArgumentOutOfRangeException(nameof(scanCode), scanCode, "Unsupported scan code for EButton mapping.")
    };
}