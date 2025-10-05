using Ogur.Abstractions;
using System;

namespace Ogur.Fishing.Host.Wpf.Services
{
    /// <summary>
    /// Adapts host session state to a cross-layer selected process accessor.
    /// </summary>
    public sealed class SelectedProcessAccessorAdapter : ISelectedProcessAccessor
    {
        private readonly ISessionState _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedProcessAccessorAdapter"/> class.
        /// </summary>
        /// <param name="session">Host session state.</param>
        public SelectedProcessAccessorAdapter(ISessionState session)
        {
            _session = session;
        }

        /// <summary>
        /// Tries to get the currently selected process.
        /// </summary>
        /// <param name="info">Selected process info when available.</param>
        /// <returns>True if available; otherwise false.</returns>
        public bool TryGetSelectedProcess(out SelectedProcessInfo? info)
        {
            var p = _session.SelectedProcess;
            if (p is null)
            {
                info = null;
                return false;
            }

            var hwnd = 0;
            var hwndProp = p.GetType().GetProperty("Hwnd")
                         ?? p.GetType().GetProperty("Handle")
                         ?? p.GetType().GetProperty("MainWindowHandle");

            if (hwndProp is not null)
            {
                var value = hwndProp.GetValue(p);
                if (value is IntPtr ip)
                    hwnd = unchecked(ip.ToInt32());
                else if (value is nint nip)
                    hwnd = unchecked((int)nip);
                else if (value is long l)
                    hwnd = unchecked((int)l);
                else if (value is int i)
                    hwnd = i;
            }

            var pid = 0;
            var pidProp = p.GetType().GetProperty("ProcessId") ?? p.GetType().GetProperty("Id");
            if (pidProp is not null && pidProp.GetValue(p) is int id)
            {
                pid = id;
            }

            var title = p.GetType().GetProperty("WindowTitle")?.GetValue(p) as string
                        ?? p.GetType().GetProperty("Title")?.GetValue(p) as string
                        ?? p.GetType().GetProperty("MainWindowTitle")?.GetValue(p) as string;

            info = new SelectedProcessInfo
            {
                ProcessId = pid,
                Hwnd = hwnd,
                Title = string.IsNullOrWhiteSpace(title) ? null : title
            };
            return pid > 0;
        }
    }
}
