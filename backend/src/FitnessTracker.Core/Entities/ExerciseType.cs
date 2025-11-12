namespace FitnessTracker.Core.Entities;

public class ExerciseType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DefaultMET { get; set; } = 5.0m;
    public bool IsSystemDefault { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    // Navigation properties
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    public ICollection<WorkoutRecord> WorkoutRecords { get; set; } = new List<WorkoutRecord>();
}
