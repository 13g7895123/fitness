namespace FitnessTracker.Shared.Dtos.ExerciseTypes
{
    public class UpdateExerciseTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DefaultMET { get; set; } = 5.0m;
        public List<int> EquipmentIds { get; set; } = new();
    }
}
