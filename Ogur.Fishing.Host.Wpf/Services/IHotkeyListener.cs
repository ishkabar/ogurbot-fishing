using System.Windows.Input;

namespace Ogur.Fishing.Host.Wpf.Services;


/// <summary>
/// Captures a single key gesture from the UI message loop.
/// </summary>
public interface IHotkeyListener
{
    /// <summary>
    /// Captures next pressed key (optionally with modifiers) from UI thread.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Captured key gesture.</returns>
    Task<KeyGesture> CaptureNextAsync(CancellationToken ct);
}