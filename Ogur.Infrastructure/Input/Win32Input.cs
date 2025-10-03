using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


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

    /// <summary>
    /// Sends a single key tap asynchronously.
    /// </summary>
    /// <param name="key">Virtual key.</param>
    public Task KeyTapAsync(VirtualKey key)
    {
        _logger.LogInformation("KeyTap {Key}", key);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends a single right mouse click asynchronously.
    /// </summary>
    public Task MouseRightClickAsync()
    {
        _logger.LogInformation("MouseRightClick");
        return Task.CompletedTask;
    }
}