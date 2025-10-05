using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ogur.Fishing.Host.Wpf.Views;

namespace Ogur.Fishing.Host.Wpf
{
    /// <summary>
    /// WPF application entry point that wires up the Host and shows the shell.
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        /// <summary>
        /// Builds the generic host with DI, logging and application services.
        /// </summary>
        /// <returns>Configured host instance.</returns>
        private static IHost BuildHost()
        {
            var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
            AppStartup.Configure(builder);
            return builder.Build();
        }

        /// <summary>
        /// Handles application startup, creates the host, shows ShellWindow first, then starts background services.
        /// </summary>
        /// <param name="e">Startup event args.</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = BuildHost();

            var shell = _host.Services.GetRequiredService<ShellWindow>();
            MainWindow = shell;
            shell.Show();

            await _host.StartAsync();
        }

        /// <summary>
        /// Handles application exit by stopping and disposing the host.
        /// </summary>
        /// <param name="e">Exit event args.</param>
        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host is not null)
            {
                try
                {
                    await _host.StopAsync(TimeSpan.FromSeconds(5));
                }
                finally
                {
                    _host.Dispose();
                }
            }

            base.OnExit(e);
        }
    }
}
