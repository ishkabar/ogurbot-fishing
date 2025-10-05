using System;
using System.Threading;
using System.Threading.Tasks;
using Ogur.Abstractions;

namespace Ogur.Infrastructure.Signals
{
    /// <summary>
    /// Minimal fishing signal source that never reports a bite.
    /// </summary>
    public sealed class NullFishingSignalSource : IFishingSignalSource
    {
        /// <summary>
        /// Immediately completes indicating no bite, so the FSM loops without delay.
        /// </summary>
        /// <param name="timeout">Maximum time to wait for a bite.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>False, indicating no bite was detected.</returns>
        public Task<bool> WaitForBiteAsync(TimeSpan timeout, CancellationToken ct)
            => Task.FromResult(false);

        public async Task<bool> xWaitForBiteAsync(TimeSpan timeout, CancellationToken ct)
        {
            await Task.Delay(timeout, ct).ConfigureAwait(false);
            return false;
        }
    }
}