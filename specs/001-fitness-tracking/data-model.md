# 資料模型文件 - 健身紀錄追蹤功能

**專案**: 健身紀錄追蹤系統  
**日期**: 2025-11-02  
**分支**: 001-fitness-tracking  
**階段**: Phase 1 - 設計

## 實體關係圖（ERD）

```text
┌─────────────────┐         ┌──────────────────┐
│      User       │         │   ExerciseType   │
├─────────────────┤         ├──────────────────┤
│ Id (PK)         │         │ Id (PK)          │
│ LineUserId      │         │ Name             │
│ DisplayName     │         │ Category         │
│ PictureUrl      │◄────┐   │ IsCustom         │
│ CreatedAt       │     │   │ UserId (FK)      │
│ UpdatedAt       │     │   │ IsActive         │
└─────────────────┘     │   │ CreatedAt        │
                        │   └──────────────────┘
                        │            ▲
                        │            │
                        │   ┌────────────────┐
                        │   │   Equipment    │
                        │   ├────────────────┤
                        │   │ Id (PK)        │
                        │   │ Name           │
                        │   │ IsActive       │
                        │   └────────────────┘
                        │            │
                        │            │
┌─────────────────┐     │   ┌────────────────────────────┐
│  WorkoutGoal    │     │   │     WorkoutRecord          │
├─────────────────┤     │   ├────────────────────────────┤
│ Id (PK)         │     └───┤ Id (PK)                    │
│ UserId (FK)     ├─────────┤ UserId (FK)                │
│ WeeklyMinutes   │         │ Date                       │
│ WeeklyCalories  │         │ ExerciseTypeId (FK)        ├───┐
│ StartDate       │         │ EquipmentId (FK, nullable) ├─┐ │
│ EndDate         │         │ DurationMinutes            │ │ │
│ IsActive        │         │ CaloriesBurned             │ │ │
│ CreatedAt       │         │ Weight (nullable)          │ │ │
│ UpdatedAt       │         │ Notes (nullable)           │ │ │
└─────────────────┘         │ CreatedAt                  │ │ │
                            │ UpdatedAt                  │ │ │
                            │ IsDeleted                  │ │ │
                            └────────────────────────────┘ │ │
                                        │                  │ │
                                        └──────────────────┘ │
                                                             │
                                                             └─────┐
                                                                   ▼
```

---

## 實體定義

### 1. User（使用者）

**用途**: 儲存通過 LINE Login 身份驗證的使用者資訊

**屬性**:
| 欄位名稱 | 型別 | 限制 | 說明 |
|---------|------|------|------|
| Id | Guid | PK, NOT NULL | 系統內部唯一識別碼 |
| LineUserId | string(100) | UNIQUE, NOT NULL | LINE 使用者 ID |
| DisplayName | string(100) | NOT NULL | 顯示名稱 |
| PictureUrl | string(500) | NULL | 大頭照 URL |
| CreatedAt | DateTime | NOT NULL | 建立時間 |
| UpdatedAt | DateTime | NOT NULL | 更新時間 |

**索引**:
- PRIMARY KEY: `Id`
- UNIQUE INDEX: `LineUserId`

**EF Core 設定**:
```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.LineUserId)
               .IsRequired()
               .HasMaxLength(100);
        
        builder.HasIndex(u => u.LineUserId)
               .IsUnique();
        
        builder.Property(u => u.DisplayName)
               .IsRequired()
               .HasMaxLength(100);
        
        builder.Property(u => u.PictureUrl)
               .HasMaxLength(500);
        
        builder.Property(u => u.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");
        
        builder.Property(u => u.UpdatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");
    }
}
```

---

### 2. ExerciseType（運動項目）

**用途**: 儲存系統預設與使用者自訂的運動項目

