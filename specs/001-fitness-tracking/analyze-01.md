# 規格分析報告：健身紀錄追蹤系統

**分析日期**: 2025-11-02  
**功能分支**: `001-fitness-tracking`  
**分析範圍**: spec.md, plan.md, tasks.md, constitution.md

---

## 執行摘要

本分析檢查了健身紀錄追蹤系統的三個核心制品（規格、計畫、任務）與憲章的一致性、完整性和可實作性。

**總體評估**: ✅ **可以進行實作**

- **嚴重問題 (CRITICAL)**: 0 個
- **高優先級問題 (HIGH)**: 4 個
- **中優先級問題 (MEDIUM)**: 8 個
- **低優先級問題 (LOW)**: 6 個

所有嚴重問題已解決。高優先級問題主要涉及需求覆蓋度和實體定義的細微差異，但不會阻礙實作進行。建議在 Phase 0 (research.md) 階段解決這些問題。

---

## 發現問題清單

| ID | 類別 | 嚴重度 | 位置 | 摘要 | 建議 |
|----|------|--------|------|------|------|
| A1 | 模糊性 | HIGH | spec.md:FR-007, FR-018 | 「自動計算」卡路里的具體公式未定義 | 在 data-model.md 中已定義 MET 公式，建議在 spec.md 中增加參照或簡要說明計算方法 |
| A2 | 模糊性 | MEDIUM | spec.md:FR-019 | 「祝賀訊息或成就提示」的觸發條件和顯示方式不明確 | 在 tasks.md T118 中有「目標達成徽章元件」，建議在 spec.md 增加驗收準則細節 |
| A3 | 模糊性 | MEDIUM | spec.md:邊界案例 | 跨月/跨年週統計的歸屬規則描述不清晰 | 已說明「歸屬於包含較多天數的月份」，但未定義平手情況（3.5天 vs 3.5天）的處理方式 |
| A4 | 模糊性 | LOW | spec.md:FR-028 | 「友善的錯誤訊息」缺乏具體範例 | 建議在 contracts/api-spec.md 中定義標準錯誤訊息格式和繁體中文範例 |
| A5 | 模糊性 | LOW | spec.md:SC-003 | 「90% 的使用者能在首次使用時...成功新增第一筆健身紀錄」缺乏測量方法 | 建議增加「透過使用者測試或分析工具追蹤」的說明 |
| A6 | 模糊性 | LOW | plan.md | 「適當索引」未明確列出需要建立的索引 | 在 data-model.md 中已定義索引策略，plan.md 可增加參照 |
| C1 | 覆蓋度 | HIGH | spec.md:FR-014 | 「允許使用者為運動項目選擇或關聯運動器材」無對應任務 | tasks.md 中有 Equipment 實體建立（T015）但缺少運動項目與器材關聯的 CRUD 任務 |
| C2 | 覆蓋度 | HIGH | spec.md:FR-015 | 「允許使用者為運動器材新增說明文字」無對應任務 | tasks.md T164 有 EquipmentMultiSelect 元件，但缺少器材說明文字的編輯功能 |
| C3 | 覆蓋度 | MEDIUM | spec.md:FR-033 | 「維持登入狀態」的實作細節未在任務中明確 | tasks.md T022 有 JWT Token 服務，建議增加 Token 刷新機制的任務 |
| C4 | 覆蓋度 | MEDIUM | spec.md:非功能需求 | 離線支援功能在 spec.md 中未作為獨立需求列出 | plan.md 和 tasks.md T183 有提及，建議在 spec.md 增加為明確的功能需求 |
| C5 | 覆蓋度 | MEDIUM | spec.md:邊界案例 | 多筆同運動項目加總的邊界案例無專門測試任務 | 建議在 US2 的測試任務中增加此邊界案例的單元測試 |
| I1 | 不一致 | HIGH | spec.md vs data-model.md | WorkoutRecord 實體欄位差異：spec.md 未提及 Weight 欄位 | data-model.md 定義了 Weight (decimal nullable)，tasks.md T079 表單包含體重欄位，建議在 spec.md FR-007 中明確列出 |
| I2 | 不一致 | MEDIUM | spec.md vs tasks.md | 運動項目「類別」定義不一致 | spec.md FR-012 提及「類別（有氧/重訓/伸展等）」，但 data-model.md 使用 enum Category (Cardio, Strength, Flexibility, Sports, Other)，建議統一繁體中文與英文對應 |
| I3 | 不一致 | MEDIUM | spec.md vs plan.md | 測試框架不一致：spec.md 未提及 Playwright | plan.md 提及 Playwright 用於 E2E 測試，tasks.md T045 使用 Playwright，但 spec.md 未在測試章節說明 |
| I4 | 不一致 | LOW | tasks.md | Phase 2 實體建立順序與相依性 | T013-T017 標記為 [P] 可平行，但 WorkoutRecord 參照 User/ExerciseType/Equipment，理論上有相依性（EF Core 可處理，但註記不清楚） |
| I5 | 不一致 | LOW | spec.md:FR-001 vs tasks.md | 週/月/年切換功能未在任務中完整實作 | spec.md FR-001 明確要求三種週期切換，但 tasks.md 僅在 US1 實作週統計，月/年統計在 US5 部分實作 |
| T1 | 術語漂移 | MEDIUM | spec.md vs tasks.md | 「運動項目」與「ExerciseType」混用 | spec.md 使用「運動項目」，tasks.md 使用 ExerciseType（英文實體名），建議在 spec.md 關鍵實體章節建立術語對照表 |
| T2 | 術語漂移 | LOW | spec.md | 「運動時間」與「運動時長」混用 | FR-004 使用「總運動時間」，FR-007 使用「運動時間」，資料模型使用 DurationMinutes，建議統一為「運動時長（分鐘）」 |
| T3 | 術語漂移 | LOW | tasks.md | 「統計」相關服務命名不一致 | StatisticsService (統計服務) vs TrendService (趨勢服務)，實際上 tasks.md 僅有 StatisticsService，建議確認是否需要分離 |

