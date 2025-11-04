namespace FitnessTracker.Shared.Dtos.Statistics
{
    public class WeeklySummaryDto
    {
        public int TotalDurationMinutes { get; set; }
        public decimal TotalCaloriesBurned { get; set; }
        public int WorkoutDays { get; set; }
        public decimal DurationChangePercentage { get; set; }
        public decimal CaloriesChangePercentage { get; set; }
    }

    public class TrendDataDto
    {
        public string Date { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public decimal CaloriesBurned { get; set; }
        public int Count { get; set; }
        public string PeriodType { get; set; } = "day"; // day, week, month
    }

    public class MonthlySummaryDto
    {
        public string Month { get; set; } = string.Empty;
        public int TotalDurationMinutes { get; set; }
        public decimal TotalCaloriesBurned { get; set; }
        public int WorkoutDays { get; set; }
        public int TotalRecords { get; set; }
        public int AverageDailyDuration { get; set; }
        public decimal AverageDailyCalories { get; set; }
    }

    public class ExerciseDistributionDto
    {
        public string ExerciseName { get; set; } = string.Empty;
        public int TotalDurationMinutes { get; set; }
        public decimal TotalCaloriesBurned { get; set; }
        public int RecordCount { get; set; }
        public decimal PercentageOfTotal { get; set; }
    }
}
