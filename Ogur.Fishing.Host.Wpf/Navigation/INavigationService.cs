using System;
using System.Windows;


namespace Ogur.Fishing.Host.Wpf.Navigation;


/// <summary>
/// Navigation service for swapping views in the shell.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to target view instance.
    /// </summary>
    /// <param name="view">View instance.</param>
    void Navigate(FrameworkElement view);
}