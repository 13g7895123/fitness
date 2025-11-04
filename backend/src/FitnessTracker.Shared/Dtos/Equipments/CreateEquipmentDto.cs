namespace FitnessTracker.Shared.Dtos.Equipments
{
    public class EquipmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class CreateEquipmentDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateEquipmentDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
