using Ampara.Domain.Enums;
using Ampara.Domain.Users;

namespace Ampara.Application.Common;

// Role names as written in the JWT, for [Authorize(Roles = ...)].
public static class Roles
{
    public const string SuperAdmin = nameof(UserRoleEnum.SuperAdmin);
    public const string Admin = nameof(UserRoleEnum.Admin);
    public const string Adopter = nameof(UserRoleEnum.Adopter);

    // The NGO team: animals, adoptions and dashboard.
    public const string Staff = $"{Admin},{SuperAdmin}";
}
