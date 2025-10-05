namespace Ogur.Capabilities.Fishing;

/// <summary>
/// Options for memory-driven fishing flow aligned with the legacy bot.
/// </summary>
public sealed class FishingMemoryOptions
{
    /// <summary>
    /// Base address where the chat message appears.
    /// </summary>
    public long ChatMessageAddress { get; init; }

    /// <summary>
    /// Offset from base address that contains the single-digit space count.
    /// </summary>
    public int SpaceCountOffset { get; init; } = 0x14;

    /// <summary>
    /// Maximum bytes to read for chat message.
    /// </summary>
    public int ChatReadLength { get; init; } = 100;

    /// <summary>
    /// Maximum bytes to read for space count string.
    /// </summary>
    public int CountReadLength { get; init; } = 3;

    /// <summary>
    /// Keys that indicate the relevant color tags or markers in chat.
    /// </summary>
    public string[] KnownKeys { get; init; } = new[] { "|cff27c112", "?g|0'?", "??" };

    /// <summary>
    /// Keys that indicate the required count phrase ("press N spaces").
    /// </summary>
    public string[] KnownCountPhrases { get; init; } =
        new[] { "nij 1 spacji aby wy", "nij 2 spacji aby wy", "nij 3 spacji aby wy" };

    /// <summary>
    /// Delay after bait key press in milliseconds.
    /// </summary>
    public int PostBaitDelayMs { get; init; } = 200;

    /// <summary>
    /// Delay between SPACE presses in milliseconds.
    /// </summary>
    public int SpaceBetweenPressMs { get; init; } = 100;

    /// <summary>
    /// Cooldown delay after the hook sequence in milliseconds.
    /// </summary>
    public int CooldownMs { get; init; } = 800;

    /// <summary>
    /// Max wait time for the chat message to appear in milliseconds.
    /// </summary>
    public int ChatWaitTimeoutMs { get; init; } = 5000;
}