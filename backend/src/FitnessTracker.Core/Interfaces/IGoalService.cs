using FitnessTracker.Shared.Dtos.Goals;

namespace FitnessTracker.Core.Interfaces
{
    public interface IGoalService
    {
        Task<GoalDto> CreateAsync(CreateGoalDto dto, int userId);
        Task<GoalDto> UpdateAsync(int id, UpdateGoalDto dto, int userId);
        Task DeleteAsync(int id, int userId);
        Task<GoalDto> GetByIdAsync(int id);
        Task<List<GoalDto>> GetAllByUserAsync(int userId);
        Task<List<GoalDto>> GetActiveByUserAsync(int userId);
        Task UpdateGoalProgressAsync(int goalId);
    }
}
