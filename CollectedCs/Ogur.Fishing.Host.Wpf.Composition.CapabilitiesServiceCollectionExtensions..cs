using Microsoft.Extensions.DependencyInjection;
using Ogur.Capabilities.Fishing;

namespace Ogur.Fishing.Host.Wpf.Composition;


/// <summary>
/// DI registrations for Ogur capabilities used by the WPF host.
/// </summary>
public static class CapabilitiesServiceCollectionExtensions
{
    /// <summary>
    /// Registers Fishing capability and its dependencies (resolved via DI).
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same collection.</returns>
    public static IServiceCollection AddFishingCapabilityHost(this IServiceCollection services)
    {
        // FishingCapability will get its own ctor deps via ActivatorUtilities/DI
        services.AddSingleton<FishingCapability>();
        return services;
    }
}