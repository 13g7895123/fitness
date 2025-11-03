using FitnessTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Data.Configurations;

public class WorkoutRecordConfiguration : IEntityTypeConfiguration<WorkoutRecord>
{
    public void Configure(EntityTypeBuilder<WorkoutRecord> builder)
    {
        builder.ToTable("WorkoutRecords");
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Date)
               .IsRequired()
               .HasColumnType("date");
        
        builder.Property(w => w.DurationMinutes)
               .IsRequired();
        
        builder.Property(w => w.CaloriesBurned)
               .IsRequired()
               .HasPrecision(18, 2);
        
        builder.Property(w => w.Weight)
               .HasPrecision(5, 2);
        
        builder.Property(w => w.Notes)
               .HasMaxLength(500);
        
        builder.Property(w => w.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);
        
        builder.HasIndex(w => new { w.UserId, w.Date, w.ExerciseTypeId })
               .IsUnique()
               .HasFilter("\"IsDeleted\" = false");
        
        builder.HasIndex(w => new { w.UserId, w.Date });
        builder.HasIndex(w => w.Date);
        
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(w => w.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<ExerciseType>()
               .WithMany()
               .HasForeignKey(w => w.ExerciseTypeId)
               .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<Equipment>()
               .WithMany()
               .HasForeignKey(w => w.EquipmentId)
               .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasQueryFilter(w => !w.IsDeleted);
    }
}
