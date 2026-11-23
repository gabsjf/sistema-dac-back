using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Common;
using Ampara.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Ampara.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<AdoptionRequest> AdoptionRequests => Set<AdoptionRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
