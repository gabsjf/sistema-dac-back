
using Ampara.Domain.Common;
using Ampara.Domain.Enums;

namespace Ampara.Domain.Animals;

// Data the team fills in when registering or editing an animal.
public sealed record AnimalProfile(
    string Name,
    SpeciesEnum Species,
    SexEnum Sex,
    AnimalSizeEnum Size,
    int ApproximateAgeMonths,
    DateOnly IntakeDate,
    string? Description,
    bool IsVaccinated,
    bool IsNeutered,
    bool IsDewormed);

public class Animal : Entity
{
    public const int NameMaxLength = 80;
    public const int DescriptionMaxLength = 1000;
    public const int PhotoUrlMaxLength = 300;
    public const int MaxAgeMonths = 360;

    public string Name { get; private set; } = string.Empty;
    public SpeciesEnum Species { get; private set; }
    public SexEnum Sex { get; private set; }
    public AnimalSizeEnum Size { get; private set; }
    public int ApproximateAgeMonths { get; private set; }
    public DateOnly IntakeDate { get; private set; }
    public string? Description { get; private set; }
    public bool IsVaccinated { get; private set; }
    public bool IsNeutered { get; private set; }
    public bool IsDewormed { get; private set; }
    public string? PhotoUrl { get; private set; }
    public AnimalStatusEnum Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsAvailable => Status == AnimalStatusEnum.Available;

    private Animal() { }

    public static Animal Register(
        AnimalProfile profile,
        AnimalStatusEnum status,
        DateTimeOffset now)
    {
        if (status == AnimalStatusEnum.Adopted)
            throw new DomainException(
                "Um animal só fica como adotado quando uma adoção é aprovada.");

        var animal = new Animal
        {
            Status = status,
            CreatedAt = now
        };

        animal.Apply(profile, now);
        return animal;
    }

    public void Update(
        AnimalProfile profile,
        AnimalStatusEnum status,
        DateTimeOffset now)
    {
        if (status != Status &&
            (status == AnimalStatusEnum.Adopted ||
             Status == AnimalStatusEnum.Adopted))
        {
            throw new DomainException(
                "A situação de adotado só muda pela aprovação de uma adoção.");
        }

        Apply(profile, now);
        Status = status;
    }

    public void SetPhoto(string url) =>
        PhotoUrl = Guard.Required(url, "foto", PhotoUrlMaxLength);

    // Marks the animal as adopted.
    public void MarkAsAdopted()
    {
        if (!IsAvailable)
            throw new DomainException(
                "Este animal não está disponível para adoção no momento.");

        Status = AnimalStatusEnum.Adopted;
    }

    // Restores the animal after undoing an adoption.
    public void RestoreAvailability()
    {
        if (Status != AnimalStatusEnum.Adopted)
            throw new DomainException(
                "Este animal não está marcado como adotado.");

        Status = AnimalStatusEnum.Available;
    }

    private void Apply(AnimalProfile profile, DateTimeOffset now)
    {
        if (profile.ApproximateAgeMonths is < 0 or > MaxAgeMonths)
            throw new DomainException(
                $"A idade aproximada deve estar entre 0 e {MaxAgeMonths} meses.");

        if (profile.IntakeDate > DateOnly.FromDateTime(now.UtcDateTime))
            throw new DomainException(
                "A data de acolhimento não pode estar no futuro.");

        Name = Guard.Required(profile.Name, "nome", NameMaxLength);
        Species = profile.Species;
        Sex = profile.Sex;
        Size = profile.Size;
        ApproximateAgeMonths = profile.ApproximateAgeMonths;
        IntakeDate = profile.IntakeDate;
        Description = Guard.Optional(
            profile.Description, "descrição", DescriptionMaxLength);

        IsVaccinated = profile.IsVaccinated;
        IsNeutered = profile.IsNeutered;
        IsDewormed = profile.IsDewormed;
    }
}
