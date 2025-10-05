namespace Ogur.Infrastructure.Signals
{
    /// <summary>
    /// Options for memory-based bite signal detection.
    /// </summary>
    public sealed class FishingMemorySignalOptions
    {
        /// <summary>
        /// Base address of the chat or indicator buffer to probe.
        /// </summary>
        public long ChatMessageAddress { get; init; }

        /// <summary>
        /// Offset for the space-count or equivalent field.
        /// </summary>
        public int SpaceCountOffset { get; init; }

        /// <summary>
        /// Byte length to read for detection buffer.
        /// </summary>
        public int ChatReadLength { get; init; } = 128;

        /// <summary>
        /// Optional known byte markers encoded as strings.
        /// </summary>
        public string[] KnownKeys { get; init; } = [];
    }
}