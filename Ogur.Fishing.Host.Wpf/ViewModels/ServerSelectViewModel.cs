using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ogur.Fishing.Host.Wpf.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.Services.Models;
using Ogur.Fishing.Host.Wpf.Views;
using Ogur.Fishing.Host.Wpf.Views;
using System.Windows;
using System.Text.RegularExpressions;




namespace Ogur.Fishing.Host.Wpf.ViewModels;


/// <summary>
/// ViewModel for the server selection screen.
/// </summary>
public sealed partial class ServerSelectViewModel : ObservableObject
{
    private readonly ISessionState _session;
    private readonly INavigationService _nav;
    
    /// <summary>
    /// Occurs when a server has been chosen.
    /// </summary>
    public event EventHandler<ServerOption>? ServerChosen;

    /// <summary>
    /// Gets the list of available servers to choose from.
    /// </summary>
    //public ObservableCollection<ServerOption> Servers { get; } = new();
    public ObservableCollection<ServerOption> EnabledServers { get; } = new();
    public ObservableCollection<ServerOption> DisabledServers { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerSelectViewModel"/> class.
    /// </summary>
    public ServerSelectViewModel(ISessionState session, INavigationService nav)
{
    _session = session;
    _nav = nav;

    var servers = new List<ServerOption>
    {
        new(Id: "proxima", Name: "Proxima", 
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/proxima.png",
            IsVisible: true, IsEnabled: true),
        
        new(Id: "tamidia2", Name: "Tamidia2 S2", 
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/tamidia2.png",
            IsVisible: true, IsEnabled: false),
        
        new(Id: "glador", Name: "Glador", 
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/glador.png",
            IsVisible: true, IsEnabled: false),
        
        new(Id: "glevia2", Name: "Glevia2", 
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/glevia2.png",
            IsVisible: true, IsEnabled: false),
        
        new(Id: "monastyr2", Name: "Monastyr2", 
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/monastyr2.png",
            IsVisible: true, IsEnabled: false),
        
        new(Id: "mt2009", Name: "MT2009", 
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/mt2009.png",
            IsVisible: true, IsEnabled: false),
        
        new(Id: "pandora", Name: "Pandora", 
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/pandora.png",
            IsVisible: true, IsEnabled: false),
        
        new(Id: "projekthard", Name: "Projekt Hard", 
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/projekthard.png",
            IsVisible: true, IsEnabled: false),
        
        new(Id: "senthia", Name: "Senthia", 
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/senthia.png",
            IsVisible: true, IsEnabled: false)
    };

    // Sort: enabled first (Proxima), then disabled alphabetically
    var sorted = servers
        .OrderByDescending(s => s.IsEnabled)
        .ThenBy(s => s.Name);

    foreach (var server in sorted)
    {
        if (server.IsEnabled)
            EnabledServers.Add(server);
        else
            DisabledServers.Add(server);
    }
}

    /// <summary>
    /// Selects the specified server option and raises a notification.
    /// Minimal change: updates window title immediately using existing username and server name.
    /// </summary>
    /// <param name="option">Server option selected by the user.</param>
    
    [RelayCommand]
    private void SelectServer(ServerOption option)
    {
        if (option is null) return;

        ServerChosen?.Invoke(this, option);
        _session.SelectedServer = option;

        // Try to get username from session; if empty, try to extract from current window title (minimal, non-invasive).
        var user = string.IsNullOrWhiteSpace(_session.Username) ? string.Empty : _session.Username.Trim();

        if (string.IsNullOrWhiteSpace(user))
        {
            var currentTitle = Application.Current?.MainWindow?.Title;
            if (!string.IsNullOrWhiteSpace(currentTitle))
            {
                var m = Regex.Match(currentTitle, @"^Ogur - Fishing Planet - (?<user>.*?)(?: \(|$)");
                if (m.Success)
                {
                    user = m.Groups["user"].Value.Trim();
                }
            }
        }

        var server = string.IsNullOrWhiteSpace(option?.Name) ? string.Empty : option.Name.Trim();

        string title;
        if (string.IsNullOrEmpty(user) && string.IsNullOrEmpty(server))
            title = "Ogur - Fishing Planet";
        else if (!string.IsNullOrEmpty(user) && string.IsNullOrEmpty(server))
            title = $"Ogur - Fishing Planet - {user}";
        else if (string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(server))
            title = $"Ogur - Fishing Planet ({server})";
        else
            title = $"Ogur - Fishing Planet - {user} ({server})";

        // Set title on UI thread (one-liner).
        Application.Current?.Dispatcher?.Invoke(() => { if (Application.Current?.MainWindow != null) Application.Current.MainWindow.Title = title; });

        _nav.Show<MainView>();
    }

}