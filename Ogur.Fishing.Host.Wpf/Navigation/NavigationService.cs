using System.Windows;


namespace Ogur.Fishing.Host.Wpf.Navigation;


/// <summary>
/// Simple navigation service that injects views into shell content presenter.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private ShellWindow? _shell;

    /// <summary>
    /// Registers shell reference for navigation.
    /// </summary>
    /// <param name="shell">Shell window.</param>
    public void RegisterShell(ShellWindow shell) => _shell = shell;

    /// <summary>
    /// Navigates to target view instance.
    /// </summary>
    /// <param name="view">View instance.</param>
    public void Navigate(FrameworkElement view)
    {
        if (_shell is null) return;
        _shell.ContentHost.Content = view;
    }
}