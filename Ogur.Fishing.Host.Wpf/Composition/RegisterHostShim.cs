using Microsoft.Extensions.DependencyInjection;


namespace Ogur.Fishing.Host.Wpf.Composition;


/// <summary>
/// Shim to satisfy calls to RegisterHost() from legacy code. No-op.
/// </summary>
public static class RegisterHostShim
{
    /// <summary>
    /// No-op extension to keep legacy call sites compiling.
    /// </summary>
    /// <param name="services">Services.</param>
    /// <returns>The same collection.</returns>
    public static IServiceCollection RegisterHost(this IServiceCollection services) => services;
}