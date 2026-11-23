using Ampara.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using Ampara.Application.Common;
using Ampara.Domain.Animals;
using Ampara.Domain.Common;

namespace Ampara.Application.Animals;

public sealed record AnimalRequest
{
    [Required, MaxLength(Animal.NameMaxLength)] public string Name { get; init; } = string.Empty;
    public SpeciesEnum Species { get; init; }
    public SexEnum Sex { get; init; }
    public AnimalSizeEnum Size { get; init; }
    [Range(0, Animal.MaxAgeMonths)] public int ApproximateAgeMonths { get; init; }
    public DateOnly IntakeDate { get; init; }
    [MaxLength(Animal.DescriptionMaxLength)] public string? Description { get; init; }
    public bool IsVaccinated { get; init; }
    public bool IsNeutered { get; init; }
    public bool IsDewormed { get; init; }
    public AnimalStatusEnum Status { get; init; } = AnimalStatusEnum.Available;

    public AnimalProfile ToProfile() => new(
        Name, Species, Sex, Size, ApproximateAgeMonths, IntakeDate, Description, IsVaccinated, IsNeutered, IsDewormed);
}

public sealed record AnimalListQuery
{
    public string? Search { get; init; }
    public SpeciesEnum? Species { get; init; }
    public AnimalStatusEnum? Status { get; init; }
    public int? Page { get; init; }
    public int? PageSize { get; init; }
}

public sealed record AnimalResponse(
    Guid Id,
    string Name,
    SpeciesEnum Species,
    SexEnum Sex,
    AnimalSizeEnum Size,
    int ApproximateAgeMonths,
    DateOnly IntakeDate,
    string? Description,
    bool IsVaccinated,
    bool IsNeutered,
    bool IsDewormed,
    string? PhotoUrl,
    AnimalStatusEnum Status)
{
    public static AnimalResponse From(Animal a) => new(
        a.Id, a.Name, a.Species, a.Sex, a.Size, a.ApproximateAgeMonths, a.IntakeDate, a.Description,
        a.IsVaccinated, a.IsNeutered, a.IsDewormed, a.PhotoUrl, a.Status);
}

// Animal registry for the team, plus the public catalog (available animals only).
public class AnimalService(IAnimalRepository animals, IUnitOfWork unitOfWork, IFileStorage storage, TimeProvider clock)
{
    public const string PhotoFolder = "animals";

    public async Task<PagedResult<AnimalResponse>> ListAsync(AnimalListQuery query, CancellationToken cancellationToken = default)
    {
        var (page, pageSize) = Paging.Normalize(query.Page, query.PageSize);
        var (items, total) = await animals.ListAsync(
            new AnimalFilter(query.Search?.Trim(), query.Species, query.Status, page, pageSize), cancellationToken);

        return new PagedResult<AnimalResponse>(items.Select(AnimalResponse.From).ToList(), page, pageSize, total);
    }

    public async Task<AnimalResponse> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        AnimalResponse.From(await FindAsync(id, cancellationToken));

    public async Task<AnimalResponse> CreateAsync(AnimalRequest request, CancellationToken cancellationToken = default)
    {
        var animal = Animal.Register(request.ToProfile(), request.Status, clock.GetUtcNow());
        animals.Add(animal);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return AnimalResponse.From(animal);
    }

    public async Task<AnimalResponse> UpdateAsync(Guid id, AnimalRequest request, CancellationToken cancellationToken = default)
    {
        var animal = await FindAsync(id, cancellationToken);
        animal.Update(request.ToProfile(), request.Status, clock.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return AnimalResponse.From(animal);
    }

    public async Task<AnimalResponse> UploadPhotoAsync(Guid id, Stream content, long length, CancellationToken cancellationToken = default)
    {
        if (length is <= 0 or > ImageFile.MaxSizeBytes)
            throw new DomainException("Envie uma foto de até 5 MB.");

        var animal = await FindAsync(id, cancellationToken);

        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);

        var extension = ImageFile.DetectExtension(buffer.GetBuffer().AsSpan(0, (int)Math.Min(buffer.Length, 8)))
            ?? throw new DomainException("A foto precisa ser JPG ou PNG.");

        buffer.Position = 0;
        var previous = animal.PhotoUrl;
        var url = await storage.SaveAsync(buffer, PhotoFolder, $"{animal.Id:N}-{clock.GetUtcNow().ToUnixTimeSeconds()}{extension}", cancellationToken);

        animal.SetPhoto(url);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (previous is not null)
            await storage.DeleteAsync(previous, cancellationToken);

        return AnimalResponse.From(animal);
    }

    public async Task<IReadOnlyList<AnimalResponse>> ListAvailableAsync(SpeciesEnum? species, CancellationToken cancellationToken = default) =>
        (await animals.ListAvailableAsync(species, cancellationToken)).Select(AnimalResponse.From).ToList();

    public async Task<AnimalResponse> GetAvailableAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var animal = await animals.GetByIdAsync(id, cancellationToken);

        if (animal is null || !animal.IsAvailable)
            throw new NotFoundException("Este animal não está disponível para adoção.");

        return AnimalResponse.From(animal);
    }

    private async Task<Animal> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await animals.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException("Animal não encontrado.");
}
