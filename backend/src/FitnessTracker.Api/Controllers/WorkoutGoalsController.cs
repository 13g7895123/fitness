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
    [Route("api/v1/workout-goals")]
    public class WorkoutGoalsController : ControllerBase
    {
        private readonly IWorkoutGoalService _workoutGoalService;

        public WorkoutGoalsController(IWorkoutGoalService workoutGoalService)
        {
            _workoutGoalService = workoutGoalService;
        }

        /// <summary>
        /// 建立新的運動目標
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<WorkoutGoalDto>>> Create([FromBody] CreateWorkoutGoalDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _workoutGoalService.CreateAsync(dto, userId);
                return Ok(ApiResponse<WorkoutGoalDto>.SuccessResponse(goal, "目標建立成功"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutGoalDto>.ErrorResponse($"建立目標失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 更新運動目標
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<WorkoutGoalDto>>> Update(Guid id, [FromBody] UpdateWorkoutGoalDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _workoutGoalService.UpdateAsync(id, dto, userId);
                return Ok(ApiResponse<WorkoutGoalDto>.SuccessResponse(goal, "目標更新成功"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<WorkoutGoalDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutGoalDto>.ErrorResponse($"更新目標失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 刪除運動目標
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _workoutGoalService.DeleteAsync(id, userId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "目標刪除成功"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"刪除目標失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 取得單一運動目標
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<WorkoutGoalDto>>> GetById(Guid id)
        {
            try
            {
                var goal = await _workoutGoalService.GetByIdAsync(id);
                if (goal == null)
                {
                    return NotFound(ApiResponse<WorkoutGoalDto>.ErrorResponse("目標不存在"));
                }
                return Ok(ApiResponse<WorkoutGoalDto>.SuccessResponse(goal));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutGoalDto>.ErrorResponse($"取得目標失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 取得使用者所有運動目標
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<WorkoutGoalDto>>>> GetAll()
        {
            try
            {
                var userId = GetCurrentUserId();
                var goals = await _workoutGoalService.GetAllByUserAsync(userId);
                return Ok(ApiResponse<List<WorkoutGoalDto>>.SuccessResponse(goals));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<WorkoutGoalDto>>.ErrorResponse($"取得目標列表失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 取得使用者目前活動的運動目標
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<WorkoutGoalDto>>> GetActive()
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _workoutGoalService.GetActiveByUserAsync(userId);
                if (goal == null)
                {
                    return NotFound(ApiResponse<WorkoutGoalDto>.ErrorResponse("沒有活動中的目標"));
                }
                return Ok(ApiResponse<WorkoutGoalDto>.SuccessResponse(goal));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutGoalDto>.ErrorResponse($"取得活動目標失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 停用運動目標
        /// </summary>
        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult<ApiResponse<WorkoutGoalDto>>> Deactivate(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _workoutGoalService.DeactivateAsync(id, userId);
                return Ok(ApiResponse<WorkoutGoalDto>.SuccessResponse(goal, "目標已停用"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<WorkoutGoalDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WorkoutGoalDto>.ErrorResponse($"停用目標失敗: {ex.Message}"));
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
