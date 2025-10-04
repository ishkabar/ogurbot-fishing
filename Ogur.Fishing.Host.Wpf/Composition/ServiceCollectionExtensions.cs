using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.Services.Implementations;
using Ogur.Fishing.Host.Wpf.Services.Models;
using Ogur.Fishing.Host.Wpf.ViewModels;
using Ogur.Fishing.Host.Wpf.Views;
using Ogur.Capabilities.Fishing;


namespace Ogur.Fishing.Host.Wpf.Composition;
/// <summary>
/// ServiceCollection extensions for WPF host composition and test stubs.
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
        // Domain/UI services used by MainViewModel
        services.AddSingleton<IBaitCatalog, BaitCatalog>();
        services.AddSingleton<IProcessQueryService, ProcessQueryService>();
        services.AddSingleton<IHotkeyListener, HotkeyListener>();

        // ViewModels (singleton – trzymają stan sesji)
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<ServerSelectViewModel>();
        services.AddSingleton<MainViewModel>(); // ← Twój rozbudowany VM zostaje

        // Views (transient – można odświeżać bez utraty stanu VM)
        services.AddTransient<LoginView>();
        services.AddTransient<ServerSelectView>();
        services.AddTransient<MainView>();
        services.AddSingleton<MainWindow>();

        return services;
    }

    /// <summary>
    /// Adds Fishing capability orchestration and its dependencies.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same collection.</returns>
    public static IServiceCollection AddFishingCapabilityHost(this IServiceCollection services)
    {
        // MVP: capability jako singleton. Jeśli docelowo będzie loader pluginów, to tu go podłączymy.
        services.AddSingleton<FishingCapability>();
        return services;
    }

    /// <summary>
    /// Registers a no-op proxy implementation for the specified interface type <typeparamref name="T"/>.
    /// Useful as a temporary stub for infrastructure services (input, screen, ocr).
    /// </summary>
    /// <typeparam name="T">Interface to proxy.</typeparam>
    /// <param name="services">Service collection.</param>
    /// <returns>The same collection.</returns>
    /// <exception cref="InvalidOperationException">Thrown when T is not an interface.</exception>
    public static IServiceCollection AddNullProxy<T>(this IServiceCollection services)
        where T : class
    {
        if (!typeof(T).IsInterface)
            throw new InvalidOperationException($"{typeof(T).Name} must be an interface to use AddNullProxy.");

        services.AddSingleton(_ => NullInterfaceProxy<T>.Create());
        return services;
    }

    /// <summary>
    /// DispatchProxy-based no-op proxy for interfaces.
    /// </summary>
    /// <typeparam name="T">Interface being proxied.</typeparam>
    private sealed class NullInterfaceProxy<T> : DispatchProxy where T : class
    {
        /// <summary>
        /// Creates a new proxy instance implementing <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Proxy instance.</returns>
        public static T Create() => (T)Create<T, NullInterfaceProxy<T>>();

        /// <summary>
        /// Intercepts all method calls and returns default values or completed tasks.
        /// </summary>
        /// <param name="targetMethod">Invoked method.</param>
        /// <param name="args">Arguments.</param>
        /// <returns>Default value compatible with method signature.</returns>
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod is null) return null;

            var returnType = targetMethod.ReturnType;
            if (returnType == typeof(void)) return null;
            if (returnType == typeof(Task)) return Task.CompletedTask;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var inner = returnType.GetGenericArguments()[0];
                var value = inner.IsValueType ? Activator.CreateInstance(inner) : null;
                var fromResult = typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(inner);
                return fromResult.Invoke(null, new[] { value });
            }

            if (returnType == typeof(ValueTask)) return default(ValueTask);
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                var inner = returnType.GetGenericArguments()[0];
                var value = inner.IsValueType ? Activator.CreateInstance(inner) : null;
                return Activator.CreateInstance(returnType, value);
            }

            return returnType.IsValueType ? Activator.CreateInstance(returnType) : null;
        }
    }
}