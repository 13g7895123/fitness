using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Repositories
{
    public class WorkoutRecordRepository : Repository<WorkoutRecord>, IWorkoutRecordRepository
    {
        private readonly FitnessTrackerDbContext _context;

        public WorkoutRecordRepository(FitnessTrackerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<WorkoutRecord>> GetByUserAsync(int userId)
        {
            return await _context.WorkoutRecords
                .Where(r => !r.IsDeleted && r.UserId == userId)
                .Include(r => r.ExerciseType)
                .OrderByDescending(r => r.ExerciseDate)
                .ToListAsync();
        }

        public async Task<List<WorkoutRecord>> GetByUserAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.WorkoutRecords
                .Where(r => !r.IsDeleted && r.UserId == userId && r.ExerciseDate >= startDate && r.ExerciseDate < endDate)
                .Include(r => r.ExerciseType)
                .OrderByDescending(r => r.ExerciseDate)
                .ToListAsync();
        }

        public async Task<List<WorkoutRecord>> GetByUserAndExerciseTypeAsync(int userId, int exerciseTypeId)
        {
            return await _context.WorkoutRecords
                .Where(r => !r.IsDeleted && r.UserId == userId && r.ExerciseTypeId == exerciseTypeId)
                .Include(r => r.ExerciseType)
                .OrderByDescending(r => r.ExerciseDate)
                .ToListAsync();
        }
    }
}
