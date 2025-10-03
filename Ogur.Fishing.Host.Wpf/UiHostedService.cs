using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Ogur.Fishing.Host.Wpf;


/// <summary>
/// Hosted service that shows the WPF shell window and controls initial navigation.
/// </summary>
public sealed class UiHostedService : IHostedService
{
    private readonly ShellWindow _shell;
    private readonly ILogger<UiHostedService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UiHostedService"/> class.
    /// </summary>
    /// <param name="shell">Shell window.</param>
    /// <param name="logger">Logger.</param>
    public UiHostedService(ShellWindow shell, ILogger<UiHostedService> logger)
    {
        _shell = shell;
        _logger = logger;
    }

    /// <summary>
    /// Starts the service and shows the shell window.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting UI host");
        _shell.Show();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the service.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping UI host");
        return Task.CompletedTask;
    }
}