// File: Ogur.Capabilities.Fishing/Adapters/FishingClickAdapter.cs
// Project: Ogur.Capabilities.Fishing
// Namespace: Ogur.Capabilities.Fishing.Adapters
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Input;

namespace Ogur.Capabilities.Fishing.Adapters
{
    /// <summary>
    /// Translates high-level input keys into scan-code presses using IKeyboardSynthesizer.
    /// </summary>
    public sealed class FishingClickAdapter
    {
        private readonly ILogger<FishingClickAdapter> _logger;
        private readonly IKeyboardSynthesizer _keys;

        /// <summary>
        /// Initializes a new instance of the <see cref="FishingClickAdapter"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="keys">Keyboard synthesizer.</param>
        public FishingClickAdapter(ILogger<FishingClickAdapter> logger, IKeyboardSynthesizer keys)
        {
            _logger = logger;
            _keys = keys;
        }

        /// <summary>
        /// Presses a logical key using legacy-compatible scan-code path.
        /// </summary>
        /// <param name="key">Logical key.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task PressAsync(InputKey key, CancellationToken ct)
        {
            var sc = InputKeyMapper.ToScanCode(key);
            await _keys.PressKey2Async(sc, ct).ConfigureAwait(false);
            _logger.LogDebug("Pressed {Key} as scan-code {ScanCode}", key, (short)sc);
        }
    }
}