using Ogur.Fishing.Host.Wpf.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ogur.Fishing.Host.Wpf.Composition;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.ViewModels;


namespace Ogur.Fishing.Host.Wpf;


/// <summary>
/// Configures application services and startup pipeline.
/// </summary>
public static class AppStartup
{
    /// <summary>
    /// Registers services for the WPF host.
    /// </summary>
    /// <param name="builder">Host application builder.</param>
    public static void Configure(HostApplicationBuilder builder)
    {
        // Core app services
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IAppFlowCoordinator, AppFlowCoordinator>();
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        ServiceCollectionExtensions.AddWpfHost(builder.Services);
        ServiceCollectionExtensions.AddFishingCapabilityHost(builder.Services);

        // Domain services (auth stub)
        builder.Services.AddSingleton<IAuthService, DummyAuthService>();


        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ServerSelectViewModel>();
        builder.Services.AddTransient<MainViewModel>();

        // Views
        builder.Services.AddSingleton<ShellWindow>();
        builder.Services.AddTransient<LoginView>();
        builder.Services.AddTransient<ServerSelectView>();
        builder.Services.AddTransient<MainView>();

        // Hosted UI bootstrap
        builder.Services.AddHostedService<UiHostedService>();
    }
}