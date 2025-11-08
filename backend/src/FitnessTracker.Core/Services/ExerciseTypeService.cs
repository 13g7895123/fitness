using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Exceptions;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.ExerciseTypes;

namespace FitnessTracker.Core.Services
{
    public class ExerciseTypeService
    {
        private readonly IExerciseTypeRepository _exerciseTypeRepository;

        public ExerciseTypeService(IExerciseTypeRepository exerciseTypeRepository)
        {
            _exerciseTypeRepository = exerciseTypeRepository;
        }

        public async Task<List<ExerciseTypeDto>> GetAllAsync()
        {
            var exerciseTypes = await _exerciseTypeRepository.GetAllWithEquipmentsAsync();
            return exerciseTypes.Select(MapToDto).ToList();
        }

        public async Task<ExerciseTypeDto?> GetByIdAsync(int id)
        {
            var exerciseType = await _exerciseTypeRepository.GetWithEquipmentsAsync(id);
            return exerciseType == null ? null : MapToDto(exerciseType);
        }

        public async Task<List<ExerciseTypeDto>> SearchAsync(string query)
        {
            var exerciseTypes = await _exerciseTypeRepository.SearchByNameAsync(query);
            return exerciseTypes.Select(MapToDto).ToList();
        }

        public async Task<ExerciseTypeDto> CreateAsync(CreateExerciseTypeDto dto)
        {
            var exerciseType = new ExerciseType
            {
                Name = dto.Name,
                Description = dto.Description,
                DefaultMET = dto.DefaultMET,
                IsSystemDefault = false,
                IsDeleted = false
            };

            await _exerciseTypeRepository.AddAsync(exerciseType);
            await _exerciseTypeRepository.SaveChangesAsync();

            // Load equipments if specified
            if (dto.EquipmentIds != null && dto.EquipmentIds.Any())
            {
                // TODO: Add logic to associate equipments
            }

            return MapToDto(exerciseType);
        }

        public async Task<ExerciseTypeDto?> UpdateAsync(int id, UpdateExerciseTypeDto dto)
        {
            var exerciseType = await _exerciseTypeRepository.GetWithEquipmentsAsync(id);
            if (exerciseType == null)
                return null;

            if (exerciseType.IsSystemDefault)
                throw new BusinessException("系統預設運動類型無法修改", "SYSTEM_DEFAULT_IMMUTABLE");

            // Check for duplicate name
            if (await _exerciseTypeRepository.IsNameExistsAsync(dto.Name, id))
                throw new ValidationException("name", "運動類型名稱已存在");

            exerciseType.Name = dto.Name;
            exerciseType.Description = dto.Description;
            exerciseType.DefaultMET = dto.DefaultMET;

            await _exerciseTypeRepository.UpdateAsync(exerciseType);
            await _exerciseTypeRepository.SaveChangesAsync();

            // TODO: Update equipment associations

            return MapToDto(exerciseType);
        }

        public async Task DeleteAsync(int id)
        {
            var exerciseTypes = await _exerciseTypeRepository.GetAllAsync();
            var exerciseType = exerciseTypes.FirstOrDefault(e => !e.IsDeleted && e.Id == id);

            if (exerciseType == null)
                throw new NotFoundException("運動類型", id);

            if (exerciseType.IsSystemDefault)
                throw new BusinessException("系統預設運動類型無法刪除", "SYSTEM_DEFAULT_IMMUTABLE");

            var workoutCount = await _exerciseTypeRepository.GetWorkoutRecordCountAsync(id);
            if (workoutCount > 0)
                throw new BusinessException("此運動類型已被使用，無法刪除", "RESOURCE_IN_USE");

            exerciseType.IsDeleted = true;
            await _exerciseTypeRepository.UpdateAsync(exerciseType);
            await _exerciseTypeRepository.SaveChangesAsync();
        }

        public async Task<bool> IsNameExistsAsync(string name, int? excludeId = null)
        {
            return await _exerciseTypeRepository.IsNameExistsAsync(name, excludeId);
        }

        private ExerciseTypeDto MapToDto(ExerciseType exerciseType)
        {
            return new ExerciseTypeDto
            {
                Id = exerciseType.Id,
                Name = exerciseType.Name,
                Description = exerciseType.Description,
                IsSystemDefault = exerciseType.IsSystemDefault,
                DefaultMET = exerciseType.DefaultMET,
                CreatedAt = exerciseType.CreatedAt,
                Equipments = exerciseType.Equipments?.Select(e => new FitnessTracker.Shared.Dtos.Equipments.EquipmentDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    IsSystemDefault = e.IsSystemDefault,
                    CreatedAt = e.CreatedAt,
                    IsDisabled = e.IsDeleted
                }).ToList() ?? new List<FitnessTracker.Shared.Dtos.Equipments.EquipmentDto>(),
                WorkoutRecordCount = 0, // TODO: Implement
                IsDisabled = exerciseType.IsDeleted
            };
        }
    }
}
