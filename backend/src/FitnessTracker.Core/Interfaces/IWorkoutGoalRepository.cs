using FitnessTracker.Core.Entities;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutGoalRepository : IRepository<Goal>
    {
        Task<List<Goal>> GetByUserAsync(Guid userId);
        Task<List<Goal>> GetActiveByUserAsync(Guid userId);
        Task<List<Goal>> GetCompletedByUserAsync(Guid userId);
    }
}
