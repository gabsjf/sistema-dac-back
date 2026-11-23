using Ampara.Domain.Common;
using Ampara.Domain.Enums;

namespace Ampara.Domain.Users;

public class User : Entity
{
    public const int NameMaxLength = 120;
    public const int EmailMaxLength = 160;
    public const int PhoneMaxLength = 30;

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public UserRoleEnum Role { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    private User() { }

    // passwordHash: Already hashed; the plain password never reaches the domain.
    public static User Register(string fullName, string email, string? phone, UserRoleEnum role, string passwordHash, DateTimeOffset now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new User
        {
            FullName = Guard.Required(fullName, "nome completo", NameMaxLength),
            Email = Guard.RequiredEmail(email, "e-mail", EmailMaxLength).ToLowerInvariant(),
            Phone = Guard.Optional(phone, "telefone", PhoneMaxLength),
            Role = role,
            PasswordHash = passwordHash,
            CreatedAt = now,
        };
    }

    // Promotes an adopter to the team (Admin) or moves an Admin back to adopter.
    public void ChangeRole(UserRoleEnum role)
    {
        if (role == UserRoleEnum.SuperAdmin)
            throw new DomainException("Não é possível promover alguém a super admin.");

        if (Role == UserRoleEnum.SuperAdmin)
            throw new DomainException("O papel do super admin não pode ser alterado.");

        Role = role;
    }
}
