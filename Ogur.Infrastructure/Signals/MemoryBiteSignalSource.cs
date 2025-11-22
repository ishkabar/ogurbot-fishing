// File: Ogur.Infrastructure/Signals/MemoryBiteSignalSource.cs
// Project: Ogur.Infrastructure
// Namespace: Ogur.Infrastructure.Signals

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ogur.Abstractions;
using Ogur.Abstractions.Memory;
using Ogur.Abstractions.Metin;
using Ogur.Infrastructure.configuration;

namespace Ogur.Infrastructure.Signals;

/// <summary>
/// Memory-probing bite signal source. Polls target process memory for known markers.
/// </summary>
public sealed class MemoryBiteSignalSource : IFishingSignalSource
{
    private readonly ILogger<MemoryBiteSignalSource> _logger;
    private readonly IProcessMemoryReader _mem;
    private readonly ISelectedProcessAccessor _processAccessor;
    private readonly FishingOptions.LegacyMemoryOptions _opt;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryBiteSignalSource"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="mem">Process memory reader.</param>
    /// <param name="processAccessor">Selected process accessor.</param>
    /// <param name="fishingOptions">Fishing options.</param>
    public MemoryBiteSignalSource(
        ILogger<MemoryBiteSignalSource> logger,
        IProcessMemoryReader mem,
        ISelectedProcessAccessor processAccessor,
        IOptions<FishingOptions> fishingOptions)
    {
        _logger = logger;
        _mem = mem;
        _processAccessor = processAccessor;
        _opt = fishingOptions.Value.Legacy;
    }

    /// <summary>
    /// Waits for the bite signal by polling configured memory region for the selected process.
    /// </summary>
    /// <param name="timeout">Max wait time.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Number of spaces to press (0 if no bite detected).</returns>
    public async Task<int> WaitForBiteAsync(TimeSpan timeout, CancellationToken ct)
{
    _logger.LogInformation("🔥 WaitForBiteAsync START");

    if (!_processAccessor.TryGetSelectedProcess(out var proc) || proc is null)
        return 0;

    var digitAddr = (nint)_processAccessor.MemoryAddress;
    var messageStartAddr = digitAddr - 0x14 + 1;

    if (digitAddr == 0)
    {
        _logger.LogError("🔥 MemoryAddress is 0!");
        return 0;
    }

    if (_opt.KnownKeys.Length == 0 || _opt.KnownCountPhrases.Length == 0)
    {
        _logger.LogError("🔥 KnownKeys or KnownCountPhrases not configured!");
        return 0;
    }

    _logger.LogInformation("🔥 Monitoring: MessageStart=0x{Start:X}, Digit=0x{Digit:X}", 
        messageStartAddr, digitAddr);

    var startTime = Environment.TickCount64;
    const int pollIntervalMs = 20;
    int probeCount = 0;

    while (Environment.TickCount64 - startTime < timeout.TotalMilliseconds && !ct.IsCancellationRequested)
    {
        probeCount++;

        if (probeCount % 50 == 0)
        {
            _logger.LogTrace("🔥 Probe #{Count} at {Elapsed}ms", probeCount,
                Environment.TickCount64 - startTime);
        }

        try
        {
            var fullMessage = await _mem.ReadStringAsync(proc.ProcessId, messageStartAddr, 80, ct);

            if (probeCount % 10 == 0 && !string.IsNullOrEmpty(fullMessage))
            {
                int maxLen = Math.Min(50, fullMessage.Length);
                var display = fullMessage.Substring(0, maxLen);
                _logger.LogTrace("🔥 Probe #{Count}: '{Text}'", probeCount, display);
            }

            if (string.IsNullOrEmpty(fullMessage))
                continue;

            bool startsWithKnownKey = false;
            foreach (var key in _opt.KnownKeys)
            {
                if (fullMessage.StartsWith(key, StringComparison.Ordinal))
                {
                    startsWithKnownKey = true;
                    break;
                }
            }

            if (!startsWithKnownKey)
                continue;

            for (int i = 0; i < _opt.KnownCountPhrases.Length; i++)
            {
                var phrase = _opt.KnownCountPhrases[i];

                if (fullMessage.IndexOf(phrase, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    int spaceCount = i + 1;

                    _logger.LogTrace("🔥 BITE! Matched: '{Phrase}' → Count: {Count}", phrase, spaceCount);
                    _logger.LogTrace("🔥 Full message: '{Text}'",
                        fullMessage.Substring(0, Math.Min(60, fullMessage.Length)));
                    
                    // ✅ RETURN kończy skanowanie!
                    _logger.LogTrace("🔥 WaitForBiteAsync RETURNING {Count}", spaceCount);
                    return spaceCount;
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("🔥 WaitForBiteAsync CANCELLED");
            return 0;
        }
        catch (Exception ex)
        {
            if (probeCount % 100 == 0)
            {
                _logger.LogError(ex, "🔥 Read failed");
            }
        }

        await Task.Delay(pollIntervalMs, ct);
    }

    _logger.LogWarning("🔥 TIMEOUT after {Probes} probes", probeCount);
    return 0;
}
}