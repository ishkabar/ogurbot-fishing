// File: Ogur.Fishing.Host.Wpf/Configuration/OgurHubOptions.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Configuration

namespace Ogur.Fishing.Host.Wpf.Configuration;

/// <summary>
/// Configuration options for Ogur.Hub integration.
/// </summary>
public sealed class OgurHubOptions
{
    /// <summary>
    /// Gets or sets the API base URL.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://api.hub.ogur.dev";

    /// <summary>
    /// Gets or sets the application identifier.
    /// </summary>
    public string ApplicationId { get; set; } = "ogur-fishing";

    /// <summary>
    /// Gets or sets the API key for authentication.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether SignalR is enabled.
    /// </summary>
    public bool EnableSignalR { get; set; } = true;

    /// <summary>
    /// Gets or sets the SignalR hub URL.
    /// </summary>
    public string SignalRUrl { get; set; } = "https://api.hub.ogur.dev/hubs/devices";

    /// <summary>
    /// Gets or sets the reconnect delay in seconds.
    /// </summary>
    public int ReconnectDelaySeconds { get; set; } = 5;

    /// <summary>
    /// Gets or sets the heartbeat interval in seconds.
    /// </summary>
    public int HeartbeatIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the telemetry batch size.
    /// </summary>
    public int TelemetryBatchSize { get; set; } = 50;

    /// <summary>
    /// Gets or sets the telemetry flush interval in seconds.
    /// </summary>
    public int TelemetryFlushIntervalSeconds { get; set; } = 60;
}