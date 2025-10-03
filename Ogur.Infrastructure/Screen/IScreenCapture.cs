using System.Drawing;
using System.Threading.Tasks;

namespace Ogur.Infrastructure.Screen;

/// <summary>
/// Abstraction for screen capture.
/// </summary>
public interface IScreenCapture
{
    /// <summary>
    /// Captures the screen to a bitmap asynchronously.
    /// </summary>
    /// <returns>Bitmap.</returns>
    Task<Bitmap> CaptureAsync();
}