using System.Drawing;
using System.Threading.Tasks;


namespace Ogur.Infrastructure.Ocr;


/// <summary>
/// Abstraction for OCR engine.
/// </summary>
public interface IOcr
{
    /// <summary>
    /// Reads text from a bitmap asynchronously.
    /// </summary>
    /// <param name="bitmap">Bitmap.</param>
    /// <returns>Recognized text.</returns>
    Task<string> ReadTextAsync(Bitmap bitmap);
}