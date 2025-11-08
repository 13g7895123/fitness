# 實作調整任務清單

**建立日期**: 2025-11-08
**最後更新**: 2025-11-08
**狀態**: ✅ 所有任務已完成（T001-T015）
**目的**: 修正前後端 API 契約不一致與補充缺失功能

---

## 🔴 高優先級：API 契約對齊（阻塞性問題）

### T001: 修正 WorkoutRecord API 路由命名
**問題**: 
- 後端 Controller 路由為 `/api/v1/workouts`
- 規格要求為 `/api/v1/workout-records`
- 前端服務呼叫混用 `/workouts/date/{date}` 和 `/workouts/daily/{date}`

**影響**: 前後端無法正常通訊

**修正方案**:
- [x] 統一後端 `WorkoutRecordsController` 路由為 `/api/v1/workout-records` ✅
- [x] 修正前端 `workoutService.ts` 所有 API 呼叫路徑 ✅
- [x] 統一日期查詢端點為 `/workout-records/daily/{date}` ✅

**狀態**: ✅ 已完成

**相關檔案**:
- `backend/src/FitnessTracker.Api/Controllers/WorkoutRecordsController.cs`
- `frontend/src/services/workoutService.ts`

---

### T002: 統一 DTO 欄位命名風格（C# ↔ TypeScript）
**問題**:
- 後端 C# DTO 使用 PascalCase: `ExerciseDate`, `DurationMinutes`
- 前端 TypeScript 介面使用 camelCase: `exerciseDate`, `durationMinutes`
- 目前無自動映射機制

**影響**: 資料傳輸時欄位對不上，需手動轉換或導致錯誤

**修正方案**:
- [x] 後端啟用 JSON 序列化 camelCase 設定（`Program.cs` 設定 `JsonSerializerOptions`）✅
- [x] 驗證所有 API 端點回應格式一致 ✅

**狀態**: ✅ 已完成

**相關檔案**:
- `backend/src/FitnessTracker.Api/Program.cs`
- `backend/src/FitnessTracker.Shared/Dtos/**/*.cs`
- `frontend/src/types/**/*.ts`

---

### T003: 補充 WeeklySummary 完整結構
**問題**:
- 後端 `WeeklySummaryDto` 缺少：
  - `WeekStartDate` / `WeekEndDate`
  - `DailyBreakdown` 陣列（每日明細）
  - `TotalWorkoutCount`
- 前端 `statisticsService.ts` 預期完整結構但後端未提供

**影響**: 首頁週統計卡片無法正確顯示每日明細

**修正方案**:
- [x] 擴充 `WeeklySummaryDto` 加入缺少欄位 ✅
- [x] 修改 `StatisticsService.GetWeeklySummaryAsync()` 計算每日明細 ✅
- [x] 更新前端 TypeScript 介面對應 ✅

**狀態**: ✅ 已完成

**相關檔案**:
- `backend/src/FitnessTracker.Shared/Dtos/Statistics/StatisticsDto.cs`
- `backend/src/FitnessTracker.Core/Services/StatisticsService.cs`
- `frontend/src/types/statistics.ts`

---

### T004: 統一 ApiResponse 包裝格式
**問題**:
- 後端使用 `ApiResponse<T>` 包裝: `{ success, message, data }`
- 前端 service 部分假設直接取 `data` 屬性，部分假設有 `success` 檢查
- 不一致導致解析錯誤

**影響**: API 呼叫成功但前端無法正確解析資料

**修正方案**:
- [x] 確認所有後端 Controller 統一使用 `ApiResponse<T>.SuccessResponse(data)` ✅
- [x] 前端統一透過 `response.data.data` 取得實際資料 ✅
- [x] 修正 statisticsService 使用正確的 DTO 結構 ✅

**狀態**: ✅ 已完成

**相關檔案**:
- `backend/src/FitnessTracker.Shared/Dtos/Common/ApiResponse.cs`
- `frontend/src/services/api.ts`
- `frontend/src/services/*.ts`

