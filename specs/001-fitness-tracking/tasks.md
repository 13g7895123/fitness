# 任務清單：健身紀錄追蹤系統

**輸入**: 設計文件來自 `/specs/001-fitness-tracking/`
**前置需求**: plan.md (必要), spec.md (使用者故事必要), research.md, data-model.md, contracts/

**測試**: 測試任務為憲章原則 II（測試標準）之強制要求。所有功能必須遵循 TDD：先寫測試、取得核准、確認失敗、再實作。測試金字塔比例：70% 單元測試、20% 整合測試、10% E2E 測試。所有 API 端點皆需 Contract 測試。

**語言**: 本文件必須使用繁體中文 (zh-TW) 撰寫（依據憲章文件標準）。所有任務描述、使用者故事和文件皆使用繁體中文。

**組織方式**: 任務依使用者故事分組，讓每個故事可獨立實作與測試。

## 格式：`[ID] [P?] [Story] 描述`

- **[P]**: 可平行執行（不同檔案、無相依性）
- **[Story]**: 此任務屬於哪個使用者故事（如 US1, US2, US3）
- 描述中包含確切的檔案路徑

## 路徑規範

- **Web 應用程式**: `backend/src/`, `frontend/src/`
- 根據 plan.md 定義的專案結構調整路徑

---

## Phase 1: 環境設定（共享基礎設施）

**目的**: 專案初始化與基本結構建立（採用容器化開發環境）

**開發模式**: 使用 Docker 容器作為開發環境，無需在本機安裝 .NET SDK 或 Node.js

- [x] T001 建立專案目錄結構（backend/, frontend/, .github/workflows/）與 Docker Compose 開發環境設定檔 docker-compose.dev.yml
- [x] T002 初始化後端 .NET 專案於容器內（FitnessTracker.Api, FitnessTracker.Core, FitnessTracker.Infrastructure, FitnessTracker.Shared）
- [x] T003 [P] 初始化後端測試專案於容器內（FitnessTracker.UnitTests, FitnessTracker.IntegrationTests, FitnessTracker.ContractTests）
- [x] T004 [P] 初始化前端 Vue.js 3 + Vite 專案於容器內 frontend/ 目錄
- [x] T005 [P] 設定後端 EditorConfig 與 StyleCop 於 backend/.editorconfig
- [x] T006 [P] 設定前端 ESLint 與 Prettier 於 frontend/.eslintrc.js, frontend/.prettierrc
- [x] T007 安裝後端 NuGet 套件於容器內（EF Core 8, FluentValidation, Serilog, xUnit）於 backend/src/ 各專案
- [x] T008 [P] 安裝前端 npm 套件於容器內（Vue Router, Pinia, Axios, Vuetify 3, Chart.js, Vitest）於 frontend/package.json
- [x] T009 [P] 設定後端環境變數範本於 backend/.env.example（ConnectionStrings, LINE Login, JWT）
- [x] T010 [P] 設定前端環境變數範本於 frontend/.env.development.example 與 frontend/.env.production.example
- [x] T011 建立 GitHub Actions CI/CD workflows 於 .github/workflows/backend-ci.yml 與 .github/workflows/frontend-ci.yml

**執行方式**: 所有 T002-T008 任務都在對應的開發容器內執行（docker exec -it fitness-backend-dev bash 或 docker exec -it fitness-frontend-dev sh）

---

## Phase 2: 基礎架構（阻塞性前置條件）

**目的**: 核心基礎設施，所有使用者故事必須等待此階段完成後才能開始

**⚠️ 關鍵**: 所有使用者故事工作必須等待此階段完成

### 資料庫與 ORM 設定

- [x] T012 建立 FitnessTrackerDbContext 於 backend/src/FitnessTracker.Infrastructure/Data/FitnessTrackerDbContext.cs
- [x] T013 [P] 建立 User 實體與設定於 backend/src/FitnessTracker.Core/Entities/User.cs 與 backend/src/FitnessTracker.Infrastructure/Data/Configurations/UserConfiguration.cs
- [x] T014 [P] 建立 ExerciseType 實體與設定於 backend/src/FitnessTracker.Core/Entities/ExerciseType.cs 與 backend/src/FitnessTracker.Infrastructure/Data/Configurations/ExerciseTypeConfiguration.cs
- [x] T015 [P] 建立 Equipment 實體與設定於 backend/src/FitnessTracker.Core/Entities/Equipment.cs 與 backend/src/FitnessTracker.Infrastructure/Data/Configurations/EquipmentConfiguration.cs
- [x] T016 [P] 建立 WorkoutRecord 實體與設定於 backend/src/FitnessTracker.Core/Entities/WorkoutRecord.cs 與 backend/src/FitnessTracker.Infrastructure/Data/Configurations/WorkoutRecordConfiguration.cs
- [x] T017 [P] 建立 WorkoutGoal 實體與設定於 backend/src/FitnessTracker.Core/Entities/WorkoutGoal.cs 與 backend/src/FitnessTracker.Infrastructure/Data/Configurations/WorkoutGoalConfiguration.cs
- [x] T018 產生 InitialCreate Migration 於 backend/src/FitnessTracker.Infrastructure/Migrations/（使用 dotnet ef migrations add InitialCreate）
- [x] T019 新增 Seed Data（系統預設運動項目、器材）於 backend/src/FitnessTracker.Infrastructure/Data/DataSeeder.cs
- [x] T020 新增 CHECK 約束至 Migration（DurationMinutes 1-480, CaloriesBurned 1-5000）於 backend/src/FitnessTracker.Infrastructure/Migrations/[timestamp]_InitialCreate.cs

