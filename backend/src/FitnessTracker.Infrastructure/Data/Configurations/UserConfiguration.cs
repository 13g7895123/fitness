using FitnessTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.LineUserId)
               .IsRequired()
               .HasMaxLength(100);
        
        builder.HasIndex(u => u.LineUserId)
               .IsUnique();
        
        builder.Property(u => u.DisplayName)
               .IsRequired()
               .HasMaxLength(100);
        
        builder.Property(u => u.PictureUrl)
               .HasMaxLength(500);
        
        builder.Property(u => u.CreatedAt)
               .IsRequired();
        
        builder.Property(u => u.UpdatedAt)
               .IsRequired();
    }
}
