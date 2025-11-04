using FitnessTracker.Core.Entities;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutGoalRepository : IRepository<Goal>
    {
        Task<List<Goal>> GetByUserAsync(int userId);
        Task<List<Goal>> GetActiveByUserAsync(int userId);
        Task<List<Goal>> GetCompletedByUserAsync(int userId);
    }
}
