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
}