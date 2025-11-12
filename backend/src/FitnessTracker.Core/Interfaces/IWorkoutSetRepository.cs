using FitnessTracker.Core.Entities;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutSetRepository
    {
        Task<WorkoutSet?> GetByIdAsync(int id);
        Task<List<WorkoutSet>> GetByWorkoutRecordIdAsync(int workoutRecordId);
        Task<WorkoutSet?> GetLastSetByExerciseTypeAsync(Guid userId, int exerciseTypeId, int excludeWorkoutRecordId);
        Task<WorkoutSet> AddAsync(WorkoutSet workoutSet);
        Task<WorkoutSet> UpdateAsync(WorkoutSet workoutSet);
        Task<bool> DeleteAsync(int id);
        Task<int> GetMaxSetNumberAsync(int workoutRecordId);
    }
}
