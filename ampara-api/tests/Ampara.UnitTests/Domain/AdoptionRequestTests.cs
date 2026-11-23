using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Common;
using Ampara.Domain.Enums;
using static Ampara.UnitTests.TestData;

namespace Ampara.UnitTests.Domain;

public class AdoptionRequestTests
{
    [Fact]
    public void Submit_ForAvailableAnimal_IsPending()
    {
        var animal = NewAnimal();
        var adopter = NewAdopter();

        var request = AdoptionRequest.Submit(animal, adopter, Details(), acceptsTerms: true, Now);

        Assert.Equal(AdoptionStatusEnum.Pending, request.Status);
        Assert.Equal(animal.Id, request.AnimalId);
        Assert.Equal(adopter.Id, request.AdopterId);
        Assert.Equal(Now, request.CreatedAt);
        Assert.Null(request.DecidedAt);
    }

    [Fact]
    public void Submit_ForAnimalInTreatment_Throws()
    {
        Assert.Throws<DomainException>(() =>
            AdoptionRequest.Submit(NewAnimal(AnimalStatusEnum.InTreatment), NewAdopter(), Details(), acceptsTerms: true, Now));
    }

    [Fact]
    public void Submit_WithoutAcceptingTerms_Throws()
    {
        Assert.Throws<DomainException>(() =>
            AdoptionRequest.Submit(NewAnimal(), NewAdopter(), Details(), acceptsTerms: false, Now));
    }

    [Fact]
    public void Submit_WithoutHousing_Throws()
    {
        var details = Details() with { Housing = "" };

        var error = Assert.Throws<DomainException>(() =>
            AdoptionRequest.Submit(NewAnimal(), NewAdopter(), details, acceptsTerms: true, Now));

        Assert.Equal("O campo moradia é obrigatório.", error.Message);
    }

    [Fact]
    public void Approve_MarksRequestApprovedAndAnimalAdopted()
    {
        var animal = NewAnimal();
        var request = PendingRequest(animal);

        request.Approve(Now.AddDays(1));

        Assert.Equal(AdoptionStatusEnum.Approved, request.Status);
        Assert.Equal(Now.AddDays(1), request.DecidedAt);
        Assert.Equal(AnimalStatusEnum.Adopted, animal.Status);
    }

    [Fact]
    public void Approve_WhenAnimalWasAlreadyAdoptedByAnotherFamily_Throws()
    {
        var animal = NewAnimal();
        var first = PendingRequest(animal);
        var second = PendingRequest(animal, NewAdopter("outra@exemplo.com"));
        first.Approve(Now);

        Assert.Throws<DomainException>(() => second.Approve(Now));
        Assert.Equal(AdoptionStatusEnum.Pending, second.Status);
    }

    [Fact]
    public void Reject_StoresReasonAndKeepsAnimalAvailable()
    {
        var animal = NewAnimal();
        var request = PendingRequest(animal);

        request.Reject("  Rotina incompatível com um cão de porte médio.  ", Now);

        Assert.Equal(AdoptionStatusEnum.Rejected, request.Status);
        Assert.Equal("Rotina incompatível com um cão de porte médio.", request.RejectionReason);
        Assert.True(animal.IsAvailable);
    }

    [Fact]
    public void Reject_WithoutReason_Throws()
    {
        Assert.Throws<DomainException>(() => PendingRequest().Reject(" ", Now));
    }

    [Fact]
    public void Decide_AlreadyDecidedRequest_Throws()
    {
        var request = PendingRequest();
        request.Approve(Now);

        var error = Assert.Throws<DomainException>(() => request.Reject("Mudança de ideia.", Now));

        Assert.Equal("Esta solicitação já foi decidida.", error.Message);
    }
}
