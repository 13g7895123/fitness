using FitnessTracker.Core.Entities;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutRecordRepository : IRepository<WorkoutRecord>
    {
        Task<List<WorkoutRecord>> GetByUserAsync(int userId);
        Task<List<WorkoutRecord>> GetByUserAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<List<WorkoutRecord>> GetByUserAndExerciseTypeAsync(int userId, int exerciseTypeId);
    }
}
