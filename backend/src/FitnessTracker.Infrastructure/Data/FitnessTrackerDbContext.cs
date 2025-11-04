using FitnessTracker.Core.Entities;
using FitnessTracker.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Data;

public class FitnessTrackerDbContext : DbContext
{
    public FitnessTrackerDbContext(DbContextOptions<FitnessTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<ExerciseType> ExerciseTypes => Set<ExerciseType>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<WorkoutRecord> WorkoutRecords => Set<WorkoutRecord>();
    public DbSet<WorkoutGoal> WorkoutGoals => Set<WorkoutGoal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ExerciseTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentConfiguration());
        modelBuilder.ApplyConfiguration(new WorkoutRecordConfiguration());
        modelBuilder.ApplyConfiguration(new WorkoutGoalConfiguration());

        // Seed data
        DataSeeder.Seed(modelBuilder);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Property("CreatedAt").CurrentValue == null ||
                    (DateTime)entry.Property("CreatedAt").CurrentValue == default)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            }

            if (entry.Property("UpdatedAt") != null)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
