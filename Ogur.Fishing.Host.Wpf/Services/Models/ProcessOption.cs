namespace Ogur.Fishing.Host.Wpf.Services.Models;


/// <summary>
/// Represents a candidate game process to attach to.
/// </summary>
public sealed class ProcessOption
{
    /// <summary>
    /// Gets or sets the OS process id.
    /// </summary>
    public int Pid { get; init; }

    /// <summary>
    /// Gets or sets the display name (process name with extras).
    /// </summary>
    public string Display { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the process start time if available.
    /// </summary>
    public DateTime? StartedAt { get; init; }

    /// <summary>
    /// Gets or sets the raw executable path if available.
    /// </summary>
    public string? Path { get; init; }
}