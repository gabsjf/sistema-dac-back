using Ampara.Application.Animals;
using Ampara.Application.Common;
using Ampara.Domain.Animals;
using Ampara.Domain.Common;
using Ampara.Domain.Enums;
using NSubstitute;
using static Ampara.UnitTests.TestData;

namespace Ampara.UnitTests.Application;

public class AnimalServiceTests
{
    private static readonly byte[] PngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];

    private readonly IAnimalRepository _animals = Substitute.For<IAnimalRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IFileStorage _storage = Substitute.For<IFileStorage>();
    private readonly AnimalService _service;

    public AnimalServiceTests() => _service = new AnimalService(_animals, _unitOfWork, _storage, Clock());

    private static AnimalRequest Request(string name = "Luna", AnimalStatusEnum status = AnimalStatusEnum.Available) => new()
    {
        Name = name,
        Species = SpeciesEnum.Canine,
        Sex = SexEnum.Female,
        Size = AnimalSizeEnum.Medium,
        ApproximateAgeMonths = 24,
        IntakeDate = new DateOnly(2026, 11, 10),
        IsVaccinated = true,
        Status = status,
    };

    [Fact]
    public async Task CreateAsync_RegistersAndSaves()
    {
        var created = await _service.CreateAsync(Request());

        _animals.Received(1).Add(Arg.Is<Animal>(a => a.Name == "Luna" && a.Status == AnimalStatusEnum.Available));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        Assert.Equal("Luna", created.Name);
    }

    [Fact]
    public async Task GetAsync_WhenMissing_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateAsync_AppliesChangesAndSaves()
    {
        var animal = NewAnimal();
        _animals.GetByIdAsync(animal.Id, Arg.Any<CancellationToken>()).Returns(animal);

        var updated = await _service.UpdateAsync(animal.Id, Request("Luna Maria", AnimalStatusEnum.InTreatment));

        Assert.Equal("Luna Maria", updated.Name);
        Assert.Equal(AnimalStatusEnum.InTreatment, updated.Status);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListAsync_ClampsPaging()
    {
        _animals.ListAsync(Arg.Any<AnimalFilter>(), Arg.Any<CancellationToken>()).Returns((Array.Empty<Animal>(), 0));

        var page = await _service.ListAsync(new AnimalListQuery { Search = " lu ", PageSize = 500 });

        await _animals.Received(1).ListAsync(
            Arg.Is<AnimalFilter>(f => f.Search == "lu" && f.Page == 1 && f.PageSize == Paging.MaxPageSize), Arg.Any<CancellationToken>());
        Assert.Equal(Paging.MaxPageSize, page.PageSize);
    }

    [Fact]
    public async Task UploadPhotoAsync_WithPng_SavesAndReplacesPreviousPhoto()
    {
        var animal = NewAnimal();
        animal.SetPhoto("/uploads/animals/old.png");
        _animals.GetByIdAsync(animal.Id, Arg.Any<CancellationToken>()).Returns(animal);
        _storage.SaveAsync(Arg.Any<Stream>(), AnimalService.PhotoFolder, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("/uploads/animals/new.png");

        var result = await _service.UploadPhotoAsync(animal.Id, new MemoryStream(PngHeader), PngHeader.Length);

        Assert.Equal("/uploads/animals/new.png", result.PhotoUrl);
        await _storage.Received(1).SaveAsync(Arg.Any<Stream>(), AnimalService.PhotoFolder,
            Arg.Is<string>(name => name.EndsWith(".png")), Arg.Any<CancellationToken>());
        await _storage.Received(1).DeleteAsync("/uploads/animals/old.png", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UploadPhotoAsync_WithFileThatIsNotAnImage_Throws()
    {
        var animal = NewAnimal();
        _animals.GetByIdAsync(animal.Id, Arg.Any<CancellationToken>()).Returns(animal);
        var content = "MZ not an image"u8.ToArray();

        var error = await Assert.ThrowsAsync<DomainException>(() =>
            _service.UploadPhotoAsync(animal.Id, new MemoryStream(content), content.Length));

        Assert.Equal("A foto precisa ser JPG ou PNG.", error.Message);
        await _storage.DidNotReceiveWithAnyArgs().SaveAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task UploadPhotoAsync_LargerThan5Mb_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _service.UploadPhotoAsync(Guid.NewGuid(), Stream.Null, ImageFile.MaxSizeBytes + 1));
    }

    [Fact]
    public async Task GetAvailableAsync_ForAdoptedAnimal_ThrowsNotFound()
    {
        var animal = NewAnimal();
        animal.MarkAsAdopted();
        _animals.GetByIdAsync(animal.Id, Arg.Any<CancellationToken>()).Returns(animal);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAvailableAsync(animal.Id));
    }
}
