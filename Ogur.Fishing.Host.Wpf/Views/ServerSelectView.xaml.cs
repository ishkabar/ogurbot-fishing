using System.Windows.Controls;
using Ogur.Fishing.Host.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.ViewModels;

namespace Ogur.Fishing.Host.Wpf.Views;


/// <summary>
/// Server selection view.
/// </summary>
public partial class ServerSelectView : UserControl
{
    private readonly INavigationService _nav;
    private readonly IServiceProvider _sp;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerSelectView"/> class.
    /// </summary>
    /// <param name="vm">View model.</param>
    /// <param name="nav">Navigation.</param>
    /// <param name="sp">Service provider.</param>
    public ServerSelectView(ServerSelectViewModel vm, INavigationService nav, IServiceProvider sp)
    {
        InitializeComponent();
        DataContext = vm;
        _nav = nav;
        _sp = sp;

        vm.ServerChosen += OnServerChosen;
    }

    /// <summary>
    /// Handles server selection and navigates forward.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Selected server.</param>
    private void OnServerChosen(object? sender, ServerOption e)
    {
        // Example: navigate to the main fishing screen after selection.
        var main = _sp.GetRequiredService<MainView>();
        _nav.Navigate(main);
    }
}