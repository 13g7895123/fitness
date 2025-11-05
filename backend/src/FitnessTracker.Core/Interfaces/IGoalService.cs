using FitnessTracker.Shared.Dtos.Goals;

namespace FitnessTracker.Core.Interfaces
{
    public interface IGoalService
    {
        Task<GoalDto> CreateAsync(CreateGoalDto dto, Guid userId);
        Task<GoalDto> UpdateAsync(int id, UpdateGoalDto dto, Guid userId);
        Task DeleteAsync(int id, Guid userId);
        Task<GoalDto> GetByIdAsync(int id);
        Task<List<GoalDto>> GetAllByUserAsync(Guid userId);
        Task<List<GoalDto>> GetActiveByUserAsync(Guid userId);
        Task UpdateGoalProgressAsync(int goalId);
    }
}
