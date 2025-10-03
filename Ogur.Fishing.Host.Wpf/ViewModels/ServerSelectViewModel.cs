using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Bot.Host.Wpf.Navigation;
using Bot.Host.Wpf.Views;


namespace Ogur.Fishing.Host.Wpf.ViewModels;


/// <summary>
/// Server selection and HWID check stub.
/// </summary>
public sealed partial class ServerSelectViewModel : ObservableObject
{
    private readonly INavigationService _nav;
    private readonly MainView _mainView;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerSelectViewModel"/> class.
    /// </summary>
    /// <param name="nav">Navigation service.</param>
    /// <param name="mainView">Main application view.</param>
    public ServerSelectViewModel(INavigationService nav, MainView mainView)
    {
        _nav = nav;
        _mainView = mainView;
        Servers = new ObservableCollection<string> { "Metin2-Global", "Metin2-PL", "Private-Server-A" };
    }

    /// <summary>
    /// Gets the list of servers.
    /// </summary>
    public ObservableCollection<string> Servers { get; }

    /// <summary>
    /// Gets or sets the selected server.
    /// </summary>
    public string? SelectedServer { get; set; }

    /// <summary>
    /// Advances to the main window.
    /// </summary>
    public IAsyncRelayCommand ProceedCommand => new AsyncRelayCommand(ProceedAsync);

    private Task ProceedAsync()
    {
        _nav.Navigate(_mainView);
        return Task.CompletedTask;
    }
}