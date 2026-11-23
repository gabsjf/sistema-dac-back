using Ampara.Application.Common;
using Ampara.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Ampara.Infrastructure.Security;

// PBKDF2 hashing from ASP.NET Core (the same algorithm Identity uses), without Identity's tables.
public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(null!, password);

    public bool Verify(string passwordHash, string password) =>
        _hasher.VerifyHashedPassword(null!, passwordHash, password) != PasswordVerificationResult.Failed;
}
