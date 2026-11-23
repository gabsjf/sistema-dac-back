using Ampara.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using static Ampara.UnitTests.TestData;

namespace Ampara.UnitTests.Infrastructure;

public class SecurityTests
{
    private readonly JwtOptions _options = new()
    {
        Issuer = "ampara-api",
        Audience = "ampara-web",
        Key = new string('k', JwtOptions.MinimumKeyLength),
        ExpirationHours = 8,
    };

    [Fact]
    public async Task JwtTokenService_Create_WritesUserClaimsAndExpiresAfterConfiguredHours()
    {
        var user = NewAdopter();

        var token = new JwtTokenService(Options.Create(_options), Clock()).Create(user);

        var result = await new JsonWebTokenHandler().ValidateTokenAsync(token.Token, new TokenValidationParameters
        {
            ValidIssuer = _options.Issuer,
            ValidAudience = _options.Audience,
            IssuerSigningKey = _options.GetSigningKey(),
            ValidateLifetime = false,
        });

        Assert.True(result.IsValid);
        Assert.Equal(user.Id.ToString(), result.Claims[JwtClaims.Subject]);
        Assert.Equal("Adopter", result.Claims[JwtClaims.Role]);
        Assert.Equal(Now.AddHours(8), token.ExpiresAt);
    }

    [Fact]
    public void PasswordHasher_VerifiesOnlyTheOriginalPassword()
    {
        var hasher = new PasswordHasher();

        var hash = hasher.Hash("segura123");

        Assert.NotEqual("segura123", hash);
        Assert.True(hasher.Verify(hash, "segura123"));
        Assert.False(hasher.Verify(hash, "outra-senha"));
    }
}
