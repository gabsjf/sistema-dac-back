using Ampara.Application.Animals;
using Ampara.Domain.Animals;
using Ampara.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Ampara.Api.Controllers;

// Adoption catalog for the public site: only animals available for adoption, no login needed.
[ApiController]
[Route("api/public/animals")]
public class PublicController(AnimalService animals) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<AnimalResponse>> List([FromQuery] SpeciesEnum? species, CancellationToken cancellationToken) =>
        animals.ListAvailableAsync(species, cancellationToken);

    [HttpGet("{id:guid}")]
    public Task<AnimalResponse> Get(Guid id, CancellationToken cancellationToken) => animals.GetAvailableAsync(id, cancellationToken);
}
