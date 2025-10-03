using System.Windows.Controls;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.ViewModels;
using System.Windows.Controls;


namespace Ogur.Fishing.Host.Wpf.Views;

/// <summary>
/// Main fishing view.
/// </summary>
public partial class MainView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    /// <param name="vm">Main view model.</param>
    public MainView(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}