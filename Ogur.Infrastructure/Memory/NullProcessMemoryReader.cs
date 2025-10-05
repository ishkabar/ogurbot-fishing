using System;
using System.Threading;
using System.Threading.Tasks;
using Ogur.Abstractions.Memory;

namespace Ogur.Infrastructure.Memory
{
    /// <summary>
    /// No-op implementation of <see cref="IProcessMemoryReader"/> that returns empty results.
    /// </summary>
    public sealed class NullProcessMemoryReader : IProcessMemoryReader
    {
        /// <summary>
        /// Reads a null-terminated string from remote process memory.
        /// </summary>
        /// <param name="processId">Process id.</param>
        /// <param name="address">Base address.</param>
        /// <param name="maxLength">Max bytes to read.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Always returns an empty string.</returns>
        public Task<string> ReadStringAsync(int processId, nint address, int maxLength, CancellationToken ct)
            => Task.FromResult(string.Empty);

        /// <summary>
        /// Checks whether memory at the given address contains expected markers.
        /// </summary>
        /// <param name="processId">Process id.</param>
        /// <param name="address">Base address.</param>
        /// <param name="maxLength">Max bytes to read.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Always returns false.</returns>
        public Task<bool> ContainsAnyAsync(int processId, nint address, int maxLength, CancellationToken ct)
            => Task.FromResult(false);
    }
}