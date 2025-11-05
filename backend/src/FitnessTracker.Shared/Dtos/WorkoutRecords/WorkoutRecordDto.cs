namespace FitnessTracker.Shared.Dtos.WorkoutRecords
{
    public class WorkoutRecordDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExerciseDate { get; set; }
        public int ExerciseTypeId { get; set; }
        public string ExerciseTypeName { get; set; } = string.Empty;
        public int? EquipmentId { get; set; }
        public string? EquipmentName { get; set; }
        public int DurationMinutes { get; set; }
        public decimal CaloriesBurned { get; set; }
        public decimal? Weight { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateWorkoutRecordDto
    {
        public DateTime ExerciseDate { get; set; }
        public int ExerciseTypeId { get; set; }
        public int? EquipmentId { get; set; }
        public int DurationMinutes { get; set; }
        public decimal CaloriesBurned { get; set; }
        public decimal? Weight { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateWorkoutRecordDto
    {
        public DateTime ExerciseDate { get; set; }
        public int ExerciseTypeId { get; set; }
        public int? EquipmentId { get; set; }
        public int DurationMinutes { get; set; }
        public decimal CaloriesBurned { get; set; }
        public decimal? Weight { get; set; }
        public string? Notes { get; set; }
    }
}
