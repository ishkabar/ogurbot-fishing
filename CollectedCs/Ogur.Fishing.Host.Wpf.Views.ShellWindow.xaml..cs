using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Views;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows;
using Ogur.Fishing.Host.Wpf.Services;


namespace Ogur.Fishing.Host.Wpf.Views;


/// <summary>
/// Main shell window. Exposes a simple method to update the window title from outside.
/// This method is intentionally minimal to avoid introducing new coupling or breaking DI.
/// </summary>
public partial class ShellWindow : BaseWindow
{
    private readonly ISessionState _session;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellWindow"/> class.
    /// </summary>
    /// <param name="session">Session state (injected).</param>
    public ShellWindow(ISessionState session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        InitializeComponent();

        Title = "Ogur fishing";
    }

    /// <summary>
    /// Updates the Shell window title using provided username and server name.
    /// Use this method from the UI thread (or call via Application.Current.Dispatcher).
    /// </summary>
    /// <param name="username">Authenticated username (may be null/empty).</param>
    /// <param name="serverName">Selected server name (may be null/empty).</param>
    public void SetSessionTitle(string? username, string? serverName)
    {
        var u = string.IsNullOrWhiteSpace(username) ? string.Empty : username.Trim();
        var s = string.IsNullOrWhiteSpace(serverName) ? string.Empty : serverName.Trim();

        if (string.IsNullOrEmpty(u) && string.IsNullOrEmpty(s))
        {
            Title = "Ogur fishing";
            return;
        }

        if (!string.IsNullOrEmpty(u) && string.IsNullOrEmpty(s))
        {
            Title = $"Ogur fishing - {u}";
            return;
        }

        if (string.IsNullOrEmpty(u) && !string.IsNullOrEmpty(s))
        {
            Title = $"Ogur fishing ({s})";
            return;
        }

        Title = $"Ogur fishing - {u} ({s})";
    }
}