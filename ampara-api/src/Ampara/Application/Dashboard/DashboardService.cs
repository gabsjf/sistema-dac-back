using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Enums;

namespace Ampara.Application.Dashboard;

public sealed record MonthlyPoint(int Year, int Month, int Intakes, int Adoptions);

public sealed record DashboardResponse(
    int TotalAnimals,
    int Available,
    int InTreatment,
    int Adopted,
    int PendingRequests,
    IReadOnlyList<MonthlyPoint> LastSixMonths);

// The indicators the NGO follows: animals by situation and intakes vs. adoptions per month.
public class DashboardService(IAnimalRepository animals, IAdoptionRequestRepository requests, TimeProvider clock)
{
    public const int Months = 6;

    public async Task<DashboardResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        var now = clock.GetUtcNow();
        var firstMonth = new DateOnly(now.Year, now.Month, 1).AddMonths(-(Months - 1));

        var byStatus = await animals.CountByStatusAsync(cancellationToken);
        var intakes = await animals.ListIntakeDatesSinceAsync(firstMonth, cancellationToken);
        var approvals = await requests.ListApprovalDatesSinceAsync(
            new DateTimeOffset(firstMonth.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero), cancellationToken);
        var pending = await requests.CountPendingAsync(cancellationToken);

        var lastSixMonths = Enumerable.Range(0, Months)
            .Select(firstMonth.AddMonths)
            .Select(month => new MonthlyPoint(
                month.Year,
                month.Month,
                intakes.Count(d => d.Year == month.Year && d.Month == month.Month),
                approvals.Count(d => d.Year == month.Year && d.Month == month.Month)))
            .ToList();

        int Count(AnimalStatusEnum status) => byStatus.TryGetValue(status, out var count) ? count : 0;

        return new DashboardResponse(
            byStatus.Values.Sum(),
            Count(AnimalStatusEnum.Available),
            Count(AnimalStatusEnum.InTreatment),
            Count(AnimalStatusEnum.Adopted),
            pending,
            lastSixMonths);
    }
}
