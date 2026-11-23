using Ampara.Application.Dashboard;
using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Enums;
using NSubstitute;
using static Ampara.UnitTests.TestData;

namespace Ampara.UnitTests.Application;

public class DashboardServiceTests
{
    private readonly IAnimalRepository _animals = Substitute.For<IAnimalRepository>();
    private readonly IAdoptionRequestRepository _requests = Substitute.For<IAdoptionRequestRepository>();
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _service = new DashboardService(_animals, _requests, Clock());
        _animals.CountByStatusAsync(Arg.Any<CancellationToken>())
            .Returns(new Dictionary<AnimalStatusEnum, int> { [AnimalStatusEnum.Available] = 3, [AnimalStatusEnum.Adopted] = 2 });
        _animals.ListIntakeDatesSinceAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>()).Returns([]);
        _requests.ListApprovalDatesSinceAsync(Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>()).Returns([]);
        _requests.CountPendingAsync(Arg.Any<CancellationToken>()).Returns(4);
    }

    [Fact]
    public async Task GetAsync_CountsAnimalsByStatusAndPendingRequests()
    {
        var dashboard = await _service.GetAsync();

        Assert.Equal(5, dashboard.TotalAnimals);
        Assert.Equal(3, dashboard.Available);
        Assert.Equal(0, dashboard.InTreatment);
        Assert.Equal(2, dashboard.Adopted);
        Assert.Equal(4, dashboard.PendingRequests);
    }

    [Fact]
    public async Task GetAsync_GroupsIntakesAndAdoptionsInTheLastSixMonths()
    {
        // Clock: 20/11/2026, so the window is jun..nov 2026.
        _animals.ListIntakeDatesSinceAsync(new DateOnly(2026, 6, 1), Arg.Any<CancellationToken>())
            .Returns([new DateOnly(2026, 6, 3), new DateOnly(2026, 11, 1), new DateOnly(2026, 11, 18)]);
        _requests.ListApprovalDatesSinceAsync(new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero), Arg.Any<CancellationToken>())
            .Returns([new DateTimeOffset(2026, 9, 10, 12, 0, 0, TimeSpan.Zero)]);

        var months = (await _service.GetAsync()).LastSixMonths;

        Assert.Equal(DashboardService.Months, months.Count);
        Assert.Equal(new MonthlyPoint(2026, 6, Intakes: 1, Adoptions: 0), months[0]);
        Assert.Equal(new MonthlyPoint(2026, 9, Intakes: 0, Adoptions: 1), months[3]);
        Assert.Equal(new MonthlyPoint(2026, 11, Intakes: 2, Adoptions: 0), months[5]);
    }
}
