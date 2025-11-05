namespace FitnessTracker.Shared.Dtos.WorkoutRecords
{
    public class WorkoutGoalDto
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

    public class CreateWorkoutGoalDto
    {
        public int? WeeklyMinutes { get; set; }
        public decimal? WeeklyCalories { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateWorkoutGoalDto
    {
        public int? WeeklyMinutes { get; set; }
        public decimal? WeeklyCalories { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