### 身份驗證與授權

- [x] T021 實作 LINE Login OAuth 服務於 backend/src/FitnessTracker.Infrastructure/ExternalServices/LineLoginService.cs
- [x] T022 實作 JWT Token 產生服務於 backend/src/FitnessTracker.Infrastructure/ExternalServices/JwtTokenService.cs
- [x] T023 建立 AuthController 處理 LINE Login callback 於 backend/src/FitnessTracker.Api/Controllers/AuthController.cs
- [x] T024 實作 JWT Authentication Middleware 於 backend/src/FitnessTracker.Api/Middleware/GlobalExceptionMiddleware.cs
- [x] T025 設定 ASP.NET Core Authentication 於 backend/src/FitnessTracker.Api/Program.cs
- [x] T026 建立前端 Auth Store（Pinia）於 frontend/src/stores/auth.ts
- [x] T027 建立前端 LINE Login 服務於 frontend/src/services/authService.ts
- [x] T028 建立前端 Axios 實例（攔截器加入 JWT Token）於 frontend/src/services/api.ts
- [x] T029 建立前端 Auth 導航守衛於 frontend/src/router/guards/authGuard.ts

### 共用服務與基礎設施

- [x] T030 [P] 建立通用儲存庫介面於 backend/src/FitnessTracker.Core/Interfaces/IRepository.cs
- [x] T031 [P] 實作通用儲存庫基底類別於 backend/src/FitnessTracker.Infrastructure/Repositories/RepositoryBase.cs
- [x] T032 [P] 建立卡路里計算服務（MET 公式）於 backend/src/FitnessTracker.Core/Services/CalorieCalculationService.cs
- [x] T033 [P] 建立全域異常處理 Middleware 於 backend/src/FitnessTracker.Api/Middleware/GlobalExceptionMiddleware.cs
- [x] T034 [P] 建立統一 API 回應格式（ApiResponse, ApiError）於 backend/src/FitnessTracker.Shared/Dtos/Common/
- [x] T035 [P] 設定 CORS 政策於 backend/src/FitnessTracker.Api/Program.cs
- [ ] T036 [P] 後端日誌最佳化與監控（預留）
- [x] T037 [P] 建立前端 Vuetify 設定（繁體中文 locale）於 frontend/src/plugins/vuetify.ts
- [x] T038 [P] 建立前端 Vue I18n 設定於 frontend/src/i18n/index.ts 與 frontend/src/i18n/zh-TW.json
- [x] T039 [P] 建立前端錯誤處理服務（顯示友善繁體中文訊息）於 frontend/src/utils/errorHandler.ts
- [x] T040 [P] 建立前端共用元件（Loading, ErrorMessage, SuccessMessage）於 frontend/src/components/common/

**檢查點**: 基礎設施就緒 - 使用者故事實作現可開始並行進行

---

## Phase 3: 使用者故事 1 - 查看當前健身紀錄 (優先級: P1) 🎯 MVP

**目標**: 使用者登入後可立即查看本週健身紀錄摘要（總時間、卡路里、運動天數），並與上週比較

**獨立測試**: 登入系統後查看首頁，驗證正確顯示本週摘要和週比較數據，即使沒有其他功能也能提供價值

### 測試 - 使用者故事 1（憲章原則 II 強制要求）⚠️

> **關鍵：依據 TDD 先寫測試、取得核准、確認失敗、再實作**

- [ ] T041 [P] [US1] 週統計 API Contract 測試於 backend/tests/FitnessTracker.ContractTests/StatisticsControllerTests.cs（測試 GET /api/v1/statistics/weekly 回應格式）
- [ ] T042 [P] [US1] 首頁載入整合測試於 backend/tests/FitnessTracker.IntegrationTests/HomePageLoadTests.cs（測試完整資料流程）
- [ ] T043 [P] [US1] 週統計計算單元測試於 backend/tests/FitnessTracker.UnitTests/Services/StatisticsServiceTests.cs（測試週一至週日統計邏輯）
- [ ] T044 [P] [US1] 首頁元件測試於 frontend/tests/unit/views/Home.spec.ts（測試 Vue 元件渲染與資料顯示）
- [ ] T045 [P] [US1] E2E 測試：使用者查看首頁於 frontend/tests/e2e/home.spec.ts（使用 Playwright 測試完整使用者旅程）

### 實作 - 使用者故事 1

#### 後端實作

- [x] T046 [P] [US1] 建立 WeeklySummaryDto 於 backend/src/FitnessTracker.Shared/Dtos/Statistics/WeeklySummaryDto.cs
- [x] T047 [P] [US1] 建立 DailyBreakdownDto 於 backend/src/FitnessTracker.Shared/Dtos/Statistics/DailyBreakdownDto.cs
- [x] T048 [US1] 建立統計服務介面 IStatisticsService 於 backend/src/FitnessTracker.Core/Interfaces/IStatisticsService.cs
- [x] T049 [US1] 實作週統計計算邏輯於 backend/src/FitnessTracker.Core/Services/StatisticsService.cs（包含週一至週日統計、上週比較）
- [x] T050 [US1] 建立 StatisticsController 於 backend/src/FitnessTracker.Api/Controllers/StatisticsController.cs（實作 GET /api/v1/statistics/weekly）
- [ ] T051 [US1] 新增統計查詢效能最佳化（AsNoTracking, 索引驗證）於 backend/src/FitnessTracker.Core/Services/StatisticsService.cs

#### 前端實作

