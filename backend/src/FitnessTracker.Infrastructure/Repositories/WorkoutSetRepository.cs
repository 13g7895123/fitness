using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Repositories
{
    public class WorkoutSetRepository : IWorkoutSetRepository
    {
        private readonly FitnessTrackerDbContext _context;

        public WorkoutSetRepository(FitnessTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<WorkoutSet?> GetByIdAsync(int id)
        {
            return await _context.WorkoutSets
                .Include(ws => ws.WorkoutRecord)
                .FirstOrDefaultAsync(ws => ws.Id == id && !ws.IsDeleted);
        }

        public async Task<List<WorkoutSet>> GetByWorkoutRecordIdAsync(int workoutRecordId)
        {
            return await _context.WorkoutSets
                .Where(ws => ws.WorkoutRecordId == workoutRecordId && !ws.IsDeleted)
                .OrderBy(ws => ws.SetNumber)
                .ToListAsync();
        }

        public async Task<WorkoutSet?> GetLastSetByExerciseTypeAsync(Guid userId, int exerciseTypeId, int excludeWorkoutRecordId)
        {
            return await _context.WorkoutSets
                .Where(ws => ws.WorkoutRecord.ExerciseTypeId == exerciseTypeId
                            && ws.WorkoutRecord.UserId == userId
                            && ws.WorkoutRecordId != excludeWorkoutRecordId
                            && !ws.IsDeleted
                            && !ws.WorkoutRecord.IsDeleted)
                .OrderByDescending(ws => ws.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<WorkoutSet> AddAsync(WorkoutSet workoutSet)
        {
            _context.WorkoutSets.Add(workoutSet);
            await _context.SaveChangesAsync();
            return workoutSet;
        }

        public async Task<WorkoutSet> UpdateAsync(WorkoutSet workoutSet)
        {
            _context.WorkoutSets.Update(workoutSet);
            await _context.SaveChangesAsync();
            return workoutSet;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var workoutSet = await GetByIdAsync(id);
            if (workoutSet == null) return false;

            workoutSet.IsDeleted = true;
            workoutSet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetMaxSetNumberAsync(int workoutRecordId)
        {
            var maxSetNumber = await _context.WorkoutSets
                .Where(ws => ws.WorkoutRecordId == workoutRecordId && !ws.IsDeleted)
                .MaxAsync(ws => (int?)ws.SetNumber);

            return maxSetNumber ?? 0;
        }
    }
}
