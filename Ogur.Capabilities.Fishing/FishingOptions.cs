namespace Ogur.Capabilities.Fishing;


/// <summary>
/// Options for Fishing capability.
/// </summary>
public sealed class FishingOptions
{
    /// <summary>
    /// Gets or sets the polling interval in milliseconds for main loop ticks.
    /// </summary>
    public int PollIntervalMs { get; init; } = 250;

    /// <summary>
    /// Gets or sets the maximum wait time in seconds for a bite event.
    /// </summary>
    public int BiteTimeoutSeconds { get; init; } = 20;

    /// <summary>
    /// Gets or sets a value indicating whether debug overlays should be shown.
    /// </summary>
    public bool DebugOverlay { get; init; }
}