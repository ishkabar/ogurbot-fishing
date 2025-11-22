using System.Threading;
using System.Threading.Tasks;
using Ogur.Abstractions;
using Ogur.Abstractions.Input;


namespace Ogur.Fishing.Host.Wpf.Services;

/// <summary>
/// No-op implementation of <see cref="IInput"/> used for design-time and fallback scenarios.
/// </summary>
public sealed class NullInput : IInput
{
    /// <summary>
    /// Sends a textual sequence using simulated keyboard input (no-op).
    /// </summary>
    /// <param name="text">Text to type.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task SendTextAsync(string text, CancellationToken ct) => Task.CompletedTask;
    
    /// <summary>
    /// Moves the cursor to the specified screen coordinates without performing any real input.
    /// </summary>
    /// <param name="x">X screen coordinate.</param>
    /// <param name="y">Y screen coordinate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task MoveCursorAsync(int x, int y, CancellationToken ct) => Task.CompletedTask;

    /// <summary>
    /// Performs a left mouse click at the current cursor position without real input.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task LeftClickAsync(CancellationToken ct) => Task.CompletedTask;
    
    /// <summary>
    /// Sends a single key press (no-op).
    /// </summary>
    /// <param name="key">Key to send.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task SendKeyAsync(InputKey key, CancellationToken ct) => Task.CompletedTask;

}