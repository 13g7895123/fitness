using FitnessTracker.Shared.Dtos.WorkoutRecords;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutRecordService
    {
        Task<WorkoutRecordDto> CreateAsync(CreateWorkoutRecordDto dto, Guid userId);
        Task<WorkoutRecordDto> UpdateAsync(int id, UpdateWorkoutRecordDto dto, Guid userId);
        Task DeleteAsync(int id, Guid userId);
        Task<WorkoutRecordDto?> GetByIdAsync(int id);
        Task<List<WorkoutRecordDto>> GetByUserAsync(Guid userId);
        Task<List<WorkoutRecordDto>> GetByUserAndDateAsync(Guid userId, DateTime date);
        Task<List<WorkoutRecordDto>> GetByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    }
}
