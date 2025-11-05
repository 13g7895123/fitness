using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.Goals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/goals")]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _goalService;

        public GoalsController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        /// <summary>
        /// 創建目標
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<GoalDto>>> Create([FromBody] CreateGoalDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _goalService.CreateAsync(dto, userId);
                return Ok(ApiResponse<GoalDto>.SuccessResponse(goal, "目標創建成功"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<GoalDto>.ErrorResponse($"創建目標失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 更新目標
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<GoalDto>>> Update(int id, [FromBody] UpdateGoalDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var goal = await _goalService.UpdateAsync(id, dto, userId);
                return Ok(ApiResponse<GoalDto>.SuccessResponse(goal, "目標更新成功"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<GoalDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<GoalDto>.ErrorResponse($"更新目標失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 刪除目標
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _goalService.DeleteAsync(id, userId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "目標刪除成功"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"刪除目標失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取單一目標
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GoalDto>>> GetById(int id)
        {
            try
            {
                var goal = await _goalService.GetByIdAsync(id);
                if (goal == null)
                {
                    return NotFound(ApiResponse<GoalDto>.ErrorResponse("目標不存在"));
                }
                return Ok(ApiResponse<GoalDto>.SuccessResponse(goal));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<GoalDto>.ErrorResponse($"獲取目標失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取用戶所有目標
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<GoalDto>>>> GetAll()
        {
            try
            {
                var userId = GetCurrentUserId();
                var goals = await _goalService.GetAllByUserAsync(userId);
                return Ok(ApiResponse<List<GoalDto>>.SuccessResponse(goals));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<GoalDto>>.ErrorResponse($"獲取目標列表失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取用戶活躍的目標
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<List<GoalDto>>>> GetActive()
        {
            try
            {
                var userId = GetCurrentUserId();
                var goals = await _goalService.GetActiveByUserAsync(userId);
                return Ok(ApiResponse<List<GoalDto>>.SuccessResponse(goals));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<GoalDto>>.ErrorResponse($"獲取活躍目標失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 更新目標進度
        /// </summary>
        [HttpPost("{id}/update-progress")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateProgress(int id)
        {
            try
            {
                await _goalService.UpdateGoalProgressAsync(id);
                return Ok(ApiResponse<object>.SuccessResponse(null, "目標進度更新成功"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"更新目標進度失敗: {ex.Message}"));
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
