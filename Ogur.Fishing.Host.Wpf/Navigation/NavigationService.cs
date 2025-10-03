using System.Windows;
using System;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Views;


namespace Ogur.Fishing.Host.Wpf.Navigation;


/// <summary>
/// Navigation service that swaps content of a registered ContentControl.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;
    private ContentControl? _host;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public NavigationService(ILogger<NavigationService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void RegisterHost(ContentControl host)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _logger.LogInformation("Navigation host registered: {Host}", host.Name);
    }

    /// <inheritdoc />
    public void Navigate(UserControl view)
    {
        if (view is null) throw new ArgumentNullException(nameof(view));
        if (_host is null) throw new InvalidOperationException("Navigation host not registered. Call RegisterHost() first.");
        _logger.LogInformation("Navigating to {View}", view.GetType().Name);
        _host.Content = view;
    }
}