using FitnessTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Data.Configurations;

public class WorkoutSetConfiguration : IEntityTypeConfiguration<WorkoutSet>
{
    public void Configure(EntityTypeBuilder<WorkoutSet> builder)
    {
        builder.ToTable("WorkoutSets");

        builder.HasKey(ws => ws.Id);

        builder.Property(ws => ws.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(ws => ws.WorkoutRecordId)
            .HasColumnName("WorkoutRecordId")
            .IsRequired();

        builder.Property(ws => ws.SetNumber)
            .HasColumnName("SetNumber")
            .IsRequired();

        builder.Property(ws => ws.Repetitions)
            .HasColumnName("Repetitions")
            .IsRequired();

        builder.Property(ws => ws.Weight)
            .HasColumnName("Weight")
            .HasColumnType("decimal(10,2)");

        builder.Property(ws => ws.Notes)
            .HasColumnName("Notes")
            .HasMaxLength(500);

        builder.Property(ws => ws.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(ws => ws.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .IsRequired();

        builder.Property(ws => ws.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasDefaultValue(false);

        // 關聯配置
        builder.HasOne(ws => ws.WorkoutRecord)
            .WithMany(wr => wr.Sets)
            .HasForeignKey(ws => ws.WorkoutRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        // 索引
        builder.HasIndex(ws => ws.WorkoutRecordId);
        builder.HasIndex(ws => new { ws.WorkoutRecordId, ws.SetNumber });
    }
}
