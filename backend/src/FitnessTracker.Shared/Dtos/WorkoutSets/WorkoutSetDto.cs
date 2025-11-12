namespace FitnessTracker.Shared.Dtos.WorkoutSets
{
    /// <summary>
    /// 訓練組數 DTO
    /// </summary>
    public class WorkoutSetDto
    {
        public int Id { get; set; }
        public int WorkoutRecordId { get; set; }
        public int SetNumber { get; set; }
        public int Repetitions { get; set; }
        public decimal? Weight { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// 新增訓練組數 DTO
    /// </summary>
    public class CreateWorkoutSetDto
    {
        public int WorkoutRecordId { get; set; }
        public int Repetitions { get; set; }
        public decimal? Weight { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// 更新訓練組數 DTO
    /// </summary>
    public class UpdateWorkoutSetDto
    {
        public int Repetitions { get; set; }
        public decimal? Weight { get; set; }
        public string? Notes { get; set; }
    }
}
