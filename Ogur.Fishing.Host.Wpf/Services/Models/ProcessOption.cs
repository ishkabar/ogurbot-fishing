namespace Ogur.Fishing.Host.Wpf.Services.Models;


/// <summary>
/// Represents a candidate game process to attach to, including basic metadata and window geometry.
/// </summary>
public sealed class ProcessOption
{
    /// <summary>
    /// Gets or sets the OS process id.
    /// </summary>
    public int Pid { get; init; }

    /// <summary>
    /// Gets or sets the display label (process name with additional info).
    /// </summary>
    public string Display { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the short display label (process name and PID only).
    /// </summary>
    public string DisplayShort { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the process start time if available.
    /// </summary>
    public DateTime? StartedAt { get; init; }

    /// <summary>
    /// Gets or sets the window width in pixels if available.
    /// </summary>
    public int? ResolutionWidth { get; init; }

    /// <summary>
    /// Gets or sets the window height in pixels if available.
    /// </summary>
    public int? ResolutionHeight { get; init; }

    /// <summary>
    /// Gets or sets the window position X (screen coordinate) if available.
    /// </summary>
    public int? WindowX { get; init; }

    /// <summary>
    /// Gets or sets the window position Y (screen coordinate) if available.
    /// </summary>
    public int? WindowY { get; init; }
    
    /// <summary>
    /// Gets or sets the main window handle (HWND). Required for activation.
    /// </summary>
    public nint Hwnd { get; init; }

    /// <summary>
    /// Gets or sets the executable path if available.
    /// </summary>
    public string? ExePath { get; init; }

    /// <summary>
    /// Returns the compact display label for list controls.
    /// </summary>
    /// <returns>Compact label.</returns>
    public override string ToString() => DisplayShort;
}