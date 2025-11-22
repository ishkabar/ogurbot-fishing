// File: Ogur.Fishing.Host.Wpf/AppStartup.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Ogur.Abstractions;
using Ogur.Abstractions.Memory;
using Ogur.Abstractions.Windows;
using Ogur.Core.DependencyInjection;
using Ogur.Core.Hub;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.Configuration;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.Services.HubIntegration;
using Ogur.Fishing.Host.Wpf.ViewModels;
using Ogur.Fishing.Host.Wpf.Views;
using Ogur.Infrastructure.Input;
using Ogur.Infrastructure.Memory;
using Ogur.Infrastructure.configuration;
using Ogur.Infrastructure.Windows;
using Ogur.Fishing.Host.Wpf.Composition;


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
        var configuration = builder.Configuration;

        // Configuration Options
        builder.Services.Configure<UiOptions>(configuration.GetSection("Ui"));

        // Ogur.Core - podstawowe serwisy
        builder.Services.AddOgurCore(configuration);
        
        // Ogur.Hub Integration z Ogur.Core
        builder.Services.AddOgurHub(configuration);

        // Override Hub options z hardcoded constants
        builder.Services.PostConfigure<HubOptions>(options =>
        {
            options.ApiKey = HubConstants.ApiKey;
            options.ApplicationName = HubConstants.ApplicationName;
            options.ApplicationVersion = HubConstants.ApplicationVersion;
        });

        // Core Infrastructure
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddSingleton<IWindowActivator, WindowActivator>();
        builder.Services.AddSingleton<IInput, Win32Input>();
        //builder.Services.AddSingleton<IProcessMemoryReader, NullProcessMemoryReader>();
        builder.Services.AddSingleton<IProcessMemoryReader, Win32ProcessMemoryReader>();


        // Navigation & Flow
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IAppFlowCoordinator, AppFlowCoordinator>();

        // WPF Host (ViewModels, Views, Services)
        builder.Services.AddWpfHost();

        // Fishing Capabilities
        builder.Services.AddFishingCapabilityHost(configuration);

        // Authentication
        builder.Services.AddSingleton<IAuthService, DummyAuthService>();

        // Custom Hub Integration Services
        builder.Services.AddSingleton<ILicenseManager, LicenseManager>();
        builder.Services.AddSingleton<HubCommandHandler>();
        builder.Services.AddHostedService<HubIntegrationService>();
        
        builder.Services.AddTransient<UpdateRequiredView>();
        builder.Services.AddTransient<UpdateRequiredViewModel>();

        // Background Services
        builder.Services.AddHostedService<UiHostedService>();
    }
}