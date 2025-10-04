namespace Ogur.Fishing.Host.Wpf.ViewModels.Messages;


/// <summary>
/// Message indicating that a user successfully signed in.
/// </summary>
public sealed record LoginSucceededMessage(string Username);