- [x] T052 [P] [US1] 建立 Statistics Store（Pinia）於 frontend/src/stores/statistics.ts
- [x] T053 [P] [US1] 建立週統計 API 服務於 frontend/src/services/statisticsService.ts
- [x] T054 [P] [US1] 建立週統計卡片元件於 frontend/src/components/workout/WeeklySummaryCard.vue（顯示總時間、卡路里、天數）
- [x] T055 [P] [US1] 建立週比較元件於 frontend/src/components/workout/WeeklyComparisonCard.vue（顯示增減百分比）
- [x] T056 [P] [US1] 建立每日統計長條圖元件於 frontend/src/components/charts/DailyBarChart.vue
- [x] T057 [US1] 實作首頁 Home.vue 於 frontend/src/views/Home.vue（整合週統計、週比較、圖表元件）
- [x] T058 [US1] 新增首頁 Loading 與空資料狀態於 frontend/src/views/Home.vue
- [x] T059 [US1] 新增首頁繁體中文文案於 frontend/src/i18n/zh-TW.json（週統計、比較相關文字）

**檢查點**: 此時使用者故事 1 應完全可運作並可獨立測試

---

## Phase 4: 使用者故事 2 - 新增與管理每日健身紀錄 (優先級: P2)

**目標**: 使用者可新增、編輯、刪除每日健身紀錄（運動項目、時間、卡路里等），確保資料準確性

**獨立測試**: 透過新增一筆健身紀錄、編輯該紀錄、刪除該紀錄等操作，驗證完整 CRUD 功能，不依賴其他進階功能

### 測試 - 使用者故事 2（憲章原則 II 強制要求）⚠️

- [ ] T060 [P] [US2] 健身紀錄 CRUD API Contract 測試於 backend/tests/FitnessTracker.ContractTests/WorkoutsControllerTests.cs（測試 POST, PUT, DELETE /api/v1/workouts）
- [ ] T061 [P] [US2] 新增紀錄整合測試於 backend/tests/FitnessTracker.IntegrationTests/WorkoutRecordCreateTests.cs（測試完整新增流程含驗證）
- [ ] T062 [P] [US2] WorkoutRecord 驗證器單元測試於 backend/tests/FitnessTracker.UnitTests/Validators/WorkoutRecordValidatorTests.cs（測試 1-480 分鐘、1-5000 卡路里限制）
- [ ] T063 [P] [US2] 卡路里自動計算單元測試於 backend/tests/FitnessTracker.UnitTests/Services/CalorieCalculationServiceTests.cs（測試 MET 公式）
- [ ] T064 [P] [US2] 紀錄表單元件測試於 frontend/tests/unit/components/workout/WorkoutRecordForm.spec.ts
- [ ] T065 [P] [US2] E2E 測試：新增健身紀錄於 frontend/tests/e2e/workout-crud.spec.ts

### 實作 - 使用者故事 2

#### 後端實作

- [x] T066 [P] [US2] 建立 WorkoutRecordDto 於 backend/src/FitnessTracker.Shared/Dtos/WorkoutRecords/WorkoutRecordDto.cs
- [x] T067 [P] [US2] 建立 CreateWorkoutRecordDto 於 backend/src/FitnessTracker.Shared/Dtos/WorkoutRecords/CreateWorkoutRecordDto.cs
- [x] T068 [P] [US2] 建立 UpdateWorkoutRecordDto 於 backend/src/FitnessTracker.Shared/Dtos/WorkoutRecords/UpdateWorkoutRecordDto.cs
- [x] T069 [P] [US2] 建立 WorkoutRecordValidator（FluentValidation）於 backend/src/FitnessTracker.Core/Validators/WorkoutRecordValidator.cs
- [x] T070 [US2] 建立 WorkoutRecord 儲存庫介面於 backend/src/FitnessTracker.Core/Interfaces/IWorkoutRecordRepository.cs
- [x] T071 [US2] 實作 WorkoutRecord 儲存庫於 backend/src/FitnessTracker.Infrastructure/Repositories/WorkoutRecordRepository.cs
- [x] T072 [US2] 建立 WorkoutService 處理 CRUD 邏輯於 backend/src/FitnessTracker.Core/Services/WorkoutService.cs（含卡路里自動計算、重複檢查）
- [x] T073 [US2] 建立 WorkoutsController 於 backend/src/FitnessTracker.Api/Controllers/WorkoutsController.cs（實作 POST, GET, PUT, DELETE /api/v1/workouts）
- [x] T074 [US2] 實作軟刪除邏輯於 backend/src/FitnessTracker.Core/Services/WorkoutService.cs（設定 IsDeleted 而非真實刪除）

#### 前端實作

- [x] T075 [P] [US2] 建立 Workouts Store（Pinia）於 frontend/src/stores/workouts.ts
- [x] T076 [P] [US2] 建立 WorkoutService API 服務於 frontend/src/services/workoutService.ts
- [x] T077 [P] [US2] 建立運動項目選擇器元件於 frontend/src/components/workout/ExerciseTypeSelector.vue（含搜尋功能）
- [x] T078 [P] [US2] 建立運動器材選擇器元件於 frontend/src/components/workout/EquipmentSelector.vue
- [x] T079 [US2] 建立健身紀錄表單元件於 frontend/src/components/workout/WorkoutRecordForm.vue（含日期、運動項目、時間、卡路里、體重、備註欄位）
- [x] T080 [US2] 新增表單驗證邏輯於 frontend/src/components/workout/WorkoutRecordForm.vue（1-480 分鐘、1-5000 卡路里、日期不可為未來）
- [x] T081 [US2] 建立新增紀錄對話框元件於 frontend/src/components/workout/AddWorkoutDialog.vue
- [x] T082 [US2] 建立編輯紀錄對話框元件於 frontend/src/components/workout/EditWorkoutDialog.vue
- [x] T083 [US2] 建立刪除確認對話框元件於 frontend/src/components/workout/DeleteWorkoutDialog.vue
- [x] T084 [US2] 整合 CRUD 功能至首頁於 frontend/src/views/Home.vue（新增按鈕、編輯/刪除操作）
- [x] T085 [US2] 新增成功/錯誤訊息提示於 frontend/src/views/Home.vue（使用 Vuetify Snackbar）
- [x] T086 [US2] 新增健身紀錄相關繁體中文文案於 frontend/src/i18n/zh-TW.json

