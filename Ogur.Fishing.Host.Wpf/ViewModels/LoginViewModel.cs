using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Views;


namespace Ogur.Fishing.Host.Wpf.ViewModels;


/// <summary>
/// Login view model (placeholder).
/// </summary>
public sealed partial class LoginViewModel : ObservableObject
{
    /// <summary>
    /// Gets or sets username.
    /// </summary>
    [ObservableProperty]
    private string _username = string.Empty;
}