**屬性**:
| 欄位名稱 | 型別 | 限制 | 說明 |
|---------|------|------|------|
| Id | Guid | PK, NOT NULL | 唯一識別碼 |
| Name | string(100) | NOT NULL | 運動名稱 |
| Category | string(50) | NOT NULL | 類別（有氧、重訓、伸展、其他） |
| IsCustom | bool | NOT NULL | 是否為使用者自訂 |
| UserId | Guid | FK, NULL | 建立者（系統預設為 NULL） |
| IsActive | bool | NOT NULL | 是否啟用 |
| CreatedAt | DateTime | NOT NULL | 建立時間 |

**索引**:
- PRIMARY KEY: `Id`
- INDEX: `(UserId, IsActive)` - 用於查詢使用者的啟用項目
- INDEX: `Name` - 用於搜尋

**預設資料** (Seed Data):
```csharp
// Migration Seed
migrationBuilder.InsertData(
    table: "ExerciseTypes",
    columns: new[] { "Id", "Name", "Category", "IsCustom", "UserId", "IsActive", "CreatedAt" },
    values: new object[,]
    {
        { Guid.NewGuid(), "跑步", "有氧", false, null, true, DateTime.UtcNow },
        { Guid.NewGuid(), "游泳", "有氧", false, null, true, DateTime.UtcNow },
        { Guid.NewGuid(), "騎自行車", "有氧", false, null, true, DateTime.UtcNow },
        { Guid.NewGuid(), "重量訓練", "重訓", false, null, true, DateTime.UtcNow },
        { Guid.NewGuid(), "瑜伽", "伸展", false, null, true, DateTime.UtcNow },
        { Guid.NewGuid(), "皮拉提斯", "伸展", false, null, true, DateTime.UtcNow },
        { Guid.NewGuid(), "其他", "其他", false, null, true, DateTime.UtcNow }
    });
```

**EF Core 設定**:
```csharp
public class ExerciseTypeConfiguration : IEntityTypeConfiguration<ExerciseType>
{
    public void Configure(EntityTypeBuilder<ExerciseType> builder)
    {
        builder.ToTable("ExerciseTypes");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(100);
        
        builder.Property(e => e.Category)
               .IsRequired()
               .HasMaxLength(50);
        
        builder.Property(e => e.IsCustom)
               .IsRequired()
               .HasDefaultValue(false);
        
        builder.Property(e => e.IsActive)
               .IsRequired()
               .HasDefaultValue(true);
        
        builder.HasIndex(e => new { e.UserId, e.IsActive });
        builder.HasIndex(e => e.Name);
        
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

### 3. Equipment（運動器材）

**用途**: 儲存可選的運動器材清單

**屬性**:
| 欄位名稱 | 型別 | 限制 | 說明 |
|---------|------|------|------|
| Id | Guid | PK, NOT NULL | 唯一識別碼 |
| Name | string(100) | NOT NULL | 器材名稱 |
| IsActive | bool | NOT NULL | 是否啟用 |

**預設資料**:
```csharp
migrationBuilder.InsertData(
    table: "Equipments",
    columns: new[] { "Id", "Name", "IsActive" },
    values: new object[,]
    {
        { Guid.NewGuid(), "跑步機", true },
        { Guid.NewGuid(), "啞鈴", true },
        { Guid.NewGuid(), "槓鈴", true },
        { Guid.NewGuid(), "瑜伽墊", true },
        { Guid.NewGuid(), "飛輪", true },
        { Guid.NewGuid(), "其他", true }
    });
