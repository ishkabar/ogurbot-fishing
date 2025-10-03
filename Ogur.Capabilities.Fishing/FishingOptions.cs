using ogur.abstractions.Primitives;

namespace Ogur.Capabilities.Fishing;

/// <summary>
/// Options for the fishing capability.
/// </summary>
public sealed class FishingOptions
{
    /// <summary>
    /// Gets or sets the polling interval in milliseconds for the main loop and OCR polling.
    /// </summary>
    public int PollIntervalMs { get; init; } = 150;

    /// <summary>
    /// Gets or sets the timeout in seconds for waiting for a bite.
    /// </summary>
    public int BiteTimeoutSeconds { get; init; } = 12;

    /// <summary>
    /// Gets or sets the capture region for bite indication (in device pixels).
    /// </summary>
    public CaptureRegion BiteIndicatorRegion { get; init; } = new CaptureRegion(0, 0, 400, 200);

    /// <summary>
    /// Gets or sets the keyword that indicates a bite in OCR text.
    /// </summary>
    public string BiteKeyword { get; init; } = "Bite";
}