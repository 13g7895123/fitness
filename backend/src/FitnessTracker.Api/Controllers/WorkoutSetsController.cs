using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.WorkoutSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/workout-sets")]
    public class WorkoutSetsController : ControllerBase
    {
        private readonly IWorkoutSetService _workoutSetService;

        public WorkoutSetsController(IWorkoutSetService workoutSetService)
        {
            _workoutSetService = workoutSetService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("無法取得使用者 ID"));
        }

        /// <summary>
        /// 新增訓練組數
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<WorkoutSetDto>>> Create([FromBody] CreateWorkoutSetDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var set = await _workoutSetService.CreateAsync(dto, userId);
                return Ok(ApiResponse<WorkoutSetDto>.SuccessResponse(set, "訓練組數新增成功"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<WorkoutSetDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutSetDto>.ErrorResponse($"新增訓練組數失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 更新訓練組數
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<WorkoutSetDto>>> Update(int id, [FromBody] UpdateWorkoutSetDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var set = await _workoutSetService.UpdateAsync(id, dto, userId);
                return Ok(ApiResponse<WorkoutSetDto>.SuccessResponse(set, "訓練組數更新成功"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<WorkoutSetDto>.ErrorResponse(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<WorkoutSetDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutSetDto>.ErrorResponse($"更新訓練組數失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 刪除訓練組數
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _workoutSetService.DeleteAsync(id, userId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "訓練組數刪除成功"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"刪除訓練組數失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 取得指定訓練記錄的所有組數
        /// </summary>
        [HttpGet("by-record/{workoutRecordId}")]
        public async Task<ActionResult<ApiResponse<List<WorkoutSetDto>>>> GetByWorkoutRecord(int workoutRecordId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var sets = await _workoutSetService.GetByWorkoutRecordAsync(workoutRecordId, userId);
                return Ok(ApiResponse<List<WorkoutSetDto>>.SuccessResponse(sets, "查詢成功"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<List<WorkoutSetDto>>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<WorkoutSetDto>>.ErrorResponse($"查詢訓練組數失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 取得上次相同運動類型的組數資料（用於預設值）
        /// </summary>
        [HttpGet("last-set/{workoutRecordId}")]
        public async Task<ActionResult<ApiResponse<WorkoutSetDto?>>> GetLastSet(int workoutRecordId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var lastSet = await _workoutSetService.GetLastSetAsync(workoutRecordId, userId);
                return Ok(ApiResponse<WorkoutSetDto?>.SuccessResponse(lastSet, "查詢成功"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutSetDto?>.ErrorResponse($"查詢上次組數失敗: {ex.Message}"));
            }
        }
    }
}
