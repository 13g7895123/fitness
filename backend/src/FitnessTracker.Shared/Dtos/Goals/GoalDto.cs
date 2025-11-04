namespace FitnessTracker.Shared.Dtos.Goals
{
    public class GoalDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class CreateGoalDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TargetValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }

    public class UpdateGoalDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TargetValue { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
