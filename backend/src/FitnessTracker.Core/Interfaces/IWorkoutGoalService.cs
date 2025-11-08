using FitnessTracker.Shared.Dtos.WorkoutRecords;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutGoalService
    {
        Task<WorkoutGoalDto> CreateAsync(CreateWorkoutGoalDto dto, Guid userId);
        Task<WorkoutGoalDto> UpdateAsync(Guid id, UpdateWorkoutGoalDto dto, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
        Task<WorkoutGoalDto?> GetByIdAsync(Guid id);
        Task<List<WorkoutGoalDto>> GetAllByUserAsync(Guid userId);
        Task<WorkoutGoalDto?> GetActiveByUserAsync(Guid userId);
        Task<WorkoutGoalDto> DeactivateAsync(Guid id, Guid userId);
    }
}
