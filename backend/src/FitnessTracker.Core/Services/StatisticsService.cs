using FitnessTracker.Core.Entities;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Shared.Dtos.Statistics;

namespace FitnessTracker.Core.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IRepository<WorkoutRecord> _workoutRecordRepository;

        public StatisticsService(IRepository<WorkoutRecord> workoutRecordRepository)
        {
            _workoutRecordRepository = workoutRecordRepository;
        }

        public async Task<WeeklySummaryDto> GetWeeklySummaryAsync(Guid userId, DateTime? date = null)
        {
            var targetDate = date ?? DateTime.UtcNow;
            var weekStart = targetDate.AddDays(-(int)targetDate.DayOfWeek);
            var weekEnd = weekStart.AddDays(7);

            var thisWeekRecords = await _workoutRecordRepository.GetAllAsync();
            thisWeekRecords = thisWeekRecords
                .Where(r => !r.IsDeleted && r.UserId == userId && r.ExerciseDate >= weekStart && r.ExerciseDate < weekEnd)
                .ToList();

            var previousWeekStart = weekStart.AddDays(-7);
            var previousWeekEnd = weekStart;

            var previousWeekRecords = await _workoutRecordRepository.GetAllAsync();
            previousWeekRecords = previousWeekRecords
                .Where(r => !r.IsDeleted && r.UserId == userId && r.ExerciseDate >= previousWeekStart && r.ExerciseDate < previousWeekEnd)
                .ToList();

            var currentTotalDuration = thisWeekRecords.Sum(r => r.DurationMinutes);
            var currentTotalCalories = thisWeekRecords.Sum(r => r.CaloriesBurned);
            var currentWorkoutDays = thisWeekRecords.Select(r => r.ExerciseDate.Date).Distinct().Count();

            var previousTotalDuration = previousWeekRecords.Sum(r => r.DurationMinutes);
            var previousTotalCalories = previousWeekRecords.Sum(r => r.CaloriesBurned);

            var durationChangePercent = previousTotalDuration > 0 
                ? ((currentTotalDuration - previousTotalDuration) / (decimal)previousTotalDuration) * 100 
                : 0;

            var caloriesChangePercent = previousTotalCalories > 0 
                ? ((currentTotalCalories - previousTotalCalories) / previousTotalCalories) * 100 
                : 0;

            return new WeeklySummaryDto
            {
                TotalDurationMinutes = currentTotalDuration,
                TotalCaloriesBurned = currentTotalCalories,
                WorkoutDays = currentWorkoutDays,
                DurationChangePercentage = (decimal)durationChangePercent,
                CaloriesChangePercentage = caloriesChangePercent
            };
        }

        public async Task<TrendDataDto> GetDailyBreakdownAsync(Guid userId, DateTime date)
        {
            var records = await _workoutRecordRepository.GetAllAsync();
            records = records
                .Where(r => !r.IsDeleted && r.UserId == userId && r.ExerciseDate.Date == date.Date)
                .ToList();

            return new TrendDataDto
            {
                Date = date.Date.ToString("yyyy-MM-dd"),
                DurationMinutes = records.Sum(r => r.DurationMinutes),
                CaloriesBurned = records.Sum(r => r.CaloriesBurned),
                Count = records.Count,
                PeriodType = "day"
            };
        }

        public async Task<List<TrendDataDto>> GetTrendsAsync(Guid userId, string periodType = "day")
        {
            var records = await _workoutRecordRepository.GetAllAsync();
            records = records.Where(r => !r.IsDeleted && r.UserId == userId).ToList();

            var result = new List<TrendDataDto>();

            if (periodType == "day")
            {
                var lastDate = DateTime.UtcNow.AddDays(-30);
                var grouped = records
                    .Where(r => r.ExerciseDate >= lastDate)
                    .GroupBy(r => r.ExerciseDate.Date)
                    .OrderBy(g => g.Key);

                foreach (var group in grouped)
                {
                    result.Add(new TrendDataDto
                    {
                        Date = group.Key.ToString("yyyy-MM-dd"),
                        DurationMinutes = group.Sum(r => r.DurationMinutes),
                        CaloriesBurned = group.Sum(r => r.CaloriesBurned),
                        Count = group.Count(),
                        PeriodType = "day"
                    });
                }
            }
            else if (periodType == "week")
            {
                var lastDate = DateTime.UtcNow.AddMonths(-3);
                var grouped = records
                    .Where(r => r.ExerciseDate >= lastDate)
                    .GroupBy(r => new { Year = r.ExerciseDate.Year, Week = GetWeekNumber(r.ExerciseDate) })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Week);

                foreach (var group in grouped)
                {
                    result.Add(new TrendDataDto
                    {
                        Date = $"Week {group.Key.Week}",
                        DurationMinutes = group.Sum(r => r.DurationMinutes),
                        CaloriesBurned = group.Sum(r => r.CaloriesBurned),
                        Count = group.Count(),
                        PeriodType = "week"
                    });
                }
            }
            else if (periodType == "month")
            {
                var lastDate = DateTime.UtcNow.AddYears(-1);
                var grouped = records
                    .Where(r => r.ExerciseDate >= lastDate)
                    .GroupBy(r => new { r.ExerciseDate.Year, r.ExerciseDate.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

                foreach (var group in grouped)
                {
                    result.Add(new TrendDataDto
                    {
                        Date = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("yyyy-MM"),
                        DurationMinutes = group.Sum(r => r.DurationMinutes),
                        CaloriesBurned = group.Sum(r => r.CaloriesBurned),
                        Count = group.Count(),
                        PeriodType = "month"
                    });
                }
            }

            return result;
        }

        public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(Guid userId, int? year = null, int? month = null)
        {
            var targetDate = new DateTime(year ?? DateTime.UtcNow.Year, month ?? DateTime.UtcNow.Month, 1);
            var monthStart = new DateTime(targetDate.Year, targetDate.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var records = await _workoutRecordRepository.GetAllAsync();
            records = records
                .Where(r => !r.IsDeleted && r.UserId == userId && r.ExerciseDate >= monthStart && r.ExerciseDate < monthEnd)
                .ToList();

            var totalDuration = records.Sum(r => r.DurationMinutes);
            var totalCalories = records.Sum(r => r.CaloriesBurned);
            var workoutDays = records.Select(r => r.ExerciseDate.Date).Distinct().Count();

            return new MonthlySummaryDto
            {
                Month = targetDate.ToString("yyyy-MM"),
                TotalDurationMinutes = totalDuration,
                TotalCaloriesBurned = totalCalories,
                WorkoutDays = workoutDays,
                TotalRecords = records.Count,
                AverageDailyDuration = workoutDays > 0 ? totalDuration / workoutDays : 0,
                AverageDailyCalories = workoutDays > 0 ? totalCalories / workoutDays : 0
            };
        }

        public async Task<List<ExerciseDistributionDto>> GetExerciseDistributionAsync(Guid userId)
        {
            var records = await _workoutRecordRepository.GetAllAsync();
            records = records.Where(r => !r.IsDeleted && r.UserId == userId).ToList();

            var totalRecords = records.Count;

            var grouped = records
                .GroupBy(r => r.ExerciseType?.Name ?? "Unknown")
                .Select(g => new ExerciseDistributionDto
                {
                    ExerciseName = g.Key,
                    TotalDurationMinutes = g.Sum(r => r.DurationMinutes),
                    TotalCaloriesBurned = g.Sum(r => r.CaloriesBurned),
                    RecordCount = g.Count(),
                    PercentageOfTotal = totalRecords > 0 ? (g.Count() / (decimal)totalRecords) * 100 : 0
                })
                .OrderByDescending(x => x.RecordCount)
                .ToList();

            return grouped;
        }

        private int GetWeekNumber(DateTime date)
        {
            var cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = cultureInfo.Calendar;
            var calendarWeekRule = cultureInfo.DateTimeFormat.CalendarWeekRule;
            var firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;

            return calendar.GetWeekOfYear(date, calendarWeekRule, firstDayOfWeek);
        }
    }
}
