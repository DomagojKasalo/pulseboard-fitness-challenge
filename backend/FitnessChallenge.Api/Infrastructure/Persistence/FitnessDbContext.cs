using FitnessChallenge.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessChallenge.Api.Infrastructure.Persistence;

public class FitnessDbContext : DbContext
{
    public FitnessDbContext(DbContextOptions<FitnessDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Activity> Activities => Set<Activity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100)
                .UseCollation("NOCASE");

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100)
                .UseCollation("NOCASE");

            entity.HasIndex(u => new { u.FirstName, u.LastName }).IsUnique();
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Sport)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(a => a.DistanceKm).HasColumnType("decimal(9,3)");
            entity.Property(a => a.Duration).HasMaxLength(16);

            entity.HasOne(a => a.User)
                .WithMany(u => u.Activities)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => new { a.UserId, a.OccurredAt });
        });
    }
}
