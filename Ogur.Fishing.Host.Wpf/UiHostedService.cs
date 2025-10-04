using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Views;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Ogur.Fishing.Host.Wpf.Services;


namespace Ogur.Fishing.Host.Wpf;

/// <summary>
/// Hosted service that initializes UI flow on application start.
/// </summary>
public sealed class UiHostedService : IHostedService
{
    private readonly ILogger<UiHostedService> _logger;
    private readonly IAppFlowCoordinator _coordinator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UiHostedService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="coordinator">App flow coordinator instance.</param>
    public UiHostedService(ILogger<UiHostedService> logger, IAppFlowCoordinator coordinator)
    {
        _logger = logger;
        _coordinator = coordinator;
    }

    /// <summary>
    /// Starts the hosted service and initializes the app flow.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UI Hosted Service starting.");
        await _coordinator.InitializeAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Stops the hosted service.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UI Hosted Service stopping.");
        return Task.CompletedTask;
    }
}