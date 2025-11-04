using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FitnessTracker.Infrastructure.Data;

public class FitnessTrackerDbContextFactory : IDesignTimeDbContextFactory<FitnessTrackerDbContext>
{
    public FitnessTrackerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FitnessTrackerDbContext>();
        optionsBuilder.UseNpgsql("Host=postgres;Port=5432;Database=fitness_tracker;Username=postgres;Password=postgres");

        return new FitnessTrackerDbContext(optionsBuilder.Options);
    }
}
