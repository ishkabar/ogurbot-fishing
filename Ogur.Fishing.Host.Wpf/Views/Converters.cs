using System;
using System.Globalization;
using System.Windows.Data;


namespace Ogur.Fishing.Host.Wpf.Views;

/// <summary>
/// Returns a caption depending on listening state.
/// </summary>
public sealed class ListeningCaptionConverter : IValueConverter
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly ListeningCaptionConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && b ? "Listening..." : "Listen";

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Negates boolean value.
/// </summary>
public sealed class BooleanNegationConverter : IValueConverter
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly BooleanNegationConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : value;

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}