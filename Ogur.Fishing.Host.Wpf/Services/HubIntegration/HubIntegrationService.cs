// File: Ogur.Fishing.Host.Wpf/Services/HubIntegration/HubIntegrationService.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Services.HubIntegration

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Hub;
using Ogur.Fishing.Host.Wpf.ViewModels;
using Ogur.Fishing.Host.Wpf.Views;

namespace Ogur.Fishing.Host.Wpf.Services.HubIntegration;

/// <summary>
/// Background service managing hub integration lifecycle.
/// </summary>
public sealed class HubIntegrationService : BackgroundService
{
    private readonly IHubClient _hubClient;
    private readonly ILicenseManager _licenseManager;
    private readonly ITelemetryReporter _telemetryReporter;
    private readonly IUpdateChecker _updateChecker;
    private readonly HubCommandHandler _commandHandler;
    private readonly ILogger<HubIntegrationService> _logger;
    private readonly IServiceProvider _serviceProvider;


    /// <summary>
    /// Initializes a new instance of the <see cref="HubIntegrationService"/> class.
    /// </summary>
    public HubIntegrationService(
        IHubClient hubClient,
        ILicenseManager licenseManager,
        ITelemetryReporter telemetryReporter,
        IUpdateChecker updateChecker,
        HubCommandHandler commandHandler,
        ILogger<HubIntegrationService> logger,
        IServiceProvider serviceProvider)
    {
        _hubClient = hubClient;
        _licenseManager = licenseManager;
        _telemetryReporter = telemetryReporter;
        _updateChecker = updateChecker;
        _commandHandler = commandHandler;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Hub integration service starting");

        // ✅ Sprawdź aktualizacje
        var updateResult = await _updateChecker.CheckForUpdatesAsync(
            HubConstants.ApplicationVersion,
            stoppingToken);

    
        if (updateResult.IsUpdateAvailable && updateResult.IsRequired)
        {
            _logger.LogCritical(
                "REQUIRED UPDATE AVAILABLE: {Current} -> {Latest}",
                updateResult.CurrentVersion,
                updateResult.LatestVersion);

            // Pokaż UpdateRequiredView
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    var viewModel = _serviceProvider.GetRequiredService<UpdateRequiredViewModel>();
                    viewModel.Initialize(
                        updateResult.CurrentVersion,
                        updateResult.LatestVersion,
                        updateResult.DownloadUrl,
                        updateResult.ReleaseNotes);

                    var view = _serviceProvider.GetRequiredService<UpdateRequiredView>();
                    view.DataContext = viewModel;

                    mainWindow.Content = view;
                }
            });

            return;
        }

        if (updateResult.IsUpdateAvailable)
        {
            _logger.LogInformation(
                "Optional update available: {Current} -> {Latest}",
                updateResult.CurrentVersion,
                updateResult.LatestVersion);
        }

        try
        {
            await _telemetryReporter.ReportEventAsync("ApplicationStarted", new
            {
                Version = typeof(App).Assembly.GetName().Version?.ToString(),
                StartedAt = DateTime.UtcNow
            }, stoppingToken);

            await _hubClient.ConnectAsync(stoppingToken);
            _logger.LogInformation("Connected to Ogur.Hub");

            _commandHandler.StartListening();

            await _licenseManager.StartPeriodicValidationAsync(stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Hub integration service stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hub integration service failed");
        }
        finally
        {
            _commandHandler.StopListening();

            await _telemetryReporter.ReportEventAsync("ApplicationStopped", new
            {
                StoppedAt = DateTime.UtcNow
            }, CancellationToken.None);

            await _hubClient.DisconnectAsync(CancellationToken.None);
        }
    }
}