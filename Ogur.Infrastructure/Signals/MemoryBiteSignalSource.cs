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

public sealed class MemoryBiteSignalSource : IFishingSignalSource
{
    private readonly ILogger<MemoryBiteSignalSource> _logger;
    private readonly IProcessMemoryReader _mem;
    private readonly ISelectedProcessAccessor _processAccessor;
    private readonly IChatBufferDetector? _chatDetector;
    private readonly FishingOptions.LegacyMemoryOptions _opt;
    private readonly Action<long>? _setMemoryAddress;

    private bool _isDetected;

    public MemoryBiteSignalSource(
        ILogger<MemoryBiteSignalSource> logger,
        IProcessMemoryReader mem,
        ISelectedProcessAccessor processAccessor,
        IOptions<FishingOptions> fishingOptions,
        IChatBufferDetector? chatDetector = null,
        Action<long>? setMemoryAddress = null)
    {
        _logger = logger;
        _mem = mem;
        _processAccessor = processAccessor;
        _chatDetector = chatDetector;
        _setMemoryAddress = setMemoryAddress;
        _opt = fishingOptions.Value.Legacy;
    }

    public async Task<int> WaitForBiteAsync(TimeSpan timeout, CancellationToken ct)
    {
        // Auto-detect on first call
        if (!_isDetected && _chatDetector is not null && _setMemoryAddress is not null)
        {
            _logger.LogInformation("🔍 First call - running auto-detection");
            
            if (_processAccessor.TryGetSelectedProcess(out var proc) && proc is not null)
            {
                var result = await _chatDetector.DetectAsync(proc.ProcessId, ct);
                
                if (result is not null)
                {
                    _setMemoryAddress((long)result.DigitAddress);
                    _isDetected = true;
                    
                    _logger.LogInformation(
                        "✅ Chat buffer detected: MessageStart=0x{Msg:X8}, Digit=0x{Digit:X8}",
                        result.MessageStartAddress,
                        result.DigitAddress);
                }
                else
                {
                    _logger.LogWarning("❌ Auto-detection failed - using manual address");
                }
            }
        }

        _logger.LogInformation("🔥 WaitForBiteAsync START");

        if (!_processAccessor.TryGetSelectedProcess(out var process) || process is null)
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
                _logger.LogWarning("🔥 Probe #{Count} at {Elapsed}ms", probeCount,
                    Environment.TickCount64 - startTime);
            }

            try
            {
                var fullMessage = await _mem.ReadStringAsync(process.ProcessId, messageStartAddr, 80, ct);

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

                        _logger.LogWarning("🔥 BITE! Matched: '{Phrase}' → Count: {Count}", phrase, spaceCount);
                        _logger.LogWarning("🔥 Full message: '{Text}'",
                            fullMessage.Substring(0, Math.Min(60, fullMessage.Length)));
                        
                        _logger.LogWarning("🔥 WaitForBiteAsync RETURNING {Count}", spaceCount);
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