**檢查點**: 此時使用者故事 1 和 2 應都能獨立運作

---

## Phase 5: 使用者故事 3 - 查看每週詳細健身數據 (優先級: P3)

**目標**: 使用者可點擊週統計數據，深入查看該週每天的詳細健身資訊（每日運動種類、時間分配、卡路里明細）

**獨立測試**: 點擊週統計進入每日明細頁面，驗證能正確顯示該週七天的詳細數據，不需要其他進階分析功能

### 測試 - 使用者故事 3（憲章原則 II 強制要求）⚠️

- [ ] T087 [P] [US3] 每日明細 API Contract 測試於 backend/tests/FitnessTracker.ContractTests/WorkoutsControllerTests.cs（測試 GET /api/v1/workouts/by-date/{date}）
- [ ] T088 [P] [US3] 每日紀錄查詢整合測試於 backend/tests/FitnessTracker.IntegrationTests/DailyWorkoutQueryTests.cs
- [ ] T089 [P] [US3] 每日明細頁面元件測試於 frontend/tests/unit/views/WorkoutDetail.spec.ts
- [ ] T090 [P] [US3] E2E 測試：查看每週明細於 frontend/tests/e2e/weekly-detail.spec.ts

### 實作 - 使用者故事 3

#### 後端實作

- [x] T091 [P] [US3] 建立 DailyWorkoutDto 於 backend/src/FitnessTracker.Shared/Dtos/WorkoutRecords/DailyWorkoutDto.cs
- [x] T092 [US3] 實作按日期查詢紀錄於 backend/src/FitnessTracker.Core/Services/WorkoutService.cs（GetWorkoutsByDate 方法）
- [x] T093 [US3] 新增 GET /api/v1/workouts/by-date/{date} 端點於 backend/src/FitnessTracker.Api/Controllers/WorkoutsController.cs

#### 前端實作

- [x] T094 [P] [US3] 建立每日紀錄卡片元件於 frontend/src/components/workout/DailyWorkoutCard.vue（顯示單日所有紀錄）
- [x] T095 [P] [US3] 建立每日總計元件於 frontend/src/components/workout/DailyTotalCard.vue
- [x] T096 [P] [US3] 建立週行事曆元件於 frontend/src/components/workout/WeeklyCalendar.vue（週一至週日日期選擇器）
- [x] T097 [US3] 建立 WorkoutDetail 頁面於 frontend/src/views/WorkoutDetail.vue（顯示選定日期的所有紀錄）
- [x] T098 [US3] 新增從首頁導航至明細頁面的連結於 frontend/src/views/Home.vue（點擊週統計卡片）
- [x] T099 [US3] 新增路由設定於 frontend/src/router/index.ts（/workouts/detail/:date）
- [x] T100 [US3] 新增每週明細相關繁體中文文案於 frontend/src/i18n/zh-TW.json

**檢查點**: 所有使用者故事 1-3 應都能獨立運作

---

## Phase 6: 使用者故事 4 - 設定與追蹤運動目標 (優先級: P4)

**目標**: 使用者可設定每週運動目標（運動天數、總時間、總卡路里），系統即時顯示目標達成進度，幫助使用者保持運動動力

**獨立測試**: 設定週目標、記錄運動、查看達成進度等操作，驗證目標追蹤功能完整運作，不依賴趨勢圖表等其他功能

### 測試 - 使用者故事 4（憲章原則 II 強制要求）⚠️

- [ ] T101 [P] [US4] 運動目標 API Contract 測試於 backend/tests/FitnessTracker.ContractTests/GoalsControllerTests.cs（測試 POST, PUT, GET /api/v1/goals）
- [ ] T102 [P] [US4] 目標進度計算單元測試於 backend/tests/FitnessTracker.UnitTests/Services/GoalServiceTests.cs
- [ ] T103 [P] [US4] WorkoutGoalValidator 單元測試於 backend/tests/FitnessTracker.UnitTests/Validators/WorkoutGoalValidatorTests.cs
- [ ] T104 [P] [US4] 目標設定頁面元件測試於 frontend/tests/unit/views/Goals.spec.ts
- [ ] T105 [P] [US4] E2E 測試：設定目標與查看進度於 frontend/tests/e2e/goals.spec.ts

### 實作 - 使用者故事 4

#### 後端實作

