// File: Ogur.Fishing.Host.Wpf/Converters/InverseBoolConverter.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Converters

using System;
using System.Globalization;
using System.Windows.Data;

namespace Ogur.Fishing.Host.Wpf.Converters;

/// <summary>
/// Inverts boolean value.
/// </summary>
public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }
}