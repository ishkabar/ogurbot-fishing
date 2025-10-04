using Ogur.Fishing.Host.Wpf.Services.Models;


namespace Ogur.Fishing.Host.Wpf.Services;


/// <summary>
/// Queries candidate game processes for attaching the capability.
/// </summary>
public interface IProcessQueryService
{
    /// <summary>
    /// Finds candidate processes, ordered by last usage (most recent first).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of process options.</returns>
    Task<IReadOnlyList<ProcessOption>> GetCandidatesAsync(CancellationToken ct);
}