using System;
using System.Globalization;
using System.Windows.Data;
using System;
using System.Globalization;
using System.Windows;
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
  /// <summary>
    /// Converts a double value to a uniform Thickness.
    /// </summary>
    public sealed class DoubleToThicknessConverter : IValueConverter
    {
        /// <summary>
        /// Converts a double to Thickness with uniform sides.
        /// </summary>
        /// <param name="value">Double value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Optional parameter.</param>
        /// <param name="culture">Culture.</param>
        /// <returns>Thickness with uniform value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = value is double dv ? dv : 0d;
            return new Thickness(d);
        }

        /// <summary>
        /// Not supported convert back.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        /// <returns>Throws NotSupportedException.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>
    /// Converts a string to Visibility.Collapsed when null or empty; otherwise Visible.
    /// </summary>
    public sealed class StringNullOrEmptyToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts string to Visibility.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        /// <returns>Visible when not empty; otherwise Collapsed.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            return string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Convert back not supported.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        /// <returns>Throws NotSupportedException.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }