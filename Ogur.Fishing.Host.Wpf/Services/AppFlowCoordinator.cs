using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Fishing.Host.Wpf.ViewModels.Messages;
using Ogur.Fishing.Host.Wpf.Views;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.ViewModels.Messages;
using Ogur.Fishing.Host.Wpf.Views;


namespace Ogur.Fishing.Host.Wpf.Services;


/// <summary>
/// Default implementation of <see cref="IAppFlowCoordinator"/> that reacts to UI messages and navigates between views.
/// </summary>
public sealed class AppFlowCoordinator : IAppFlowCoordinator,
    IRecipient<LoginSucceededMessage>
{
    private readonly ILogger<AppFlowCoordinator> _logger;
    private readonly IMessenger _messenger;
    private readonly INavigationService _navigation;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppFlowCoordinator"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="messenger">Messenger used to subscribe to app messages.</param>
    /// <param name="navigation">Navigation service.</param>
    public AppFlowCoordinator(
        ILogger<AppFlowCoordinator> logger,
        IMessenger messenger,
        INavigationService navigation)
    {
        _logger = logger;
        _messenger = messenger;
        _navigation = navigation;
    }

    /// <summary>
    /// Initializes coordinator subscriptions and sets the initial view.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    public Task InitializeAsync(CancellationToken ct)
    {
        _messenger.RegisterAll(this);
        _logger.LogInformation("AppFlowCoordinator initialized. Showing LoginView.");
        _navigation.Show<LoginView>();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles login success by navigating to the server selection view.
    /// </summary>
    /// <param name="message">Login succeeded message.</param>
    public void Receive(LoginSucceededMessage message)
    {
        _logger.LogInformation("Login succeeded for {User}. Navigating to ServerSelectView.", message.Username);
        _navigation.Show<ServerSelectView>();
    }
}