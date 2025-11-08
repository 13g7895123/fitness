using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.WorkoutRecords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/workout-records")]
    public class WorkoutRecordsController : ControllerBase
    {
        private readonly IWorkoutRecordService _workoutRecordService;

        public WorkoutRecordsController(IWorkoutRecordService workoutRecordService)
        {
            _workoutRecordService = workoutRecordService;
        }

        /// <summary>
        /// 創建運動紀錄
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<WorkoutRecordDto>>> Create([FromBody] CreateWorkoutRecordDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var record = await _workoutRecordService.CreateAsync(dto, userId);
                return Ok(ApiResponse<WorkoutRecordDto>.SuccessResponse(record, "運動紀錄新增成功"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutRecordDto>.ErrorResponse($"新增運動紀錄失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 更新運動紀錄
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<WorkoutRecordDto>>> Update(int id, [FromBody] UpdateWorkoutRecordDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var record = await _workoutRecordService.UpdateAsync(id, dto, userId);
                return Ok(ApiResponse<WorkoutRecordDto>.SuccessResponse(record, "運動紀錄更新成功"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<WorkoutRecordDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutRecordDto>.ErrorResponse($"更新運動紀錄失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 刪除運動紀錄
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _workoutRecordService.DeleteAsync(id, userId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "運動紀錄刪除成功"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"刪除運動紀錄失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取單一運動紀錄
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<WorkoutRecordDto>>> GetById(int id)
        {
            try
            {
                var record = await _workoutRecordService.GetByIdAsync(id);
                if (record == null)
                {
                    return NotFound(ApiResponse<WorkoutRecordDto>.ErrorResponse("運動紀錄不存在"));
                }
                return Ok(ApiResponse<WorkoutRecordDto>.SuccessResponse(record));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutRecordDto>.ErrorResponse($"獲取運動紀錄失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取用戶所有運動紀錄（支援分頁）
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<WorkoutRecordDto>>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                var paginatedRecords = await _workoutRecordService.GetPagedByUserAsync(userId, pageNumber, pageSize);
                return Ok(ApiResponse<PaginatedResponse<WorkoutRecordDto>>.SuccessResponse(paginatedRecords));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PaginatedResponse<WorkoutRecordDto>>.ErrorResponse($"獲取運動紀錄列表失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取指定日期的運動紀錄
        /// </summary>
        [HttpGet("daily/{date}")]
        public async Task<ActionResult<ApiResponse<List<WorkoutRecordDto>>>> GetByDate(DateTime date)
        {
            try
            {
                var userId = GetCurrentUserId();
                var records = await _workoutRecordService.GetByUserAndDateAsync(userId, date);
                return Ok(ApiResponse<List<WorkoutRecordDto>>.SuccessResponse(records));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<WorkoutRecordDto>>.ErrorResponse($"獲取每日運動紀錄失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取日期範圍內的運動紀錄
        /// </summary>
        [HttpGet("range")]
        public async Task<ActionResult<ApiResponse<List<WorkoutRecordDto>>>> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var records = await _workoutRecordService.GetByUserAndDateRangeAsync(userId, startDate, endDate);
                return Ok(ApiResponse<List<WorkoutRecordDto>>.SuccessResponse(records));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<WorkoutRecordDto>>.ErrorResponse($"獲取運動紀錄失敗: {ex.Message}"));
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("無法獲取使用者資訊");
            }
            return userId;
        }
    }
}
