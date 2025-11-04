using FitnessTracker.Core.Services;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.Equipments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/equipments")]
    public class EquipmentsController : ControllerBase
    {
        private readonly EquipmentService _equipmentService;

        public EquipmentsController(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<EquipmentDto>>>> GetAll()
        {
            try
            {
                var equipments = await _equipmentService.GetAllAsync();
                return Ok(ApiResponse<List<EquipmentDto>>.SuccessResponse(equipments));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<EquipmentDto>>.ErrorResponse($"獲取器材列表失敗: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EquipmentDto>>> GetById(int id)
        {
            try
            {
                var equipment = await _equipmentService.GetByIdAsync(id);
                if (equipment == null)
                {
                    return NotFound(ApiResponse<EquipmentDto>.ErrorResponse("器材不存在"));
                }
                return Ok(ApiResponse<EquipmentDto>.SuccessResponse(equipment));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<EquipmentDto>.ErrorResponse($"獲取器材失敗: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<EquipmentDto>>> Create([FromBody] CreateEquipmentDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(ApiResponse<EquipmentDto>.ErrorResponse("無效的用戶身份"));
                }

                var equipment = await _equipmentService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = equipment.Id }, ApiResponse<EquipmentDto>.SuccessResponse(equipment));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<EquipmentDto>.ErrorResponse($"建立器材失敗: {ex.Message}"));
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponse<EquipmentDto>>> Update(int id, [FromBody] UpdateEquipmentDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(ApiResponse<EquipmentDto>.ErrorResponse("無效的用戶身份"));
                }

                var equipment = await _equipmentService.UpdateAsync(id, dto);
                if (equipment == null)
                {
                    return NotFound(ApiResponse<EquipmentDto>.ErrorResponse("器材不存在"));
                }

                return Ok(ApiResponse<EquipmentDto>.SuccessResponse(equipment));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<EquipmentDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<EquipmentDto>.ErrorResponse($"更新器材失敗: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse("無效的用戶身份"));
                }

                await _equipmentService.DeleteAsync(id);
                return Ok(ApiResponse<object>.SuccessResponse(null, "器材已刪除"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"刪除器材失敗: {ex.Message}"));
            }
        }
    }
}
