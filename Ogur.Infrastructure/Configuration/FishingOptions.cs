// File: Ogur.Infrastructure/configuration/FishingOptions.cs
// Project: Ogur.Infrastructure
// Namespace: Ogur.Infrastructure.configuration

namespace Ogur.Infrastructure.configuration;

/// <summary>
/// Consolidated options for the fishing capability including FSM behavior, memory detection, and timing.
/// </summary>
public sealed class FishingOptions
{
    /// <summary>
    /// Gets or sets the polling interval in milliseconds for the main FSM loop.
    /// </summary>
    public int PollIntervalMs { get; init; } = 250;

    /// <summary>
    /// Gets or sets the timeout in seconds for waiting for a bite signal.
    /// </summary>
    public int BiteTimeoutSeconds { get; init; } = 20;

    /// <summary>
    /// Gets or sets whether to show debug overlay in UI.
    /// </summary>
    public bool DebugOverlay { get; init; }

    /// <summary>
    /// Gets or sets the delay after bait key press in milliseconds.
    /// </summary>
    public int PostBaitDelayMs { get; init; } = 200;

    /// <summary>
    /// Gets or sets the delay between consecutive SPACE presses in milliseconds.
    /// </summary>
    public int SpaceBetweenPressMs { get; init; } = 100;

    /// <summary>
    /// Gets or sets the cooldown delay after hook sequence in milliseconds.
    /// </summary>
    public int CooldownMs { get; init; } = 800;

    /// <summary>
    /// Gets or sets the maximum wait time for chat message appearance in milliseconds.
    /// </summary>
    public int ChatWaitTimeoutMs { get; init; } = 5000;

    /// <summary>
    /// Gets or sets timing options for fishing automation.
    /// </summary>
    public TimingOptions Timing { get; init; } = new();

    /// <summary>
    /// Gets or sets the legacy memory-based detection options.
    /// </summary>
    public LegacyMemoryOptions Legacy { get; init; } = new();

    /// <summary>
    /// Timing configuration for fishing cycle delays.
    /// </summary>
    public sealed class TimingOptions
    {
        /// <summary>
        /// Gets or sets minimum cast animation delay in milliseconds.
        /// </summary>
        public int CastAnimationMinMs { get; init; } = 1200;

        /// <summary>
        /// Gets or sets maximum cast animation delay in milliseconds.
        /// </summary>
        public int CastAnimationMaxMs { get; init; } = 2000;

        /// <summary>
        /// Gets or sets minimum hook to animation delay in milliseconds.
        /// </summary>
        public int HookToAnimationMinMs { get; init; } = 500;

        /// <summary>
        /// Gets or sets maximum hook to animation delay in milliseconds.
        /// </summary>
        public int HookToAnimationMaxMs { get; init; } = 801;

        /// <summary>
        /// Gets or sets minimum pull animation delay in milliseconds.
        /// </summary>
        public int PullAnimationMinMs { get; init; } = 1500;

        /// <summary>
        /// Gets or sets maximum pull animation delay in milliseconds.
        /// </summary>
        public int PullAnimationMaxMs { get; init; } = 2201;

        /// <summary>
        /// Gets or sets minimum safety cooldown delay in milliseconds.
        /// </summary>
        public int SafetyCooldownMinMs { get; init; } = 4250;

        /// <summary>
        /// Gets or sets maximum safety cooldown delay in milliseconds.
        /// </summary>
        public int SafetyCooldownMaxMs { get; init; } = 4801;

        /// <summary>
        /// Gets or sets minimum timeout cooldown delay in milliseconds.
        /// </summary>
        public int TimeoutCooldownMinMs { get; init; } = 1000;

        /// <summary>
        /// Gets or sets maximum timeout cooldown delay in milliseconds.
        /// </summary>
        public int TimeoutCooldownMaxMs { get; init; } = 1500;

        /// <summary>
        /// Gets or sets minimum delay before cast in milliseconds.
        /// </summary>
        public int PreCastDelayMinMs { get; init; } = 150;

        /// <summary>
        /// Gets or sets maximum delay before cast in milliseconds.
        /// </summary>
        public int PreCastDelayMaxMs { get; init; } = 150;

        /// <summary>
        /// Gets or sets minimum delay after window activation in milliseconds.
        /// </summary>
        public int PostActivationDelayMinMs { get; init; } = 150;

        /// <summary>
        /// Gets or sets maximum delay after window activation in milliseconds.
        /// </summary>
        public int PostActivationDelayMaxMs { get; init; } = 150;

        /// <summary>
        /// Gets or sets minimum delay after bait selection in milliseconds.
        /// </summary>
        public int PostBaitDelayMinMs { get; init; } = 200;

        /// <summary>
        /// Gets or sets maximum delay after bait selection in milliseconds.
        /// </summary>
        public int PostBaitDelayMaxMs { get; init; } = 200;

        /// <summary>
        /// Gets or sets minimum delay before first hook in milliseconds.
        /// </summary>
        public int PreHookDelayMinMs { get; init; } = 20;

        /// <summary>
        /// Gets or sets maximum delay before first hook in milliseconds.
        /// </summary>
        public int PreHookDelayMaxMs { get; init; } = 61;

        /// <summary>
        /// Gets or sets minimum delay between hook presses in milliseconds.
        /// </summary>
        public int BetweenHookDelayMinMs { get; init; } = 20;

        /// <summary>
        /// Gets or sets maximum delay between hook presses in milliseconds.
        /// </summary>
        public int BetweenHookDelayMaxMs { get; init; } = 61;
    }

    /// <summary>
    /// Legacy memory-based bite detection options.
    /// </summary>
    public sealed class LegacyMemoryOptions
    {
        /// <summary>
        /// Gets or sets the base memory address where chat messages appear.
        /// </summary>
        public long ChatMessageAddress { get; init; }

        /// <summary>
        /// Gets or sets the offset from ChatMessageAddress that contains the space count digit.
        /// </summary>
        public int SpaceCountOffset { get; init; } = 20;

        /// <summary>
        /// Gets or sets the maximum bytes to read for chat message detection.
        /// </summary>
        public int ChatReadLength { get; init; } = 100;

        /// <summary>
        /// Gets or sets the maximum bytes to read for space count string.
        /// </summary>
        public int CountReadLength { get; init; } = 3;

        /// <summary>
        /// Gets or sets the known color tags or markers in chat that indicate bite.
        /// </summary>
        public string[] KnownKeys { get; init; } = new[] { "|cff27c112", "?g|0'?", "??" };

        /// <summary>
        /// Gets or sets the known phrases that indicate required space count.
        /// </summary>
        public string[] KnownCountPhrases { get; init; } =
            new[] { "nij 1 spacji aby wy", "nij 2 spacji aby wy", "nij 3 spacji aby wy" };
    }
}