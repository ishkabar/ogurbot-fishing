using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ogur.abstractions;
using ogur.abstractions.Primitives;


namespace Ogur.Infrastructure.Screen;



/// <summary>
/// DXGI/BitBlt-based screen capture stub.
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
    /// Captures a rectangular region of the screen.
    /// </summary>
    /// <param name="region">Capture region in device pixels.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Raw image bytes (stubbed BGRA32 buffer).</returns>
    public Task<byte[]> CaptureRegionAsync(CaptureRegion region, CancellationToken ct)
    {
        _logger.LogDebug("Capture region {X},{Y} {W}x{H}", region.X, region.Y, region.Width, region.Height);

        var bytesPerPixel = 4;
        var buffer = new byte[region.Width * region.Height * bytesPerPixel];
        return Task.FromResult(buffer);
    }
}