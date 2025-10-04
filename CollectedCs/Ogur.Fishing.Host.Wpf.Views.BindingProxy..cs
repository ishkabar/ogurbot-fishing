using System.Windows;

namespace Ogur.Fishing.Host.Wpf.Views;


/// <summary>
/// Binding proxy to enable binding inside style setters.
/// </summary>
public sealed class BindingProxy : Freezable
{
    /// <summary>
    /// Gets or sets the proxied data object.
    /// </summary>
    public object? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <summary>
    /// Dependency property for <see cref="Data"/>.
    /// </summary>
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

    /// <summary>
    /// Creates the binding proxy instance.
    /// </summary>
    /// <returns>New instance.</returns>
    protected override Freezable CreateInstanceCore() => new BindingProxy();
}