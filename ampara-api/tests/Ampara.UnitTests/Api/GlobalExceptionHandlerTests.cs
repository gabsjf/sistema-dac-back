using System.Text.Json;
using Ampara.Api;
using Ampara.Application.Common;
using Ampara.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ampara.UnitTests.Api;

public class GlobalExceptionHandlerTests
{
    public static TheoryData<Exception, int> KnownExceptions => new()
    {
        { new DomainException("Animal indisponível para adoção."), StatusCodes.Status422UnprocessableEntity },
        { new NotFoundException("Animal não encontrado."), StatusCodes.Status404NotFound },
        { new UnauthorizedException("Faça login."), StatusCodes.Status401Unauthorized },
        { new ConflictException("E-mail já usado."), StatusCodes.Status409Conflict },
    };

    [Theory]
    [MemberData(nameof(KnownExceptions))]
    public async Task TryHandleAsync_WithKnownException_ReturnsMatchingStatusAndMessage(Exception exception, int expectedStatus)
    {
        var (status, body) = await HandleAsync(exception);

        Assert.Equal(expectedStatus, status);
        Assert.Equal(exception.Message, body.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task TryHandleAsync_WithUnexpectedException_Returns500WithoutLeakingDetails()
    {
        var (status, body) = await HandleAsync(new InvalidOperationException("connection string secret"));

        Assert.Equal(StatusCodes.Status500InternalServerError, status);
        Assert.DoesNotContain("secret", body.GetRawText());
    }

    private static async Task<(int Status, JsonElement Body)> HandleAsync(Exception exception)
    {
        var services = new ServiceCollection().AddLogging().AddProblemDetails().BuildServiceProvider();
        var context = new DefaultHttpContext { RequestServices = services };
        context.Response.Body = new MemoryStream();

        var handler = new GlobalExceptionHandler(
            services.GetRequiredService<IProblemDetailsService>(), NullLogger<GlobalExceptionHandler>.Instance);

        Assert.True(await handler.TryHandleAsync(context, exception, CancellationToken.None));

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        return (context.Response.StatusCode, document.RootElement.Clone());
    }
}
