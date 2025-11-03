namespace FitnessTracker.Core.Entities;

public class WorkoutGoal
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int? WeeklyMinutes { get; set; }
    public decimal? WeeklyCalories { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
