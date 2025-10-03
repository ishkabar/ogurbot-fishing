using System.Windows;
using Bot.Host.Wpf.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace Ogur.Fishing.Host.Wpf;

/// <summary>
/// Shell window hosting navigation content.
/// </summary>
public partial class ShellWindow : Window
{
    /// <summary>
    /// Gets the content host.
    /// </summary>
    public ContentControl ContentHost => ContentHostField;

    private readonly INavigationService _nav;
    private readonly ServiceProvider _sp;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellWindow"/> class.
    /// </summary>
    /// <param name="nav">Navigation service.</param>
    /// <param name="sp">Service provider.</param>
    public ShellWindow(INavigationService nav, ServiceProvider sp)
    {
        InitializeComponent();
        ContentHostField = (ContentControl)FindName("ContentHost")!;
        _nav = nav;
        _sp = sp;
        ((NavigationService)_nav).RegisterShell(this);

        var login = _sp.GetRequiredService<LoginView>();
        _nav.Navigate(login);
    }

    private ContentControl ContentHostField { get; }
}