using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.WorkoutRecords;

namespace FitnessTracker.Core.Services
{
    public class WorkoutRecordService : IWorkoutRecordService
    {
        private readonly IWorkoutRecordRepository _workoutRecordRepository;
        private readonly IExerciseTypeRepository _exerciseTypeRepository;
        private readonly IRepository<Equipment> _equipmentRepository;

        public WorkoutRecordService(
            IWorkoutRecordRepository workoutRecordRepository,
            IExerciseTypeRepository exerciseTypeRepository,
            IRepository<Equipment> equipmentRepository)
        {
            _workoutRecordRepository = workoutRecordRepository;
            _exerciseTypeRepository = exerciseTypeRepository;
            _equipmentRepository = equipmentRepository;
        }

        public async Task<WorkoutRecordDto> CreateAsync(CreateWorkoutRecordDto dto, Guid userId)
        {
            var record = new WorkoutRecord
            {
                UserId = userId,
                ExerciseDate = dto.ExerciseDate,
                ExerciseTypeId = dto.ExerciseTypeId,
                EquipmentId = dto.EquipmentId,
                DurationMinutes = dto.DurationMinutes,
                CaloriesBurned = dto.CaloriesBurned,
                Weight = dto.Weight,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _workoutRecordRepository.AddAsync(record);
            await _workoutRecordRepository.SaveChangesAsync();

            return await MapToDto(record);
        }

        public async Task<WorkoutRecordDto> UpdateAsync(int id, UpdateWorkoutRecordDto dto, Guid userId)
        {
            var records = await _workoutRecordRepository.GetByUserAsync(userId);
            var record = records.FirstOrDefault(r => r.Id == id && !r.IsDeleted);
            if (record == null)
            {
                throw new UnauthorizedAccessException("無權限更新此紀錄");
            }

            record.ExerciseDate = dto.ExerciseDate;
            record.ExerciseTypeId = dto.ExerciseTypeId;
            record.EquipmentId = dto.EquipmentId;
            record.DurationMinutes = dto.DurationMinutes;
            record.CaloriesBurned = dto.CaloriesBurned;
            record.Weight = dto.Weight;
            record.Notes = dto.Notes;
            record.UpdatedAt = DateTime.UtcNow;

            await _workoutRecordRepository.UpdateAsync(record);
            await _workoutRecordRepository.SaveChangesAsync();

            return await MapToDto(record);
        }

        public async Task DeleteAsync(int id, Guid userId)
        {
            var records = await _workoutRecordRepository.GetByUserAsync(userId);
            var record = records.FirstOrDefault(r => r.Id == id && !r.IsDeleted);
            if (record == null)
            {
                throw new UnauthorizedAccessException("無權限刪除此紀錄");
            }

            record.IsDeleted = true;
            record.UpdatedAt = DateTime.UtcNow;
            await _workoutRecordRepository.UpdateAsync(record);
            await _workoutRecordRepository.SaveChangesAsync();
        }

        public async Task<WorkoutRecordDto?> GetByIdAsync(int id)
        {
            var records = await _workoutRecordRepository.GetAllAsync();
            var record = records.FirstOrDefault(r => r.Id == id && !r.IsDeleted);
            if (record == null)
            {
                return null;
            }
            return await MapToDto(record);
        }

        public async Task<List<WorkoutRecordDto>> GetByUserAsync(Guid userId)
        {
            var records = await _workoutRecordRepository.GetByUserAsync(userId);
            var filteredRecords = records.Where(r => !r.IsDeleted).ToList();

            var dtos = new List<WorkoutRecordDto>();
            foreach (var record in filteredRecords)
            {
                dtos.Add(await MapToDto(record));
            }
            return dtos;
        }

        public async Task<PaginatedResponse<WorkoutRecordDto>> GetPagedByUserAsync(Guid userId, int pageNumber, int pageSize)
        {
            var (items, total) = await _workoutRecordRepository.GetPagedByUserAsync(userId, pageNumber, pageSize);

            var dtos = new List<WorkoutRecordDto>();
            foreach (var record in items)
            {
                dtos.Add(await MapToDto(record));
            }

            return PaginatedResponse<WorkoutRecordDto>.Create(dtos, total, pageNumber, pageSize);
        }

        public async Task<List<WorkoutRecordDto>> GetByUserAndDateAsync(Guid userId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);
            return await GetByUserAndDateRangeAsync(userId, startDate, endDate);
        }

        public async Task<List<WorkoutRecordDto>> GetByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            var records = await _workoutRecordRepository.GetByUserAndDateRangeAsync(userId, startDate, endDate);
            var filteredRecords = records.Where(r => !r.IsDeleted).ToList();

            var dtos = new List<WorkoutRecordDto>();
            foreach (var record in filteredRecords)
            {
                dtos.Add(await MapToDto(record));
            }
            return dtos;
        }

        private async Task<WorkoutRecordDto> MapToDto(WorkoutRecord record)
        {
            var exerciseTypes = await _exerciseTypeRepository.GetAllAsync();
            var exerciseType = exerciseTypes.FirstOrDefault(e => !e.IsDeleted && e.Id == record.ExerciseTypeId);

            Equipment? equipment = null;
            if (record.EquipmentId.HasValue)
            {
                var equipments = await _equipmentRepository.GetAllAsync();
                equipment = equipments.FirstOrDefault(e => !e.IsDeleted && e.Id == record.EquipmentId.Value);
            }

            return new WorkoutRecordDto
            {
                Id = record.Id,
                UserId = record.UserId,
                ExerciseDate = record.ExerciseDate,
                ExerciseTypeId = record.ExerciseTypeId,
                ExerciseTypeName = exerciseType?.Name ?? string.Empty,
                EquipmentId = record.EquipmentId,
                EquipmentName = equipment?.Name,
                DurationMinutes = record.DurationMinutes,
                CaloriesBurned = record.CaloriesBurned,
                Weight = record.Weight,
                Notes = record.Notes,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt
            };
        }
    }
}
