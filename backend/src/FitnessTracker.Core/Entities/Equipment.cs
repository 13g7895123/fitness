namespace FitnessTracker.Core.Entities;

public class Equipment
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
