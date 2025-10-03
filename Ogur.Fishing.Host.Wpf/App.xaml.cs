using System.Configuration;
using System.Data;
using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ogur.Host.Wpf.Navigation;
using ogur.Host.Wpf.ViewModels;
using ogur.Host.Wpf.Views;
using ogur.Capabilities.Fishing;
using ogur.Infrastructure.Input;
using ogur.Infrastructure.Screen;
using ogur.Infrastructure.Ocr;

namespace Ogur.Fishing.Host.Wpf;


/// <summary>
/// WPF application bootstrapper with HostBuilder and DI.
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    /// <summary>
    /// Application startup override.
    /// </summary>
    /// <param name="e">Args.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(cfg =>
            {
                cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddNLog();
            })
            .ConfigureServices((ctx, services) =>
            {
                services.Configure<FishingOptions>(ctx.Configuration.GetSection("Fishing"));

                services.AddSingleton<IInput, Win32Input>();
                services.AddSingleton<IScreenCapture, DxgiScreenCapture>();
                services.AddSingleton<IOcr, TesseractOcr>();

                services.AddSingleton<FishingCapability>();
                services.AddSingleton<FishingPlugin>();

                services.AddSingleton<INavigationService, NavigationService>();

                services.AddSingleton<LoginViewModel>();
                services.AddSingleton<ServerSelectViewModel>();
                services.AddSingleton<MainViewModel>();

                services.AddTransient<LoginView>();
                services.AddTransient<ServerSelectView>();
                services.AddTransient<MainView>();
                services.AddSingleton<ShellWindow>();

                services.AddHostedService<UiHostedService>();
            })
            .Build();

        _host.Start();
        base.OnStartup(e);
    }

    /// <summary>
    /// Application exit override.
    /// </summary>
    /// <param name="e">Args.</param>
    protected override void OnExit(ExitEventArgs e)
    {
        _host?.Dispose();
        base.OnExit(e);
    }
}