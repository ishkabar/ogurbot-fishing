using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ogur.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Ogur.Capabilities.Fishing;


/// <summary>
/// Plugin exposing the Fishing capability.
/// </summary>
public sealed class FishingPlugin : IApplicationPlugin
{
    /// <summary>
    /// Gets the plugin name.
    /// </summary>
    public string Name => "FishingPlugin";

    private readonly List<IApplicationCapability> _capabilities = new();

    /// <summary>
    /// Gets the set of bot capabilities provided by this plugin.
    /// </summary>
    public IEnumerable<IApplicationCapability> Capabilities => _capabilities;

    /// <summary>
    /// Initializes the plugin with the given service provider.
    /// </summary>
    /// <param name="sp">Service provider for dependency resolution.</param>
    /// <param name="ct">Cancellation token.</param>
    public async ValueTask InitializeAsync(IServiceProvider sp, CancellationToken ct)
    {
        var cap = sp.GetRequiredService<FishingCapability>();
        _capabilities.Add(cap);
        await Task.CompletedTask;
    }
}