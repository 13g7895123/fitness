using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Statistics;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Services;

/// <summary>
/// 統計服務實現
/// </summary>
public class StatisticsService : IStatisticsService
{
    private readonly DbContext _context;

    public StatisticsService(DbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 取得當前週的統計摘要
    /// </summary>
    public async Task<WeeklySummaryDto> GetWeeklySummaryAsync(Guid userId, DateTime? date = null)
    {
        var referenceDate = date ?? DateTime.UtcNow;
        var weekStartDate = GetMonday(referenceDate);
        return await CalculateWeeklyStatisticsAsync(userId, weekStartDate);
    }

    /// <summary>
    /// 計算指定週的統計資料
    /// </summary>
    public async Task<WeeklySummaryDto> CalculateWeeklyStatisticsAsync(Guid userId, DateTime weekStartDate)
    {
        if (weekStartDate.DayOfWeek != DayOfWeek.Monday)
        {
            weekStartDate = GetMonday(weekStartDate);
        }

        var weekEndDate = weekStartDate.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
        var previousWeekStartDate = weekStartDate.AddDays(-7);
        var previousWeekEndDate = previousWeekStartDate.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

        var thisWeekRecords = await _context.Set<Data.WorkoutRecord>()
            .AsNoTracking()
            .Where(r => r.UserId == userId && 
                       r.Date >= weekStartDate.Date && 
                       r.Date <= weekEndDate.Date &&
                       !r.IsDeleted)
            .ToListAsync();

        var previousWeekRecords = await _context.Set<Data.WorkoutRecord>()
            .AsNoTracking()
            .Where(r => r.UserId == userId && 
                       r.Date >= previousWeekStartDate.Date && 
                       r.Date <= previousWeekEndDate.Date &&
                       !r.IsDeleted)
            .ToListAsync();

        var totalDuration = thisWeekRecords.Sum(r => r.DurationMinutes);
        var totalCalories = thisWeekRecords.Sum(r => r.CaloriesBurned);
        var workoutDays = thisWeekRecords.Select(r => r.Date.Date).Distinct().Count();

        var previousTotalDuration = previousWeekRecords.Sum(r => r.DurationMinutes);
        var previousTotalCalories = previousWeekRecords.Sum(r => r.CaloriesBurned);
        var previousWorkoutDays = previousWeekRecords.Select(r => r.Date.Date).Distinct().Count();

        var dailyBreakdown = BuildDailyBreakdown(thisWeekRecords, weekStartDate);

        var durationChange = CalculatePercentChange(previousTotalDuration, totalDuration);
        var caloriesChange = CalculatePercentChange(previousTotalCalories, totalCalories);
        var workoutDaysChange = CalculatePercentChange(previousWorkoutDays, workoutDays);

        return new WeeklySummaryDto
        {
            WeekStartDate = weekStartDate.Date.ToString("yyyy-MM-dd"),
            WeekEndDate = weekEndDate.Date.ToString("yyyy-MM-dd"),
            TotalDurationMinutes = totalDuration,
            TotalCaloriesBurned = totalCalories,
            WorkoutDays = workoutDays,
            TotalWorkoutCount = thisWeekRecords.Count,
            DailyBreakdown = dailyBreakdown,
            DurationChangePercent = durationChange,
            CaloriesChangePercent = caloriesChange,
            WorkoutDaysChangePercent = workoutDaysChange
        };
    }

    /// <summary>
    /// 取得每日明細
    /// </summary>
    public async Task<DailyBreakdownDto> GetDailyBreakdownAsync(Guid userId, DateTime date)
    {
        var dayRecords = await _context.Set<Data.WorkoutRecord>()
            .AsNoTracking()
            .Where(r => r.UserId == userId && 
                       r.Date.Date == date.Date &&
                       !r.IsDeleted)
            .ToListAsync();

        var totalDuration = dayRecords.Sum(r => r.DurationMinutes);
        var totalCalories = dayRecords.Sum(r => r.CaloriesBurned);

        return new DailyBreakdownDto
        {
            Date = date.Date.ToString("yyyy-MM-dd"),
            DayOfWeek = (int)date.DayOfWeek,
            DurationMinutes = totalDuration,
            CaloriesBurned = totalCalories,
            WorkoutCount = dayRecords.Count
        };
    }

    private List<DailyBreakdownDto> BuildDailyBreakdown(List<Data.WorkoutRecord> records, DateTime weekStart)
    {
        var breakdown = new List<DailyBreakdownDto>();

        for (int i = 0; i < 7; i++)
        {
            var date = weekStart.AddDays(i);
            var dayRecords = records.Where(r => r.Date.Date == date.Date).ToList();

            breakdown.Add(new DailyBreakdownDto
            {
                Date = date.Date.ToString("yyyy-MM-dd"),
                DayOfWeek = (int)date.DayOfWeek,
                DurationMinutes = dayRecords.Sum(r => r.DurationMinutes),
                CaloriesBurned = dayRecords.Sum(r => r.CaloriesBurned),
                WorkoutCount = dayRecords.Count
            });
        }

        return breakdown;
    }

    private decimal CalculatePercentChange(decimal previousValue, decimal currentValue)
    {
        if (previousValue == 0)
        {
            return currentValue > 0 ? 100 : 0;
        }

        return Math.Round(((currentValue - previousValue) / previousValue) * 100, 2);
    }

    private DateTime GetMonday(DateTime date)
    {
        var daysFromMonday = (int)date.DayOfWeek - 1;
        if (daysFromMonday < 0) daysFromMonday = 6;
        return date.AddDays(-daysFromMonday).Date;
    }
}
