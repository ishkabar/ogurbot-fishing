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
}

/// <summary>
/// Default in-memory implementation of <see cref="ISessionState"/>.
/// </summary>
public sealed class SessionState : ISessionState
{
    /// <summary>
    /// Gets or sets the currently selected server for this session.
    /// </summary>
    public ServerOption? SelectedServer { get; set; }
}