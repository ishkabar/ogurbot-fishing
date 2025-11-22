// File: Ogur.Infrastructure/Memory/Win32ProcessMemoryReader.cs
// Project: Ogur.Infrastructure
// Namespace: Ogur.Infrastructure.Memory

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Memory;

namespace Ogur.Infrastructure.Memory;

/// <summary>
/// Win32 implementation of process memory reader using ReadProcessMemory API.
/// </summary>
public sealed class Win32ProcessMemoryReader : IProcessMemoryReader
{
    private readonly ILogger<Win32ProcessMemoryReader> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Win32ProcessMemoryReader"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    public Win32ProcessMemoryReader(ILogger<Win32ProcessMemoryReader> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Reads a string from remote process memory.
    /// </summary>
    /// <param name="processId">Process ID.</param>
    /// <param name="address">Memory address to read from.</param>
    /// <param name="maxLength">Maximum bytes to read.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>String read from memory, or empty string on failure.</returns>
    public Task<string> ReadStringAsync(int processId, nint address, int maxLength, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            try
            {
                var handle = OpenProcess(ProcessAccessFlags.VmRead, false, processId);
                if (handle == IntPtr.Zero)
                {
                    _logger.LogDebug("Failed to open process {Pid} for reading", processId);
                    return string.Empty;
                }

                try
                {
                    var buffer = new byte[maxLength];
                    bool success = ReadProcessMemory(handle, address, buffer, maxLength, out _);

                    if (!success)
                    {
                        _logger.LogTrace("ReadProcessMemory failed for PID={Pid}, Addr=0x{Addr:X}", processId, address);
                        return string.Empty;
                    }

                    // Find null terminator
                    int nullIndex = Array.IndexOf(buffer, (byte)0);
                    int length = nullIndex >= 0 ? nullIndex : maxLength;

                    // Decode as UTF-8
                    return Encoding.UTF8.GetString(buffer, 0, length);
                }
                finally
                {
                    CloseHandle(handle);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read memory from process {Pid}", processId);
                return string.Empty;
            }
        }, ct);
    }

    /// <summary>
    /// Checks if memory at address contains any non-zero bytes.
    /// </summary>
    /// <param name="processId">Process ID.</param>
    /// <param name="address">Memory address to check.</param>
    /// <param name="maxLength">Maximum bytes to check.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if any non-zero byte found, false otherwise.</returns>
    public Task<bool> ContainsAnyAsync(int processId, nint address, int maxLength, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            try
            {
                var handle = OpenProcess(ProcessAccessFlags.VmRead, false, processId);
                if (handle == IntPtr.Zero)
                {
                    return false;
                }

                try
                {
                    var buffer = new byte[maxLength];
                    bool success = ReadProcessMemory(handle, address, buffer, maxLength, out _);

                    if (!success)
                    {
                        return false;
                    }

                    // Check if any non-zero byte exists
                    return buffer.Any(b => b != 0);
                }
                finally
                {
                    CloseHandle(handle);
                }
            }
            catch
            {
                return false;
            }
        }, ct);
    }

    #region Win32 API

    [Flags]
    private enum ProcessAccessFlags : uint
    {
        VmRead = 0x0010
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    #endregion
}