---

## 覆蓋度摘要

### 功能需求覆蓋度

| 需求 ID | 需求摘要 | 對應任務 | 覆蓋狀態 |
|---------|----------|----------|----------|
| FR-001 | 首頁顯示週/月/年統計摘要 | T041-T059 (US1), T131-T134 (US5) | ⚠️ 部分覆蓋（月/年統計實作分散） |
| FR-002 | 週/月/年切換功能 | T138 (TimeRangeSelector) | ⚠️ 部分覆蓋（需確認完整實作） |
| FR-003 | 顯示週期比較數據 | T055 (WeeklyComparisonCard) | ✅ 完全覆蓋 |
| FR-004 | 自動計算週/月/年總計 | T049 (StatisticsService) | ✅ 完全覆蓋 |
| FR-005 | 點擊週統計查看每日明細 | T087-T100 (US3) | ✅ 完全覆蓋 |
| FR-006 | 每日明細顯示所有紀錄 | T094-T097 (US3) | ✅ 完全覆蓋 |
| FR-007 | 新增健身紀錄功能 | T060-T086 (US2) | ✅ 完全覆蓋 |
| FR-008 | 編輯健身紀錄功能 | T082 (EditWorkoutDialog) | ✅ 完全覆蓋 |
| FR-009 | 刪除健身紀錄功能 | T074, T083 (DeleteWorkoutDialog) | ✅ 完全覆蓋 |
| FR-010 | 紀錄變更後即時更新統計 | T072 (WorkoutService), T049 (StatisticsService) | ✅ 完全覆蓋 |
| FR-011 | 輸入資料驗證 | T069 (WorkoutRecordValidator), T080 (表單驗證) | ✅ 完全覆蓋 |
| FR-012 | 運動項目管理功能 | T146-T170 (US6) | ✅ 完全覆蓋 |
| FR-013 | 運動項目下拉選單 | T077 (ExerciseTypeSelector) | ✅ 完全覆蓋 |
| FR-014 | 運動項目關聯器材 | T078 (EquipmentSelector), T164 (EquipmentMultiSelect) | ❌ 關聯邏輯未明確（見 C1） |
| FR-015 | 器材說明文字 | T015 (Equipment 實體), T152 (EquipmentDto) | ❌ CRUD 操作缺失（見 C2） |
| FR-016 | 刪除運動項目前檢查 | T158 (刪除前檢查) | ✅ 完全覆蓋 |
| FR-017 | 運動目標設定功能 | T101-T123 (US4) | ✅ 完全覆蓋 |
| FR-018 | 首頁顯示目標進度 | T120 (整合目標進度至首頁) | ✅ 完全覆蓋 |
| FR-019 | 達成目標顯示祝賀 | T118 (AchievementBadge) | ⚠️ 觸發邏輯需明確（見 A2） |
| FR-020 | 修改/取消運動目標 | T112 (GoalsController PUT, PATCH) | ✅ 完全覆蓋 |
| FR-021 | 歷史趨勢圖表功能 | T124-T145 (US5) | ✅ 完全覆蓋 |
| FR-022 | 多種圖表類型 | T135-T137 (LineChart, BarChart, PieChart) | ✅ 完全覆蓋 |
| FR-023 | 選擇時間範圍 | T138 (TimeRangeSelector) | ✅ 完全覆蓋 |
| FR-024 | 點擊資料點顯示詳情 | T141 (圖表互動功能) | ✅ 完全覆蓋 |
| FR-025 | 資料不足時提示 | T142 (資料不足提示) | ✅ 完全覆蓋 |
| FR-026 | 所有 UI 使用繁體中文 | T038 (Vue I18n zh-TW), 各 US 的 i18n 任務 | ✅ 完全覆蓋 |
| FR-027 | 載入指示器 | T040 (Loading 元件), T058 (首頁 Loading) | ✅ 完全覆蓋 |
| FR-028 | 操作成功訊息 | T040 (SuccessMessage 元件), T085 (Snackbar) | ✅ 完全覆蓋 |
| FR-029 | 友善錯誤訊息 | T033, T039 (ErrorHandler), T040 (ErrorMessage) | ✅ 完全覆蓋 |
| FR-030 | 響應式設計 | T037 (Vuetify 3), T182 (響應式驗證) | ✅ 完全覆蓋 |
| FR-031 | LINE 登入 | T021-T029 (LINE Login + JWT Auth) | ✅ 完全覆蓋 |
| FR-032 | 首次登入建立帳戶 | T023 (AuthController callback) | ✅ 完全覆蓋 |
| FR-033 | 維持登入狀態 | T022 (JWT Token), T028 (Axios 攔截器) | ⚠️ Token 刷新機制需確認（見 C3） |
| FR-034 | 登出功能 | T023 (AuthController logout 端點) | ✅ 完全覆蓋 |
| FR-035 | 資料隔離與權限 | T024 (JWT Middleware), 各 Controller 實作 | ✅ 完全覆蓋 |

