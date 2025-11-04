using FitnessTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Data;

public static class DataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // TODO: Update seed data to match new schema
        // SeedExerciseTypes(modelBuilder);
        // SeedEquipments(modelBuilder);
    }

    /*private static void SeedExerciseTypes(ModelBuilder modelBuilder)
    {
        var exerciseTypes = new[]
        {
            new ExerciseType
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "跑步",
                Category = "有氧",
                IsCustom = false,
                UserId = null,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ExerciseType
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "游泳",
                Category = "有氧",
                IsCustom = false,
                UserId = null,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ExerciseType
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "騎自行車",
                Category = "有氧",
                IsCustom = false,
                UserId = null,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ExerciseType
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "重量訓練",
                Category = "重訓",
                IsCustom = false,
                UserId = null,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ExerciseType
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "瑜伽",
                Category = "伸展",
                IsCustom = false,
                UserId = null,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ExerciseType
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "皮拉提斯",
                Category = "伸展",
                IsCustom = false,
                UserId = null,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ExerciseType
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Name = "其他",
                Category = "其他",
                IsCustom = false,
                UserId = null,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        modelBuilder.Entity<ExerciseType>().HasData(exerciseTypes);
    }

    private static void SeedEquipments(ModelBuilder modelBuilder)
    {
        var equipments = new[]
        {
            new Equipment
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "跑步機",
                IsActive = true
            },
            new Equipment
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "啞鈴",
                IsActive = true
            },
            new Equipment
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "槓鈴",
                IsActive = true
            },
            new Equipment
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Name = "瑜伽墊",
                IsActive = true
            },
            new Equipment
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Name = "飛輪",
                IsActive = true
            },
            new Equipment
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Name = "其他",
                IsActive = true
            }
        };

        modelBuilder.Entity<Equipment>().HasData(equipments);
    }*/
}
