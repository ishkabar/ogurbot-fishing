using System.Threading;
using System.Threading.Tasks;

namespace Ogur.Fishing.Host.Wpf.Services;


/// <summary>
/// Coordinates high-level application flow and navigation in response to domain UI events.
/// </summary>
public interface IAppFlowCoordinator
{
    /// <summary>
    /// Initializes coordinator subscriptions and performs initial navigation.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when initialization is done.</returns>
    Task InitializeAsync(CancellationToken ct);
}