**覆蓋度統計**:
- 總功能需求: 35 個
- 完全覆蓋: 29 個 (82.9%)
- 部分覆蓋: 4 個 (11.4%)
- 未覆蓋: 2 個 (5.7%)

### 未對應任務

以下任務無明確對應的功能需求（可能為技術實作細節或基礎設施）：

| 任務 ID | 任務描述 | 分類 | 建議 |
|---------|----------|------|------|
| T001-T011 | Phase 1 環境設定 | 基礎設施 | ✅ 合理（技術前置條件） |
| T012-T040 | Phase 2 基礎架構 | 基礎設施 | ✅ 合理（核心技術棧） |
| T020 | CHECK 約束至 Migration | 技術細節 | ✅ 合理（資料完整性保障） |
| T051 | 統計查詢效能最佳化 | 非功能需求 | ✅ 合理（效能需求相關） |
| T171-T204 | Phase 9 完善與驗證 | 品質保證 | ✅ 合理（憲章合規驗證） |

**評估**: 所有未對應任務皆為合理的技術實作需求或品質保證活動，不影響功能需求覆蓋度。

---

## 憲章對齊問題

### ✅ 無嚴重違規

所有三個制品（spec.md, plan.md, tasks.md）皆符合憲章 v1.1.0 的強制要求：

