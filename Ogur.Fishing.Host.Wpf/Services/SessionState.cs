using Ogur.Fishing.Host.Wpf.ViewModels;
using Ogur.Fishing.Host.Wpf.Services.Models;



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
    
    /// <summary>
    /// Gets or sets the currently selected bait option.
    /// </summary>
    public BaitOption? SelectedBait { get; set; }

    /// <summary>
    /// Gets or sets the currently selected game process.
    /// </summary>
    public ProcessOption? SelectedProcess { get; set; }
}