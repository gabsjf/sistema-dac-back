using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Enums;
using Ampara.Domain.Users;

namespace Ampara.UnitTests;

// Builders shared by the tests, all pinned to the same fixed clock.
public static class TestData
{
    public static readonly DateTimeOffset Now = new(2026, 11, 20, 15, 0, 0, TimeSpan.Zero);

    public static TimeProvider Clock() => new FixedClock(Now);

    public static AnimalProfile Profile(string name = "Luna", int ageMonths = 24, DateOnly? intakeDate = null) =>
        new(name, SpeciesEnum.Canine, SexEnum.Female, AnimalSizeEnum.Medium, ageMonths, intakeDate ?? new DateOnly(2026, 11, 10),
            "Carinhosa e companheira.", true, true, false);

    public static Animal NewAnimal(AnimalStatusEnum status = AnimalStatusEnum.Available) => Animal.Register(Profile(), status, Now);

    public static User NewAdopter(string email = "ana@exemplo.com") =>
        User.Register("Ana Souza", email, "(11) 99999-0123", UserRoleEnum.Adopter, "hash", Now);

    public static ApplicantDetails Details() =>
        new("(11) 99999-0123", "Apartamento próprio, com telas.", "Trabalho híbrido.", "Quero dar um lar.");

    public static AdoptionRequest PendingRequest(Animal? animal = null, User? adopter = null) =>
        AdoptionRequest.Submit(animal ?? NewAnimal(), adopter ?? NewAdopter(), Details(), acceptsTerms: true, Now);

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
