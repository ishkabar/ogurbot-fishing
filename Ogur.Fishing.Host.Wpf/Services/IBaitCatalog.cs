using Ogur.Fishing.Host.Wpf.Services.Models;

namespace Ogur.Fishing.Host.Wpf.Services;


/// <summary>
/// Provides available bait slots mapped to keyboard keys.
/// </summary>
public interface IBaitCatalog
{
    /// <summary>
    /// Gets all bait slot options (1..4, F1..F4).
    /// </summary>
    /// <returns>List of bait slot options.</returns>
    IReadOnlyList<BaitOption> GetAll();
}