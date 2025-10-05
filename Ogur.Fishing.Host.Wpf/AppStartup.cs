using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions;
using Ogur.Abstractions.Input;
using Ogur.Abstractions.Windows;
using Ogur.Capabilities.Fishing.DependencyInjection;
using Ogur.Fishing.Host.Wpf.Adapters;
using Ogur.Fishing.Host.Wpf.Configuration;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.Services.Implementations;
using Ogur.Fishing.Host.Wpf.ViewModels;
using Ogur.Fishing.Host.Wpf.Views;
using Ogur.Infrastructure.DependencyInjection;

namespace Ogur.Fishing.Host.Wpf;

/// <summary>
/// Configures application services and startup pipeline.
/// </summary>
public static class AppStartup
{
    /// <summary>
    /// Registers services for the WPF host application.
    /// </summary>
    /// <param name="builder">Host application builder.</param>
    public static void Configure(HostApplicationBuilder builder)
    {
        IConfiguration configuration = builder.Configuration;

        builder.Services.Configure<UiOptions>(configuration.GetSection("Ui"));
        builder.Services.Configure<FishingRuntimeOptions>(configuration.GetSection("Fishing:Host"));

        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IAppFlowCoordinator, AppFlowCoordinator>();
        builder.Services.AddSingleton<IAuthService, DummyAuthService>();
        builder.Services.AddSingleton<IBaitCatalog, BaitCatalog>();
        builder.Services.AddSingleton<IProcessQueryService, ProcessQueryService>();
        builder.Services.AddSingleton<ISelectedProcessAccessor, SelectedProcessAccessorAdapter>();
        builder.Services.AddSingleton<IInput, NullInput>();
        builder.Services.AddSingleton<IHotkeyListener, HotkeyListener>();

        builder.Services.AddSingleton<ISessionState, SessionState>();

        // === Run gate: musi być zarejestrowany, żeby nie było null w MainViewModel ===
        builder.Services.AddSingleton<IFishingRunGate, FishingRunGate>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ServerSelectViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddSingleton<ShellWindow>();
        builder.Services.AddTransient<LoginView>();
        builder.Services.AddTransient<ServerSelectView>();
        builder.Services.AddTransient<MainView>();

        // Infrastructure (EButton keyboard, window activation, signals via config)
        builder.Services.AddOgurInfrastructure(configuration);

        // Capability: Fishing
        builder.Services.AddFishingCapability();

        // Hosted services
        builder.Services.AddHostedService<UiHostedService>();

        // ⚠️ U Ciebie były DWA executory – powodowało to pętlę. Zostawiamy dokładnie JEDEN.
        builder.Services.AddHostedService<FishingActionExecutor>();

        // DEV: diagnostyka EButton – opcjonalnie zostaw, jeśli chcesz test na starcie
        builder.Services.AddSingleton<IHostedService, EButtonDiagnosticService>();

        // Diagnostyka bindowania gate'a (teraz zadziała, bo IFishingRunGate jest już w DI)
        builder.Services.AddSingleton(provider =>
        {
            var log = provider.GetRequiredService<ILoggerFactory>().CreateLogger("RunGateDiag");
            var gate = provider.GetRequiredService<IFishingRunGate>();
            log.LogInformation("RunGate bound to {Type}", gate.GetType().FullName);
            return new object();
        });
    }
}
