using System.Windows.Controls;
using Ogur.Fishing.Host.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.ViewModels;

namespace Ogur.Fishing.Host.Wpf.Views;


/// <summary>
/// View allowing the user to select a Metin2 server.
/// </summary>
public partial class ServerSelectView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerSelectView"/> class.
    /// </summary>
    /// <param name="vm">The server select view model resolved from DI.</param>
    /// <param name="nav">Navigation service used to display the main view.</param>
    public ServerSelectView(ServerSelectViewModel vm, INavigationService nav)
    {
        InitializeComponent();
        DataContext = vm;

        vm.ServerChosen += (_, __) =>
        {
            // Navigate to MainView when a server tile is clicked.
            nav.Show<MainView>();
        };
    }
}