#### 程式碼品質（原則 I）
- ✅ plan.md 明確定義型別安全需求（C# 強型別、TypeScript 嚴格模式）
- ✅ plan.md 指定 linting 工具（EditorConfig, ESLint, Prettier, StyleCop）
- ✅ tasks.md T171-T176 包含品質驗證任務（覆蓋率、複雜度、linting）
- ✅ plan.md 架構考量複雜度限制（函式 <10, 模組 <15）

#### 測試標準（原則 II）
- ✅ tasks.md 強制執行 TDD（所有 US 皆有「測試」子階段，標註「先寫測試、取得核准、確認失敗、再實作」）
- ✅ tasks.md T177 驗證測試金字塔比例（70/20/10）
- ✅ tasks.md 所有 US 皆包含 Contract 測試任務（T041, T060, T087, T101, T124, T146）
- ✅ tasks.md T178 包含效能迴歸測試
- ✅ tasks.md T179 包含無障礙性測試（axe-core）

#### 使用者體驗一致性（原則 III）
- ✅ plan.md 指定 Vuetify 3 設計系統
- ✅ plan.md 記錄 WCAG 2.1 AA 要求
- ✅ plan.md 定義響應式斷點（320px, 768px, 1024px）
- ✅ tasks.md T040, T058 包含載入狀態任務
- ✅ tasks.md T033, T039 包含錯誤處理策略
- ✅ tasks.md T183 包含離線支援測試
- ✅ **spec.md, plan.md, tasks.md 全部使用繁體中文撰寫**
- ✅ tasks.md 所有 US 皆有繁體中文 i18n 任務（T059, T086, T100, T123, T145, T170）

#### 文件標準
- ✅ spec.md 使用繁體中文（驗證於 tasks.md T187）
- ✅ plan.md 使用繁體中文（驗證於 tasks.md T188）
- ✅ tasks.md 使用繁體中文（驗證於 tasks.md T189）
- ✅ API 文件要求繁體中文（驗證於 tasks.md T191）

#### 效能需求（原則 IV）
- ✅ plan.md 定義頁面載入目標（<2s on 3G, <1s 寬頻）
- ✅ plan.md 指定互動回應目標（點擊 <100ms, 表單 <200ms）
- ✅ plan.md 定義 API 延遲目標（p95 <500ms 讀取, <1000ms 寫入）
- ✅ tasks.md T193-T198 包含效能測試與監控任務

**評估**: 本專案完全符合憲章所有強制性原則，無需提出修正。

---

## 建議改善事項

### 高優先級（建議在 Phase 0 完成前解決）

1. **[C1] 補充運動項目與器材關聯功能的任務** (HIGH)
   - **問題**: FR-014 要求「允許使用者為運動項目選擇或關聯運動器材」，但 tasks.md US6 僅有 EquipmentSelector 元件（T078, T164），缺少關聯邏輯的 CRUD 任務
   - **建議**: 在 US6 後端實作中增加任務：
     - 建立 ExerciseTypeEquipment 多對多關聯表（若 data-model.md 中需要）
     - 實作 ExerciseTypeService 中的器材關聯方法（AddEquipment, RemoveEquipment）
     - 新增 API 端點 POST /exercise-types/{id}/equipments 與 DELETE /exercise-types/{id}/equipments/{equipmentId}
   - **影響**: 中等（US6 功能不完整，但不阻礙其他故事）

2. **[C2] 補充器材說明文字的 CRUD 功能** (HIGH)
   - **問題**: FR-015 要求「允許使用者為運動器材新增說明文字」，但 tasks.md 僅有 Equipment 實體定義（T015）和 DTO（T152），缺少說明文字的編輯介面
   - **建議**: 在 US6 中增加任務：
     - 建立 EquipmentForm 元件（frontend/src/components/settings/EquipmentForm.vue）
     - 實作 EquipmentsController 的 POST/PUT 端點（目前僅有 GET）
     - 在 Settings.vue 中增加器材管理標籤頁
   - **影響**: 中等（FR-015 未完全實作）

