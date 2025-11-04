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
        
        builder.Property(e => e.Category)
               .IsRequired()
               .HasMaxLength(50);
        
        builder.Property(e => e.IsCustom)
               .IsRequired()
               .HasDefaultValue(false);
        
        builder.Property(e => e.IsActive)
               .IsRequired()
               .HasDefaultValue(true);
        
        builder.HasIndex(e => new { e.UserId, e.IsActive });
        builder.HasIndex(e => e.Name);
        
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
