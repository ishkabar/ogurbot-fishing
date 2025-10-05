namespace Ogur.Fishing.Host.Wpf.Configuration;

/// <summary>
/// Runtime options for the Fishing capability behavior in the host.
/// </summary>
public sealed class FishingRuntimeOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the bot should activate (focus) the target window before sending input.
    /// </summary>
    public bool FocusTargetWindow { get; init; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether focus preference should be persisted for next runs.
    /// </summary>
    public bool RememberFocusPreference { get; init; } = true;
}