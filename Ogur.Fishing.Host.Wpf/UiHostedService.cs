using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Views;


namespace Ogur.Fishing.Host.Wpf;

/// <summary>
/// IHostedService that initializes and shows the WPF ShellWindow.
/// </summary>
public sealed class UiHostedService : IHostedService
{
    private readonly ILogger<UiHostedService> _logger;
    private readonly ShellWindow _shell;

    /// <summary>
    /// Initializes a new instance of the <see cref="UiHostedService"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="shell">Shell window instance.</param>
    public UiHostedService(ILogger<UiHostedService> logger, ShellWindow shell)
    {
        _logger = logger;
        _shell = shell;
    }

    /// <summary>
    /// Starts the UI hosted service and shows the shell window.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting UI hosted service.");
        _shell.Show();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the UI hosted service.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping UI hosted service.");
        return Task.CompletedTask;
    }
}