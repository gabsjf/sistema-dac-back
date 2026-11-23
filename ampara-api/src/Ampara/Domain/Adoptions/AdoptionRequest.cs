
using Ampara.Domain.Animals;
using Ampara.Domain.Common;
using Ampara.Domain.Enums;
using Ampara.Domain.Users;

namespace Ampara.Domain.Adoptions;

// What the adopter tells the NGO in the adoption form.
public sealed record ApplicantDetails(
    string Phone,
    string Housing,
    string Routine,
    string? Message);

public class AdoptionRequest : Entity
{
    public const int PhoneMaxLength = 30;
    public const int TextMaxLength = 1000;
    public const int ReasonMaxLength = 500;

    public Guid AnimalId { get; private set; }
    public Animal Animal { get; private set; } = null!;

    public Guid AdopterId { get; private set; }
    public User Adopter { get; private set; } = null!;

    public string Phone { get; private set; } = string.Empty;
    public string Housing { get; private set; } = string.Empty;
    public string Routine { get; private set; } = string.Empty;
    public string? Message { get; private set; }

    public AdoptionStatusEnum Status { get; private set; }
    public string? RejectionReason { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? DecidedAt { get; private set; }

    private AdoptionRequest() { }

    public static AdoptionRequest Submit(
        Animal animal,
        User adopter,
        ApplicantDetails details,
        bool acceptsTerms,
        DateTimeOffset now)
    {
        if (!animal.IsAvailable)
            throw new DomainException(
                "Este animal não está disponível para adoção no momento.");

        if (!acceptsTerms)
            throw new DomainException(
                "Confirme que tem 18 anos ou mais e concorda com a análise da ONG.");

        return new AdoptionRequest
        {
            AnimalId = animal.Id,
            Animal = animal,
            AdopterId = adopter.Id,
            Adopter = adopter,

            Phone = Guard.Required(
                details.Phone, "telefone", PhoneMaxLength),

            Housing = Guard.Required(
                details.Housing, "moradia", TextMaxLength),

            Routine = Guard.Required(
                details.Routine, "rotina", TextMaxLength),

            Message = Guard.Optional(
                details.Message, "mensagem", TextMaxLength),

            Status = AdoptionStatusEnum.Pending,
            CreatedAt = now
        };
    }

    // Approves the request and marks the animal as adopted.
    public void Approve(DateTimeOffset now)
    {
        EnsurePending();

        Animal.MarkAsAdopted();

        Status = AdoptionStatusEnum.Approved;
        DecidedAt = now;
        RejectionReason = null;
    }

    public void Reject(string reason, DateTimeOffset now)
    {
        EnsurePending();

        RejectionReason = Guard.Required(
            reason, "motivo", ReasonMaxLength);

        Status = AdoptionStatusEnum.Rejected;
        DecidedAt = now;
    }

    // Undoes an approval and makes the animal available again.
    public void UndoApproval()
    {
        if (Status != AdoptionStatusEnum.Approved)
            throw new DomainException(
                "Somente uma adoção aprovada pode ser desfeita.");

        Animal.RestoreAvailability();

        Status = AdoptionStatusEnum.Pending;
        DecidedAt = null;
        RejectionReason = null;
    }

    private void EnsurePending()
    {
        if (Status != AdoptionStatusEnum.Pending)
            throw new DomainException(
                "Esta solicitação já foi decidida.");
    }
}
