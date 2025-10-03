using Microsoft.Extensions.DependencyInjection;


namespace Ogur.Fishing.Host.Wpf.Composition;

/// <summary>
/// Host registration extensions placeholder.
/// </summary>
public static class HostRegistrationExtensions
{
    /// <summary>
    /// No-op placeholder to satisfy calls to RegisterHost() from Shell.
    /// Replace with real registrations when available.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same collection.</returns>
    public static IServiceCollection RegisterHost(this IServiceCollection services) => services;
}