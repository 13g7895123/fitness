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

        // 當週進度（動態計算）
        public int CurrentWeekMinutes { get; set; }
        public decimal CurrentWeekCalories { get; set; }

        // 達成百分比（動態計算）
        public decimal MinutesAchievementPercent { get; set; }
        public decimal CaloriesAchievementPercent { get; set; }

        // 是否達成（動態計算）
        public bool IsMinutesAchieved { get; set; }
        public bool IsCaloriesAchieved { get; set; }

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