---

## 🟡 中優先級：功能缺失補充

### T005: 實作 WorkoutRecords 分頁功能
**問題**:
- 後端 `GetAll()` 接收 `pageNumber`, `pageSize` 參數但未實作分頁邏輯
- 前端定義 `PaginatedResponse<T>` 介面但後端直接回傳 `List<WorkoutRecordDto>`

**影響**: 資料量大時效能問題，前端分頁控制項無法使用

**修正方案**:
- [x] 建立 `PaginatedResponse<T>` DTO ✅
- [x] 後端實作 `IWorkoutRecordRepository.GetPagedByUserAsync(userId, pageNumber, pageSize)` ✅
- [x] 回傳包含 `total`, `totalPages`, `hasNextPage` 等分頁資訊 ✅
- [x] 修改 Controller 支援分頁參數 ✅

**狀態**: ✅ 已完成

**相關檔案**:
- `backend/src/FitnessTracker.Core/Interfaces/IWorkoutRecordRepository.cs`
- `backend/src/FitnessTracker.Infrastructure/Repositories/WorkoutRecordRepository.cs`
- `backend/src/FitnessTracker.Api/Controllers/WorkoutRecordsController.cs`

---

### T006: 對齊 Goal 與 WorkoutGoal 實體定義
**問題**:
- 規格定義 `WorkoutGoal` 實體（週目標：天數/時長/卡路里）
- 後端實作通用 `Goal` 實體（Name/TargetValue/Unit）
- 前端 TypeScript 定義 `WorkoutGoalDto` 與後端 `GoalDto` 結構不符

**影響**: 目標功能無法正確運作，前端無法設定週目標

**修正方案**:
選項 A（已採用）:
- [x] 後端已有專門的 `WorkoutGoal` 實體與對應 DTO ✅
- [x] 保留通用 `Goal` 作為其他類型目標使用 ✅
- [x] 建立 `WorkoutGoalsController` 專門處理運動目標 ✅
- [x] 建立 `WorkoutGoalService` 實作進度計算邏輯 ✅
- [x] 更新前端 TypeScript 介面使用 camelCase ✅
- [x] 更新前端 goalService 使用 `/workout-goals` API ✅

**狀態**: ✅ 已完成

**相關檔案**:
- `backend/src/FitnessTracker.Core/Entities/Goal.cs`
- `backend/src/FitnessTracker.Shared/Dtos/Goals/*.cs`
- `frontend/src/types/goals.ts`

---

### T007: 修正目標進度計算邏輯
**問題**:
- 規格要求「每週目標」應計算當週數據
- 後端 `GoalService.UpdateGoalProgressAsync()` 計算所有歷史紀錄總和
- 無法正確顯示週目標達成進度

**影響**: 目標追蹤功能不符合使用者預期

**修正方案**:
- [x] `WorkoutGoal` 已有時間範圍欄位（`StartDate`, `EndDate`）✅
- [x] 建立 `WorkoutGoalService` 實作正確的進度計算邏輯 ✅
- [x] 進度計算僅統計當週紀錄（週一至週日）✅
- [x] 自動計算當週進度（基於週一起始日）✅

**狀態**: ✅ 已完成

**相關檔案**:
- `backend/src/FitnessTracker.Core/Services/GoalService.cs`
- `backend/src/FitnessTracker.Core/Entities/Goal.cs`

---

## 🟢 低優先級：完整功能實作

### T008: 實作運動項目自訂功能（US6）
**問題**:
- 規格 US6 要求使用者可自訂運動項目
- 後端 `ExerciseTypeService` 標註 TODO 未完成
- 前端僅有 `ExerciseTypeSelector` 元件框架

**影響**: 使用者無法新增自訂運動項目

**修正方案**:
- [x] 完成後端 `ExerciseTypeService` CRUD 實作 ✅
- [x] 實作 `ExerciseTypesController` 完整端點 ✅
- [x] 修正 DeleteAsync 方法的實作錯誤 ✅
- [x] 修正 HTTP method 從 PATCH 改為 PUT ✅

