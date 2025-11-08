using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Exceptions;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Goals;

namespace FitnessTracker.Core.Services
{
    public class GoalService : IGoalService
    {
        private readonly IRepository<Goal> _goalRepository;
        private readonly IRepository<WorkoutRecord> _workoutRecordRepository;

        public GoalService(
            IRepository<Goal> goalRepository,
            IRepository<WorkoutRecord> workoutRecordRepository)
        {
            _goalRepository = goalRepository;
            _workoutRecordRepository = workoutRecordRepository;
        }

        public async Task<GoalDto> CreateAsync(CreateGoalDto dto, Guid userId)
        {
            var goal = new Goal
            {
                Name = dto.Name,
                Description = dto.Description,
                TargetValue = dto.TargetValue,
                CurrentValue = 0,
                Unit = dto.Unit,
                UserId = userId,
                DueDate = dto.DueDate,
                IsCompleted = false,
                IsDeleted = false
            };

            await _goalRepository.AddAsync(goal);
            await _goalRepository.SaveChangesAsync();

            return MapToDto(goal);
        }

        public async Task<GoalDto> UpdateAsync(int id, UpdateGoalDto dto, Guid userId)
        {
            var goals = await _goalRepository.GetAllAsync();
            var goal = goals.FirstOrDefault(g => !g.IsDeleted && g.Id == id && g.UserId == userId);

            if (goal == null)
                throw new NotFoundException("目標", id);

            goal.Name = dto.Name;
            goal.Description = dto.Description;
            goal.TargetValue = dto.TargetValue;
            goal.DueDate = dto.DueDate;

            await _goalRepository.UpdateAsync(goal);
            await _goalRepository.SaveChangesAsync();

            return MapToDto(goal);
        }

        public async Task DeleteAsync(int id, Guid userId)
        {
            var goals = await _goalRepository.GetAllAsync();
            var goal = goals.FirstOrDefault(g => !g.IsDeleted && g.Id == id && g.UserId == userId);

            if (goal == null)
                throw new NotFoundException("目標", id);

            goal.IsDeleted = true;
            await _goalRepository.UpdateAsync(goal);
            await _goalRepository.SaveChangesAsync();
        }

        public async Task<GoalDto> GetByIdAsync(int id)
        {
            var goals = await _goalRepository.GetAllAsync();
            var goal = goals.FirstOrDefault(g => !g.IsDeleted && g.Id == id);

            if (goal == null)
                throw new NotFoundException("目標", id);

            return MapToDto(goal);
        }

        public async Task<List<GoalDto>> GetAllByUserAsync(Guid userId)
        {
            var goals = await _goalRepository.GetAllAsync();
            return goals
                .Where(g => !g.IsDeleted && g.UserId == userId)
                .OrderByDescending(g => g.CreatedAt)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<List<GoalDto>> GetActiveByUserAsync(Guid userId)
        {
            var goals = await _goalRepository.GetAllAsync();
            return goals
                .Where(g => !g.IsDeleted && g.UserId == userId && !g.IsCompleted)
                .OrderByDescending(g => g.CreatedAt)
                .Select(MapToDto)
                .ToList();
        }

        public async Task UpdateGoalProgressAsync(int goalId)
        {
            var goals = await _goalRepository.GetAllAsync();
            var goal = goals.FirstOrDefault(g => !g.IsDeleted && g.Id == goalId);

            if (goal == null)
                throw new NotFoundException("目標", goalId);

            var records = await _workoutRecordRepository.GetAllAsync();
            
            if (goal.Unit.ToLower() == "minutes")
            {
                goal.CurrentValue = records
                    .Where(r => !r.IsDeleted && r.UserId == goal.UserId)
                    .Sum(r => r.DurationMinutes);
            }
            else if (goal.Unit.ToLower() == "calories")
            {
                goal.CurrentValue = records
                    .Where(r => !r.IsDeleted && r.UserId == goal.UserId)
                    .Sum(r => r.CaloriesBurned);
            }
            else if (goal.Unit.ToLower() == "workouts")
            {
                goal.CurrentValue = records
                    .Where(r => !r.IsDeleted && r.UserId == goal.UserId)
                    .Count();
            }

            goal.IsCompleted = goal.CurrentValue >= goal.TargetValue;
            await _goalRepository.UpdateAsync(goal);
            await _goalRepository.SaveChangesAsync();
        }

        private GoalDto MapToDto(Goal goal)
        {
            return new GoalDto
            {
                Id = goal.Id,
                Name = goal.Name,
                Description = goal.Description,
                TargetValue = goal.TargetValue,
                CurrentValue = goal.CurrentValue,
                Unit = goal.Unit,
                CreatedAt = goal.CreatedAt,
                DueDate = goal.DueDate,
                IsCompleted = goal.IsCompleted
            };
        }
    }
}
