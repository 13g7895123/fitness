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

        public async Task<List<WorkoutRecord>> GetByUserAsync(Guid userId)
        {
            return await _context.WorkoutRecords
                .AsNoTracking()
                .Where(r => !r.IsDeleted && r.UserId == userId)
                .Include(r => r.ExerciseType)
                .OrderByDescending(r => r.ExerciseDate)
                .ToListAsync();
        }

        public async Task<(List<WorkoutRecord> Items, int Total)> GetPagedByUserAsync(Guid userId, int pageNumber, int pageSize)
        {
            var query = _context.WorkoutRecords
                .AsNoTracking()
                .Where(r => !r.IsDeleted && r.UserId == userId)
                .Include(r => r.ExerciseType)
                .OrderByDescending(r => r.ExerciseDate);

            var total = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<List<WorkoutRecord>> GetByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.WorkoutRecords
                .AsNoTracking()
                .Where(r => !r.IsDeleted && r.UserId == userId && r.ExerciseDate >= startDate && r.ExerciseDate < endDate)
                .Include(r => r.ExerciseType)
                .OrderByDescending(r => r.ExerciseDate)
                .ToListAsync();
        }

        public async Task<List<WorkoutRecord>> GetByUserAndExerciseTypeAsync(Guid userId, int exerciseTypeId)
        {
            return await _context.WorkoutRecords
                .AsNoTracking()
                .Where(r => !r.IsDeleted && r.UserId == userId && r.ExerciseTypeId == exerciseTypeId)
                .Include(r => r.ExerciseType)
                .OrderByDescending(r => r.ExerciseDate)
                .ToListAsync();
        }
    }
}
