using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Bot.Host.Wpf.Navigation;
using Bot.Host.Wpf.Views;


namespace Ogur.Fishing.Host.Wpf.ViewModels;


/// <summary>
/// Login view model stub providing future auth hook.
/// </summary>
public sealed partial class LoginViewModel : ObservableObject
{
    private readonly INavigationService _nav;
    private readonly ServerSelectView _serverSelectView;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
    /// </summary>
    /// <param name="nav">Navigation service.</param>
    /// <param name="serverSelectView">Server select view.</param>
    public LoginViewModel(INavigationService nav, ServerSelectView serverSelectView)
    {
        _nav = nav;
        _serverSelectView = serverSelectView;
    }

    /// <summary>
    /// Command to continue to server selection.
    /// </summary>
    public IAsyncRelayCommand ContinueCommand => new AsyncRelayCommand(ContinueAsync);

    private Task ContinueAsync()
    {
        _nav.Navigate(_serverSelectView);
        return Task.CompletedTask;
    }
}