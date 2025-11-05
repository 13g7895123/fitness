using FitnessTracker.Core.Entities;

namespace FitnessTracker.Core.Interfaces
{
    public interface IExerciseTypeRepository : IRepository<ExerciseType>
    {
        Task<List<ExerciseType>> SearchByNameAsync(string query);
        Task<bool> IsNameExistsAsync(string name, int? excludeId = null);
        Task<List<ExerciseType>> GetSystemDefaultAsync();
        Task<List<ExerciseType>> GetCustomByUserAsync(Guid userId);
        Task<int> GetWorkoutRecordCountAsync(int exerciseTypeId);
        Task<ExerciseType?> GetWithEquipmentsAsync(int id);
        Task<List<ExerciseType>> GetAllWithEquipmentsAsync();
    }
}
