using Ampara.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Ampara.Infrastructure.Persistence;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> ListAsync(
        string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Users.AsNoTracking();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(u => EF.Functions.ILike(u.FullName, $"%{search}%") || EF.Functions.ILike(u.Email, $"%{search}%"));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public void Add(User user) => db.Users.Add(user);
}
