using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace Ogur.Infrastructure.Ocr;


/// <summary>
/// Tesseract OCR implementation stub.
/// </summary>
public sealed class TesseractOcr : IOcr
{
    private readonly ILogger<TesseractOcr> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TesseractOcr"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public TesseractOcr(ILogger<TesseractOcr> logger) => _logger = logger;

    /// <summary>
    /// Reads text from a bitmap asynchronously.
    /// </summary>
    /// <param name="bitmap">Bitmap.</param>
    /// <returns>Recognized text.</returns>
    public Task<string> ReadTextAsync(Bitmap bitmap)
    {
        _logger.LogDebug("OCR on bitmap {W}x{H}", bitmap.Width, bitmap.Height);
        return Task.FromResult(string.Empty);
    }
}