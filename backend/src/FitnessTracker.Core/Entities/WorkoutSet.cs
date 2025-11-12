namespace FitnessTracker.Core.Entities;

/// <summary>
/// 訓練組數記錄
/// </summary>
public class WorkoutSet
{
    public int Id { get; set; }

    /// <summary>
    /// 所屬的訓練記錄 ID
    /// </summary>
    public int WorkoutRecordId { get; set; }

    /// <summary>
    /// 所屬的訓練記錄
    /// </summary>
    public WorkoutRecord WorkoutRecord { get; set; } = null!;

    /// <summary>
    /// 組數順序 (第幾組)
    /// </summary>
    public int SetNumber { get; set; }

    /// <summary>
    /// 次數 (重複次數)
    /// </summary>
    public int Repetitions { get; set; }

    /// <summary>
    /// 重量 (公斤)
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否已刪除
    /// </summary>
    public bool IsDeleted { get; set; }
}