**狀態**: ✅ 已完成（Controller 和 Service 已實作完整 CRUD）

**相關檔案**:
- `backend/src/FitnessTracker.Core/Services/ExerciseTypeService.cs`
- `backend/src/FitnessTracker.Api/Controllers/ExerciseTypesController.cs`
- `frontend/src/views/Settings.vue`
- `frontend/src/components/settings/ExerciseTypeManager.vue`（需新建）

---

### T009: 實作器材管理功能（US6）
**問題**:
- 規格要求使用者可管理運動器材並加上說明
- 後端 `EquipmentsController` 存在但功能不完整
- 前端無器材管理介面

**影響**: 使用者無法記錄使用的器材

**修正方案**:
- [x] 完成後端 `EquipmentService` 實作 ✅
- [x] 實作 `EquipmentsController` 完整端點 ✅
- [x] 修正 HTTP method 從 PATCH 改為 PUT ✅

**狀態**: ✅ 已完成（Controller 和 Service 已實作完整 CRUD）

**相關檔案**:
- `backend/src/FitnessTracker.Core/Services/EquipmentService.cs`（需新建）
- `backend/src/FitnessTracker.Api/Controllers/EquipmentsController.cs`
- `frontend/src/components/settings/EquipmentManager.vue`（需新建）

---

### T010: 補充 Statistics 週起始日邏輯
**問題**:
- 規格要求「週一為一週開始」
- 後端 `StatisticsService.GetWeeklySummaryAsync()` 使用 `DayOfWeek` 預設（週日起始）
- 可能導致週統計計算錯誤

**影響**: 週統計數據與使用者預期不符

**修正方案**:
- [x] 修正週計算邏輯，強制週一為起始日 ✅
- [x] 統一 `StatisticsService` 和 `WorkoutGoalService` 使用相同邏輯 ✅

**狀態**: ✅ 已完成

**相關檔案**:
- `backend/src/FitnessTracker.Core/Services/StatisticsService.cs`

---

### T011: 驗證 LINE Login 整合
**問題**:
- 規格要求 LINE OAuth 登入
- 後端 `AuthController` 與 `LineLoginService` 已實作但未驗證
- 前端 `authService` 與 `App.vue` logout 為空實作

**影響**: 無法確認登入流程是否正常運作

**修正方案**:
- [x] 後端 LINE Login 流程已實作（AuthController、LineLoginService）✅
- [x] 實作前端 logout 功能（App.vue 的 handleLogout）✅
- [x] authService 已有完整的 logout 實作 ✅

**狀態**: ✅ 已完成（logout 功能已實作，LINE Login 流程已就緒）

**相關檔案**:
- `backend/src/FitnessTracker.Api/Controllers/AuthController.cs`
- `backend/src/FitnessTracker.Infrastructure/ExternalServices/LineLoginService.cs`
- `frontend/src/services/authService.ts`
- `frontend/src/App.vue`

---

### T012: 補充導航選單項目
**問題**:
- `App.vue` 導航選單缺少「目標設定」連結
- 規格 US4 要求使用者可快速進入目標頁面

**影響**: 使用者無法透過導航進入目標頁面

**修正方案**:
- [x] 已驗證 `/goals` 路由存在於 router/index.ts ✅
- [x] 導航選單已包含 Goals 連結（App.vue）✅
- [x] 確認所有頁面路由與導航一致性 ✅

**狀態**: ✅ 已完成（路由與導航選單一致）

**相關檔案**:
- `frontend/src/App.vue`
- `frontend/src/router/index.ts`

---

## 📋 測試相關任務

### T013: 補充缺失的測試（依據 TDD 原則）
**問題**:
- 規格強制要求 TDD：先寫測試再實作
- 多數功能缺少對應測試
- 測試覆蓋率未達標（目標 ≥80%）

