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
    public ObservableCollection<ServerOption> Servers { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerSelectViewModel"/> class.
    /// </summary>
    public ServerSelectViewModel(ISessionState session, INavigationService nav)
    {
        _session = session;
        _nav = nav;
        Servers.Add(new ServerOption(
            Id: "proxima",
            Name: "Proxima",
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/proxima.png"));

        Servers.Add(new ServerOption(
            Id: "tamidia2",
            Name: "Tamidia2 S2",
            IconPath: "pack://application:,,,/Ogur.Fishing.Host.Wpf;component/Assets/Servers/tamidia2.png"));
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
                var m = Regex.Match(currentTitle, @"^Ogur fishing - (?<user>.*?)(?: \(|$)");
                if (m.Success)
                {
                    user = m.Groups["user"].Value.Trim();
                }
            }
        }

        var server = string.IsNullOrWhiteSpace(option?.Name) ? string.Empty : option.Name.Trim();

        string title;
        if (string.IsNullOrEmpty(user) && string.IsNullOrEmpty(server))
            title = "Ogur fishing";
        else if (!string.IsNullOrEmpty(user) && string.IsNullOrEmpty(server))
            title = $"Ogur fishing - {user}";
        else if (string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(server))
            title = $"Ogur fishing ({server})";
        else
            title = $"Ogur fishing - {user} ({server})";

        // Set title on UI thread (one-liner).
        Application.Current?.Dispatcher?.Invoke(() => { if (Application.Current?.MainWindow != null) Application.Current.MainWindow.Title = title; });

        _nav.Show<MainView>();
    }

}