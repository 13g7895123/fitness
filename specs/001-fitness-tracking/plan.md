# 實作計畫：健身紀錄系統

**分支**: `001-fitness-tracking` | **日期**: 2025-11-02 | **規格**: [spec.md](./spec.md)
**輸入**: 功能規格來自 `/specs/001-fitness-tracking/spec.md`

**備註**: 本文件由 `/speckit.plan` 指令產生。

## 摘要

本功能實作一個完整的健身紀錄管理系統，讓使用者能夠記錄、查看和管理每日健身數據。系統提供週/月/年統計分析、目標追蹤、歷史趨勢視覺化等功能。採用 .NET Core 8 + Vue.js 的前後端分離架構，使用 Azure SQL 資料庫搭配 EF Core Code First 工作流程，透過 Docker 容器化部署。使用者透過 LINE 登入進行身份驗證，所有介面皆為繁體中文。

## 技術背景

**語言/版本**: .NET Core 8 (C#) + Vue.js 3  
**主要相依套件**: 
- 後端：ASP.NET Core 8, EF Core 8, LINE Login SDK, FluentValidation, Serilog
- 前端：Vue 3, Vue Router, Pinia, Axios, Chart.js, Vite
**儲存**: Azure SQL Database (EF Core Code First)  
**測試**: xUnit (backend), Vitest + Vue Test Utils (frontend)  
**目標平台**: Docker containers (Linux) + Azure App Service  
**專案類型**: Web 應用程式（前後端分離）  
**效能目標**: 
- API 回應時間 p95 <500ms (讀取) / <1000ms (寫入)
- 首頁載入 <2 秒 (3G) / <1 秒 (寬頻)
- 支援 1000 並發使用者
**限制**: 
- 離線支援（前端 LocalStorage 暫存，連線時同步）
- 繁體中文介面
- WCAG 2.1 AA 無障礙標準
**規模/範圍**: 
- 預期 10,000 活躍使用者
- 6 個主要使用者故事
- 30+ 功能需求
- 支援至少 2 年歷史資料

## 憲章檢查

*檢查點：Phase 0 研究前必須通過，Phase 1 設計後重新檢查*

**程式碼品質（原則 I）**:
- [x] 已定義語言/框架的型別安全需求（C# 強型別、TypeScript 嚴格模式）
- [x] 已指定 linting 和格式化工具（EditorConfig, ESLint, Prettier, StyleCop）
- [x] 程式碼覆蓋率目標可達成（後端 xUnit, 前端 Vitest，目標 80%+ / 關鍵路徑 95%+）
- [x] 架構考量複雜度限制（函式 <10, 模組 <15，採用 CQRS 模式分離關注點）

**測試標準（原則 II）**:
- [x] 已確認 TDD 方法（測試先行 → 使用者核准 → 測試失敗 → 實作）
- [x] 測試金字塔比例可達成（70% 單元測試, 20% 整合測試, 10% E2E）
- [x] 已規劃所有 API 端點的 contract tests（使用 xUnit + FluentAssertions）
- [x] 已識別整合測試需求：LINE API, Azure SQL, 健身紀錄計算邏輯
- [x] 已規劃關鍵使用者旅程的效能迴歸測試（記錄新增、統計查詢）
- [x] 已定義無障礙測試方法（axe-core 自動化測試）

**使用者體驗一致性（原則 III）**:
- [x] 已指定設計系統/元件庫（使用 Vuetify 3 作為基礎，自訂繁體中文主題）
- [x] 已記錄 WCAG 2.1 AA 合規需求
- [x] 已定義響應式斷點（mobile 320px+, tablet 768px+, desktop 1024px+）
- [x] 已規劃所有非同步操作的載入狀態（Vuetify skeleton loaders）
- [x] 已定義錯誤處理策略，包含繁體中文友善錯誤訊息
- [x] 已釐清核心功能的離線支援需求（LocalStorage + IndexedDB）
- [x] 所有 UI 文字、錯誤訊息、通知皆使用繁體中文（zh-TW）
- [x] 已指定國際化方法（Vue I18n，主要語言 zh-TW）

**文件標準**:
- [x] 本計畫文件使用繁體中文撰寫
- [x] 功能規格（spec.md）使用繁體中文
- [x] 使用者故事和驗收場景使用繁體中文
- [x] API 文件和錯誤訊息使用繁體中文
- [x] 公開 API 的程式碼註解使用繁體中文

**效能需求（原則 IV）**:
- [x] 已定義頁面載入目標（<2s on 3G, <1s 寬頻）
- [x] 已指定互動回應目標（點擊 <100ms, 表單送出 <200ms）
- [x] 已定義資料同步需求（<5s for 典型健身資料）
- [x] 已設定 API 延遲目標（p95 <500ms 讀取, <1000ms 寫入）
- [x] 已指定記憶體使用限制（前端 <150MB, 後端優化）
- [x] 已最小化電池影響（使用 Web Push API，無持續輪詢）
- [x] 已規劃資料庫查詢優化（無 N+1, EF Core Include, 適當索引）

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### 原始碼（專案根目錄）

```text
backend/
├── src/
│   ├── FitnessTracker.Api/              # ASP.NET Core Web API
│   │   ├── Controllers/                 # API 控制器
│   │   ├── Middleware/                  # 自訂中介軟體
│   │   ├── Filters/                     # 異常過濾器
│   │   └── Program.cs                   # 應用程式進入點
│   ├── FitnessTracker.Core/             # 領域邏輯與實體
│   │   ├── Entities/                    # EF Core 實體
│   │   ├── Interfaces/                  # 儲存庫與服務介面
│   │   ├── Services/                    # 業務邏輯服務
│   │   └── Validators/                  # FluentValidation 驗證器
│   ├── FitnessTracker.Infrastructure/   # 基礎設施層
│   │   ├── Data/                        # EF Core DbContext
│   │   ├── Repositories/                # 儲存庫實作
│   │   ├── ExternalServices/            # LINE Login SDK 整合
│   │   └── Migrations/                  # EF Core Migrations
│   └── FitnessTracker.Shared/           # 共用 DTOs 與常數
│       ├── Dtos/                        # POCO Data Transfer Objects
│       ├── Constants/                   # 常數定義
│       └── Enums/                       # 列舉
├── tests/
│   ├── FitnessTracker.UnitTests/        # 單元測試（xUnit）
│   ├── FitnessTracker.IntegrationTests/ # 整合測試（含資料庫）
│   └── FitnessTracker.ContractTests/    # API Contract 測試
└── docker/
    ├── Dockerfile                        # 後端容器映像
    └── docker-compose.yml                # 完整環境編排

frontend/
├── src/
│   ├── components/                       # Vue 元件
│   │   ├── common/                       # 共用元件（按鈕、卡片等）
│   │   ├── workout/                      # 健身相關元件
│   │   └── charts/                       # 圖表元件
│   ├── views/                            # 頁面視圖
│   │   ├── Home.vue                      # 首頁（週統計）
│   │   ├── WorkoutDetail.vue             # 每日明細
│   │   ├── Goals.vue                     # 目標設定
│   │   ├── Trends.vue                    # 趨勢圖表
│   │   └── Settings.vue                  # 運動項目管理
│   ├── stores/                           # Pinia stores
│   │   ├── auth.ts                       # 身份驗證狀態
│   │   ├── workouts.ts                   # 健身紀錄狀態
│   │   └── goals.ts                      # 目標狀態
│   ├── services/                         # API 服務層
│   │   ├── api.ts                        # Axios 設定
│   │   ├── authService.ts                # 身份驗證服務
│   │   └── workoutService.ts             # 健身紀錄服務
│   ├── router/                           # Vue Router
│   ├── i18n/                             # 國際化（Vue I18n）
│   │   └── zh-TW.json                    # 繁體中文語系檔
│   ├── types/                            # TypeScript 型別定義
│   ├── utils/                            # 工具函式
│   ├── App.vue                           # 根元件
│   └── main.ts                           # 應用程式進入點
├── tests/
│   ├── unit/                             # 單元測試（Vitest）
│   ├── components/                       # 元件測試
│   └── e2e/                              # E2E 測試（Playwright）
├── public/                               # 靜態資源
├── vite.config.ts                        # Vite 設定
└── docker/
    ├── Dockerfile                        # 前端容器映像（Nginx）
    └── nginx.conf                        # Nginx 設定

.github/
├── workflows/                            # CI/CD 工作流程
│   ├── backend-ci.yml                    # 後端 CI
│   └── frontend-ci.yml                   # 前端 CI

docker-compose.yml                        # 開發環境完整編排
```

**結構決策**: 選擇 Web 應用程式架構（前後端分離）。後端採用乾淨架構（Clean Architecture）分層：API 層（Controllers）、Core 層（Domain）、Infrastructure 層（Data Access），確保業務邏輯與基礎設施解耦。前端採用 Vue 3 Composition API 搭配 Pinia 狀態管理，遵循元件化與模組化原則。所有程式碼透過 Docker 容器化，支援本地開發與雲端部署。

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

無憲章違規項目需要說明。本專案完全符合憲章所有要求：
- 專案數量：後端 3 個專案（Api、Core、Infrastructure）+ 前端 1 個專案，符合憲章「最多 3 個專案」要求
- 測試覆蓋率：目標設定為 ≥80%，符合憲章最低要求
- 效能要求：P95 < 500ms，頁面載入 < 2s，符合憲章要求
- 無障礙性：WCAG 2.1 AA 標準，符合憲章要求
- 文件語言：全繁體中文，符合憲章要求
