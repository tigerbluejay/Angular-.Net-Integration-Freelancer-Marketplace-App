using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

/// <summary>
/// Issues signed JWT bearer tokens used to authenticate API requests and SignalR hub connections.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<AppUser> _userManager;

    /// <summary>
    /// Creates the service with the configuration (for <c>TokenKey</c>) and user manager (for role lookups) it depends on.
    /// </summary>
    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        _config = config;
        _userManager = userManager;
    }

    /// <summary>
    /// Creates a signed JWT for <paramref name="user"/>, embedding a <see cref="ClaimTypes.NameIdentifier"/>
    /// claim for their username and one <see cref="ClaimTypes.Role"/> claim per role assigned to them.
    /// </summary>
    /// <param name="user">The user to issue a token for.</param>
    /// <returns>A compact JWT string, signed with HMAC-SHA512 using the configured <c>TokenKey</c>, valid for 7 days.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <c>TokenKey</c> configuration value is missing or shorter than 64 characters.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="user"/> has no username.
    /// </exception>
    public async Task<string> CreateToken(AppUser user)
    {
        var tokenKey = _config["TokenKey"] ?? throw new InvalidOperationException("Cannot access token key from appsettings");

        if (tokenKey.Length < 64) throw new InvalidOperationException("Your tokenKey needs to be longer");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        if (user.UserName == null) throw new ArgumentException("No username for user");

        // populate claims contained in the JWT token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserName)
        };

        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // generate token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            // add claims
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        // create the token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}