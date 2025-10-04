using Ogur.Fishing.Host.Wpf.ViewModels;


namespace Ogur.Fishing.Host.Wpf.Services;

/// <summary>
/// Default in-memory implementation of <see cref="ISessionState"/>.
/// Lightweight POCO — intentionally no INotifyPropertyChanged to keep changes minimal and non-invasive.
/// </summary>
public sealed class SessionState : ISessionState
{
    /// <summary>
    /// Gets or sets the currently selected server for this session.
    /// </summary>
    public ServerOption? SelectedServer { get; set; }

    /// <summary>
    /// Gets or sets the currently authenticated username for this session.
    /// </summary>
    public string? Username { get; set; }
}