using System.Threading;
using System.Threading.Tasks;

namespace Ogur.Fishing.Host.Wpf.Services;


/// <summary>
/// Dummy authentication service used for MVP flow.
/// </summary>
public sealed class DummyAuthService : IAuthService
{
    /// <summary>
    /// Authenticates by checking non-empty credentials.
    /// </summary>
    /// <param name="username">User name.</param>
    /// <param name="password">Password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if both inputs are non-empty.</returns>
    public Task<bool> AuthenticateAsync(string username, string password, CancellationToken ct) =>
        Task.FromResult(!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password));
}