using Ampara.Application.Common;
using Ampara.Application.Users;
using Ampara.Domain.Common;
using Ampara.Domain.Enums;
using Ampara.Domain.Users;
using NSubstitute;
using static Ampara.UnitTests.TestData;

namespace Ampara.UnitTests.Application;

public class UserServiceTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UserService _service;

    public UserServiceTests() => _service = new UserService(_users, _unitOfWork);

    [Fact]
    public async Task ChangeRoleAsync_PromotesAdopterToAdminAndSaves()
    {
        var user = NewAdopter();
        _users.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        var response = await _service.ChangeRoleAsync(user.Id, new ChangeRoleRequest { Role = UserRoleEnum.Admin });

        Assert.Equal(UserRoleEnum.Admin, response.Role);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ChangeRoleAsync_ForMissingUser_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.ChangeRoleAsync(Guid.NewGuid(), new ChangeRoleRequest { Role = UserRoleEnum.Admin }));
    }

    [Fact]
    public async Task ChangeRoleAsync_ToSuperAdmin_ThrowsAndDoesNotSave()
    {
        var user = NewAdopter();
        _users.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        await Assert.ThrowsAsync<DomainException>(() =>
            _service.ChangeRoleAsync(user.Id, new ChangeRoleRequest { Role = UserRoleEnum.SuperAdmin }));
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListAsync_TrimsSearchAndClampsPaging()
    {
        _users.ListAsync(Arg.Any<string?>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((new[] { NewAdopter() }, 1));

        var page = await _service.ListAsync(new UserListQuery { Search = " ana ", Page = 0, PageSize = 500 });

        await _users.Received(1).ListAsync("ana", 1, Paging.MaxPageSize, Arg.Any<CancellationToken>());
        Assert.Single(page.Items);
    }
}
