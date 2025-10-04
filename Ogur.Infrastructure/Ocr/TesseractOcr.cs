using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ogur.abstractions;


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
    /// Reads text from an image buffer (stub).
    /// </summary>
    /// <param name="image">Image bytes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Recognized text.</returns>
    public Task<string> ReadTextAsync(byte[] image, CancellationToken ct)
    {
        _logger.LogDebug("OCR on image buffer length={Length}", image.Length);
        return Task.FromResult(string.Empty);
    }
}