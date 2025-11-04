using FitnessTracker.Shared.Dtos.Statistics;

namespace FitnessTracker.Core.Interfaces
{
    public interface IStatisticsService
    {
        Task<WeeklySummaryDto> GetWeeklySummaryAsync(int userId, DateTime? date = null);
        Task<TrendDataDto> GetDailyBreakdownAsync(int userId, DateTime date);
        Task<List<TrendDataDto>> GetTrendsAsync(int userId, string periodType = "day");
        Task<MonthlySummaryDto> GetMonthlySummaryAsync(int userId, int? year = null, int? month = null);
        Task<List<ExerciseDistributionDto>> GetExerciseDistributionAsync(int userId);
    }
}
