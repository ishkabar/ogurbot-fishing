// File: Bot.Host.Wpf/Views/LoginView.xaml.cs
// Project: Bot.Host.Wpf
// Namespace: Bot.Host.Wpf.Views
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.ViewModels;


using System.Windows.Controls;
using Ogur.Fishing.Host.Wpf.ViewModels;

namespace Ogur.Fishing.Host.Wpf.Views;

/// <summary>
/// Login view.
/// </summary>
public partial class LoginView : UserControl
{
    private readonly INavigationService _nav;
    private readonly IServiceProvider _sp;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginView"/> class.
    /// </summary>
    /// <param name="vm">View model.</param>
    /// <param name="nav">Navigation.</param>
    /// <param name="sp">Service provider.</param>
    public LoginView(LoginViewModel vm, INavigationService nav, IServiceProvider sp)
    {
        InitializeComponent();
        DataContext = vm;
        _nav = nav;
        _sp = sp;
    }

    /// <summary>
    /// Continues to server selection.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Args.</param>
    private void OnContinue(object sender, RoutedEventArgs e)
    {
        _nav.Navigate(_sp.GetRequiredService<ServerSelectView>());
    }
}