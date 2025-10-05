// File: Ogur.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs
// Project: Ogur.Infrastructure
// Namespace: Ogur.Infrastructure.DependencyInjection
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions;
using Ogur.Abstractions.Input;
using Ogur.Abstractions.Windows;
using Ogur.Core.Input.Adapters;
using Ogur.Infrastructure.Configuration;
using Ogur.Infrastructure.Input;
using Ogur.Infrastructure.Signals;
using Ogur.Infrastructure.Windows;

namespace Ogur.Infrastructure.DependencyInjection;

/// <summary>
/// Aggregates DI registrations for the infrastructure layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services:
    /// <list type="bullet">
    /// <item><description><see cref="IKeyboardSynthesizer"/> → <see cref="EButtonKeyboardSynthesizer"/></description></item>
    /// <item><description><see cref="IInput"/> → <see cref="Win32Input"/></description></item>
    /// <item><description><see cref="IWindowActivator"/> → <see cref="WindowActivator"/></description></item>
    /// <item><description><see cref="IFishingSignalSource"/></description> selected by configuration (Null or Memory)</item>
    /// </list>
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddOgurInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Keyboard: STRICTLY EButton backend (as required)
        services.AddSingleton<IKeyboardSynthesizer, EButtonKeyboardSynthesizer>();

        // Mouse & generic input (scan-code mouse if needed)
        services.AddSingleton<IInput, Win32Input>();

        // Window activation: choose exactly ONE implementation.
        // We use the WindowActivator you posted (ShowWindow + SetForegroundWindow).
        services.AddSingleton<IWindowActivator, WindowActivator>();

        // ---- Fishing signal source (switchable) ----
        var mode = configuration.GetValue<string>("Fishing:Signals:Mode") ?? "Null";
        if (string.Equals(mode, "Memory", System.StringComparison.OrdinalIgnoreCase))
        {
            services.Configure<FishingMemorySignalOptions>(configuration.GetSection("Fishing:Signals:Memory"));
            services.AddSingleton<IFishingSignalSource, MemoryBiteSignalSource>();
        }
        else
        {
            services.AddSingleton<IFishingSignalSource, NullFishingSignalSource>();
        }

        // ---- Self-check log: which concrete types were bound ----
        services.AddSingleton(provider =>
        {
            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("InfraDiagnostics");
            var ks = provider.GetRequiredService<IKeyboardSynthesizer>();
            var wa = provider.GetRequiredService<IWindowActivator>();
            var inp = provider.GetRequiredService<IInput>();
            var sig = provider.GetRequiredService<IFishingSignalSource>();
            logger.LogInformation("DI: IKeyboardSynthesizer -> {Type}", ks.GetType().FullName);
            logger.LogInformation("DI: IWindowActivator   -> {Type}", wa.GetType().FullName);
            logger.LogInformation("DI: IInput             -> {Type}", inp.GetType().FullName);
            logger.LogInformation("DI: IFishingSignalSource -> {Type}", sig.GetType().FullName);
            return new object(); // sentinel; not used elsewhere
        });

        return services;
    }
}