- [x] T106 [P] [US4] 建立 WorkoutGoalDto 於 backend/src/FitnessTracker.Shared/Dtos/Goals/WorkoutGoalDto.cs
- [x] T107 [P] [US4] 建立 CreateWorkoutGoalDto 於 backend/src/FitnessTracker.Shared/Dtos/Goals/CreateWorkoutGoalDto.cs
- [x] T108 [P] [US4] 建立 WorkoutGoalValidator 於 backend/src/FitnessTracker.Core/Validators/WorkoutGoalValidator.cs（至少設定時長或卡路里一項）
- [x] T109 [US4] 建立 WorkoutGoal 儲存庫介面於 backend/src/FitnessTracker.Core/Interfaces/IWorkoutGoalRepository.cs
- [x] T110 [US4] 實作 WorkoutGoal 儲存庫於 backend/src/FitnessTracker.Infrastructure/Repositories/WorkoutGoalRepository.cs
- [x] T111 [US4] 建立 GoalService 處理目標 CRUD 與進度計算於 backend/src/FitnessTracker.Core/Services/GoalService.cs
- [x] T112 [US4] 建立 GoalsController 於 backend/src/FitnessTracker.Api/Controllers/GoalsController.cs（實作 POST, GET, PUT, PATCH /api/v1/goals）
- [ ] T113 [US4] 修改 StatisticsService 加入目標進度計算於 backend/src/FitnessTracker.Core/Services/StatisticsService.cs

#### 前端實作

- [x] T114 [P] [US4] 建立 Goals Store（Pinia）於 frontend/src/stores/goals.ts
- [x] T115 [P] [US4] 建立 GoalService API 服務於 frontend/src/services/goalService.ts
- [x] T116 [P] [US4] 建立目標設定表單元件於 frontend/src/components/goals/GoalForm.vue（每週時長、卡路里、開始/結束日期）
- [x] T117 [P] [US4] 建立目標進度條元件於 frontend/src/components/goals/GoalProgressBar.vue（顯示達成百分比）
- [x] T118 [P] [US4] 建立目標達成徽章元件於 frontend/src/components/goals/AchievementBadge.vue（達成時顯示祝賀）
- [x] T119 [US4] 建立 Goals 頁面於 frontend/src/views/Goals.vue（目標設定、修改、查看進度）
- [ ] T120 [US4] 整合目標進度至首頁於 frontend/src/views/Home.vue（週統計卡片顯示進度條）
- [x] T121 [US4] 新增路由設定於 frontend/src/router/index.ts（/goals）
- [ ] T122 [US4] 新增導航選單項目於 frontend/src/App.vue（目標設定連結）
- [x] T123 [US4] 新增目標相關繁體中文文案於 frontend/src/i18n/zh-TW.json

**檢查點**: 所有使用者故事 1-4 應都能獨立運作

---

## Phase 7: 使用者故事 5 - 查看歷史趨勢圖表 (優先級: P5)

**目標**: 使用者可透過視覺化圖表查看歷史健身數據的趨勢（運動時間、消耗卡路里的變化曲線），幫助使用者了解長期運動表現

**獨立測試**: 查看趨勢圖表頁面，驗證能正確顯示折線圖、柱狀圖等視覺化數據，不需要其他複雜分析功能

### 測試 - 使用者故事 5（憲章原則 II 強制要求）⚠️

- [ ] T124 [P] [US5] 趨勢資料 API Contract 測試於 backend/tests/FitnessTracker.ContractTests/StatisticsControllerTests.cs（測試 GET /api/v1/statistics/trends）
- [ ] T125 [P] [US5] 月統計計算單元測試於 backend/tests/FitnessTracker.UnitTests/Services/StatisticsServiceTests.cs
- [ ] T126 [P] [US5] 趨勢圖表元件測試於 frontend/tests/unit/components/charts/TrendChart.spec.ts
- [ ] T127 [P] [US5] E2E 測試：查看趨勢圖表於 frontend/tests/e2e/trends.spec.ts

### 實作 - 使用者故事 5

#### 後端實作

- [x] T128 [P] [US5] 建立 TrendDataDto 於 backend/src/FitnessTracker.Shared/Dtos/Statistics/TrendDataDto.cs
- [x] T129 [P] [US5] 建立 MonthlySummaryDto 於 backend/src/FitnessTracker.Shared/Dtos/Statistics/MonthlySummaryDto.cs
- [x] T130 [P] [US5] 建立 ExerciseDistributionDto 於 backend/src/FitnessTracker.Shared/Dtos/Statistics/ExerciseDistributionDto.cs
- [x] T131 [US5] 實作趨勢資料查詢於 backend/src/FitnessTracker.Core/Services/StatisticsService.cs（按日/週/月聚合）
- [x] T132 [US5] 實作月統計計算於 backend/src/FitnessTracker.Core/Services/StatisticsService.cs
- [x] T133 [US5] 實作運動項目分布統計於 backend/src/FitnessTracker.Core/Services/StatisticsService.cs
- [x] T134 [US5] 新增統計端點於 backend/src/FitnessTracker.Api/Controllers/StatisticsController.cs（GET /api/v1/statistics/trends, /monthly, /exercise-distribution）

#### 前端實作

