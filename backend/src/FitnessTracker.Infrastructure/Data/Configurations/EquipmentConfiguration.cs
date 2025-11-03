using FitnessTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Data.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipments");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(100);
        
        builder.Property(e => e.IsActive)
               .IsRequired()
               .HasDefaultValue(true);
    }
}
