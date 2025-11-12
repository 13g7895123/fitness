using FitnessTracker.Core.Entities;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutGoalRepository : IRepository<WorkoutGoal>
    {
        Task<List<WorkoutGoal>> GetByUserAsync(Guid userId);
        Task<List<WorkoutGoal>> GetActiveByUserAsync(Guid userId);
        Task<List<WorkoutGoal>> GetCompletedByUserAsync(Guid userId);
    }
}
