using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Equipments;

namespace FitnessTracker.Core.Services
{
    public class EquipmentService
    {
        private readonly IRepository<Equipment> _equipmentRepository;

        public EquipmentService(IRepository<Equipment> equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

        public async Task<List<EquipmentDto>> GetAllAsync()
        {
            var equipments = await _equipmentRepository.GetAllAsync();
            return equipments
                .Where(e => !e.IsDeleted)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<EquipmentDto?> GetByIdAsync(int id)
        {
            var equipments = await _equipmentRepository.GetAllAsync();
            var equipment = equipments.FirstOrDefault(e => !e.IsDeleted && e.Id == id);
            return equipment == null ? null : MapToDto(equipment);
        }

        public async Task<EquipmentDto> CreateAsync(CreateEquipmentDto dto)
        {
            var equipment = new Equipment
            {
                Name = dto.Name,
                Description = dto.Description,
                IsSystemDefault = false,
                IsDeleted = false
            };

            await _equipmentRepository.AddAsync(equipment);
            await _equipmentRepository.SaveChangesAsync();

            return MapToDto(equipment);
        }

        public async Task<EquipmentDto?> UpdateAsync(int id, UpdateEquipmentDto dto)
        {
            var equipments = await _equipmentRepository.GetAllAsync();
            var equipment = equipments.FirstOrDefault(e => !e.IsDeleted && e.Id == id);

            if (equipment == null)
                return null;

            if (equipment.IsSystemDefault)
                throw new InvalidOperationException("系統預設器材無法修改");

            equipment.Name = dto.Name;
            equipment.Description = dto.Description;

            await _equipmentRepository.UpdateAsync(equipment);
            await _equipmentRepository.SaveChangesAsync();

            return MapToDto(equipment);
        }

        public async Task DeleteAsync(int id)
        {
            var equipments = await _equipmentRepository.GetAllAsync();
            var equipment = equipments.FirstOrDefault(e => !e.IsDeleted && e.Id == id);

            if (equipment == null)
                throw new KeyNotFoundException($"Equipment with id {id} not found");

            if (equipment.IsSystemDefault)
                throw new InvalidOperationException("系統預設器材無法刪除");

            equipment.IsDeleted = true;
            await _equipmentRepository.UpdateAsync(equipment);
            await _equipmentRepository.SaveChangesAsync();
        }

        private EquipmentDto MapToDto(Equipment equipment)
        {
            return new EquipmentDto
            {
                Id = equipment.Id,
                Name = equipment.Name,
                Description = equipment.Description,
                IsSystemDefault = equipment.IsSystemDefault,
                CreatedAt = equipment.CreatedAt,
                IsDisabled = equipment.IsDeleted
            };
        }
    }
}
