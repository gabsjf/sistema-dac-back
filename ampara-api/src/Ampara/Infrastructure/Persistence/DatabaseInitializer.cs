using Ampara.Application.Common;
using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Enums;
using Ampara.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Ampara.Infrastructure.Persistence;

// Development only: applies the migrations and inserts demo data (the SuperAdmin, an adopter, 7 animals and
// 4 adoption requests), so the dashboard already has something to show.
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var db = provider.GetRequiredService<AppDbContext>();
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DatabaseInitializer));

        try
        {
            await db.Database.MigrateAsync();

            if (await db.Users.AnyAsync())
                return;

            var password = provider.GetRequiredService<IConfiguration>()["Seed:DemoPassword"];
            if (string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning("Seed:DemoPassword não configurado; os dados de demonstração não foram criados.");
                return;
            }

            await SeedAsync(db, provider.GetRequiredService<IPasswordHasher>(), password, provider.GetRequiredService<TimeProvider>().GetUtcNow());
        }
        catch (Exception ex) when (ex is Npgsql.NpgsqlException or InvalidOperationException)
        {
            // Lets the API start (Swagger) even before PostgreSQL is running.
            logger.LogError(ex, "Não foi possível preparar o banco. Confira a connection string e se o PostgreSQL está rodando.");
        }
    }

    private static async Task SeedAsync(AppDbContext db, IPasswordHasher hasher, string password, DateTimeOffset now)
    {
        var hash = hasher.Hash(password);
        var admin = User.Register("Mariana Costa", "admin@ampara.local", null, UserRoleEnum.SuperAdmin, hash, now.AddDays(-150));
        var adopter = User.Register("Ana Souza", "adotante@ampara.local", "(11) 99999-0123", UserRoleEnum.Adopter, hash, now.AddDays(-150));
        db.Users.AddRange(admin, adopter);

        var today = DateOnly.FromDateTime(now.UtcDateTime);
        Animal Register(string name, SpeciesEnum species, SexEnum sex, AnimalSizeEnum size, int ageMonths, int daysAgo, string description, AnimalStatusEnum status = AnimalStatusEnum.Available) =>
            Animal.Register(new AnimalProfile(name, species, sex, size, ageMonths, today.AddDays(-daysAgo), description, true, true, true), status, now.AddDays(-daysAgo));

        var luna = Register("Luna", SpeciesEnum.Canine, SexEnum.Female, AnimalSizeEnum.Medium, 24, 10, "Carinhosa e companheira. Adora passeios tranquilos.");
        var chico = Register("Chico", SpeciesEnum.Canine, SexEnum.Male, AnimalSizeEnum.Medium, 36, 20, "Brincalhão e cheio de energia.");
        var mel = Register("Mel", SpeciesEnum.Feline, SexEnum.Female, AnimalSizeEnum.Small, 18, 35, "Tranquila e curiosa, gosta de colo.");
        var thor = Register("Thor", SpeciesEnum.Canine, SexEnum.Male, AnimalSizeEnum.Large, 48, 40, "Grande e dócil, em tratamento de pele.", AnimalStatusEnum.InTreatment);
        var nina = Register("Nina", SpeciesEnum.Feline, SexEnum.Female, AnimalSizeEnum.Small, 12, 45, "Curiosa e tranquila, adora janelas.");
        var bento = Register("Bento", SpeciesEnum.Canine, SexEnum.Male, AnimalSizeEnum.Small, 60, 95, "Calmo e companheiro.");
        var pipoca = Register("Pipoca", SpeciesEnum.Feline, SexEnum.Male, AnimalSizeEnum.Small, 8, 140, "Agitado e muito carinhoso.");
        db.Animals.AddRange(luna, chico, mel, thor, nina, bento, pipoca);

        var details = new ApplicantDetails(
            "(11) 99999-0123",
            "Apartamento próprio, com telas de proteção.",
            "Trabalho híbrido, com companhia em casa todos os dias.",
            "Quero oferecer um lar seguro e cheio de carinho.");

        AdoptionRequest Approved(Animal animal, int daysAgo)
        {
            var request = AdoptionRequest.Submit(animal, adopter, details, true, now.AddDays(-daysAgo - 3));
            request.Approve(now.AddDays(-daysAgo));
            return request;
        }

        db.AdoptionRequests.AddRange(
            Approved(pipoca, 110),
            Approved(bento, 50),
            Approved(nina, 3),
            AdoptionRequest.Submit(luna, adopter, details, true, now.AddHours(-20)));

        await db.SaveChangesAsync();
    }
}
