using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions;


namespace Ogur.Infrastructure.Input;
/// <summary>
/// Windows input implementation placeholder.
/// </summary>
public sealed class Win32Input : IInput
{
    private readonly ILogger<Win32Input> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Win32Input"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    public Win32Input(ILogger<Win32Input> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Sends a left mouse click.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task.</returns>
    public Task LeftClickAsync(CancellationToken ct)
    {
        _logger.LogDebug("LeftClickAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Moves the cursor to specified coordinates.
    /// </summary>
    /// <param name="x">X position.</param>
    /// <param name="y">Y position.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task.</returns>
    public Task MoveCursorAsync(int x, int y, CancellationToken ct)
    {
        _logger.LogDebug("MoveCursorAsync({X}, {Y})", x, y);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends text input to the active window.
    /// </summary>
    /// <param name="text">Text to send.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task.</returns>
    public Task SendTextAsync(string text, CancellationToken ct)
    {
        _logger.LogDebug("SendTextAsync(\"{Text}\")", text);
        return Task.CompletedTask;
    }
}