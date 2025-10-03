using System.Threading.Tasks;

namespace Ogur.Infrastructure.Input;


/// <summary>
/// Abstraction for simulated input.
/// </summary>
public interface IInput
{
    /// <summary>
    /// Sends a single key tap asynchronously.
    /// </summary>
    /// <param name="key">Virtual key.</param>
    Task KeyTapAsync(VirtualKey key);

    /// <summary>
    /// Sends a single right mouse click asynchronously.
    /// </summary>
    Task MouseRightClickAsync();
}

/// <summary>
/// Virtual keys used by input simulation.
/// </summary>
public enum VirtualKey
{
    /// <summary>
    /// Space key.
    /// </summary>
    Space = 0x20,

    /// <summary>
    /// Interaction key.
    /// </summary>
    F = 0x46
}