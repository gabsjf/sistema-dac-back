using Ampara.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ampara.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService auth) : ControllerBase
{
    // Creates an adopter account and logs in.
    [HttpPost("register")]
    public Task<AuthResponse> Register(RegisterRequest request, CancellationToken cancellationToken) =>
        auth.RegisterAsync(request, cancellationToken);

    [HttpPost("login")]
    public Task<AuthResponse> Login(LoginRequest request, CancellationToken cancellationToken) =>
        auth.LoginAsync(request, cancellationToken);

    [Authorize]
    [HttpGet("me")]
    public Task<UserResponse> Me(CancellationToken cancellationToken) => auth.MeAsync(cancellationToken);
}
