using Ampara.Domain.Enums;
using Ampara.Domain.Users;

namespace Ampara.Application.Common;

// Who is calling the API, read from the JWT.
public interface ICurrentUser
{
    Guid? UserId { get; }
    UserRoleEnum? Role { get; }
}

public static class CurrentUserExtensions
{
    public static Guid RequireUserId(this ICurrentUser user) =>
        user.UserId ?? throw new UnauthorizedException("Faça login para continuar.");

    // Admin or SuperAdmin.
    public static bool IsStaff(this ICurrentUser user) => user.Role is UserRoleEnum.Admin or UserRoleEnum.SuperAdmin;
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string passwordHash, string password);
}

public sealed record AccessToken(string Token, DateTimeOffset ExpiresAt);

public interface ITokenService
{
    AccessToken Create(User user);
}

public interface IFileStorage
{
    // Returns the public relative URL of the stored file, e.g. /uploads/animals/abc.jpg.
    Task<string> SaveAsync(Stream content, string folder, string fileName, CancellationToken cancellationToken = default);

    Task DeleteAsync(string url, CancellationToken cancellationToken = default);
}
