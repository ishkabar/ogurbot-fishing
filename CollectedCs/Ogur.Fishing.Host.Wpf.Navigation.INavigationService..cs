using System;
using System.Windows;
using System.Windows.Controls;



namespace Ogur.Fishing.Host.Wpf.Navigation;


/// <summary>
/// Provides view navigation within the shell window.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Displays a view of the specified type inside the shell region.
    /// </summary>
    /// <typeparam name="TView">The WPF view type to display.</typeparam>
    void Show<TView>() where TView : System.Windows.FrameworkElement;
}