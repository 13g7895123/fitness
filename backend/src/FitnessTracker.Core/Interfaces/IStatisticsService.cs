using FitnessTracker.Shared.Dtos.Statistics;

namespace FitnessTracker.Core.Interfaces
{
    public interface IStatisticsService
    {
        Task<WeeklySummaryDto> GetWeeklySummaryAsync(Guid userId, DateTime? date = null);
        Task<TrendDataDto> GetDailyBreakdownAsync(Guid userId, DateTime date);
        Task<List<TrendDataDto>> GetTrendsAsync(Guid userId, string periodType = "day");
        Task<MonthlySummaryDto> GetMonthlySummaryAsync(Guid userId, int? year = null, int? month = null);
        Task<List<ExerciseDistributionDto>> GetExerciseDistributionAsync(Guid userId);
    }
}
