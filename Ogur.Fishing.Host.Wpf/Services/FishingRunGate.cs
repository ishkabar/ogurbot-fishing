// File: Ogur.Fishing.Host.Wpf/Services/FishingRunGate.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Services
namespace Ogur.Fishing.Host.Wpf.Services;

/// <summary>
/// Gate that controls whether fishing actions should be executed.
/// </summary>
public sealed class FishingRunGate : IFishingRunGate
{
    private int _enabled;

    /// <summary>
    /// Gets a value indicating whether execution is enabled.
    /// </summary>
    public bool Enabled => System.Threading.Volatile.Read(ref _enabled) == 1;

    /// <summary>
    /// Enables the gate.
    /// </summary>
    public void Enable() => System.Threading.Interlocked.Exchange(ref _enabled, 1);

    /// <summary>
    /// Disables the gate.
    /// </summary>
    public void Disable() => System.Threading.Interlocked.Exchange(ref _enabled, 0);
}

/// <summary>
/// Contract for run gate.
/// </summary>
public interface IFishingRunGate
{
    /// <summary>
    /// Gets a value indicating whether execution is enabled.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// Enables the gate.
    /// </summary>
    void Enable();

    /// <summary>
    /// Disables the gate.
    /// </summary>
    void Disable();
}