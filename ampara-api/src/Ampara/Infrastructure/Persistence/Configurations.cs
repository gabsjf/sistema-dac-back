using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ampara.Infrastructure.Persistence;

// Enums are stored as text so the tables are readable straight from the database.

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.Property(u => u.FullName).HasMaxLength(User.NameMaxLength).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(User.EmailMaxLength).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Phone).HasMaxLength(User.PhoneMaxLength);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        builder.Property(u => u.PasswordHash).HasMaxLength(200).IsRequired();
    }
}

public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
{
    public void Configure(EntityTypeBuilder<Animal> builder)
    {
        builder.ToTable("Animals");
        builder.Property(a => a.Name).HasMaxLength(Animal.NameMaxLength).IsRequired();
        builder.Property(a => a.Species).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Sex).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Size).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Description).HasMaxLength(Animal.DescriptionMaxLength);
        builder.Property(a => a.PhotoUrl).HasMaxLength(Animal.PhotoUrlMaxLength);
        builder.HasIndex(a => a.Status);
    }
}

public class AdoptionRequestConfiguration : IEntityTypeConfiguration<AdoptionRequest>
{
    public void Configure(EntityTypeBuilder<AdoptionRequest> builder)
    {
        builder.ToTable("AdoptionRequests");
        builder.HasOne(r => r.Animal).WithMany().HasForeignKey(r => r.AnimalId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.Adopter).WithMany().HasForeignKey(r => r.AdopterId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(r => r.Phone).HasMaxLength(AdoptionRequest.PhoneMaxLength).IsRequired();
        builder.Property(r => r.Housing).HasMaxLength(AdoptionRequest.TextMaxLength).IsRequired();
        builder.Property(r => r.Routine).HasMaxLength(AdoptionRequest.TextMaxLength).IsRequired();
        builder.Property(r => r.Message).HasMaxLength(AdoptionRequest.TextMaxLength);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.RejectionReason).HasMaxLength(AdoptionRequest.ReasonMaxLength);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => new { r.AdopterId, r.AnimalId });
    }
}
