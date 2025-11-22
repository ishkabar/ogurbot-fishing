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
    /// Gets or sets the legacy memory-based detection options.
    /// </summary>
    public LegacyMemoryOptions Legacy { get; init; } = new();

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