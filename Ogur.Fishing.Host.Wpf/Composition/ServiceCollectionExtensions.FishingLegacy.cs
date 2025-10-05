using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Ogur.Abstractions;
using Ogur.Capabilities.Fishing;
using Ogur.Fishing.Host.Wpf.Services;

namespace Ogur.Host.Wpf.Composition
{
    /// <summary>
    /// Registers the modern fishing capability (FSM + OCR + SendInput) and related services.
    /// </summary>
    public static class ServiceCollectionExtensions_Fishing
    {
        /// <summary>
        /// Adds the fishing capability and its background executor to the DI container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="cfg">Application configuration.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
        public static IServiceCollection AddFishingCapabilityHost(this IServiceCollection services, IConfiguration cfg)
        {
            services.Configure<FishingOptions>(cfg.GetSection("Fishing"));

            services.AddSingleton<FishingCapability>();
            services.AddSingleton<IBotCapability>(sp => sp.GetRequiredService<FishingCapability>());

            services.AddHostedService<FishingActionExecutor>();

            return services;
        }
    }
}