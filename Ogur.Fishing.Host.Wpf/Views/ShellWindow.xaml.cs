using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Views;

namespace Ogur.Fishing.Host.Wpf.Views;

/// <summary>
/// Main shell window hosting the navigation content.
/// </summary>
public partial class ShellWindow : Window
{
    private readonly INavigationService _nav;
    private readonly IServiceProvider _sp;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellWindow"/> class.
    /// </summary>
    /// <param name="nav">Navigation service.</param>
    /// <param name="sp">Service provider.</param>
    public ShellWindow(INavigationService nav, IServiceProvider sp)
    {
        InitializeComponent();
        _nav = nav;
        _sp = sp;

        _nav.RegisterHost(PART_ContentHost);

        // Start screen: your existing LoginView
        var login = _sp.GetRequiredService<LoginView>();
        _nav.Navigate(login);
    }
}