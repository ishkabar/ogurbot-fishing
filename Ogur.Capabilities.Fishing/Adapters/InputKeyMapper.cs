// File: Ogur.Capabilities.Fishing/Adapters/InputKeyMapper.cs
// Project: Ogur.Capabilities.Fishing
// Namespace: Ogur.Capabilities.Fishing.Adapters
using System;
using Ogur.Abstractions.Input;

namespace Ogur.Capabilities.Fishing.Adapters
{
    /// <summary>
    /// Maps numeric bait slots or function keys to logical and scan-code keys.
    /// </summary>
    /// <summary>
    /// Maps numeric bait slots or function keys to logical and scan-code keys (UI-agnostic).
    /// </summary>
    public static class InputKeyMapper
    {
        /// <summary>
        /// Converts numeric hotbar or function index to <see cref="InputKey"/>.
        /// </summary>
        /// <param name="value">Key value (1..4 digits, 101..104 F-keys, 200 Space).</param>
        /// <returns>Mapped <see cref="InputKey"/>.</returns>
        public static InputKey ToInputKey(int value) => value switch
        {
            1 => InputKey.D1,
            2 => InputKey.D2,
            3 => InputKey.D3,
            4 => InputKey.D4,
            101 => InputKey.F1,
            102 => InputKey.F2,
            103 => InputKey.F3,
            104 => InputKey.F4,
            200 => InputKey.Space,
            _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unsupported bait key value: {value}")
        };

        /// <summary>
        /// Converts an <see cref="InputKey"/> into a <see cref="ScanCode"/>.
        /// </summary>
        /// <param name="key">Input key.</param>
        /// <returns>Corresponding scan code.</returns>
        public static ScanCode ToScanCode(InputKey key) => key switch
        {
            InputKey.D1 => ScanCode.D1,
            InputKey.D2 => ScanCode.D2,
            InputKey.D3 => ScanCode.D3,
            InputKey.D4 => ScanCode.D4,
            InputKey.F1 => ScanCode.F1,
            InputKey.F2 => ScanCode.F2,
            InputKey.F3 => ScanCode.F3,
            InputKey.F4 => ScanCode.F4,
            InputKey.Space => ScanCode.Space,
            _ => throw new ArgumentOutOfRangeException(nameof(key), $"Unsupported InputKey: {key}")
        };
    }
}