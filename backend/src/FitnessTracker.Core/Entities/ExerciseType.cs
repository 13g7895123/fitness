namespace FitnessTracker.Core.Entities;

public class ExerciseType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
    public Guid? UserId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