```

**EF Core 設定**:
```csharp
public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipments");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(100);
        
        builder.Property(e => e.IsActive)
               .IsRequired()
               .HasDefaultValue(true);
    }
}
```

---

### 4. WorkoutRecord（健身紀錄）

**用途**: 儲存使用者的健身紀錄

**屬性**:
| 欄位名稱 | 型別 | 限制 | 說明 |
|---------|------|------|------|
| Id | Guid | PK, NOT NULL | 唯一識別碼 |
| UserId | Guid | FK, NOT NULL | 使用者 ID |
| Date | DateTime | NOT NULL | 日期（僅日期部分，不含時間） |
| ExerciseTypeId | Guid | FK, NOT NULL | 運動項目 ID |
| EquipmentId | Guid | FK, NULL | 運動器材 ID（可選） |
| DurationMinutes | int | NOT NULL | 運動時長（分鐘）CHECK (1 ≤ DurationMinutes ≤ 480) |
| CaloriesBurned | decimal(18,2) | NOT NULL | 消耗熱量（大卡）CHECK (1 ≤ CaloriesBurned ≤ 5000) |
| Weight | decimal(5,2) | NULL | 體重（公斤）用於卡路里計算 |
| Notes | string(500) | NULL | 備註 |
| CreatedAt | DateTime | NOT NULL | 建立時間 |
| UpdatedAt | DateTime | NOT NULL | 更新時間 |
| IsDeleted | bool | NOT NULL | 軟刪除標記 |

**索引**:
- PRIMARY KEY: `Id`
- UNIQUE INDEX: `(UserId, Date, ExerciseTypeId)` - 防止重複紀錄
- INDEX: `(UserId, Date)` - 用於日期範圍查詢
- INDEX: `Date` - 用於統計查詢

**EF Core 設定**:
```csharp
public class WorkoutRecordConfiguration : IEntityTypeConfiguration<WorkoutRecord>
{
    public void Configure(EntityTypeBuilder<WorkoutRecord> builder)
    {
        builder.ToTable("WorkoutRecords");
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Date)
               .IsRequired()
               .HasColumnType("date");
        
        builder.Property(w => w.DurationMinutes)
               .IsRequired();
        
        builder.Property(w => w.CaloriesBurned)
               .IsRequired()
               .HasPrecision(18, 2);
        
        builder.Property(w => w.Weight)
               .HasPrecision(5, 2);
        
        builder.Property(w => w.Notes)
               .HasMaxLength(500);
        
        builder.Property(w => w.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);
        
        // 索引
        builder.HasIndex(w => new { w.UserId, w.Date, w.ExerciseTypeId })
               .IsUnique()
               .HasFilter("[IsDeleted] = 0");  // 只對未刪除紀錄生效
        
        builder.HasIndex(w => new { w.UserId, w.Date });
        builder.HasIndex(w => w.Date);
        
