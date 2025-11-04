using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Repositories
{
    public class ExerciseTypeRepository : Repository<ExerciseType>, IExerciseTypeRepository
    {
        private readonly FitnessTrackerDbContext _context;

        public ExerciseTypeRepository(FitnessTrackerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ExerciseType>> SearchByNameAsync(string query)
        {
            return await _context.ExerciseTypes
                .Where(e => !e.IsDeleted && e.Name.Contains(query))
                .Include(e => e.Equipments)
                .ToListAsync();
        }

        public async Task<bool> IsNameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.ExerciseTypes.Where(e => !e.IsDeleted && e.Name == name);
            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<List<ExerciseType>> GetSystemDefaultAsync()
        {
            return await _context.ExerciseTypes
                .Where(e => !e.IsDeleted && e.IsSystemDefault)
                .Include(e => e.Equipments)
                .ToListAsync();
        }

        public async Task<List<ExerciseType>> GetCustomByUserAsync(int userId)
        {
            return await _context.ExerciseTypes
                .Where(e => !e.IsDeleted && !e.IsSystemDefault && e.CreatedByUserId == userId)
                .Include(e => e.Equipments)
                .ToListAsync();
        }

        public async Task<int> GetWorkoutRecordCountAsync(int exerciseTypeId)
        {
            return await _context.WorkoutRecords
                .Where(r => !r.IsDeleted && r.ExerciseTypeId == exerciseTypeId)
                .CountAsync();
        }

        public async Task<ExerciseType?> GetWithEquipmentsAsync(int id)
        {
            return await _context.ExerciseTypes
                .Where(e => !e.IsDeleted && e.Id == id)
                .Include(e => e.Equipments)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ExerciseType>> GetAllWithEquipmentsAsync()
        {
            return await _context.ExerciseTypes
                .Where(e => !e.IsDeleted)
                .Include(e => e.Equipments)
                .ToListAsync();
        }
    }
}
