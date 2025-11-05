using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Common;
using FitnessTracker.Shared.Dtos.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        /// <summary>
        /// 獲取週統計資料
        /// </summary>
        /// <param name="date">指定日期（可選，預設為當前日期）</param>
        [HttpGet("weekly")]
        public async Task<ActionResult<ApiResponse<WeeklySummaryDto>>> GetWeeklySummary([FromQuery] DateTime? date = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var weeklySummary = await _statisticsService.GetWeeklySummaryAsync(userId, date);
                return Ok(ApiResponse<WeeklySummaryDto>.SuccessResponse(weeklySummary));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WeeklySummaryDto>.ErrorResponse($"獲取週統計資料失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取月統計資料
        /// </summary>
        /// <param name="year">年份（可選）</param>
        /// <param name="month">月份（可選）</param>
        [HttpGet("monthly")]
        public async Task<ActionResult<ApiResponse<MonthlySummaryDto>>> GetMonthlySummary(
            [FromQuery] int? year = null,
            [FromQuery] int? month = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var monthlySummary = await _statisticsService.GetMonthlySummaryAsync(userId, year, month);
                return Ok(ApiResponse<MonthlySummaryDto>.SuccessResponse(monthlySummary));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<MonthlySummaryDto>.ErrorResponse($"獲取月統計資料失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取每日明細資料
        /// </summary>
        /// <param name="date">指定日期</param>
        [HttpGet("daily")]
        public async Task<ActionResult<ApiResponse<TrendDataDto>>> GetDailyBreakdown([FromQuery] DateTime date)
        {
            try
            {
                var userId = GetCurrentUserId();
                var dailyData = await _statisticsService.GetDailyBreakdownAsync(userId, date);
                return Ok(ApiResponse<TrendDataDto>.SuccessResponse(dailyData));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<TrendDataDto>.ErrorResponse($"獲取每日明細失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取趨勢資料
        /// </summary>
        /// <param name="periodType">週期類型: day(日), week(週), month(月)</param>
        [HttpGet("trends")]
        public async Task<ActionResult<ApiResponse<List<TrendDataDto>>>> GetTrends([FromQuery] string periodType = "day")
        {
            try
            {
                var userId = GetCurrentUserId();
                var trends = await _statisticsService.GetTrendsAsync(userId, periodType);
                return Ok(ApiResponse<List<TrendDataDto>>.SuccessResponse(trends));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<TrendDataDto>>.ErrorResponse($"獲取趨勢資料失敗: {ex.Message}"));
            }
        }

        /// <summary>
        /// 獲取運動類型分布
        /// </summary>
        [HttpGet("exercise-distribution")]
        public async Task<ActionResult<ApiResponse<List<ExerciseDistributionDto>>>> GetExerciseDistribution()
        {
            try
            {
                var userId = GetCurrentUserId();
                var distribution = await _statisticsService.GetExerciseDistributionAsync(userId);
                return Ok(ApiResponse<List<ExerciseDistributionDto>>.SuccessResponse(distribution));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<ExerciseDistributionDto>>.ErrorResponse($"獲取運動類型分布失敗: {ex.Message}"));
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
