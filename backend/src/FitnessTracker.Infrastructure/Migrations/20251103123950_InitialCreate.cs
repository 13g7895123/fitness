using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitnessTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LineUserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PictureUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseTypes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeeklyMinutes = table.Column<int>(type: "integer", nullable: true),
                    WeeklyCalories = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    ExerciseTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    CaloriesBurned = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutRecords_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkoutRecords_ExerciseTypes_ExerciseTypeId",
                        column: x => x.ExerciseTypeId,
                        principalTable: "ExerciseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkoutRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), true, "跑步機" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), true, "啞鈴" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), true, "槓鈴" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), true, "瑜伽墊" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), true, "飛輪" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), true, "其他" }
                });

            migrationBuilder.InsertData(
                table: "ExerciseTypes",
                columns: new[] { "Id", "Category", "CreatedAt", "IsActive", "Name", "UserId" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "有氧", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "跑步", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "有氧", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "游泳", null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "有氧", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "騎自行車", null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "重訓", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "重量訓練", null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "伸展", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "瑜伽", null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "伸展", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "皮拉提斯", null },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "其他", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "其他", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTypes_Name",
                table: "ExerciseTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTypes_UserId_IsActive",
                table: "ExerciseTypes",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_LineUserId",
                table: "Users",
                column: "LineUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutGoals_UserId_IsActive",
                table: "WorkoutGoals",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutRecords_Date",
                table: "WorkoutRecords",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutRecords_EquipmentId",
                table: "WorkoutRecords",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutRecords_ExerciseTypeId",
                table: "WorkoutRecords",
                column: "ExerciseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutRecords_UserId_Date",
                table: "WorkoutRecords",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutRecords_UserId_Date_ExerciseTypeId",
                table: "WorkoutRecords",
                columns: new[] { "UserId", "Date", "ExerciseTypeId" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutGoals");

            migrationBuilder.DropTable(
                name: "WorkoutRecords");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "ExerciseTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
