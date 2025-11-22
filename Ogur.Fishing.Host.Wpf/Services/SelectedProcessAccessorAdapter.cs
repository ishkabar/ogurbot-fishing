// File: Ogur.Fishing.Host.Wpf/Services/SelectedProcessAccessorAdapter.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Services

using System.Diagnostics.CodeAnalysis;
using Ogur.Abstractions;

namespace Ogur.Fishing.Host.Wpf.Services;

/// <summary>
/// Adapter that bridges ISessionState to ISelectedProcessAccessor.
/// </summary>
public sealed class SelectedProcessAccessorAdapter : ISelectedProcessAccessor
{
    private readonly ISessionState _session;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectedProcessAccessorAdapter"/> class.
    /// </summary>
    /// <param name="session">Session state instance.</param>
    public SelectedProcessAccessorAdapter(ISessionState session)
    {
        _session = session;
    }

    /// <summary>
    /// Tries to get the currently selected process information.
    /// </summary>
    /// <param name="info">Selected process info when available.</param>
    /// <returns>True if a process is selected; otherwise false.</returns>
    public bool TryGetSelectedProcess([NotNullWhen(true)] out SelectedProcessInfo? info)
    {
        var proc = _session.SelectedProcess;
        
        if (proc is null || proc.Hwnd == 0)
        {
            info = null;
            return false;
        }

        info = new SelectedProcessInfo
        {
            ProcessId = proc.Pid,
            Hwnd = proc.Hwnd,  // ✅ ZMIENIONE Z WindowHandle
            Title = proc.Display
        };

        return true;
    }

    /// <summary>
    /// Gets the memory address for bite detection from session state.
    /// </summary>
    public long MemoryAddress => _session.MemoryAddress;
}