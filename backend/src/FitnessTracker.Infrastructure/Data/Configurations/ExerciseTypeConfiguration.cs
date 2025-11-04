using FitnessTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Data.Configurations;

public class ExerciseTypeConfiguration : IEntityTypeConfiguration<ExerciseType>
{
    public void Configure(EntityTypeBuilder<ExerciseType> builder)
    {
        builder.ToTable("ExerciseTypes");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(100);
        
        builder.Property(e => e.Description)
               .HasMaxLength(500);
        
        builder.Property(e => e.DefaultMET)
               .IsRequired()
               .HasPrecision(5, 2);
        
        builder.Property(e => e.IsSystemDefault)
               .IsRequired()
               .HasDefaultValue(false);
        
        builder.Property(e => e.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);
        
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.IsSystemDefault);
        
        // Many-to-many relationship with Equipment
        builder.HasMany(e => e.Equipments)
               .WithMany(eq => eq.ExerciseTypes);
        
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
