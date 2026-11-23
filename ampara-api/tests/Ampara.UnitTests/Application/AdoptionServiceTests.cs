using Ampara.Application.Adoptions;
using Ampara.Application.Common;
using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Common;
using Ampara.Domain.Enums;
using Ampara.Domain.Users;
using NSubstitute;
using static Ampara.UnitTests.TestData;

namespace Ampara.UnitTests.Application;

public class AdoptionServiceTests
{
    private readonly IAdoptionRequestRepository _requests = Substitute.For<IAdoptionRequestRepository>();
    private readonly IAnimalRepository _animals = Substitute.For<IAnimalRepository>();
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly AdoptionService _service;

    private readonly User _adopter = NewAdopter();
    private readonly Animal _animal = NewAnimal();

    public AdoptionServiceTests()
    {
        _service = new AdoptionService(_requests, _animals, _users, _unitOfWork, _currentUser, Clock());
        LogInAs(_adopter.Id, UserRoleEnum.Adopter);
        _users.GetByIdAsync(_adopter.Id, Arg.Any<CancellationToken>()).Returns(_adopter);
        _animals.GetByIdAsync(_animal.Id, Arg.Any<CancellationToken>()).Returns(_animal);
    }

    private void LogInAs(Guid userId, UserRoleEnum role)
    {
        _currentUser.UserId.Returns(userId);
        _currentUser.Role.Returns(role);
    }

    private SubmitAdoptionRequest Submit() => new()
    {
        AnimalId = _animal.Id, Phone = "(11) 99999-0123", Housing = "Casa com quintal.", Routine = "Home office.", AcceptsTerms = true,
    };

    private AdoptionRequest Existing()
    {
        var request = PendingRequest(_animal, _adopter);
        _requests.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);
        return request;
    }

    [Fact]
    public async Task SubmitAsync_CreatesPendingRequestForTheLoggedAdopter()
    {
        var response = await _service.SubmitAsync(Submit());

        _requests.Received(1).Add(Arg.Is<AdoptionRequest>(r => r.AdopterId == _adopter.Id && r.AnimalId == _animal.Id));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        Assert.Equal(AdoptionStatusEnum.Pending, response.Status);
        Assert.Equal("Ana Souza", response.Applicant.FullName);
    }

    [Fact]
    public async Task SubmitAsync_WithPendingRequestForSameAnimal_ThrowsConflict()
    {
        _requests.HasPendingAsync(_adopter.Id, _animal.Id, Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.SubmitAsync(Submit()));
        _requests.DidNotReceive().Add(Arg.Any<AdoptionRequest>());
    }

    [Fact]
    public async Task SubmitAsync_ForMissingAnimal_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.SubmitAsync(Submit() with { AnimalId = Guid.NewGuid() }));
    }

    [Fact]
    public async Task GetAsync_ByTheAdopterWhoSubmitted_ReturnsRequest()
    {
        var request = Existing();

        var response = await _service.GetAsync(request.Id);

        Assert.Equal(request.Id, response.Id);
    }

    [Fact]
    public async Task GetAsync_ByAnotherAdopter_ThrowsNotFound()
    {
        var request = Existing();
        LogInAs(Guid.NewGuid(), UserRoleEnum.Adopter);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAsync(request.Id));
    }

    [Theory]
    [InlineData(UserRoleEnum.Admin)]
    [InlineData(UserRoleEnum.SuperAdmin)]
    public async Task GetAsync_ByTheTeam_ReturnsAnyRequest(UserRoleEnum role)
    {
        var request = Existing();
        LogInAs(Guid.NewGuid(), role);

        var response = await _service.GetAsync(request.Id);

        Assert.Equal(request.Id, response.Id);
    }

    [Fact]
    public async Task ApproveAsync_MarksAnimalAsAdoptedAndSaves()
    {
        var request = Existing();

        var response = await _service.ApproveAsync(request.Id);

        Assert.Equal(AdoptionStatusEnum.Approved, response.Status);
        Assert.Equal(AnimalStatusEnum.Adopted, _animal.Status);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RejectAsync_StoresTheReason()
    {
        var request = Existing();

        var response = await _service.RejectAsync(request.Id, new RejectAdoptionRequest { Reason = "Moradia sem telas." });

        Assert.Equal(AdoptionStatusEnum.Rejected, response.Status);
        Assert.Equal("Moradia sem telas.", response.RejectionReason);
    }

    [Fact]
    public async Task ApproveAsync_ForMissingRequest_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.ApproveAsync(Guid.NewGuid()));
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
