using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ogur.Abstractions;
using Ogur.Abstractions.Memory;

namespace Ogur.Infrastructure.Signals
{
    /// <summary>
    /// Memory-probing bite signal source. Polls target process memory for known markers.
    /// </summary>
    public sealed class MemoryBiteSignalSource : IFishingSignalSource
    {
        private readonly ILogger<MemoryBiteSignalSource> _logger;
        private readonly IProcessMemoryReader _mem;
        private readonly ISelectedProcessAccessor _processAccessor;
        private readonly FishingMemorySignalOptions _opt;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBiteSignalSource"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="mem">Process memory reader.</param>
        /// <param name="processAccessor">Selected process accessor.</param>
        /// <param name="opt">Options binding.</param>
        public MemoryBiteSignalSource(
            ILogger<MemoryBiteSignalSource> logger,
            IProcessMemoryReader mem,
            ISelectedProcessAccessor processAccessor,
            IOptions<FishingMemorySignalOptions> opt)
        {
            _logger = logger;
            _mem = mem;
            _processAccessor = processAccessor;
            _opt = opt.Value;
        }

        /// <summary>
        /// Waits for the bite signal by polling configured memory region for the selected process.
        /// </summary>
        /// <param name="timeout">Max wait time.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True when a bite marker is detected; otherwise false.</returns>
        public async Task<bool> WaitForBiteAsync(TimeSpan timeout, CancellationToken ct)
        {
            if (!_processAccessor.TryGetSelectedProcess(out var proc) || proc is null)
            {
                _logger.LogDebug("No selected process; cannot probe memory.");
                await Task.Delay(timeout, ct);
                return false;
            }

            if (proc.ProcessId <= 0)
            {
                _logger.LogWarning("Invalid process id for memory probing.");
                await Task.Delay(timeout, ct);
                return false;
            }

            var baseAddr = (nint)_opt.ChatMessageAddress;
            var t0 = Environment.TickCount64;
            const int pollMs = 50;

            while (Environment.TickCount64 - t0 < timeout.TotalMilliseconds && !ct.IsCancellationRequested)
            {
                try
                {
                    var hasAny = await _mem.ContainsAnyAsync(proc.ProcessId, baseAddr, _opt.ChatReadLength, ct).ConfigureAwait(false);
                    if (hasAny)
                    {
                        if (_opt.KnownKeys is { Length: > 0 })
                        {
                            var text = await _mem.ReadStringAsync(proc.ProcessId, baseAddr, _opt.ChatReadLength, ct).ConfigureAwait(false);
                            if (!string.IsNullOrEmpty(text) &&
                                _opt.KnownKeys.Any(marker => text.Contains(marker, StringComparison.Ordinal)))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Memory probe failed; retrying.");
                }

                await Task.Delay(pollMs, ct).ConfigureAwait(false);
            }

            return false;
        }
    }
}
