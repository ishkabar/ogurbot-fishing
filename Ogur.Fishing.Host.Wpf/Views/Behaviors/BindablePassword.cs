using System.Windows;
using System.Windows.Controls;

namespace Ogur.Fishing.Host.Wpf.Views.Behaviors;

/// <summary>
/// Enables two-way binding of the plain-text password to a ViewModel property.
/// WPF PasswordBox is sealed and does not expose a bindable Password DP, so we use an attached property.
/// </summary>
public static class BindablePassword
{
    /// <summary>
    /// Gets the bound password from the specified dependency object.
    /// </summary>
    /// <param name="obj">Dependency object (PasswordBox).</param>
    /// <returns>Bound password string.</returns>
    public static string GetBoundPassword(DependencyObject obj) => (string)obj.GetValue(BoundPasswordProperty);

    /// <summary>
    /// Sets the bound password for the specified dependency object.
    /// </summary>
    /// <param name="obj">Dependency object (PasswordBox).</param>
    /// <param name="value">Password string to set.</param>
    public static void SetBoundPassword(DependencyObject obj, string value) => obj.SetValue(BoundPasswordProperty, value);

    /// <summary>
    /// Attached property that holds the bound password string. Two-way by default.
    /// </summary>
    public static readonly DependencyProperty BoundPasswordProperty =
        DependencyProperty.RegisterAttached(
            "BoundPassword",
            typeof(string),
            typeof(BindablePassword),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

    /// <summary>
    /// Gets a value indicating whether the behavior is attached.
    /// </summary>
    /// <param name="obj">Dependency object (PasswordBox).</param>
    /// <returns>True if attached, otherwise false.</returns>
    public static bool GetAttach(DependencyObject obj) => (bool)obj.GetValue(AttachProperty);

    /// <summary>
    /// Sets a value indicating whether the behavior is attached.
    /// </summary>
    /// <param name="obj">Dependency object (PasswordBox).</param>
    /// <param name="value">True to attach behavior.</param>
    public static void SetAttach(DependencyObject obj, bool value) => obj.SetValue(AttachProperty, value);

    /// <summary>
    /// Attached property that toggles the behavior hookup to PasswordChanged.
    /// </summary>
    public static readonly DependencyProperty AttachProperty =
        DependencyProperty.RegisterAttached(
            "Attach",
            typeof(bool),
            typeof(BindablePassword),
            new PropertyMetadata(false, OnAttachChanged));

    private static readonly DependencyProperty IsUpdatingProperty =
        DependencyProperty.RegisterAttached(
            "IsUpdating",
            typeof(bool),
            typeof(BindablePassword),
            new PropertyMetadata(false));

    private static bool GetIsUpdating(DependencyObject obj) => (bool)obj.GetValue(IsUpdatingProperty);
    private static void SetIsUpdating(DependencyObject obj, bool value) => obj.SetValue(IsUpdatingProperty, value);

    private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PasswordBox pb)
            return;

        if ((bool)e.NewValue)
            pb.PasswordChanged += HandlePasswordChanged;
        else
            pb.PasswordChanged -= HandlePasswordChanged;
    }

    private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PasswordBox pb)
            return;

        // Prevent feedback loop
        if (!GetIsUpdating(pb))
        {
            pb.PasswordChanged -= HandlePasswordChanged;
            pb.Password = e.NewValue as string ?? string.Empty;
            pb.PasswordChanged += HandlePasswordChanged;
        }
    }

    private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not PasswordBox pb)
            return;

        SetIsUpdating(pb, true);
        SetBoundPassword(pb, pb.Password);
        SetIsUpdating(pb, false);
    }
}