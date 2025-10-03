using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace Ogur.Fishing.Host.Wpf.Composition;


  /// <summary>
    /// DispatchProxy that returns default values for all methods of any interface.
    /// Useful as a temporary DI stub to let the app boot without concrete infra implementations.
    /// </summary>
    public class NullDispatchProxy : DispatchProxy
    {
        /// <summary>
        /// Intercepts interface calls and returns default values for return types.
        /// </summary>
        /// <param name="targetMethod">Invoked method info.</param>
        /// <param name="args">Invocation arguments.</param>
        /// <returns>Default value for the return type or a completed task.</returns>
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod is null) return null;

            var returnType = targetMethod.ReturnType;

            if (returnType == typeof(void))
                return null;

            if (returnType == typeof(Task))
                return Task.CompletedTask;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var t = returnType.GenericTypeArguments[0];
                var result = GetDefault(t);
                var fromResult = typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(t);
                return fromResult.Invoke(null, new[] { result });
            }

            return GetDefault(returnType);
        }

        private static object? GetDefault(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
    }

    /// <summary>
    /// DI helpers for registering null proxies for interfaces.
    /// </summary>
    public static class NullProxyServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a default 'do-nothing' implementation for the given interface.
        /// </summary>
        /// <typeparam name="TInterface">Interface type to stub.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <returns>The same collection.</returns>
        public static IServiceCollection AddNullProxy<TInterface>(this IServiceCollection services)
            where TInterface : class
        {
            services.AddSingleton<TInterface>(_ => DispatchProxy.Create<TInterface, NullDispatchProxy>());
            return services;
        }
    }