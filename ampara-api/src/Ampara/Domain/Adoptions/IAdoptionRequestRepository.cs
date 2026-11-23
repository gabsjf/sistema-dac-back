using Ampara.Domain.Enums;

namespace Ampara.Domain.Adoptions;

public interface IAdoptionRequestRepository
{
    // Loads the request with its animal and adopter.
    Task<AdoptionRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<AdoptionRequest> Items, int TotalCount)> ListAsync(AdoptionStatusEnum? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdoptionRequest>> ListByAdopterAsync(Guid adopterId, CancellationToken cancellationToken = default);
    Task<bool> HasPendingAsync(Guid adopterId, Guid animalId, CancellationToken cancellationToken = default);
    Task<int> CountPendingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DateTimeOffset>> ListApprovalDatesSinceAsync(DateTimeOffset since, CancellationToken cancellationToken = default);
    void Add(AdoptionRequest request);
}