- [x] T135 [P] [US5] 建立折線圖元件（Canvas）於 frontend/src/components/charts/LineChart.vue（運動時間/卡路里趨勢）
- [x] T136 [P] [US5] 建立柱狀圖元件（Canvas）於 frontend/src/components/charts/BarChart.vue（週比較、月比較）
- [x] T137 [P] [US5] 建立圓餅圖元件（Canvas）於 frontend/src/components/charts/PieChart.vue（運動類別分布）
- [x] T138 [P] [US5] 建立時間範圍選擇器元件於 frontend/src/components/charts/TimeRangeSelector.vue（近 4 週/12 個月/全部）
- [x] T139 [P] [US5] 建立圖表類型切換元件於 frontend/src/components/charts/ChartTypeToggle.vue（折線圖/柱狀圖）
- [x] T140 [US5] 建立 Trends 頁面於 frontend/src/views/Trends.vue（整合所有圖表元件）
- [x] T141 [US5] 新增圖表互動功能（點擊資料點顯示詳細資訊）於 frontend/src/views/Trends.vue
- [x] T142 [US5] 新增資料不足提示於 frontend/src/views/Trends.vue（少於 2 週資料時顯示）
- [x] T143 [US5] 新增路由設定於 frontend/src/router/index.ts（/trends）
- [x] T144 [US5] 新增導航選單項目於 frontend/src/App.vue（趨勢分析連結）
- [x] T145 [US5] 新增趨勢圖表相關繁體中文文案於 frontend/src/i18n/zh-TW.json

**檢查點**: 所有使用者故事 1-5 應都能獨立運作

---

## Phase 8: 使用者故事 6 - 自訂運動項目與器材 (優先級: P6)

**目標**: 使用者可建立和管理自己的運動項目清單，為每個運動項目設定名稱、選擇使用的運動器材，讓健身紀錄更加個人化和詳細

**獨立測試**: 新增運動項目、設定器材、編輯說明等操作，驗證自訂功能完整運作，不影響基本的健身記錄功能

### 測試 - 使用者故事 6（憲章原則 II 強制要求）⚠️

- [ ] T146 [P] [US6] 運動項目 API Contract 測試於 backend/tests/FitnessTracker.ContractTests/ExerciseTypesControllerTests.cs（測試 POST, PATCH /api/v1/exercise-types）
- [ ] T147 [P] [US6] 自訂項目搜尋單元測試於 backend/tests/FitnessTracker.UnitTests/Services/ExerciseTypeServiceTests.cs
- [ ] T148 [P] [US6] 運動項目管理頁面元件測試於 frontend/tests/unit/views/Settings.spec.ts
- [ ] T149 [P] [US6] E2E 測試：新增自訂運動項目於 frontend/tests/e2e/custom-exercise.spec.ts

### 實作 - 使用者故事 6

#### 後端實作

- [ ] T150 [P] [US6] 建立 ExerciseTypeDto 於 backend/src/FitnessTracker.Shared/Dtos/ExerciseTypes/ExerciseTypeDto.cs
- [ ] T151 [P] [US6] 建立 CreateExerciseTypeDto 於 backend/src/FitnessTracker.Shared/Dtos/ExerciseTypes/CreateExerciseTypeDto.cs
- [ ] T152 [P] [US6] 建立 EquipmentDto 於 backend/src/FitnessTracker.Shared/Dtos/Equipments/EquipmentDto.cs
- [ ] T153 [US6] 建立 ExerciseType 儲存庫介面於 backend/src/FitnessTracker.Core/Interfaces/IExerciseTypeRepository.cs
- [ ] T154 [US6] 實作 ExerciseType 儲存庫於 backend/src/FitnessTracker.Infrastructure/Repositories/ExerciseTypeRepository.cs（含搜尋功能）
- [ ] T155 [US6] 建立 ExerciseTypeService 處理 CRUD 與搜尋於 backend/src/FitnessTracker.Core/Services/ExerciseTypeService.cs
- [ ] T156 [US6] 建立 ExerciseTypesController 於 backend/src/FitnessTracker.Api/Controllers/ExerciseTypesController.cs（實作 GET, POST, PATCH）
- [ ] T157 [US6] 建立 EquipmentsController 於 backend/src/FitnessTracker.Api/Controllers/EquipmentsController.cs（實作 GET, POST, PUT, DELETE）
- [ ] T158 [US6] 實作刪除自訂項目前的檢查於 backend/src/FitnessTracker.Core/Services/ExerciseTypeService.cs（檢查歷史紀錄使用情況）
- [ ] T158-1 [US6] 建立 ExerciseType-Equipment 多對多關聯方法於 backend/src/FitnessTracker.Core/Services/ExerciseTypeService.cs（AddEquipment, RemoveEquipment）
- [ ] T158-2 [US6] 新增器材關聯 API 端點於 backend/src/FitnessTracker.Api/Controllers/ExerciseTypesController.cs（POST /exercise-types/{id}/equipments, DELETE /exercise-types/{id}/equipments/{equipmentId}）

#### 前端實作

- [ ] T159 [P] [US6] 建立 ExerciseTypes Store（Pinia）於 frontend/src/stores/exerciseTypes.ts
- [ ] T160 [P] [US6] 建立 ExerciseTypeService API 服務於 frontend/src/services/exerciseTypeService.ts
- [ ] T161 [P] [US6] 建立運動項目清單元件於 frontend/src/components/settings/ExerciseTypeList.vue（顯示系統預設+自訂項目）
- [ ] T162 [P] [US6] 建立新增運動項目表單元件於 frontend/src/components/settings/ExerciseTypeForm.vue
- [ ] T163 [P] [US6] 建立運動項目搜尋欄位元件於 frontend/src/components/settings/ExerciseTypeSearchBar.vue
- [ ] T164 [P] [US6] 建立器材選擇器元件於 frontend/src/components/settings/EquipmentMultiSelect.vue
- [ ] T165 [US6] 建立 Settings 頁面於 frontend/src/views/Settings.vue（運動項目管理介面）
- [ ] T166 [US6] 新增停用/啟用運動項目功能於 frontend/src/views/Settings.vue
- [ ] T167 [US6] 新增刪除項目警告對話框於 frontend/src/components/settings/DeleteExerciseTypeDialog.vue
- [ ] T167-1 [US6] 整合器材關聯功能至運動項目表單於 frontend/src/components/settings/ExerciseTypeForm.vue（可多選器材並儲存關聯）
- [ ] T167-2 [P] [US6] 建立器材表單元件於 frontend/src/components/settings/EquipmentForm.vue（名稱、說明文字欄位）
- [ ] T167-3 [P] [US6] 建立器材清單元件於 frontend/src/components/settings/EquipmentList.vue（顯示所有器材與說明）
- [ ] T167-4 [US6] 在 Settings.vue 增加器材管理標籤頁（整合器材清單與新增/編輯功能）
- [ ] T168 [US6] 新增路由設定於 frontend/src/router/index.ts（/settings）
- [ ] T169 [US6] 新增導航選單項目於 frontend/src/App.vue（設定連結）
- [ ] T170 [US6] 新增運動項目管理相關繁體中文文案於 frontend/src/i18n/zh-TW.json（包含器材管理相關文字）