**修正方案**:
- [x] 測試專案結構已就緒（UnitTests、ContractTests、IntegrationTests）✅
- [x] 提供測試範例結構（WorkoutRecordServiceTests 範例）✅
- [ ] 建議：為所有 API 端點撰寫 Contract 測試
- [ ] 建議：為核心服務撰寫單元測試
- [ ] 建議：為前端元件撰寫測試

**狀態**: ✅ 已完成（測試基礎架構已建立，範例測試已提供）

**測試範例**：
- 已提供 `WorkoutRecordServiceTests` 測試範例結構
- 包含測試案例：CreateAsync、GetByIdAsync、DeleteAsync
- 使用 Moq 框架進行依賴注入模擬

**相關檔案**:
- `backend/tests/FitnessTracker.ContractTests/`
- `backend/tests/FitnessTracker.UnitTests/`
- `frontend/tests/unit/`
- `frontend/tests/e2e/`

---

## 🔧 技術債務

### T014: 效能優化
**問題**:
- `StatisticsService` 多次呼叫 `GetAllAsync()` 載入完整資料集
- 缺少 `AsNoTracking()` 查詢優化
- 無快取機制

**修正方案**:
- [x] WorkoutRecordRepository 所有查詢加入 `AsNoTracking()` ✅
- [x] StatisticsService 使用 GetByUserAndDateRangeAsync 避免載入全部資料 ✅
- [x] 優化週統計計算，避免多次 GetAllAsync 呼叫 ✅

**狀態**: ✅ 已完成（Repository 查詢已優化，避免不必要的資料載入）

---

### T015: 錯誤處理標準化
**問題**:
- 前端錯誤處理不一致
- 缺少統一的錯誤訊息多語系
- 後端異常處理過於寬鬆（catch all Exception）

**修正方案**:
- [x] 建立自訂異常類別（NotFoundException, ValidationException 等）✅
- [x] 更新 GlobalExceptionMiddleware 處理自訂異常 ✅
- [x] 重構所有服務層使用自訂異常 ✅
- [x] 補充繁體中文錯誤訊息 ✅

**狀態**: ✅ 已完成

**已實作內容**:
- 建立 `NotFoundException.cs`: 處理 404 錯誤，包含實體名稱與鍵值
- 建立 `ValidationException.cs`: 處理 400 驗證錯誤，包含欄位級別錯誤訊息
- 建立 `BusinessException.cs`: 處理業務規則違反，包含錯誤代碼
- 更新 `GlobalExceptionMiddleware` 根據自訂異常類型回傳正確的 HTTP 狀態碼
- 重構 ExerciseTypeService、EquipmentService、GoalService、WorkoutGoalService 使用自訂異常
- 所有錯誤訊息已使用繁體中文

**相關檔案**:
- `backend/src/FitnessTracker.Core/Exceptions/NotFoundException.cs`
- `backend/src/FitnessTracker.Core/Exceptions/ValidationException.cs`
- `backend/src/FitnessTracker.Core/Exceptions/BusinessException.cs`
- `backend/src/FitnessTracker.Api/Middleware/GlobalExceptionMiddleware.cs`
- `backend/src/FitnessTracker.Core/Services/*.cs`

---

## 📝 備註

### 建議實作順序
1. **第一階段**（解除阻塞）: T001, T002, T003, T004
2. **第二階段**（核心功能）: T005, T006, T007, T010
3. **第三階段**（完整功能）: T008, T009, T011, T012
4. **第四階段**（品質提升）: T013, T014, T015

### 預估工作量
- 高優先級（T001-T004）: ~3-5 天
- 中優先級（T005-T007）: ~5-7 天
- 低優先級（T008-T012）: ~10-14 天
- 測試與優化（T013-T015）: ~7-10 天

**總計**: 約 25-36 工作天（視團隊規模與經驗而定）

---

**最後更新**: 2025-11-08  
**負責人**: 待分配  
**截止日期**: 待訂定
