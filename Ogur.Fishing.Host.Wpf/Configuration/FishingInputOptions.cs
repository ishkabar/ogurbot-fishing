namespace Ogur.Fishing.Host.Wpf.Configuration;

/// <summary>
/// Options controlling how the fishing input is synthesized using IKeyboardSynthesizer.PressKey2Async.
/// </summary>
public sealed class FishingInputOptions
{
    /// <summary>
    /// Gets or sets bait selection mode (Numeric1To0 or FKeys).
    /// </summary>
    public BaitMode Mode { get; init; } = BaitMode.Numeric1To0;

    /// <summary>
    /// Gets or sets selected bait slot number (1..10 for numeric, 1..12 for F-keys).
    /// </summary>
    public int BaitSlot { get; init; } = 1;

    /// <summary>
    /// Gets or sets delay in milliseconds between bait selection and cast.
    /// </summary>
    public int DelayAfterBaitMs { get; init; } = 60;
}

/// <summary>
/// Determines which keys represent bait slots.
/// </summary>
public enum BaitMode
{
    /// <summary>
    /// Uses 1..0 number row (scan-codes 0x02..0x0B).
    /// </summary>
    Numeric1To0,

    /// <summary>
    /// Uses F1..F12 (scan-codes 0x3B..0x58).
    /// </summary>
    FKeys
}