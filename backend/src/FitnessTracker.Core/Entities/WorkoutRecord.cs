namespace FitnessTracker.Core.Entities;

public class WorkoutRecord
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime ExerciseDate { get; set; }
    public int ExerciseTypeId { get; set; }
    public ExerciseType ExerciseType { get; set; } = null!;
    public int? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }
    public int DurationMinutes { get; set; }
    public decimal CaloriesBurned { get; set; }
    public decimal? Weight { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}
