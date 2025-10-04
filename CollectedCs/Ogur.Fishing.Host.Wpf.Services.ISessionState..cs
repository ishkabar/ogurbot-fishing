using Ogur.Fishing.Host.Wpf.ViewModels;

namespace Ogur.Fishing.Host.Wpf.Services;

/// <summary>
/// Holds per-session UI state shared across view models.
/// </summary>
public interface ISessionState
{
    /// <summary>
    /// Gets or sets the currently selected server for this session.
    /// </summary>
    ServerOption? SelectedServer { get; set; }

    /// <summary>
    /// Gets or sets the currently authenticated username for this session.
    /// </summary>
    string? Username { get; set; }
}