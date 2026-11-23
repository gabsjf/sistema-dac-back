
using Ampara.Application.Adoptions;
using Ampara.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ampara.Api.Controllers;

[ApiController]
[Route("api/adoptions")]
[Authorize]
public class AdoptionsController(
    AdoptionService adoptions) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Adopter)]
    public async Task<ActionResult<AdoptionResponse>> Submit(
        SubmitAdoptionRequest request,
        CancellationToken cancellationToken)
    {
        var created = await adoptions.SubmitAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(Get),
            new { id = created.Id },
            created);
    }

    // The logged adopter's requests.
    [HttpGet("mine")]
    [Authorize(Roles = Roles.Adopter)]
    public Task<IReadOnlyList<AdoptionResponse>> Mine(
        CancellationToken cancellationToken) =>
        adoptions.ListMineAsync(cancellationToken);

    [HttpGet]
    [Authorize(Roles = Roles.Staff)]
    public Task<PagedResult<AdoptionResponse>> List(
        [FromQuery] AdoptionListQuery query,
        CancellationToken cancellationToken) =>
        adoptions.ListAsync(query, cancellationToken);

    // The NGO opens any request; an adopter only their own.
    [HttpGet("{id:guid}")]
    public Task<AdoptionResponse> Get(
        Guid id,
        CancellationToken cancellationToken) =>
        adoptions.GetAsync(id, cancellationToken);

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = Roles.Staff)]
    public Task<AdoptionResponse> Approve(
        Guid id,
        CancellationToken cancellationToken) =>
        adoptions.ApproveAsync(id, cancellationToken);

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = Roles.Staff)]
    public Task<AdoptionResponse> Reject(
        Guid id,
        RejectAdoptionRequest request,
        CancellationToken cancellationToken) =>
        adoptions.RejectAsync(
            id, request, cancellationToken);

    // Undo an approved adoption.
    [HttpPost("{id:guid}/undo-approval")]
    [Authorize(Roles = Roles.Staff)]
    public Task<AdoptionResponse> UndoApproval(
        Guid id,
        CancellationToken cancellationToken) =>
        adoptions.UndoApprovalAsync(id, cancellationToken);
}
