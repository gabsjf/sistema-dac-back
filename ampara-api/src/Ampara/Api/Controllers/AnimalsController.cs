using Ampara.Application.Animals;
using Ampara.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ampara.Api.Controllers;

// Animal registry, used by the NGO team.
[ApiController]
[Route("api/animals")]
[Authorize(Roles = Roles.Staff)]
public class AnimalsController(AnimalService animals) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<AnimalResponse>> List([FromQuery] AnimalListQuery query, CancellationToken cancellationToken) =>
        animals.ListAsync(query, cancellationToken);

    [HttpGet("{id:guid}")]
    public Task<AnimalResponse> Get(Guid id, CancellationToken cancellationToken) => animals.GetAsync(id, cancellationToken);

    [HttpPost]
    public async Task<ActionResult<AnimalResponse>> Create(AnimalRequest request, CancellationToken cancellationToken)
    {
        var created = await animals.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public Task<AnimalResponse> Update(Guid id, AnimalRequest request, CancellationToken cancellationToken) =>
        animals.UpdateAsync(id, request, cancellationToken);

    // Main photo: JPG or PNG up to 5 MB, sent as the multipart field "file".
    [HttpPost("{id:guid}/photo")]
    [RequestSizeLimit(ImageFile.MaxSizeBytes + 64 * 1024)]
    public async Task<AnimalResponse> UploadPhoto(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        return await animals.UploadPhotoAsync(id, stream, file.Length, cancellationToken);
    }
}
