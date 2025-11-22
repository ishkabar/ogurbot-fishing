// File: Ogur.Fishing.Host.Wpf/Configuration/UpdateOptions.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Configuration

namespace Ogur.Fishing.Host.Wpf.Configuration;

/// <summary>
/// Configuration options for application updates.
/// </summary>
public sealed class UpdateOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to check for updates on startup.
    /// </summary>
    public bool CheckOnStartup { get; set; } = true;

    /// <summary>
    /// Gets or sets the update check interval in hours.
    /// </summary>
    public int CheckIntervalHours { get; set; } = 6;

    /// <summary>
    /// Gets or sets a value indicating whether to auto-download updates.
    /// </summary>
    public bool AutoDownload { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to auto-install updates.
    /// </summary>
    public bool AutoInstall { get; set; } = false;
}