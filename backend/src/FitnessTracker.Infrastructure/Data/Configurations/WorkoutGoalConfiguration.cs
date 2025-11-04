using FitnessTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Data.Configurations;

public class WorkoutGoalConfiguration : IEntityTypeConfiguration<WorkoutGoal>
{
    public void Configure(EntityTypeBuilder<WorkoutGoal> builder)
    {
        builder.ToTable("WorkoutGoals");
        builder.HasKey(g => g.Id);
        
        builder.Property(g => g.WeeklyMinutes);
        
        builder.Property(g => g.WeeklyCalories)
               .HasPrecision(18, 2);
        
        builder.Property(g => g.StartDate)
               .IsRequired()
               .HasColumnType("date");
        
        builder.Property(g => g.EndDate)
               .HasColumnType("date");
        
        builder.Property(g => g.IsActive)
               .IsRequired()
               .HasDefaultValue(true);
        
        builder.HasIndex(g => new { g.UserId, g.IsActive });
        
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(g => g.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
