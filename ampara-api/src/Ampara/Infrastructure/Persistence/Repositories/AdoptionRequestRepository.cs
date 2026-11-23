using Ampara.Domain.Adoptions;
using Ampara.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ampara.Infrastructure.Persistence;

public class AdoptionRequestRepository(AppDbContext db) : IAdoptionRequestRepository
{
    private IQueryable<AdoptionRequest> WithDetails => db.AdoptionRequests.Include(r => r.Animal).Include(r => r.Adopter);

    public Task<AdoptionRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        WithDetails.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<AdoptionRequest> Items, int TotalCount)> ListAsync(
        AdoptionStatusEnum? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = WithDetails.AsNoTracking();

        if (status is { } value)
            query = query.Where(r => r.Status == value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<AdoptionRequest>> ListByAdopterAsync(Guid adopterId, CancellationToken cancellationToken = default) =>
        await WithDetails.AsNoTracking()
            .Where(r => r.AdopterId == adopterId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<bool> HasPendingAsync(Guid adopterId, Guid animalId, CancellationToken cancellationToken = default) =>
        db.AdoptionRequests.AnyAsync(
            r => r.AdopterId == adopterId && r.AnimalId == animalId && r.Status == AdoptionStatusEnum.Pending, cancellationToken);

    public Task<int> CountPendingAsync(CancellationToken cancellationToken = default) =>
        db.AdoptionRequests.CountAsync(r => r.Status == AdoptionStatusEnum.Pending, cancellationToken);

    public async Task<IReadOnlyList<DateTimeOffset>> ListApprovalDatesSinceAsync(DateTimeOffset since, CancellationToken cancellationToken = default) =>
        await db.AdoptionRequests
            .Where(r => r.Status == AdoptionStatusEnum.Approved && r.DecidedAt >= since)
            .Select(r => r.DecidedAt!.Value)
            .ToListAsync(cancellationToken);

    public void Add(AdoptionRequest request) => db.AdoptionRequests.Add(request);
}
