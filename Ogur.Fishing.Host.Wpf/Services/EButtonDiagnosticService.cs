// File: Ogur.Fishing.Host.Wpf/Services/EButtonDiagnosticService.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Services
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Input;

/// <summary>
/// One-shot diagnostic caller for IKeyboardSynthesizer (EButton) to verify that PressKey2Async actually runs.
/// </summary>
public sealed class EButtonDiagnosticService : IHostedService
{
    private readonly ILogger<EButtonDiagnosticService> _logger;
    private readonly IKeyboardSynthesizer _keys;

    public EButtonDiagnosticService(ILogger<EButtonDiagnosticService> logger, IKeyboardSynthesizer keys)
    {
        _logger = logger;
        _keys = keys;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // nie blokujemy startu — uruchamiamy test asynchronicznie
        _ = RunTestAsync(CancellationToken.None);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task RunTestAsync(CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("EButtonDiagnostic: start");

            // przykładowy scan-code (dopasuj do tego, którego używasz do przynęty)
            var baitSc = (ScanCode)0x04; // example: '3' if to jest mapping u Ciebie
            var spaceSc = ScanCode.Space;

            _logger.LogInformation("EButtonDiagnostic: pressing bait scan-code 0x{Sc:X}", (short)baitSc);
            await _keys.PressKey2Async(baitSc, ct).ConfigureAwait(false);
            _logger.LogInformation("EButtonDiagnostic: pressed bait");

            await Task.Delay(80, ct).ConfigureAwait(false);

            _logger.LogInformation("EButtonDiagnostic: pressing space scan-code 0x{Sc:X}", (short)spaceSc);
            await _keys.PressKey2Async(spaceSc, ct).ConfigureAwait(false);
            _logger.LogInformation("EButtonDiagnostic: pressed space");

            // powtórka z logowaniem dodatkowych informacji
            await Task.Delay(250, ct).ConfigureAwait(false);
            _logger.LogInformation("EButtonDiagnostic: finished");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EButtonDiagnostic: failed");
        }
    }
}