3. **[I1] 統一 WorkoutRecord 欄位定義** (HIGH)
   - **問題**: data-model.md 定義 WorkoutRecord 包含 Weight (decimal nullable)，tasks.md T079 表單也包含體重欄位，但 spec.md FR-007 未明確列出
   - **建議**: 在 spec.md FR-007 中明確列出所有欄位：
     ```
     FR-007: 系統必須提供新增健身紀錄功能，記錄欄位包含：
     - 日期（必填）
     - 運動項目（必填）
     - 運動時長（分鐘，必填，1-480）
     - 消耗卡路里（大卡，系統自動計算預設值，使用者可手動調整，1-5000）
     - 體重（公斤，選填）
     - 備註（選填，最多 500 字）
     ```
   - **影響**: 低（不影響實作，但規格文件應完整）

4. **[A1] 明確卡路里計算公式** (HIGH)
   - **問題**: spec.md FR-007 和 FR-018 多次提及「自動計算」卡路里，但未說明計算方法，data-model.md 已定義 MET 公式
   - **建議**: 在 spec.md 澄清事項或功能需求中增加：
     ```
     Q: 系統如何自動計算消耗卡路里？
     A: 使用代謝當量（MET）公式：卡路里 = MET 值 × 體重（公斤） × 時間（小時）
        如未提供體重，使用預設值 70 公斤。各運動項目的 MET 值參見 data-model.md。
     ```
   - **影響**: 中等（影響使用者理解與測試驗收）

### 中優先級（可在實作過程中逐步修正）

5. **[C3] 明確 Token 刷新機制** (MEDIUM)
   - **問題**: FR-033 要求「維持登入狀態」，tasks.md T022 有 JWT Token 服務，但未明確 Token 刷新機制
   - **建議**: 
     - 確認 T023 AuthController 是否包含 refresh token 端點（contracts/api-spec.md 可能已定義）
     - 若未定義，在 US2 之前增加任務：實作 Token 自動刷新邏輯於 frontend/src/services/api.ts
   - **影響**: 低（基本 JWT 已覆蓋，刷新為進階優化）

6. **[I2] 統一運動項目類別術語** (MEDIUM)
   - **問題**: spec.md 使用「有氧/重訓/伸展」，data-model.md 使用 enum (Cardio, Strength, Flexibility, Sports, Other)
   - **建議**: 在 spec.md 關鍵實體章節增加類別對照表：
     ```
     運動項目類別 (ExerciseCategory):
     - 有氧運動 (Cardio): 跑步、游泳、騎自行車
     - 重量訓練 (Strength): 舉重、器械訓練
     - 伸展運動 (Flexibility): 瑜伽、皮拉提斯
     - 運動競技 (Sports): 籃球、網球等
     - 其他 (Other): 使用者自訂項目
     ```
   - **影響**: 低（術語不一致但意義明確）

7. **[A3] 明確跨月週統計規則** (MEDIUM)
   - **問題**: spec.md 邊界案例提到「週的定義：週一至週日，跨月的週歸屬於包含較多天數的月份」，但未定義平手情況（3.5 天 vs 3.5 天）
   - **建議**: 在 spec.md 澄清事項中增加：
     ```
     Q: 當一週剛好跨兩個月且各佔 3.5 天時，如何歸屬？
     A: 歸屬於週日所在的月份（週的最後一天決定）
     ```
   - **影響**: 低（極端邊界案例，實際發生機率低）

8. **[C4] 將離線支援列為明確功能需求** (MEDIUM)
   - **問題**: plan.md 和 tasks.md T183 提及離線支援，但 spec.md 未作為獨立功能需求
   - **建議**: 在 spec.md 增加功能需求：
     ```
     FR-036: 系統必須支援核心功能的離線操作，包含：
     - 離線瀏覽已載入的健身紀錄
     - 離線新增健身紀錄（儲存於本地）
     - 連線恢復時自動同步本地變更至伺服器
     - 同步衝突時優先保留最新資料並通知使用者
     ```
   - **影響**: 低（技術實作已規劃，補充規格文件）

