namespace Ampara.Domain.Enums;

// SuperAdmin: the NGO owner (only one, from the seed), who also manages the team.
// Admin: promoted by the SuperAdmin; runs animals, adoptions and the dashboard.
// Adopter: anyone who signs up.
public enum UserRoleEnum
{
    Admin,
    Adopter,
    SuperAdmin,
}
