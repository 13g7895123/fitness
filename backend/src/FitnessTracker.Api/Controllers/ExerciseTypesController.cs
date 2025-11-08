using FitnessTracker.Core.Services;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.ExerciseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/exercise-types")]
    public class ExerciseTypesController : ControllerBase
    {
        private readonly ExerciseTypeService _exerciseTypeService;

        public ExerciseTypesController(ExerciseTypeService exerciseTypeService)
        {
            _exerciseTypeService = exerciseTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ExerciseTypeDto>>>> GetAll()
        {
            try
            {
                var exerciseTypes = await _exerciseTypeService.GetAllAsync();
                return Ok(ApiResponse<List<ExerciseTypeDto>>.SuccessResponse(exerciseTypes));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<ExerciseTypeDto>>.ErrorResponse($"獲取運動類型列表失敗: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ExerciseTypeDto>>> GetById(int id)
        {
            try
            {
                var exerciseType = await _exerciseTypeService.GetByIdAsync(id);
                if (exerciseType == null)
                {
                    return NotFound(ApiResponse<ExerciseTypeDto>.ErrorResponse("運動類型不存在"));
                }
                return Ok(ApiResponse<ExerciseTypeDto>.SuccessResponse(exerciseType));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ExerciseTypeDto>.ErrorResponse($"獲取運動類型失敗: {ex.Message}"));
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<List<ExerciseTypeDto>>>> Search([FromQuery] string query)
        {
            try
            {
                var exerciseTypes = await _exerciseTypeService.SearchAsync(query);
                return Ok(ApiResponse<List<ExerciseTypeDto>>.SuccessResponse(exerciseTypes));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<ExerciseTypeDto>>.ErrorResponse($"搜尋運動類型失敗: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ExerciseTypeDto>>> Create([FromBody] CreateExerciseTypeDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(ApiResponse<ExerciseTypeDto>.ErrorResponse("無效的用戶身份"));
                }

                // Check for duplicate name
                if (await _exerciseTypeService.IsNameExistsAsync(dto.Name))
                {
                    return BadRequest(ApiResponse<ExerciseTypeDto>.ErrorResponse("運動類型名稱已存在"));
                }

                var exerciseType = await _exerciseTypeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = exerciseType.Id }, ApiResponse<ExerciseTypeDto>.SuccessResponse(exerciseType));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ExerciseTypeDto>.ErrorResponse($"建立運動類型失敗: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ExerciseTypeDto>>> Update(int id, [FromBody] UpdateExerciseTypeDto dto)
        {
            var exerciseType = await _exerciseTypeService.UpdateAsync(id, dto);
            if (exerciseType == null)
            {
                return NotFound(ApiResponse<ExerciseTypeDto>.ErrorResponse("運動類型不存在"));
            }

            return Ok(ApiResponse<ExerciseTypeDto>.SuccessResponse(exerciseType, "運動類型更新成功"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            await _exerciseTypeService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "運動類型已刪除"));
        }
    }
}
