// File: Ogur.Fishing.Host.Wpf/Composition/ServiceCollectionExtensions.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Composition

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Abstractions;
using Ogur.Abstractions.Metin;
using Ogur.Abstractions.Events;
using Ogur.Infrastructure.Events;  
using Ogur.Capabilities.Fishing;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.Services.Implementations;
using Ogur.Fishing.Host.Wpf.ViewModels;
using Ogur.Fishing.Host.Wpf.Views;
using Ogur.Infrastructure.Signals;
using Ogur.Infrastructure.configuration;

namespace Ogur.Fishing.Host.Wpf.Composition;

/// <summary>
/// ServiceCollection extensions for WPF host composition.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds WPF host services, view-models and views to DI.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same collection.</returns>
    public static IServiceCollection AddWpfHost(this IServiceCollection services)
    {
        services.AddSingleton<IBaitCatalog, BaitCatalog>();
        services.AddSingleton<IProcessQueryService, ProcessQueryService>();
        services.AddSingleton<ISessionState, SessionState>();

        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<ServerSelectViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddTransient<LoginView>();
        services.AddTransient<ServerSelectView>();
        services.AddTransient<MainView>();

        services.AddSingleton<ShellWindow>();
        services.AddSingleton<MainWindow>();

        return services;
    }

    /// <summary>
    /// Adds Fishing capability orchestration and its dependencies.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="cfg">Application configuration.</param>
    /// <returns>The same collection.</returns>
    public static IServiceCollection AddFishingCapabilityHost(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        services.Configure<FishingOptions>(cfg.GetSection("Fishing"));

        services.AddSingleton<IEventBus, InMemoryEventBus>();
        services.AddSingleton<ISelectedProcessAccessor, SelectedProcessAccessorAdapter>();
        services.AddSingleton<IFishingSignalSource, MemoryBiteSignalSource>();  


        services.AddSingleton<FishingCapability>();
        services.AddSingleton<IApplicationCapability>(sp => sp.GetRequiredService<FishingCapability>());

        services.AddHostedService<FishingActionExecutor>();

        return services;
    }

    /// <summary>
    /// Registers a no-op proxy implementation for the specified interface type.
    /// Useful as a temporary stub for infrastructure services during development.
    /// </summary>
    /// <typeparam name="TInterface">Interface to proxy.</typeparam>
    /// <param name="services">Service collection.</param>
    /// <returns>The same collection.</returns>
    public static IServiceCollection AddNullProxy<TInterface>(this IServiceCollection services)
        where TInterface : class
    {
        if (!typeof(TInterface).IsInterface)
        {
            throw new InvalidOperationException($"{typeof(TInterface).Name} must be an interface.");
        }

        services.AddSingleton<TInterface>(_ => DispatchProxy.Create<TInterface, NullDispatchProxy>());
        return services;
    }

    /// <summary>
    /// No-op placeholder for legacy RegisterHost() calls. Can be removed when no longer referenced.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same collection.</returns>
    public static IServiceCollection RegisterHost(this IServiceCollection services) => services;
}

/// <summary>
/// DispatchProxy that returns default values for all interface method calls.
/// </summary>
internal sealed class NullDispatchProxy : DispatchProxy
{
    /// <summary>
    /// Intercepts interface calls and returns default values or completed tasks.
    /// </summary>
    /// <param name="targetMethod">Invoked method info.</param>
    /// <param name="args">Invocation arguments.</param>
    /// <returns>Default value for the return type or a completed task.</returns>
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod is null)
        {
            return null;
        }

        var returnType = targetMethod.ReturnType;

        if (returnType == typeof(void))
        {
            return null;
        }

        if (returnType == typeof(Task))
        {
            return Task.CompletedTask;
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var innerType = returnType.GenericTypeArguments[0];
            var defaultValue = GetDefault(innerType);
            var fromResult = typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(innerType);
            return fromResult.Invoke(null, new[] { defaultValue });
        }

        if (returnType == typeof(ValueTask))
        {
            return default(ValueTask);
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            var innerType = returnType.GenericTypeArguments[0];
            var defaultValue = GetDefault(innerType);
            return Activator.CreateInstance(returnType, defaultValue);
        }

        return GetDefault(returnType);
    }

    private static object? GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}