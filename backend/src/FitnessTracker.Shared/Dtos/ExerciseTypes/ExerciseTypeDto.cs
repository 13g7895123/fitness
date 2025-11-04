using FitnessTracker.Shared.Dtos.Equipments;

namespace FitnessTracker.Shared.Dtos.ExerciseTypes
{
    public class ExerciseTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemDefault { get; set; }
        public decimal DefaultMET { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<EquipmentDto> Equipments { get; set; } = new();
        public int WorkoutRecordCount { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class CreateExerciseTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DefaultMET { get; set; } = 5.0m;
        public List<int> EquipmentIds { get; set; } = new();
    }
}
