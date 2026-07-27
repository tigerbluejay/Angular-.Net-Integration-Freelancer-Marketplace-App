using API.Entities;

namespace API.Interfaces;

/// <summary>
/// Issues signed JWT bearer tokens for authenticated users.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a signed JWT access token for <paramref name="user"/>, embedding their
    /// username and role claims so the API and SignalR hubs can authenticate/authorize them.
    /// </summary>
    /// <param name="user">The user to issue a token for. Must have a non-null <c>UserName</c>.</param>
    /// <returns>A compact, signed JWT string valid for 7 days.</returns>
    Task<string> CreateToken(AppUser user);
}