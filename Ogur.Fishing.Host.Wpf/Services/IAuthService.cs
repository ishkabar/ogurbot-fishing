
using System.Threading;
using System.Threading.Tasks;

namespace Ogur.Fishing.Host.Wpf.Services;


/// <summary>
/// Provides authentication operations for the host application.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Attempts to authenticate a user with provided credentials.
    /// </summary>
    /// <param name="username">User name.</param>
    /// <param name="password">Plain password (stub for MVP).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if authentication succeeded.</returns>
    Task<bool> AuthenticateAsync(string username, string password, CancellationToken ct);
}