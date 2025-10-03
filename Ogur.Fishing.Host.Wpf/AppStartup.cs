using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.ViewModels;
using Ogur.Fishing.Host.Wpf.Views;
using Ogur.Fishing.Host.Wpf.Composition;
using System.Windows;


using HostComposition = Ogur.Fishing.Host.Wpf.Composition.ServiceCollectionExtensions;
using CapabilitiesComposition = Ogur.Fishing.Host.Wpf.Composition.CapabilitiesServiceCollectionExtensions;
using NullProxy = Ogur.Fishing.Host.Wpf.Composition.NullProxyServiceCollectionExtensions;



namespace Ogur.Fishing.Host.Wpf;
/// <summary>
/// WPF application entry helpers.
/// </summary>
public static class AppStartup
{
    /// <summary>
    /// Builds the host with DI registrations.
    /// </summary>
    /// <returns>Host.</returns>
    public static IHost BuildHost()
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices(s =>
            {
                s.AddLogging();

                // Jawne wywołania – zero dwuznaczności:
                HostComposition.AddWpfHost(s);
                CapabilitiesComposition.AddFishingCapabilityHost(s);

                // Stuby infra (na razie puste implementacje)
                NullProxy.AddNullProxy<ogur.abstractions.IInput>(s);
                NullProxy.AddNullProxy<ogur.abstractions.IScreenCapture>(s);
                NullProxy.AddNullProxy<ogur.abstractions.IOcr>(s);

                // Nawigacja + shell
                s.AddSingleton<INavigationService, NavigationService>();
                s.AddSingleton<ShellWindow>();
            })
            .Build();
    }

    /// <summary>
    /// Shows the shell window. Do not call app.Run() here.
    /// </summary>
    /// <param name="app">Application.</param>
    /// <param name="host">Host.</param>
    public static void Run(Application app, IHost host)
    {
        var shell = host.Services.GetRequiredService<ShellWindow>();
        app.MainWindow = shell;
        shell.Show();
    }
}