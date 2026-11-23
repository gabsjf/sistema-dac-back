using System.Security.Claims;
using System.Text;
using Ampara.Application.Common;
using Ampara.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Ampara.Infrastructure.Security;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public const int MinimumKeyLength = 32;

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    // HMAC signing key. Never committed: set it with dotnet user-secrets.
    public string Key { get; set; } = string.Empty;

    public int ExpirationHours { get; set; } = 8;

    public SymmetricSecurityKey GetSigningKey() => new(Encoding.UTF8.GetBytes(Key));
}

// Claim names written in the token and read back by the API's CurrentUser.
public static class JwtClaims
{
    public const string Subject = "sub";
    public const string Email = "email";
    public const string Name = "name";
    public const string Role = "role";
}

public class JwtTokenService(IOptions<JwtOptions> options, TimeProvider clock) : ITokenService
{
    public AccessToken Create(User user)
    {
        var jwt = options.Value;
        var now = clock.GetUtcNow();
        var expiresAt = now.AddHours(jwt.ExpirationHours);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtClaims.Subject, user.Id.ToString()),
                new Claim(JwtClaims.Email, user.Email),
                new Claim(JwtClaims.Name, user.FullName),
                new Claim(JwtClaims.Role, user.Role.ToString()),
            ]),
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(jwt.GetSigningKey(), SecurityAlgorithms.HmacSha256),
        };

        return new AccessToken(new JsonWebTokenHandler().CreateToken(descriptor), expiresAt);
    }
}
