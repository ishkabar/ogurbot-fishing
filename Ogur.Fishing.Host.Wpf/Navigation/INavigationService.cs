using System;
using System.Windows;
using System.Windows.Controls;



namespace Ogur.Fishing.Host.Wpf.Navigation;


/// <summary>
/// Provides simple view navigation hosted inside a ContentControl.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Registers the target content host that will display navigated views.
    /// </summary>
    /// <param name="host">Content control to host views.</param>
    void RegisterHost(ContentControl host);

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="view">Target view.</param>
    void Navigate(UserControl view);
}