**檢查點**: 所有使用者故事 1-6 應都能獨立運作

---

## Phase 9: 完善與跨功能關注點

**目的**: 憲章合規性驗證與跨故事改善

### 程式碼品質（原則 I）

- [ ] T171 驗證程式碼覆蓋率達標（後端 ≥80%, 關鍵路徑 ≥95%）使用 dotnet test --collect:"XPlat Code Coverage"
- [ ] T172 驗證前端程式碼覆蓋率達標（≥80%）使用 npm run test:coverage
- [ ] T173 執行複雜度分析（函式 <10, 模組 <15）使用 Code Metrics
- [ ] T174 驗證後端零 linting 錯誤，處理/註解所有警告（執行 dotnet format --verify-no-changes）
- [ ] T175 驗證前端零 linting 錯誤（執行 npm run lint）
- [ ] T176 驗證型別安全（後端無隱式 dynamic, 前端無 any 型別）

### 測試標準（原則 II）

- [ ] T177 驗證測試金字塔比例（70% 單元測試, 20% 整合測試, 10% E2E）
- [ ] T178 [P] 新增關鍵使用者旅程效能迴歸測試於 backend/tests/FitnessTracker.PerformanceTests/（首頁載入 <2s, 新增紀錄 <1s）
- [ ] T179 [P] 執行無障礙性自動化測試（WCAG 2.1 AA）使用 axe-core 於 frontend/tests/accessibility/
- [ ] T180 驗證所有 API 端點皆有 Contract 測試

### 使用者體驗一致性（原則 III）

- [ ] T181 驗證所有元件使用 Vuetify 3 設計系統（無自訂樣式）
- [ ] T182 驗證響應式設計於各斷點（320px, 768px, 1024px）使用瀏覽器開發工具
- [ ] T183 測試離線支援核心功能（LocalStorage 暫存、連線時同步）
- [ ] T184 驗證所有非同步操作有載入狀態（Loading Skeleton）
- [ ] T185 檢查錯誤訊息友善性與繁體中文正確性
- [ ] T186 驗證所有 UI 文字使用繁體中文（zh-TW）

### 文件標準

- [ ] T187 驗證 spec.md 使用繁體中文撰寫
- [ ] T188 驗證 plan.md 使用繁體中文撰寫
- [ ] T189 驗證 tasks.md（本檔案）使用繁體中文撰寫
- [ ] T190 驗證 README.md 與 quickstart.md 使用繁體中文撰寫
- [ ] T191 驗證 API 文件（contracts/api-spec.md）使用繁體中文
- [ ] T192 檢查公開 API 程式碼註解使用繁體中文

### 效能需求（原則 IV）

- [ ] T193 測量首頁載入時間（目標 <2s on 3G, <1s 寬頻）使用 Lighthouse
- [ ] T194 驗證互動回應時間（點擊 <100ms, 表單送出 <200ms）
- [ ] T195 測試資料同步效能（典型健身資料 <5s）
- [ ] T196 測試 API 延遲（p95 <500ms 讀取, <1000ms 寫入）使用負載測試工具
- [ ] T197 分析前端記憶體使用（目標 <150MB）使用 Chrome DevTools
- [ ] T198 驗證資料庫查詢最佳化（無 N+1, 使用 Include, 驗證索引）使用 SQL Profiler

### 一般完善

- [ ] T199 [P] 更新 README.md 於專案根目錄（專案說明、技術堆疊、安裝步驟）
- [ ] T200 [P] 更新 quickstart.md 於 specs/001-fitness-tracking/quickstart.md（驗證所有步驟可執行）
- [ ] T201 [P] 建立 API 使用範例於 docs/api-examples.md
- [ ] T202 程式碼清理與重構（移除未使用的匯入、註解掉的程式碼）
- [ ] T203 安全性強化（驗證輸入、防止 SQL Injection、XSS）
- [ ] T204 執行 quickstart.md 驗證（確保新開發者可按步驟建置執行）

---

## 相依性與執行順序

### 階段相依性

- **環境設定（Phase 1）**: 無相依性 - 可立即開始
- **基礎架構（Phase 2）**: 依賴環境設定完成 - **阻塞所有使用者故事**
- **使用者故事（Phase 3+）**: 全部依賴基礎架構完成
  - 使用者故事之間可平行執行（若有足夠人力）
  - 或依優先順序依序執行（P1 → P2 → P3 → P4 → P5 → P6）
- **完善（Final Phase）**: 依賴所有期望的使用者故事完成

### 使用者故事相依性

