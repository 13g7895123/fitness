namespace FitnessTracker.Core.Services;

public interface ICalorieCalculationService
{
    decimal CalculateCalories(string exerciseName, int durationMinutes, decimal? weight);
    decimal EstimateCalories(string exerciseName, int durationMinutes);
}

public class CalorieCalculationService : ICalorieCalculationService
{
    private readonly Dictionary<string, double> _metValues = new()
    {
        { "跑步", 8.0 },
        { "游泳", 7.0 },
        { "騎自行車", 6.0 },
        { "重量訓練", 5.0 },
        { "瑜伽", 3.0 },
        { "皮拉提斯", 3.5 },
        { "其他", 4.0 }
    };

    public decimal CalculateCalories(string exerciseName, int durationMinutes, decimal? weight)
    {
        if (!weight.HasValue)
        {
            throw new ArgumentException("計算卡路里需要體重資料");
        }

        if (!_metValues.TryGetValue(exerciseName, out var met))
        {
            met = 4.0;
        }

        var hours = durationMinutes / 60.0;
        var calories = (decimal)(met * (double)weight.Value * hours);

        return Math.Round(calories, 2);
    }

    public decimal EstimateCalories(string exerciseName, int durationMinutes)
    {
        const decimal averageWeight = 70m;
        return CalculateCalories(exerciseName, durationMinutes, averageWeight);
    }
}
