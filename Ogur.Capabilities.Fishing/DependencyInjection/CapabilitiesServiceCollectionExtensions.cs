// File: Ogur.Capabilities.Fishing/DependencyInjection/CapabilitiesServiceCollectionExtensions.cs
// Project: Ogur.Capabilities.Fishing
// Namespace: Ogur.Capabilities.Fishing.DependencyInjection
using Microsoft.Extensions.DependencyInjection;
using Ogur.Capabilities.Fishing.Adapters;
using Ogur.Infrastructure.Signals;


namespace Ogur.Capabilities.Fishing.DependencyInjection
{
    /// <summary>
    /// Aggregates DI registrations for Fishing capability.
    /// </summary>
    /// <summary>
    /// Aggregates DI registrations for Fishing capability.
    /// </summary>
    public static class CapabilitiesServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Fishing capability services (UI-agnostic).
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>The same service collection instance.</returns>
        public static IServiceCollection AddFishingCapability(this IServiceCollection services)
        {
            services.AddSingleton<FishingClickAdapter>();
            services.AddSingleton<FishingCapability>();

            // Rejestruj bez fabryki – prosto i pewnie:
            services.AddSingleton<IBotCapability, FishingCapability>();

            // Plugin, jeśli host enumeruje IBotPluginV1
            services.AddSingleton<IBotPluginV1, FishingPlugin>();

            // IFishingSignalSource rejestruj w Infrastructure (Null/Memory/etc.) – nie tutaj.
            return services;
        }
    }
}