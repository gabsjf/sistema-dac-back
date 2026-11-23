using Ampara.Application.Auth;
using Ampara.Application.Common;
using Ampara.Domain.Common;
using Ampara.Domain.Enums;
using Ampara.Domain.Users;

namespace Ampara.Application.Users;

public sealed record UserListQuery
{
    public string? Search { get; init; }
    public int? Page { get; init; }
    public int? PageSize { get; init; }
}

public sealed record ChangeRoleRequest
{
    public UserRoleEnum Role { get; init; }
}

// The SuperAdmin's "Equipe" tab: finds people and promotes or demotes them.
public class UserService(IUserRepository users, IUnitOfWork unitOfWork)
{
    public async Task<PagedResult<UserResponse>> ListAsync(UserListQuery query, CancellationToken cancellationToken = default)
    {
        var (page, pageSize) = Paging.Normalize(query.Page, query.PageSize);
        var (items, total) = await users.ListAsync(query.Search?.Trim(), page, pageSize, cancellationToken);
        return new PagedResult<UserResponse>(items.Select(UserResponse.From).ToList(), page, pageSize, total);
    }

    public async Task<UserResponse> ChangeRoleAsync(Guid id, ChangeRoleRequest request, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException("Usuário não encontrado.");
        user.ChangeRole(request.Role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return UserResponse.From(user);
    }
}
