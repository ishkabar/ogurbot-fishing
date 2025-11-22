// File: Ogur.Fishing.Host.Wpf/Views/LoginView.xaml.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Views

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Ogur.Fishing.Host.Wpf.ViewModels;

namespace Ogur.Fishing.Host.Wpf.Views;

/// <summary>
/// Login view hosting username/password form.
/// </summary>
public partial class LoginView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginView"/> class.
    /// </summary>
    /// <param name="vm">Login view model resolved from DI.</param>
    public LoginView(LoginViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
    
    /// <summary>
    /// Opens registration page in default browser.
    /// </summary>
    private void RegisterButton_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://hub.ogur.dev/Account/Register",
            UseShellExecute = true
        });
    }
}