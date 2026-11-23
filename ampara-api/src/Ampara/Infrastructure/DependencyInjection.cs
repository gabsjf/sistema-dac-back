using Ampara.Application.Common;
using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Common;
using Ampara.Domain.Users;
using Ampara.Infrastructure.Persistence;
using Ampara.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ampara.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' não configurada.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IAdoptionRequestRepository, AdoptionRequestRepository>();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddJwtAuthentication(configuration);

        return services;
    }

    private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(JwtOptions.SectionName);
        var jwt = section.Get<JwtOptions>() ?? new JwtOptions();

        if (jwt.Key.Length < JwtOptions.MinimumKeyLength)
            throw new InvalidOperationException(
                $"Configure 'Jwt:Key' com pelo menos {JwtOptions.MinimumKeyLength} caracteres: " +
                "dotnet user-secrets set \"Jwt:Key\" \"<chave-aleatória>\" --project src/Ampara");

        services.Configure<JwtOptions>(section);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = jwt.GetSigningKey(),
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = JwtClaims.Name,
                    RoleClaimType = JwtClaims.Role,
                };
            });
    }
}
