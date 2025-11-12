using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.WorkoutSets;

namespace FitnessTracker.Core.Services
{
    public class WorkoutSetService : IWorkoutSetService
    {
        private readonly IWorkoutSetRepository _workoutSetRepository;
        private readonly IWorkoutRecordRepository _workoutRecordRepository;

        public WorkoutSetService(
            IWorkoutSetRepository workoutSetRepository,
            IWorkoutRecordRepository workoutRecordRepository)
        {
            _workoutSetRepository = workoutSetRepository;
            _workoutRecordRepository = workoutRecordRepository;
        }

        public async Task<WorkoutSetDto> CreateAsync(CreateWorkoutSetDto dto, Guid userId)
        {
            // 驗證 WorkoutRecord 是否屬於該用戶
            var workoutRecords = await _workoutRecordRepository.GetByUserAsync(userId);
            var workoutRecord = workoutRecords.FirstOrDefault(wr => wr.Id == dto.WorkoutRecordId && !wr.IsDeleted);

            if (workoutRecord == null)
            {
                throw new UnauthorizedAccessException("無權限操作此訓練記錄");
            }

            // 計算組數順序
            var maxSetNumber = await _workoutSetRepository.GetMaxSetNumberAsync(dto.WorkoutRecordId);

            var workoutSet = new WorkoutSet
            {
                WorkoutRecordId = dto.WorkoutRecordId,
                SetNumber = maxSetNumber + 1,
                Repetitions = dto.Repetitions,
                Weight = dto.Weight,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdSet = await _workoutSetRepository.AddAsync(workoutSet);
            return MapToDto(createdSet);
        }

        public async Task<WorkoutSetDto> UpdateAsync(int id, UpdateWorkoutSetDto dto, Guid userId)
        {
            var workoutSet = await _workoutSetRepository.GetByIdAsync(id);

            if (workoutSet == null)
            {
                throw new KeyNotFoundException("找不到該訓練組數");
            }

            if (workoutSet.WorkoutRecord.UserId != userId)
            {
                throw new UnauthorizedAccessException("無權限操作此訓練組數");
            }

            workoutSet.Repetitions = dto.Repetitions;
            workoutSet.Weight = dto.Weight;
            workoutSet.Notes = dto.Notes;
            workoutSet.UpdatedAt = DateTime.UtcNow;

            var updatedSet = await _workoutSetRepository.UpdateAsync(workoutSet);
            return MapToDto(updatedSet);
        }

        public async Task DeleteAsync(int id, Guid userId)
        {
            var workoutSet = await _workoutSetRepository.GetByIdAsync(id);

            if (workoutSet == null)
            {
                throw new KeyNotFoundException("找不到該訓練組數");
            }

            if (workoutSet.WorkoutRecord.UserId != userId)
            {
                throw new UnauthorizedAccessException("無權限操作此訓練組數");
            }

            await _workoutSetRepository.DeleteAsync(id);
        }

        public async Task<List<WorkoutSetDto>> GetByWorkoutRecordAsync(int workoutRecordId, Guid userId)
        {
            // 驗證權限
            var workoutRecords = await _workoutRecordRepository.GetByUserAsync(userId);
            var workoutRecord = workoutRecords.FirstOrDefault(wr => wr.Id == workoutRecordId && !wr.IsDeleted);

            if (workoutRecord == null)
            {
                throw new UnauthorizedAccessException("無權限查看此訓練記錄");
            }

            var sets = await _workoutSetRepository.GetByWorkoutRecordIdAsync(workoutRecordId);
            return sets.Select(MapToDto).ToList();
        }

        public async Task<WorkoutSetDto?> GetLastSetAsync(int workoutRecordId, Guid userId)
        {
            // 取得當前的訓練記錄
            var workoutRecords = await _workoutRecordRepository.GetByUserAsync(userId);
            var currentRecord = workoutRecords.FirstOrDefault(wr => wr.Id == workoutRecordId && !wr.IsDeleted);

            if (currentRecord == null)
            {
                return null;
            }

            // 查找上一次相同運動類型的最後一組
            var lastSet = await _workoutSetRepository.GetLastSetByExerciseTypeAsync(
                userId,
                currentRecord.ExerciseTypeId,
                workoutRecordId
            );

            return lastSet != null ? MapToDto(lastSet) : null;
        }

        private WorkoutSetDto MapToDto(WorkoutSet workoutSet)
        {
            return new WorkoutSetDto
            {
                Id = workoutSet.Id,
                WorkoutRecordId = workoutSet.WorkoutRecordId,
                SetNumber = workoutSet.SetNumber,
                Repetitions = workoutSet.Repetitions,
                Weight = workoutSet.Weight,
                Notes = workoutSet.Notes,
                CreatedAt = workoutSet.CreatedAt,
                UpdatedAt = workoutSet.UpdatedAt
            };
        }
    }
}
