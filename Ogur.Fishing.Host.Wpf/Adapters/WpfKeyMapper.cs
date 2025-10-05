// File: Ogur.Fishing.Host.Wpf/Adapters/WpfKeyMapper.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Adapters
using System;
using System.Windows.Input;
using Ogur.Abstractions.Input;
using Ogur.Fishing.Host.Wpf.Adapters;

namespace Ogur.Fishing.Host.Wpf.Adapters
{
    /// <summary>
    /// Maps WPF <see cref="Key"/> to domain <see cref="InputKey"/>.
    /// </summary>
    public static class WpfKeyMapper
    {
        /// <summary>
        /// Converts a WPF <see cref="Key"/> to <see cref="InputKey"/>.
        /// </summary>
        /// <param name="key">WPF key from UI selection.</param>
        /// <returns>Mapped <see cref="InputKey"/>.</returns>
        public static InputKey ToInputKey(Key key) => key switch
        {
            Key.D1 => InputKey.D1,
            Key.D2 => InputKey.D2,
            Key.D3 => InputKey.D3,
            Key.D4 => InputKey.D4,
            Key.F1 => InputKey.F1,
            Key.F2 => InputKey.F2,
            Key.F3 => InputKey.F3,
            Key.F4 => InputKey.F4,
            Key.Space => InputKey.Space,
            _ => throw new ArgumentOutOfRangeException(nameof(key), $"Unsupported WPF key: {key}")
        };
    }
}