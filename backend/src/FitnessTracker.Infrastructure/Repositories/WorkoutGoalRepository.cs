using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Repositories
{
    public class WorkoutGoalRepository : Repository<Goal>, IWorkoutGoalRepository
    {
        private readonly FitnessTrackerDbContext _context;

        public WorkoutGoalRepository(FitnessTrackerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Goal>> GetByUserAsync(int userId)
        {
            return await _context.Goals
                .Where(g => !g.IsDeleted && g.UserId == userId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Goal>> GetActiveByUserAsync(int userId)
        {
            return await _context.Goals
                .Where(g => !g.IsDeleted && g.UserId == userId && !g.IsCompleted)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Goal>> GetCompletedByUserAsync(int userId)
        {
            return await _context.Goals
                .Where(g => !g.IsDeleted && g.UserId == userId && g.IsCompleted)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }
    }
}
