namespace FitnessTracker.Core.Entities;

public class WorkoutRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public Guid ExerciseTypeId { get; set; }
    public Guid? EquipmentId { get; set; }
    public int DurationMinutes { get; set; }
    public decimal CaloriesBurned { get; set; }
    public decimal? Weight { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
