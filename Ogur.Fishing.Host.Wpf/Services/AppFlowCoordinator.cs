using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Fishing.Host.Wpf.ViewModels.Messages;
using Ogur.Fishing.Host.Wpf.Views;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Navigation;



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
    private readonly ISessionState _session;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppFlowCoordinator"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="messenger">Messenger used to subscribe to app messages.</param>
    /// <param name="navigation">Navigation service.</param>
    public AppFlowCoordinator(
        ILogger<AppFlowCoordinator> logger,
        IMessenger messenger,
        INavigationService navigation,
        ISessionState session)
    {
        _logger = logger;
        _messenger = messenger;
        _navigation = navigation;
        _session = session;
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
    /// Handles login success by navigating to the server selection view
    /// and updating the window title to include the username (minimal, non-invasive).
    /// </summary>
    /// <param name="message">Login succeeded message.</param>
    public void Receive(LoginSucceededMessage message)
    {
        _logger.LogInformation("Login succeeded for {User}. Navigating to ServerSelectView.", message?.Username);

        // Update main window title safely on UI thread, minimal change (no DI changes).
        try
        {
            var user = string.IsNullOrWhiteSpace(message?.Username) ? null : message.Username.Trim();

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                // Prefer ShellWindow if it is set as MainWindow (most likely in your app).
                if (Application.Current?.MainWindow is Views.ShellWindow shell)
                {
                    if (string.IsNullOrEmpty(user))
                    {
                        shell.Title = "Ogur - Fishing Planet";
                    }
                    else
                    {
                        shell.Title = $"Ogur - Fishing Planet - {user}";
                    }
                }
                // Fallback: just set MainWindow.Title if not the ShellWindow instance
                else if (Application.Current?.MainWindow != null)
                {
                    Application.Current.MainWindow.Title = string.IsNullOrEmpty(user)
                        ? "Ogur - Fishing Planet"
                        : $"Ogur - Fishing Planet - {user}";
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update main window title after login. This is non-fatal.");
        }

        // keep original navigation behavior
        _navigation.Show<ServerSelectView>();
    }

}