using Ampara.Domain.Animals;
using Ampara.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ampara.Infrastructure.Persistence;

public class AnimalRepository(AppDbContext db) : IAnimalRepository
{
    public Task<Animal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Animals.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Animal> Items, int TotalCount)> ListAsync(AnimalFilter filter, CancellationToken cancellationToken = default)
    {
        var query = db.Animals.AsNoTracking();

        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(a => EF.Functions.ILike(a.Name, $"%{filter.Search}%"));

        if (filter.Species is { } species)
            query = query.Where(a => a.Species == species);

        if (filter.Status is { } status)
            query = query.Where(a => a.Status == status);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.IntakeDate).ThenBy(a => a.Name)
            .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<Animal>> ListAvailableAsync(SpeciesEnum? species, CancellationToken cancellationToken = default)
    {
        var query = db.Animals.AsNoTracking().Where(a => a.Status == AnimalStatusEnum.Available);

        if (species is { } value)
            query = query.Where(a => a.Species == value);

        return await query.OrderByDescending(a => a.IntakeDate).ThenBy(a => a.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<AnimalStatusEnum, int>> CountByStatusAsync(CancellationToken cancellationToken = default) =>
        await db.Animals
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

    public async Task<IReadOnlyList<DateOnly>> ListIntakeDatesSinceAsync(DateOnly since, CancellationToken cancellationToken = default) =>
        await db.Animals.Where(a => a.IntakeDate >= since).Select(a => a.IntakeDate).ToListAsync(cancellationToken);

    public void Add(Animal animal) => db.Animals.Add(animal);
}
