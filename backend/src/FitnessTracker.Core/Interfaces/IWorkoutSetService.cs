using FitnessTracker.Shared.Dtos.WorkoutSets;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutSetService
    {
        Task<WorkoutSetDto> CreateAsync(CreateWorkoutSetDto dto, Guid userId);
        Task<WorkoutSetDto> UpdateAsync(int id, UpdateWorkoutSetDto dto, Guid userId);
        Task DeleteAsync(int id, Guid userId);
        Task<List<WorkoutSetDto>> GetByWorkoutRecordAsync(int workoutRecordId, Guid userId);
        Task<WorkoutSetDto?> GetLastSetAsync(int workoutRecordId, Guid userId);
    }
}
