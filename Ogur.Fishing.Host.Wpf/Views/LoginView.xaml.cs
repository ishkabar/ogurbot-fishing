// File: Bot.Host.Wpf/Views/LoginView.xaml.cs
// Project: Bot.Host.Wpf
// Namespace: Bot.Host.Wpf.Views

using System.Windows.Controls;
using Bot.Host.Wpf.ViewModels;

namespace Ogur.Fishing.Host.Wpf.Views;

/// <summary>
/// Login view.
/// </summary>
public partial class LoginView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginView"/> class.
    /// </summary>
    /// <param name="vm">View model.</param>
    public LoginView(LoginViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}