        // 關聯
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(w => w.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<ExerciseType>()
               .WithMany()
               .HasForeignKey(w => w.ExerciseTypeId)
               .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<Equipment>()
               .WithMany()
               .HasForeignKey(w => w.EquipmentId)
               .OnDelete(DeleteBehavior.SetNull);
        
        // 全域查詢過濾器（軟刪除）
        builder.HasQueryFilter(w => !w.IsDeleted);
        
        // CHECK 約束（需手動加入 Migration）
        // CHECK ([DurationMinutes] >= 1 AND [DurationMinutes] <= 480)
        // CHECK ([CaloriesBurned] >= 1 AND [CaloriesBurned] <= 5000)
    }
}
```

**Migration 補充（CHECK 約束）**:
```csharp
migrationBuilder.Sql(@"
    ALTER TABLE WorkoutRecords 
    ADD CONSTRAINT CHK_WorkoutRecords_DurationMinutes 
    CHECK (DurationMinutes >= 1 AND DurationMinutes <= 480);
    
    ALTER TABLE WorkoutRecords 
    ADD CONSTRAINT CHK_WorkoutRecords_CaloriesBurned 
    CHECK (CaloriesBurned >= 1 AND CaloriesBurned <= 5000);
");
```

---

### 5. WorkoutGoal（運動目標）

**用途**: 儲存使用者的運動目標

**屬性**:
| 欄位名稱 | 型別 | 限制 | 說明 |
|---------|------|------|------|
| Id | Guid | PK, NOT NULL | 唯一識別碼 |
| UserId | Guid | FK, NOT NULL | 使用者 ID |
| WeeklyMinutes | int | NULL | 每週目標時長（分鐘） |
| WeeklyCalories | decimal(18,2) | NULL | 每週目標熱量（大卡） |
| StartDate | DateTime | NOT NULL | 目標開始日期 |
| EndDate | DateTime | NULL | 目標結束日期（NULL 表示持續有效） |
| IsActive | bool | NOT NULL | 是否啟用 |
| CreatedAt | DateTime | NOT NULL | 建立時間 |
| UpdatedAt | DateTime | NOT NULL | 更新時間 |

**索引**:
- PRIMARY KEY: `Id`
- INDEX: `(UserId, IsActive)` - 用於查詢使用者的啟用目標

**EF Core 設定**:
```csharp
public class WorkoutGoalConfiguration : IEntityTypeConfiguration<WorkoutGoal>
{
    public void Configure(EntityTypeBuilder<WorkoutGoal> builder)
    {
        builder.ToTable("WorkoutGoals");
        builder.HasKey(g => g.Id);
        
        builder.Property(g => g.WeeklyMinutes);
        
        builder.Property(g => g.WeeklyCalories)
               .HasPrecision(18, 2);
        
        builder.Property(g => g.StartDate)
               .IsRequired()
               .HasColumnType("date");
        
        builder.Property(g => g.EndDate)
               .HasColumnType("date");
        
        builder.Property(g => g.IsActive)
               .IsRequired()
               .HasDefaultValue(true);
        
        builder.HasIndex(g => new { g.UserId, g.IsActive });
        
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(g => g.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

## 資料驗證規則

### FluentValidation 驗證器

**WorkoutRecordValidator**:
```csharp
public class WorkoutRecordValidator : AbstractValidator<WorkoutRecord>
{
    public WorkoutRecordValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("使用者 ID 為必填");
        
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("日期為必填")
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("日期不可為未來");
        
        RuleFor(x => x.ExerciseTypeId)
            .NotEmpty()
            .WithMessage("運動項目為必填");
        
        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(1, 480)
            .WithMessage("運動時長必須介於 1 至 480 分鐘");
        
        RuleFor(x => x.CaloriesBurned)
            .InclusiveBetween(1, 5000)
            .WithMessage("消耗熱量必須介於 1 至 5000 大卡");
        
        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .When(x => x.Weight.HasValue)
            .WithMessage("體重必須大於 0");
        
        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("備註不可超過 500 字元");
    }
}
```

**WorkoutGoalValidator**:
```csharp
public class WorkoutGoalValidator : AbstractValidator<WorkoutGoal>
{
    public WorkoutGoalValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("使用者 ID 為必填");
        
        RuleFor(x => x)
            .Must(x => x.WeeklyMinutes.HasValue || x.WeeklyCalories.HasValue)
            .WithMessage("每週目標時長或熱量至少需設定一項");
        
        RuleFor(x => x.WeeklyMinutes)
            .GreaterThan(0)
            .When(x => x.WeeklyMinutes.HasValue)
            .WithMessage("每週目標時長必須大於 0");
        
        RuleFor(x => x.WeeklyCalories)
            .GreaterThan(0)
            .When(x => x.WeeklyCalories.HasValue)
            .WithMessage("每週目標熱量必須大於 0");
        
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("目標開始日期為必填");
        
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("目標結束日期必須晚於開始日期");
    }
}
```

---

## 卡路里自動計算邏輯

**計算公式** (根據 FR-026):
```csharp
public class CalorieCalculationService : ICalorieCalculationService
{
    // MET 值（代謝當量）- 每種運動的標準值
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
    
    /// <summary>
    /// 計算消耗熱量
    /// 公式: 卡路里 = MET × 體重(kg) × 時間(小時)
    /// </summary>
    public decimal CalculateCalories(string exerciseName, int durationMinutes, decimal? weight)
    {
        if (!weight.HasValue)
        {
            throw new ArgumentException("計算卡路里需要體重資料");
        }
        
        if (!_metValues.TryGetValue(exerciseName, out var met))
        {
            met = 4.0;  // 預設值
        }
        
        var hours = durationMinutes / 60.0;
        var calories = (decimal)(met * (double)weight.Value * hours);
        
        return Math.Round(calories, 2);
    }
    
    /// <summary>
    /// 提供預估值（使用平均體重 70kg）
    /// </summary>
    public decimal EstimateCalories(string exerciseName, int durationMinutes)
    {
        const decimal averageWeight = 70m;
        return CalculateCalories(exerciseName, durationMinutes, averageWeight);
    }
}
```

**使用範例**:
```csharp
// WorkoutService.cs
public async Task<WorkoutRecord> CreateWorkoutRecord(WorkoutRecordDto dto)
{
    var exerciseType = await _context.ExerciseTypes.FindAsync(dto.ExerciseTypeId);
    
    // 若未提供卡路里，則自動計算
    if (!dto.CaloriesBurned.HasValue)
    {
        dto.CaloriesBurned = dto.Weight.HasValue
            ? _calorieService.CalculateCalories(exerciseType.Name, dto.DurationMinutes, dto.Weight)
            : _calorieService.EstimateCalories(exerciseType.Name, dto.DurationMinutes);
    }
    
    var record = new WorkoutRecord
    {
        UserId = dto.UserId,
        Date = dto.Date,
        ExerciseTypeId = dto.ExerciseTypeId,
        DurationMinutes = dto.DurationMinutes,
        CaloriesBurned = dto.CaloriesBurned.Value,
        Weight = dto.Weight,
        Notes = dto.Notes
    };
    
    _context.WorkoutRecords.Add(record);
    await _context.SaveChangesAsync();
    
    return record;
}
```

---

## 資料庫 Migration 腳本

**初始 Migration**:
```bash
# 產生 Migration
dotnet ef migrations add InitialCreate --project src/FitnessTracker.Infrastructure --startup-project src/FitnessTracker.Api

# 更新資料庫
dotnet ef database update --project src/FitnessTracker.Infrastructure --startup-project src/FitnessTracker.Api
```

**SQL Schema 預覽**:
```sql
-- Users
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LineUserId NVARCHAR(100) NOT NULL UNIQUE,
    DisplayName NVARCHAR(100) NOT NULL,
    PictureUrl NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- ExerciseTypes
CREATE TABLE ExerciseTypes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    IsCustom BIT NOT NULL DEFAULT 0,
    UserId UNIQUEIDENTIFIER NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE INDEX IX_ExerciseTypes_UserId_IsActive ON ExerciseTypes(UserId, IsActive);
CREATE INDEX IX_ExerciseTypes_Name ON ExerciseTypes(Name);

-- Equipments
CREATE TABLE Equipments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- WorkoutRecords
CREATE TABLE WorkoutRecords (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Date DATE NOT NULL,
    ExerciseTypeId UNIQUEIDENTIFIER NOT NULL,
    EquipmentId UNIQUEIDENTIFIER NULL,
    DurationMinutes INT NOT NULL,
    CaloriesBurned DECIMAL(18,2) NOT NULL,
    Weight DECIMAL(5,2) NULL,
    Notes NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (ExerciseTypeId) REFERENCES ExerciseTypes(Id),
    FOREIGN KEY (EquipmentId) REFERENCES Equipments(Id) ON DELETE SET NULL,
    CONSTRAINT CHK_WorkoutRecords_DurationMinutes CHECK (DurationMinutes >= 1 AND DurationMinutes <= 480),
    CONSTRAINT CHK_WorkoutRecords_CaloriesBurned CHECK (CaloriesBurned >= 1 AND CaloriesBurned <= 5000)
);
CREATE UNIQUE INDEX IX_WorkoutRecords_UserId_Date_ExerciseTypeId ON WorkoutRecords(UserId, Date, ExerciseTypeId) WHERE IsDeleted = 0;
CREATE INDEX IX_WorkoutRecords_UserId_Date ON WorkoutRecords(UserId, Date);
CREATE INDEX IX_WorkoutRecords_Date ON WorkoutRecords(Date);

-- WorkoutGoals
CREATE TABLE WorkoutGoals (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    WeeklyMinutes INT NULL,
    WeeklyCalories DECIMAL(18,2) NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE INDEX IX_WorkoutGoals_UserId_IsActive ON WorkoutGoals(UserId, IsActive);
```

---

## 查詢效能考量

### 常見查詢與索引對應

1. **取得使用者本週紀錄** (FR-001):
   ```csharp
   var records = await _context.WorkoutRecords
       .AsNoTracking()
       .Where(w => w.UserId == userId && w.Date >= startDate && w.Date <= endDate)
       .Include(w => w.ExerciseType)
       .OrderBy(w => w.Date)
       .ToListAsync();
   ```
   **使用索引**: `IX_WorkoutRecords_UserId_Date`

2. **取得特定日期紀錄** (FR-004):
   ```csharp
   var records = await _context.WorkoutRecords
       .AsNoTracking()
       .Where(w => w.UserId == userId && w.Date == date)
       .Include(w => w.ExerciseType)
       .Include(w => w.Equipment)
       .ToListAsync();
   ```
   **使用索引**: `IX_WorkoutRecords_UserId_Date`

3. **搜尋運動項目** (FR-022):
   ```csharp
   var types = await _context.ExerciseTypes
       .AsNoTracking()
       .Where(e => (e.UserId == userId || !e.IsCustom) && e.IsActive)
       .Where(e => e.Name.Contains(keyword))
       .ToListAsync();
   ```
   **使用索引**: `IX_ExerciseTypes_Name`, `IX_ExerciseTypes_UserId_IsActive`

4. **計算目標進度** (FR-018):
   ```csharp
   var weeklyTotal = await _context.WorkoutRecords
       .Where(w => w.UserId == userId && w.Date >= startOfWeek && w.Date <= endOfWeek)
       .GroupBy(w => 1)
       .Select(g => new
       {
           TotalMinutes = g.Sum(w => w.DurationMinutes),
           TotalCalories = g.Sum(w => w.CaloriesBurned)
       })
       .FirstOrDefaultAsync();
   ```
   **使用索引**: `IX_WorkoutRecords_UserId_Date` (Covering Index 可進一步最佳化)

---

## 資料一致性保證

### 交易管理

**新增紀錄時檢查重複** (FR-015):
```csharp
public async Task<WorkoutRecord> CreateWorkoutRecord(WorkoutRecordDto dto)
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        // 檢查是否已存在相同紀錄
        var exists = await _context.WorkoutRecords
            .AnyAsync(w => w.UserId == dto.UserId 
                        && w.Date == dto.Date 
                        && w.ExerciseTypeId == dto.ExerciseTypeId 
                        && !w.IsDeleted);
        
        if (exists)
        {
            throw new DuplicateRecordException("該日期已有相同運動項目的紀錄");
        }
        
        var record = new WorkoutRecord { /* ... */ };
        _context.WorkoutRecords.Add(record);
        await _context.SaveChangesAsync();
        
        await transaction.CommitAsync();
        return record;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### 軟刪除實作

```csharp
public async Task DeleteWorkoutRecord(Guid id)
{
    var record = await _context.WorkoutRecords
        .IgnoreQueryFilters()  // 忽略全域過濾器
        .FirstOrDefaultAsync(w => w.Id == id);
    
    if (record == null)
        throw new NotFoundException("紀錄不存在");
    
    record.IsDeleted = true;
    record.UpdatedAt = DateTime.UtcNow;
    
    await _context.SaveChangesAsync();
}
```

---

## 測試資料產生

**單元測試用 Fixture**:
```csharp
public class WorkoutRecordFixture
{
    public static WorkoutRecord CreateValid(Guid userId, Guid exerciseTypeId)
    {
        return new WorkoutRecord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = DateTime.Today,
            ExerciseTypeId = exerciseTypeId,
            DurationMinutes = 30,
            CaloriesBurned = 200m,
            Weight = 70m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }
    
    public static WorkoutRecord CreateWithInvalidDuration(Guid userId, Guid exerciseTypeId)
    {
        var record = CreateValid(userId, exerciseTypeId);
        record.DurationMinutes = 500;  // 超過 480 分鐘限制
        return record;
    }
}
```

---

## 下一步

資料模型已完成定義，接下來將產生：
1. **API Contracts** (`contracts/`)：定義 RESTful API 規格
2. **Quickstart Guide** (`quickstart.md`)：開發環境設定指南
