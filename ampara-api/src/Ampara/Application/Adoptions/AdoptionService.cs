
using Ampara.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using Ampara.Application.Common;
using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Common;
using Ampara.Domain.Users;

namespace Ampara.Application.Adoptions;

public sealed record SubmitAdoptionRequest
{
    public Guid AnimalId { get; init; }

    [Required, MaxLength(AdoptionRequest.PhoneMaxLength)]
    public string Phone { get; init; } = string.Empty;

    [Required, MaxLength(AdoptionRequest.TextMaxLength)]
    public string Housing { get; init; } = string.Empty;

    [Required, MaxLength(AdoptionRequest.TextMaxLength)]
    public string Routine { get; init; } = string.Empty;

    [MaxLength(AdoptionRequest.TextMaxLength)]
    public string? Message { get; init; }

    public bool AcceptsTerms { get; init; }
}

public sealed record RejectAdoptionRequest
{
    [Required, MaxLength(AdoptionRequest.ReasonMaxLength)]
    public string Reason { get; init; } = string.Empty;
}

public sealed record AdoptionListQuery
{
    public AdoptionStatusEnum? Status { get; init; }
    public int? Page { get; init; }
    public int? PageSize { get; init; }
}

public sealed record AdoptionAnimal(
    Guid Id,
    string Name,
    SpeciesEnum Species,
    SexEnum Sex,
    AnimalSizeEnum Size,
    int ApproximateAgeMonths,
    string? PhotoUrl);

public sealed record AdoptionApplicant(
    Guid Id,
    string FullName,
    string Email,
    string Phone,
    string Housing,
    string Routine,
    string? Message);

public sealed record AdoptionResponse(
    Guid Id,
    AdoptionStatusEnum Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DecidedAt,
    string? RejectionReason,
    AdoptionAnimal Animal,
    AdoptionApplicant Applicant)
{
    public static AdoptionResponse From(AdoptionRequest r) => new(
        r.Id,
        r.Status,
        r.CreatedAt,
        r.DecidedAt,
        r.RejectionReason,

        new AdoptionAnimal(
            r.Animal.Id,
            r.Animal.Name,
            r.Animal.Species,
            r.Animal.Sex,
            r.Animal.Size,
            r.Animal.ApproximateAgeMonths,
            r.Animal.PhotoUrl),

        new AdoptionApplicant(
            r.Adopter.Id,
            r.Adopter.FullName,
            r.Adopter.Email,
            r.Phone,
            r.Housing,
            r.Routine,
            r.Message));
}

// The adopter applies; the NGO approves or rejects.
public class AdoptionService(
    IAdoptionRequestRepository requests,
    IAnimalRepository animals,
    IUserRepository users,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    TimeProvider clock)
{
    public async Task<AdoptionResponse> SubmitAsync(
        SubmitAdoptionRequest request,
        CancellationToken cancellationToken = default)
    {
        var adopter = await users.GetByIdAsync(
            currentUser.RequireUserId(), cancellationToken)
            ?? throw new UnauthorizedException(
                "Faça login para continuar.");

        var animal = await animals.GetByIdAsync(
            request.AnimalId, cancellationToken)
            ?? throw new NotFoundException(
                "Animal não encontrado.");

        if (await requests.HasPendingAsync(
            adopter.Id, animal.Id, cancellationToken))
        {
            throw new ConflictException(
                "Você já tem uma solicitação em análise para este animal.");
        }

        var adoption = AdoptionRequest.Submit(
            animal,
            adopter,
            new ApplicantDetails(
                request.Phone,
                request.Housing,
                request.Routine,
                request.Message),
            request.AcceptsTerms,
            clock.GetUtcNow());

        requests.Add(adoption);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AdoptionResponse.From(adoption);
    }

    public async Task<IReadOnlyList<AdoptionResponse>> ListMineAsync(
        CancellationToken cancellationToken = default) =>
        (await requests.ListByAdopterAsync(
            currentUser.RequireUserId(),
            cancellationToken))
        .Select(AdoptionResponse.From)
        .ToList();

    public async Task<PagedResult<AdoptionResponse>> ListAsync(
        AdoptionListQuery query,
        CancellationToken cancellationToken = default)
    {
        var (page, pageSize) = Paging.Normalize(
            query.Page, query.PageSize);

        var (items, total) = await requests.ListAsync(
            query.Status, page, pageSize, cancellationToken);

        return new PagedResult<AdoptionResponse>(
            items.Select(AdoptionResponse.From).ToList(),
            page,
            pageSize,
            total);
    }

    public async Task<AdoptionResponse> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var adoption = await FindAsync(id, cancellationToken);

        if (!currentUser.IsStaff() &&
            adoption.AdopterId != currentUser.UserId)
        {
            throw new NotFoundException(
                "Solicitação não encontrada.");
        }

        return AdoptionResponse.From(adoption);
    }

    public async Task<AdoptionResponse> ApproveAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var adoption = await FindAsync(id, cancellationToken);

        adoption.Approve(clock.GetUtcNow());

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AdoptionResponse.From(adoption);
    }

    public async Task<AdoptionResponse> RejectAsync(
        Guid id,
        RejectAdoptionRequest request,
        CancellationToken cancellationToken = default)
    {
        var adoption = await FindAsync(id, cancellationToken);

        adoption.Reject(request.Reason, clock.GetUtcNow());

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AdoptionResponse.From(adoption);
    }

    // Undo an approved adoption.
    public async Task<AdoptionResponse> UndoApprovalAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var adoption = await FindAsync(id, cancellationToken);

        adoption.UndoApproval();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AdoptionResponse.From(adoption);
    }

    private async Task<AdoptionRequest> FindAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        await requests.GetByIdAsync(id, cancellationToken)
        ?? throw new NotFoundException(
            "Solicitação não encontrada.");
}
