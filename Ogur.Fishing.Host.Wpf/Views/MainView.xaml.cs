using System.Windows.Controls;
using Bot.Host.Wpf.ViewModels;


namespace Ogur.Fishing.Host.Wpf.Views;


/// <summary>
/// Main application view.
/// </summary>
public partial class MainView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    /// <param name="vm">View model.</param>
    public MainView(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}