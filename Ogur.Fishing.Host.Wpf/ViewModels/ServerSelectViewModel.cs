using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Views;


namespace Ogur.Fishing.Host.Wpf.ViewModels;


/// <summary>
/// ViewModel for the server selection screen.
/// </summary>
public sealed partial class ServerSelectViewModel : ObservableObject
{
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
    public ServerSelectViewModel()
    {
        // For now hard-coded; later read from appsettings.json via IOptions<ServersOptions>.
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
    /// </summary>
    /// <param name="option">Server option selected by the user.</param>
    [RelayCommand]
    private void SelectServer(ServerOption option)
    {
        // TODO: persist selection via configuration (IOptions snapshot or a profile store).
        ServerChosen?.Invoke(this, option);
    }
}