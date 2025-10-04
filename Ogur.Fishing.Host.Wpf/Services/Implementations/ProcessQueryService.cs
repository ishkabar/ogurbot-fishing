using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Services.Models;

namespace Ogur.Fishing.Host.Wpf.Services.Implementations;

/// <summary>
/// Process query that filters Metin2-like processes and orders by recency.
/// </summary>
public sealed class ProcessQueryService : IProcessQueryService
{
    private readonly ILogger<ProcessQueryService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessQueryService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    public ProcessQueryService(ILogger<ProcessQueryService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<ProcessOption>> GetCandidatesAsync(CancellationToken ct)
    {
        var names = new[] { "metin2client", "metin2client.exe", "metin2", "client" };

        var list = new List<ProcessOption>();
        foreach (var p in Process.GetProcesses())
        {
            if (ct.IsCancellationRequested) break;

            var name = p.ProcessName?.ToLowerInvariant() ?? string.Empty;
            if (!names.Any(n => name.Contains(n))) continue;

            DateTime? started = null;
            string? path = null;
            try
            {
                started = p.StartTime;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Cannot read StartTime for PID {Pid}", p.Id);
            }

            try
            {
                path = p.MainModule?.FileName;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Cannot read MainModule for PID {Pid}", p.Id);
            }

            var label = $"{p.ProcessName} (PID {p.Id})"
                        + (started is not null ? $" – started {started:yyyy-MM-dd HH:mm:ss}" : string.Empty);

            list.Add(new ProcessOption
            {
                Pid = p.Id,
                Display = label,
                StartedAt = started,
                Path = path
            });
        }

        var ordered = list
            .OrderByDescending(x => x.StartedAt ?? DateTime.MinValue)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<ProcessOption>>(ordered);
    }
}