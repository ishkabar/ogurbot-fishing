namespace Ogur.Fishing.Host.Wpf.ViewModels;


/// <summary>
/// Represents a server entry visible in the server selection screen.
/// </summary>
/// <param name="Id">Unique server identifier.</param>
/// <param name="Name">Display name.</param>
/// <param name="IconPath">Pack URI to an embedded PNG resource.</param>
public sealed record ServerOption(string Id, string Name, string IconPath);