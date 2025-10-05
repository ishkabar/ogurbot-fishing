// File: Ogur.Fishing.Host.Wpf/Services/InputKeyMapper.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Services

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Ogur.Abstractions.Input;

namespace Ogur.Fishing.Host.Wpf.Services
{
    /// <summary>
    /// Maps WPF <see cref="Key"/> values to <see cref="InputKey"/> values used by input abstraction.
    /// Supports only keys relevant for the fishing capability.
    /// </summary>
    public static class InputKeyMapper
    {
        private static readonly Dictionary<Key, InputKey> Map = new()
        {
            { Key.Space, InputKey.Space },
            { Key.F1, InputKey.F1 },
            { Key.F2, InputKey.F2 },
            { Key.F3, InputKey.F3 },
            { Key.F4, InputKey.F4 },
            { Key.D1, InputKey.D1 },
            { Key.D2, InputKey.D2 },
            { Key.D3, InputKey.D3 },
            { Key.D4, InputKey.D4 }
        };

        /// <summary>
        /// Converts a WPF <see cref="Key"/> to an <see cref="InputKey"/>.
        /// </summary>
        /// <param name="key">WPF key to convert.</param>
        /// <returns>Mapped <see cref="InputKey"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the key is not supported.</exception>
        public static InputKey ToInputKey(Key key)
        {
            if (Map.TryGetValue(key, out var mapped))
                return mapped;

            throw new ArgumentOutOfRangeException(nameof(key), key, $"Unsupported key for InputKey mapping: {key}");
        }
    }
}