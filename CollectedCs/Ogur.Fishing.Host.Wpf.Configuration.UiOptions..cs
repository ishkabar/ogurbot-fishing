using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ogur.Fishing.Host.Wpf.Configuration;


/// <summary>
/// UI section bound from appsettings.json.
/// </summary>
public sealed class UiOptions
{
    /// <summary>
    /// Gets or sets default window/view metrics.
    /// </summary>
    public UiPresetOptions Default { get; init; } = new();

    /// <summary>
    /// Gets or sets login window/view metrics.
    /// </summary>
    public UiPresetOptions Login { get; init; } = new();
}

/// <summary>
/// Single preset (window size, fonts, spacing).
/// </summary>
public sealed class UiPresetOptions
{
    /// <summary>
    /// Gets or sets window width in device-independent units.
    /// </summary>
    public double WindowWidth { get; init; } = 1200;

    /// <summary>
    /// Gets or sets window height in device-independent units.
    /// </summary>
    public double WindowHeight { get; init; } = 800;

    /// <summary>
    /// Gets or sets base font size.
    /// </summary>
    public double FontSizeBase { get; init; } = 14;

    /// <summary>
    /// Gets or sets title font size.
    /// </summary>
    public double FontSizeTitle { get; init; } = 24;

    /// <summary>
    /// Gets or sets default control height.
    /// </summary>
    public double ControlHeight { get; init; } = 38;

    /// <summary>
    /// Gets or sets default corner radius.
    /// </summary>
    public double CornerRadius { get; init; } = 8;

    /// <summary>
    /// Gets or sets default spacing.
    /// </summary>
    public double Spacing { get; init; } = 12;
}

/// <summary>
/// Bindable theme snapshot consumed by WPF resources.
/// </summary>
public sealed class UiTheme : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets window width.
    /// </summary>
    public double WindowWidth { get => _windowWidth; init { _windowWidth = value; } }
    private double _windowWidth;

    /// <summary>
    /// Gets or sets window height.
    /// </summary>
    public double WindowHeight { get => _windowHeight; init { _windowHeight = value; } }
    private double _windowHeight;

    /// <summary>
    /// Gets or sets base font size.
    /// </summary>
    public double FontSizeBase { get; init; }

    /// <summary>
    /// Gets or sets title font size.
    /// </summary>
    public double FontSizeTitle { get; init; }

    /// <summary>
    /// Gets or sets default control height.
    /// </summary>
    public double ControlHeight { get; init; }

    /// <summary>
    /// Gets or sets default corner radius.
    /// </summary>
    public double CornerRadius { get; init; }

    /// <summary>
    /// Gets or sets default spacing.
    /// </summary>
    public double Spacing { get; init; }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises a property changed notification.
    /// </summary>
    /// <param name="name">Property name.</param>
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}