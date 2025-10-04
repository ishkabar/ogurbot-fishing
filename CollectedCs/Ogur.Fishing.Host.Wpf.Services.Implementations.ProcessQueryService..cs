// File: Services/Implementations/ProcessQueryService.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Services.Implementations

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Services.Models;

namespace Ogur.Fishing.Host.Wpf.Services.Implementations;

/// <summary>
/// Process query that filters Metin2-like processes, captures window geometry, and orders by recency.
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

    private static readonly string[] Names =
    {
        "metin2",
        "metin2client",
        "mt2009",
        "mt2",
        "gameclient",
        "game mmorpg",
        "proxima",
        "tamidia",
        "tamidia2",
        "proxima_launcher",
        "patcher",
        "mt2009 patcher",
        "client",
        "ymir"
    };

    /// <inheritdoc />
    public Task<IReadOnlyList<ProcessOption>> GetCandidatesAsync(CancellationToken ct)
    {
        var list = new List<ProcessOption>();

        foreach (var p in Process.GetProcesses())
        {
            if (ct.IsCancellationRequested) break;

            var name = p.ProcessName?.ToLowerInvariant() ?? string.Empty;
            if (!Names.Any(n => name.Contains(n))) continue;

            DateTime? started = null;
            string? path = null;

            try { started = p.StartTime; } catch (Exception ex) { _logger.LogDebug(ex, "Cannot read StartTime for PID {Pid}", p.Id); }
            try { path = p.MainModule?.FileName; } catch (Exception ex) { _logger.LogDebug(ex, "Cannot read MainModule for PID {Pid}", p.Id); }

            int? width = null, height = null, x = null, y = null;
            try
            {
                var hWnd = p.MainWindowHandle;
                if (hWnd != IntPtr.Zero && GetWindowRect(hWnd, out var rect))
                {
                    width = rect.Right - rect.Left;
                    height = rect.Bottom - rect.Top;
                    x = rect.Left;
                    y = rect.Top;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Cannot get window geometry for PID {Pid}", p.Id);
            }

            var shortLabel = $"{p.ProcessName} (PID {p.Id})";
            var fullLabel = shortLabel
                            + (started is not null ? $" – started {started:yyyy-MM-dd HH:mm:ss}" : string.Empty)
                            + (width is not null && height is not null ? $" – {width}x{height} @ ({x},{y})" : string.Empty);

            list.Add(new ProcessOption
            {
                Pid = p.Id,
                Display = fullLabel,
                DisplayShort = shortLabel,
                StartedAt = started,
                Path = path,
                ResolutionWidth = width,
                ResolutionHeight = height,
                WindowX = x,
                WindowY = y
            });
        }

        var ordered = list
            .OrderByDescending(x => x.StartedAt ?? DateTime.MinValue)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<ProcessOption>>(ordered);
    }

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
