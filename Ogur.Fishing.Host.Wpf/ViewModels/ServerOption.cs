// File: Ogur.Fishing.Host.Wpf/ViewModels/ServerOption.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.ViewModels

namespace Ogur.Fishing.Host.Wpf.ViewModels;

/// <summary>
/// Represents a server entry visible in the server selection screen.
/// </summary>
/// <param name="Id">Unique server identifier.</param>
/// <param name="Name">Display name.</param>
/// <param name="IconPath">Pack URI to an embedded PNG resource.</param>
/// <param name="IsVisible">Whether the server should be visible in the list.</param>
/// <param name="IsEnabled">Whether the server can be selected (false = grayed out).</param>
public sealed record ServerOption(
    string Id, 
    string Name, 
    string IconPath, 
    bool IsVisible = true, 
    bool IsEnabled = true);