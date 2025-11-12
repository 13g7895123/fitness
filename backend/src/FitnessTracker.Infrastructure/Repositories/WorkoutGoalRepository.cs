using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Repositories
{
    public class WorkoutGoalRepository : Repository<WorkoutGoal>, IWorkoutGoalRepository
    {
        private readonly FitnessTrackerDbContext _context;

        public WorkoutGoalRepository(FitnessTrackerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<WorkoutGoal>> GetByUserAsync(Guid userId)
        {
            return await _context.WorkoutGoals
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<WorkoutGoal>> GetActiveByUserAsync(Guid userId)
        {
            return await _context.WorkoutGoals
                .Where(g => g.UserId == userId && g.IsActive)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<WorkoutGoal>> GetCompletedByUserAsync(Guid userId)
        {
            return await _context.WorkoutGoals
                .Where(g => g.UserId == userId && !g.IsActive && g.EndDate < DateTime.UtcNow)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }
    }
}
