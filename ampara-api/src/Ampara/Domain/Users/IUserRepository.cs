namespace Ampara.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<User> Items, int TotalCount)> ListAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    void Add(User user);
}
