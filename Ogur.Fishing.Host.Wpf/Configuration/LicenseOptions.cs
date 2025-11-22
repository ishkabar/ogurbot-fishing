// File: Ogur.Fishing.Host.Wpf/Configuration/LicenseOptions.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Configuration

namespace Ogur.Fishing.Host.Wpf.Configuration;

/// <summary>
/// Configuration options for license management.
/// </summary>
public sealed class LicenseOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether valid license is required.
    /// </summary>
    public bool RequireValidLicense { get; set; } = true;

    /// <summary>
    /// Gets or sets the license check interval in minutes.
    /// </summary>
    public int CheckIntervalMinutes { get; set; } = 60;

    /// <summary>
    /// Gets or sets the grace period in minutes after license expires.
    /// </summary>
    public int GracePeriodMinutes { get; set; } = 1440;
}