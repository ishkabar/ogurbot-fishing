// File: Ogur.Fishing.Host.Wpf/AppStartup.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ogur.Abstractions.Windows;
using Ogur.Fishing.Host.Wpf.Composition;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.ViewModels;
using Ogur.Fishing.Host.Wpf.Views;
using Ogur.Infrastructure.Windows;
using Ogur.Abstractions;
using Ogur.Infrastructure.Memory;
using Ogur.Abstractions.Memory;

namespace Ogur.Fishing.Host.Wpf
{
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
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IAppFlowCoordinator, AppFlowCoordinator>();
            builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            builder.Services.AddSingleton<IWindowActivator, WindowActivator>();
            builder.Services.AddSingleton<IInput, NullInput>();
            builder.Services.AddSingleton<IInput, NullInput>();
            builder.Services.AddSingleton<IProcessMemoryReader, NullProcessMemoryReader>();


            ServiceCollectionExtensions.AddWpfHost(builder.Services);
            ServiceCollectionExtensions.AddFishingCapabilityHost(builder.Services, builder.Configuration);

            builder.Services.AddSingleton<IAuthService, DummyAuthService>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<ServerSelectViewModel>();
            builder.Services.AddTransient<MainViewModel>();

            builder.Services.AddSingleton<ShellWindow>();
            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<ServerSelectView>();
            builder.Services.AddTransient<MainView>();

            builder.Services.AddHostedService<UiHostedService>();
        }
    }
}
