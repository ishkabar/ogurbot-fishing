using System.Windows.Controls;
using Bot.Host.Wpf.ViewModels;


namespace Ogur.Fishing.Host.Wpf.Views;


/// <summary>
/// Server selection view.
/// </summary>
public partial class ServerSelectView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerSelectView"/> class.
    /// </summary>
    /// <param name="vm">View model.</param>
    public ServerSelectView(ServerSelectViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}