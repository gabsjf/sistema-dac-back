using Ampara.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using Ampara.Application.Common;
using Ampara.Domain.Common;
using Ampara.Domain.Users;

namespace Ampara.Application.Auth;

public sealed record RegisterRequest
{
    [Required, MaxLength(User.NameMaxLength)] public string FullName { get; init; } = string.Empty;
    [Required, EmailAddress, MaxLength(User.EmailMaxLength)] public string Email { get; init; } = string.Empty;
    [MaxLength(User.PhoneMaxLength)] public string? Phone { get; init; }
    [Required] public string Password { get; init; } = string.Empty;
}

public sealed record LoginRequest
{
    [Required] public string Email { get; init; } = string.Empty;
    [Required] public string Password { get; init; } = string.Empty;
}

public sealed record UserResponse(Guid Id, string FullName, string Email, string? Phone, UserRoleEnum Role)
{
    public static UserResponse From(User user) => new(user.Id, user.FullName, user.Email, user.Phone, user.Role);
}

public sealed record AuthResponse(string AccessToken, DateTimeOffset ExpiresAt, UserResponse User);

// Sign-up (adopters only; the admin comes from the seed) and login with JWT.
public class AuthService(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ICurrentUser currentUser,
    TimeProvider clock)
{
    public const int MinPasswordLength = 8;

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Password.Length < MinPasswordLength)
            throw new DomainException($"A senha deve ter pelo menos {MinPasswordLength} caracteres.");

        var email = Normalize(request.Email);
        if (await users.EmailExistsAsync(email, cancellationToken))
            throw new ConflictException("Já existe uma conta com este e-mail.");

        var user = User.Register(request.FullName, email, request.Phone, UserRoleEnum.Adopter,
            passwordHasher.Hash(request.Password), clock.GetUtcNow());

        users.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Issue(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByEmailAsync(Normalize(request.Email), cancellationToken);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
            throw new UnauthorizedException("E-mail ou senha inválidos.");

        return Issue(user);
    }

    public async Task<UserResponse> MeAsync(CancellationToken cancellationToken = default)
    {
        var user = await users.GetByIdAsync(currentUser.RequireUserId(), cancellationToken)
            ?? throw new UnauthorizedException("Faça login para continuar.");

        return UserResponse.From(user);
    }

    private AuthResponse Issue(User user)
    {
        var token = tokenService.Create(user);
        return new AuthResponse(token.Token, token.ExpiresAt, UserResponse.From(user));
    }

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
