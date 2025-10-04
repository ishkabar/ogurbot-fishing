using System.Windows.Input;


namespace Ogur.Fishing.Host.Wpf.Services.Models;


/// <summary>
/// Represents a bait slot bound to a keyboard key (e.g., D1..D4, F1..F4).
/// </summary>
public sealed class BaitOption
{
    /// <summary>
    /// Gets or sets a stable identifier (e.g., "slot_1", "slot_f1").
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the user-facing label (e.g., "1", "F1").
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the key used to trigger this bait slot.
    /// </summary>
    public Key Key { get; init; }
}