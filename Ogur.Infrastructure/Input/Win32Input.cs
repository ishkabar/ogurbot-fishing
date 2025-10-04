using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ogur.abstractions;


namespace Ogur.Infrastructure.Input;

/// <summary>
/// Windows input implementation stub using SendInput P/Invoke in future.
/// </summary>
public sealed class Win32Input : IInput
{
    private readonly ILogger<Win32Input> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Win32Input"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public Win32Input(ILogger<Win32Input> logger) => _logger = logger;

    /// <inheritdoc />
    public Task KeyPressAsync(ConsoleKey key, CancellationToken ct)
    {
        _logger.LogInformation("KeyPress {Key}", key);
        // TODO: implement SendInput P/Invoke here
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task MouseClickAsync(int x, int y, CancellationToken ct)
    {
        _logger.LogInformation("MouseClick at {X},{Y}", x, y);
        // TODO: implement mouse event P/Invoke here
        return Task.CompletedTask;
    }
}