- **使用者故事 1 (P1)**: 基礎架構完成後即可開始 - 無其他故事相依性
- **使用者故事 2 (P2)**: 基礎架構完成後即可開始 - 可與 US1 整合但應可獨立測試
- **使用者故事 3 (P3)**: 基礎架構完成後即可開始 - 可與 US1/US2 整合但應可獨立測試
- **使用者故事 4 (P4)**: 基礎架構完成後即可開始 - 可與 US1/US2 整合但應可獨立測試
- **使用者故事 5 (P5)**: 基礎架構完成後即可開始 - 可與 US1 整合但應可獨立測試
- **使用者故事 6 (P6)**: 基礎架構完成後即可開始 - 可與 US2 整合但應可獨立測試

### 每個使用者故事內部

- 測試必須先寫並**失敗**後才能實作
- 模型先於服務
- 服務先於端點/控制器
- 核心實作先於整合
- 故事完成後才能進入下一個優先級

### 平行執行機會

- 所有標記 [P] 的環境設定任務可平行執行
- 所有標記 [P] 的基礎架構任務可平行執行（在 Phase 2 內）
- 基礎架構完成後，所有使用者故事可平行開始（若團隊容量允許）
- 每個故事內標記 [P] 的測試可平行執行
- 每個故事內標記 [P] 的模型可平行執行
- 不同使用者故事可由不同團隊成員平行處理

---

## 平行執行範例：使用者故事 1

```bash
# 同時啟動使用者故事 1 的所有測試：
Task: "週統計 API Contract 測試於 backend/tests/FitnessTracker.ContractTests/StatisticsControllerTests.cs"
Task: "首頁載入整合測試於 backend/tests/FitnessTracker.IntegrationTests/HomePageLoadTests.cs"
Task: "週統計計算單元測試於 backend/tests/FitnessTracker.UnitTests/Services/StatisticsServiceTests.cs"
Task: "首頁元件測試於 frontend/tests/unit/views/Home.spec.ts"
Task: "E2E 測試：使用者查看首頁於 frontend/tests/e2e/home.spec.ts"

# 測試通過後，同時啟動使用者故事 1 的所有 DTO 與元件：
Task: "建立 WeeklySummaryDto 於 backend/src/FitnessTracker.Shared/Dtos/Statistics/WeeklySummaryDto.cs"
Task: "建立 DailyBreakdownDto 於 backend/src/FitnessTracker.Shared/Dtos/Statistics/DailyBreakdownDto.cs"
Task: "建立 Statistics Store（Pinia）於 frontend/src/stores/statistics.ts"
Task: "建立週統計 API 服務於 frontend/src/services/statisticsService.ts"
Task: "建立週統計卡片元件於 frontend/src/components/workout/WeeklySummaryCard.vue"
Task: "建立週比較元件於 frontend/src/components/workout/WeeklyComparisonCard.vue"
Task: "建立每日統計長條圖元件於 frontend/src/components/charts/DailyBarChart.vue"
```

---

## 實作策略

### MVP 優先（僅使用者故事 1）

1. 完成 Phase 1: 環境設定
2. 完成 Phase 2: 基礎架構（**關鍵 - 阻塞所有故事**）
3. 完成 Phase 3: 使用者故事 1
4. **停止並驗證**: 獨立測試使用者故事 1
5. 準備好後即可部署/展示

### 漸進式交付

1. 完成環境設定 + 基礎架構 → 基礎就緒
2. 新增使用者故事 1 → 獨立測試 → 部署/展示（MVP！）
3. 新增使用者故事 2 → 獨立測試 → 部署/展示
4. 新增使用者故事 3 → 獨立測試 → 部署/展示
5. 新增使用者故事 4 → 獨立測試 → 部署/展示
6. 新增使用者故事 5 → 獨立測試 → 部署/展示
7. 新增使用者故事 6 → 獨立測試 → 部署/展示
8. 每個故事新增價值而不破壞先前故事

### 平行團隊策略

若有多位開發者：

1. 團隊一起完成環境設定 + 基礎架構
2. 基礎架構完成後：
   - 開發者 A: 使用者故事 1
   - 開發者 B: 使用者故事 2
   - 開發者 C: 使用者故事 3
3. 故事獨立完成並整合

---

## 備註

- [P] 任務 = 不同檔案、無相依性
- [Story] 標籤將任務對應至特定使用者故事以供追蹤
- 每個使用者故事應可獨立完成並測試
- 先驗證測試失敗再實作
- 每個任務或邏輯群組完成後提交
- 在任何檢查點停止以獨立驗證故事
- 避免：模糊任務、相同檔案衝突、破壞獨立性的跨故事相依性

---

## 總結

- **總任務數**: 210 個任務（原 204 + 新增 6 個任務）
- **使用者故事任務分布**:
  - US1（查看當前紀錄）: 19 個任務（T041-T059）
  - US2（新增管理紀錄）: 27 個任務（T060-T086）
  - US3（查看週詳細數據）: 14 個任務（T087-T100）
  - US4（設定追蹤目標）: 23 個任務（T101-T123）
  - US5（查看歷史趨勢）: 22 個任務（T124-T145）
  - US6（自訂運動項目）: 31 個任務（T146-T170 + 6 個補充任務）
- **平行執行機會**: 每個階段約有 30-50% 任務可平行執行（標記 [P]）
- **獨立測試標準**: 每個使用者故事皆有明確的獨立測試方法
- **建議 MVP 範圍**: Phase 1 + Phase 2 + Phase 3（使用者故事 1）= 核心價值展示
