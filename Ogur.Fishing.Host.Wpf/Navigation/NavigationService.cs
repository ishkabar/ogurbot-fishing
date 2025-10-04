using System.Windows;
using System;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Views;
using System.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Views;



namespace Ogur.Fishing.Host.Wpf.Navigation;

/// <summary>
/// Default navigation service that swaps content inside ShellWindow.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<NavigationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="sp">Service provider used to resolve views.</param>
    /// <param name="logger">Logger instance.</param>
    public NavigationService(IServiceProvider sp, ILogger<NavigationService> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    /// <summary>
    /// Displays a view of the specified type inside the shell region.
    /// </summary>
    /// <typeparam name="TView">The WPF view type to display.</typeparam>
    public void Show<TView>() where TView : FrameworkElement
    {
        var dispatcher = Application.Current?.Dispatcher 
                         ?? throw new InvalidOperationException("WPF Application dispatcher not available.");
        
        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => Show<TView>());
            return;
        }

        if (Application.Current.MainWindow is not ShellWindow shell)
            throw new InvalidOperationException("ShellWindow not found.");

        var host = shell.ContentPresenter 
                   ?? throw new InvalidOperationException("Shell content presenter not available.");

        var view = _sp.GetRequiredService<TView>();
        _logger.LogDebug("Navigating to {View}.", typeof(TView).Name);
        host.Content = view;
    }
}