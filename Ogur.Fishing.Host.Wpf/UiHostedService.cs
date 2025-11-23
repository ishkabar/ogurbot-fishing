// File: Ogur.Fishing.Host.Wpf/UiHostedService.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.Services.HubIntegration;

namespace Ogur.Fishing.Host.Wpf;

/// <summary>
/// Hosted service that initializes UI flow on application start.
/// </summary>
public sealed class UiHostedService : IHostedService
{
    private readonly ILogger<UiHostedService> _logger;
    private readonly IAppFlowCoordinator _coordinator;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="UiHostedService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="coordinator">App flow coordinator instance.</param>
    /// <param name="serviceProvider">Service provider for resolving dependencies.</param>
    public UiHostedService(
        ILogger<UiHostedService> logger, 
        IAppFlowCoordinator coordinator,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _coordinator = coordinator;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Starts the hosted service and initializes the app flow.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UI Hosted Service starting");

        try
        {
            // Start HubIntegrationService manually and wait for startup checks
            var hubService = _serviceProvider.GetRequiredService<HubIntegrationService>();
        
            _logger.LogInformation("Starting HubIntegrationService");
        
            // Start the service in background
            _ = Task.Run(() => hubService.StartAsync(cancellationToken), cancellationToken);
        
            // Wait for startup checks to complete (with timeout)
            var waitTask = Task.Run(() => hubService.StartupComplete.Wait(cancellationToken), cancellationToken);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        
            var completedTask = await Task.WhenAny(waitTask, timeoutTask);
        
            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("HubIntegrationService startup check timed out");
            }

            // ✅ DON'T initialize UI if update is required
            if (hubService.RequiredUpdateBlocked)
            {
                _logger.LogInformation("Required update detected - HubIntegrationService will handle UI");
            
                // Wait for HubIntegrationService to show UpdateRequiredView
                await Task.Delay(Timeout.Infinite, cancellationToken);
                return;
            }

            _logger.LogInformation("Initializing UI flow");
            await _coordinator.InitializeAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start UI hosted service");
            throw;
        }
    }

    /// <summary>
    /// Stops the hosted service.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UI Hosted Service stopping");
        return Task.CompletedTask;
    }
}