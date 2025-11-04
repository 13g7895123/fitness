namespace FitnessTracker.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string LineUserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
