using Ampara.Application.Common;
using Ampara.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Ampara.Api;

// Turns known exceptions into RFC 7807 ProblemDetails responses.
// Unknown exceptions become a generic 500 without leaking details.
public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            DomainException e => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada", e.Message),
            NotFoundException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
            UnauthorizedException e => (StatusCodes.Status401Unauthorized, "Não autenticado", e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, "Conflito", e.Message),
            _ => (StatusCodes.Status500InternalServerError, "Erro inesperado", "Ocorreu um erro inesperado. Tente novamente."),
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Status = status, Title = title, Detail = detail },
        });
    }
}
