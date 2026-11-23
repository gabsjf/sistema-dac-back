using Ampara.Application.Auth;
using Ampara.Application.Common;
using Ampara.Domain.Common;
using Ampara.Domain.Enums;
using Ampara.Domain.Users;
using NSubstitute;
using static Ampara.UnitTests.TestData;

namespace Ampara.UnitTests.Application;

public class AuthServiceTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _hasher.Hash(Arg.Any<string>()).Returns("hashed");
        _tokens.Create(Arg.Any<User>()).Returns(new AccessToken("jwt", Now.AddHours(8)));
        _service = new AuthService(_users, _unitOfWork, _hasher, _tokens, _currentUser, Clock());
    }

    [Fact]
    public async Task RegisterAsync_CreatesAdopterAndReturnsToken()
    {
        var response = await _service.RegisterAsync(new RegisterRequest
        {
            FullName = "Ana Souza", Email = " Ana@Exemplo.com ", Password = "segura123",
        });

        _users.Received(1).Add(Arg.Is<User>(u => u.Email == "ana@exemplo.com" && u.Role == UserRoleEnum.Adopter && u.PasswordHash == "hashed"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        Assert.Equal("jwt", response.AccessToken);
        Assert.Equal(UserRoleEnum.Adopter, response.User.Role);
    }

    [Fact]
    public async Task RegisterAsync_WithShortPassword_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() => _service.RegisterAsync(new RegisterRequest
        {
            FullName = "Ana Souza", Email = "ana@exemplo.com", Password = "1234567",
        }));

        _users.DidNotReceive().Add(Arg.Any<User>());
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsConflict()
    {
        _users.EmailExistsAsync("ana@exemplo.com", Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.RegisterAsync(new RegisterRequest
        {
            FullName = "Ana Souza", Email = "ana@exemplo.com", Password = "segura123",
        }));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var user = NewAdopter();
        _users.GetByEmailAsync("ana@exemplo.com", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify(user.PasswordHash, "segura123").Returns(true);

        var response = await _service.LoginAsync(new LoginRequest { Email = "ANA@exemplo.com", Password = "segura123" });

        Assert.Equal("jwt", response.AccessToken);
        Assert.Equal(user.Id, response.User.Id);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ThrowsUnauthorized()
    {
        var user = NewAdopter();
        _users.GetByEmailAsync("ana@exemplo.com", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify(user.PasswordHash, "errada").Returns(false);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _service.LoginAsync(new LoginRequest { Email = "ana@exemplo.com", Password = "errada" }));
    }

    [Fact]
    public async Task LoginAsync_WithUnknownEmail_ThrowsUnauthorized()
    {
        var error = await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _service.LoginAsync(new LoginRequest { Email = "ninguem@exemplo.com", Password = "segura123" }));

        Assert.Equal("E-mail ou senha inválidos.", error.Message);
    }

    [Fact]
    public async Task MeAsync_ReturnsTheLoggedUser()
    {
        var user = NewAdopter();
        _currentUser.UserId.Returns(user.Id);
        _users.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        var me = await _service.MeAsync();

        Assert.Equal("Ana Souza", me.FullName);
    }
}
