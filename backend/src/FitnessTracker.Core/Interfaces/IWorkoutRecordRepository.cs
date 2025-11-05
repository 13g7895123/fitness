using FitnessTracker.Core.Entities;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutRecordRepository : IRepository<WorkoutRecord>
    {
        Task<List<WorkoutRecord>> GetByUserAsync(Guid userId);
        Task<List<WorkoutRecord>> GetByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<List<WorkoutRecord>> GetByUserAndExerciseTypeAsync(Guid userId, int exerciseTypeId);
    }
}