9. **[T1] 建立術語對照表** (MEDIUM)
   - **問題**: spec.md 使用中文術語（運動項目、器材），tasks.md 使用英文實體名（ExerciseType, Equipment）
   - **建議**: 在 spec.md 關鍵實體章節增加術語對照表：
     ```
     | 規格術語 | 資料模型名稱 | 說明 |
     |----------|--------------|------|
     | 運動項目 | ExerciseType | 運動的種類，如跑步、游泳 |
     | 運動器材 | Equipment | 運動使用的器材，如跑步機、啞鈴 |
     | 健身紀錄 | WorkoutRecord | 單次運動的記錄 |
     | 運動目標 | WorkoutGoal | 使用者設定的週期性目標 |
     | 使用者 | User | 系統使用者 |
     ```
   - **影響**: 低（提升文件可讀性）

10. **[I5] 確認週/月/年切換實作範圍** (MEDIUM)
    - **問題**: spec.md FR-001, FR-002 明確要求週/月/年切換，但 tasks.md US1 僅實作週統計，月/年統計散佈於 US5
    - **建議**: 
      - 選項 A（建議）: 在 spec.md 澄清 US1 MVP 僅需週統計，月/年為進階功能（US5）
      - 選項 B: 在 US1 增加月/年統計任務（會增加 MVP 複雜度）
    - **影響**: 低（優先級已明確，僅需文件對齊）

11. **[A2] 明確目標達成提示的觸發邏輯** (MEDIUM)
    - **問題**: FR-019 要求「達成目標時顯示祝賀訊息」，tasks.md T118 有 AchievementBadge 元件，但觸發條件不明確
    - **建議**: 在 spec.md FR-019 增加驗收準則：
      ```
      驗收準則：
      - 當使用者完成某次健身記錄後，系統即時檢查是否達成目標
      - 若達成（進度 ≥100%），首頁顯示祝賀徽章並播放簡短動畫
      - 徽章顯示 3 秒後自動淡出，使用者可點擊關閉
      - 每個目標週期僅顯示一次祝賀訊息
      ```
    - **影響**: 低（實作細節，不影響核心功能）

12. **[C5] 增加邊界案例測試** (MEDIUM)
    - **問題**: spec.md 邊界案例提到「同一天多筆相同運動項目自動加總」，但 tasks.md US2 測試未明確涵蓋
    - **建議**: 在 T062 (WorkoutRecordValidator 單元測試) 中增加測試案例：
      ```
      測試場景：
      1. 使用者在同一天新增兩筆「跑步」記錄（早上 30 分鐘、晚上 40 分鐘）
      2. 系統正確儲存兩筆分別的紀錄
      3. 每日統計查詢時，系統回傳兩筆紀錄並正確計算總計（70 分鐘）
      ```
    - **影響**: 低（邊界案例，現有架構應能正確處理）

### 低優先級（可選改善，不影響實作）

13. **[A4] 定義標準錯誤訊息格式** (LOW)
    - **建議**: 在 contracts/api-spec.md 中增加錯誤訊息範例章節
    - **影響**: 極低（實作過程中自然產生）

14. **[A5] 增加成功標準測量方法** (LOW)
    - **建議**: 在 spec.md SC-003 增加「透過 Google Analytics 或使用者測試追蹤」
    - **影響**: 極低（非技術實作需求）

15. **[A6] 明確索引策略參照** (LOW)
    - **建議**: 在 plan.md 增加「詳細索引策略參見 data-model.md」
    - **影響**: 極低（已在 data-model.md 定義）

16. **[I4] 釐清 Phase 2 實體相依性** (LOW)
    - **建議**: 在 tasks.md Phase 2 註記中說明「標記 [P] 表示可同時建立 .cs 檔案，EF Core 會在 Migration 時解析外鍵相依性」
    - **影響**: 極低（技術細節，不影響執行）

