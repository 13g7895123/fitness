using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Exceptions;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.WorkoutRecords;

namespace FitnessTracker.Core.Services
{
    public class WorkoutGoalService : IWorkoutGoalService
    {
        private readonly IWorkoutGoalRepository _goalRepository;
        private readonly IWorkoutRecordRepository _workoutRecordRepository;

        public WorkoutGoalService(
            IWorkoutGoalRepository goalRepository,
            IWorkoutRecordRepository workoutRecordRepository)
        {
            _goalRepository = goalRepository;
            _workoutRecordRepository = workoutRecordRepository;
        }

        public async Task<WorkoutGoalDto> CreateAsync(CreateWorkoutGoalDto dto, Guid userId)
        {
            // 停用現有的活動目標
            var existingGoals = await _goalRepository.GetAllAsync();
            var activeGoal = existingGoals.FirstOrDefault(g => g.UserId == userId && g.IsActive);
            if (activeGoal != null)
            {
                activeGoal.IsActive = false;
                activeGoal.UpdatedAt = DateTime.UtcNow;
                await _goalRepository.UpdateAsync(activeGoal);
            }

            var goal = new WorkoutGoal
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                WeeklyMinutes = dto.WeeklyMinutes,
                WeeklyCalories = dto.WeeklyCalories,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _goalRepository.AddAsync(goal);
            await _goalRepository.SaveChangesAsync();

            return await MapToDtoWithProgressAsync(goal);
        }

        public async Task<WorkoutGoalDto> UpdateAsync(Guid id, UpdateWorkoutGoalDto dto, Guid userId)
        {
            var goals = await _goalRepository.GetAllAsync();
            var goal = goals.FirstOrDefault(g => g.Id == id && g.UserId == userId);

            if (goal == null)
                throw new NotFoundException("運動目標", id);

            goal.WeeklyMinutes = dto.WeeklyMinutes;
            goal.WeeklyCalories = dto.WeeklyCalories;
            goal.EndDate = dto.EndDate;
            goal.IsActive = dto.IsActive;
            goal.UpdatedAt = DateTime.UtcNow;

            await _goalRepository.UpdateAsync(goal);
            await _goalRepository.SaveChangesAsync();

            return await MapToDtoWithProgressAsync(goal);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var goals = await _goalRepository.GetAllAsync();
            var goal = goals.FirstOrDefault(g => g.Id == id && g.UserId == userId);

            if (goal == null)
                throw new NotFoundException("運動目標", id);

            await _goalRepository.DeleteAsync(goal);
            await _goalRepository.SaveChangesAsync();
        }

        public async Task<WorkoutGoalDto?> GetByIdAsync(Guid id)
        {
            var goals = await _goalRepository.GetAllAsync();
            var goal = goals.FirstOrDefault(g => g.Id == id);

            if (goal == null)
                return null;

            return await MapToDtoWithProgressAsync(goal);
        }

        public async Task<List<WorkoutGoalDto>> GetAllByUserAsync(Guid userId)
        {
            var goals = await _goalRepository.GetAllAsync();
            var userGoals = goals.Where(g => g.UserId == userId).OrderByDescending(g => g.CreatedAt).ToList();

            var dtos = new List<WorkoutGoalDto>();
            foreach (var goal in userGoals)
            {
                dtos.Add(await MapToDtoWithProgressAsync(goal));
            }
            return dtos;
        }

        public async Task<WorkoutGoalDto?> GetActiveByUserAsync(Guid userId)
        {
            var goals = await _goalRepository.GetAllAsync();
            var activeGoal = goals.FirstOrDefault(g => g.UserId == userId && g.IsActive);

            if (activeGoal == null)
                return null;

            return await MapToDtoWithProgressAsync(activeGoal);
        }

        public async Task<WorkoutGoalDto> DeactivateAsync(Guid id, Guid userId)
        {
            var goals = await _goalRepository.GetAllAsync();
            var goal = goals.FirstOrDefault(g => g.Id == id && g.UserId == userId);

            if (goal == null)
                throw new NotFoundException("運動目標", id);

            goal.IsActive = false;
            goal.UpdatedAt = DateTime.UtcNow;

            await _goalRepository.UpdateAsync(goal);
            await _goalRepository.SaveChangesAsync();

            return await MapToDtoWithProgressAsync(goal);
        }

        private async Task<WorkoutGoalDto> MapToDtoWithProgressAsync(WorkoutGoal goal)
        {
            // 計算當週的起始日（週一）和結束日
            var now = DateTime.UtcNow;
            var daysSinceMonday = ((int)now.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var weekStart = now.Date.AddDays(-daysSinceMonday);
            var weekEnd = weekStart.AddDays(7);

            // 獲取當週的運動紀錄
            var records = await _workoutRecordRepository.GetByUserAndDateRangeAsync(goal.UserId, weekStart, weekEnd);
            var weekRecords = records.Where(r => !r.IsDeleted).ToList();

            var currentMinutes = weekRecords.Sum(r => r.DurationMinutes);
            var currentCalories = weekRecords.Sum(r => r.CaloriesBurned);

            var minutesPercent = goal.WeeklyMinutes.HasValue && goal.WeeklyMinutes.Value > 0
                ? (decimal)currentMinutes / goal.WeeklyMinutes.Value * 100
                : 0;

            var caloriesPercent = goal.WeeklyCalories.HasValue && goal.WeeklyCalories.Value > 0
                ? currentCalories / goal.WeeklyCalories.Value * 100
                : 0;

            return new WorkoutGoalDto
            {
                Id = goal.Id,
                UserId = goal.UserId,
                WeeklyMinutes = goal.WeeklyMinutes,
                WeeklyCalories = goal.WeeklyCalories,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate,
                IsActive = goal.IsActive,
                CurrentWeekMinutes = currentMinutes,
                CurrentWeekCalories = currentCalories,
                MinutesAchievementPercent = minutesPercent,
                CaloriesAchievementPercent = caloriesPercent,
                IsMinutesAchieved = goal.WeeklyMinutes.HasValue && currentMinutes >= goal.WeeklyMinutes.Value,
                IsCaloriesAchieved = goal.WeeklyCalories.HasValue && currentCalories >= goal.WeeklyCalories.Value,
                CreatedAt = goal.CreatedAt,
                UpdatedAt = goal.UpdatedAt
            };
        }
    }
}
