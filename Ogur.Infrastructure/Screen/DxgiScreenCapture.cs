using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace Ogur.Infrastructure.Screen;


/// <summary>
/// DXGI-based screen capture stub.
/// </summary>
public sealed class DxgiScreenCapture : IScreenCapture
{
    private readonly ILogger<DxgiScreenCapture> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DxgiScreenCapture"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public DxgiScreenCapture(ILogger<DxgiScreenCapture> logger) => _logger = logger;

    /// <summary>
    /// Captures the screen to a bitmap asynchronously.
    /// </summary>
    /// <returns>Bitmap.</returns>
    public Task<Bitmap> CaptureAsync()
    {
        _logger.LogDebug("Capture screen");
        return Task.FromResult(new Bitmap(64, 64));
    }
}