17. **[T2] 統一「運動時間」術語** (LOW)
    - **建議**: 在 spec.md 全文搜尋並統一為「運動時長（分鐘）」
    - **影響**: 極低（術語不一致但意義明確）

18. **[T3] 確認 TrendService 命名** (LOW)
    - **建議**: 在 tasks.md 確認是否所有趨勢功能皆由 StatisticsService 處理，或需單獨 TrendService
    - **影響**: 極低（實作決策，不影響規格）

---

## 指標摘要

### 整體統計
- **總功能需求**: 35 個
- **總任務**: 204 個
- **使用者故事**: 6 個
- **關鍵實體**: 5 個

### 覆蓋度指標
- **需求覆蓋率**: 82.9% 完全覆蓋，11.4% 部分覆蓋，5.7% 需補充
- **無需求任務**: 0 個（所有技術任務皆有合理性）
- **測試覆蓋**: 100%（所有 6 個 US 皆有完整測試階段）

### 品質指標
- **模糊性問題**: 6 個（1 HIGH, 3 MEDIUM, 2 LOW）
- **覆蓋度問題**: 5 個（2 HIGH, 3 MEDIUM）
- **不一致性問題**: 5 個（1 HIGH, 2 MEDIUM, 2 LOW）
- **術語漂移**: 3 個（1 MEDIUM, 2 LOW）

### 憲章合規
- **原則 I（程式碼品質）**: ✅ 完全合規
- **原則 II（測試標準）**: ✅ 完全合規
- **原則 III（使用者體驗）**: ✅ 完全合規
- **原則 IV（效能需求）**: ✅ 完全合規
- **文件標準**: ✅ 完全合規（全繁體中文）

---

## 下一步行動

### ✅ 可以繼續進行實作

所有 CRITICAL 問題已解決。HIGH 和 MEDIUM 問題不會阻礙實作進行，但建議在以下階段處理：

### 建議行動順序

1. **立即執行（實作前）**:
   - 解決 C1、C2（補充 FR-014, FR-015 的任務）→ 在 tasks.md US6 增加 2-3 個任務
   - 解決 I1、A1（統一 WorkoutRecord 欄位、明確卡路里公式）→ 更新 spec.md FR-007

2. **Phase 0（research.md 生成時）**:
   - 參考本分析報告，將 LINE Login 整合、MET 公式計算、離線同步策略納入研究範圍
   - 確認 Token 刷新機制（C3）的最佳實踐

3. **Phase 1（data-model.md / contracts 生成時）**:
   - 驗證 ExerciseType-Equipment 關聯設計（C1）
   - 確認 Equipment 是否需要 CRUD API（C2）
   - 建立術語對照表（T1）

4. **實作過程中（可選）**:
   - 逐步補充錯誤訊息範例（A4）
   - 調整週/月/年切換的優先級說明（I5）
   - 增加邊界案例測試（C5）

### 命令建議

```bash
# 選項 1：直接開始實作（接受現有規格）
/speckit.implement

# 選項 2：先修正 HIGH 優先級問題（建議）
# 手動編輯 spec.md, tasks.md 後執行：
/speckit.analyze  # 重新分析驗證

# 選項 3：先生成 research.md（推薦）
/speckit.plan  # 會自動生成 Phase 0 research.md
```

---

## 補救建議

您是否希望我為以下 HIGH 優先級問題提供具體的補救編輯建議？

**HIGH 優先級問題（4 個）**:
1. **C1**: 補充運動項目與器材關聯功能的任務
2. **C2**: 補充器材說明文字的 CRUD 功能
3. **I1**: 統一 WorkoutRecord 欄位定義（spec.md vs data-model.md）
4. **A1**: 明確卡路里計算公式

如需要，我可以提供：
- 需要新增的任務描述（task ID 和具體內容）
- spec.md 中需要修改的具體段落（oldString/newString）
- data-model.md 或 contracts/api-spec.md 的調整建議

請回覆「yes」以獲取具體補救方案，或回覆「no」直接進入實作階段。

---

**報告生成時間**: 2025-11-02  
**分析工具版本**: speckit.analyze v1.0  
**constitution 版本**: v1.1.0
