using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions;


namespace Ogur.Infrastructure.Ocr;


/// <summary>
/// Tesseract OCR implementation placeholder.
/// </summary>
public sealed class TesseractOcr : IOcr
{
    private readonly ILogger<TesseractOcr> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TesseractOcr"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    public TesseractOcr(ILogger<TesseractOcr> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Recognizes text from image bytes.
    /// </summary>
    /// <param name="imageBytes">Image bytes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Recognized text.</returns>
    public Task<string> RecognizeAsync(byte[] imageBytes, CancellationToken ct)
    {
        _logger.LogDebug("RecognizeAsync(bytes: {Length})", imageBytes?.Length ?? 0);
        return Task.FromResult(string.Empty);
    }
}