# Vuetify 移除進度報告

**日期**: 2025-11-06  
**狀態**: 部分完成 - Settings 頁面已完全遷移到 Tailwind CSS

## ✅ 已完成工作

### 1. 移除 Vuetify 依賴
- ✅ 從 `package.json` 移除 `vuetify` (^3.7.4)
- ✅ 從 `package.json` 移除 `@mdi/font` (^7.4.47)
- ✅ 從 `package.json` 移除 `vite-plugin-vuetify` (^2.0.4)
- ✅ 從 `vite.config.ts` 移除 vuetify 插件
- ✅ 從 `main.ts` 移除 vuetify 導入和使用
- ✅ 刪除 `src/plugins/vuetify.ts`
- ✅ 執行 `npm install` 清理依賴

### 2. 配置 Tailwind CSS v4
- ✅ 安裝 `@tailwindcss/postcss`
- ✅ 更新 `postcss.config.js` 使用 `@tailwindcss/postcss`
- ✅ 更新 `tailwind.config.js` 添加自定義主題顏色
- ✅ 更新 `global.css` 使用 `@import 'tailwindcss'`
- ✅ 配置主題色彩（primary, secondary）
- ✅ 添加自定義滾動條樣式

### 3. 重寫 Settings 頁面（7 個組件）

#### ✅ ExerciseTypeSearchBar.vue
- 移除 `<v-text-field>`
- 使用原生 `<input>` + Tailwind
- 添加搜尋圖標和清除按鈕
- 使用 SVG 圖標替代 MDI

#### ✅ DeleteConfirmDialog.vue
- 移除 `<v-dialog>`, `<v-card>`
- 使用 `<Teleport>` + Modal 覆蓋層
- 添加過渡動畫（Transition）
- 自定義確認對話框樣式

#### ✅ ExerciseTypeList.vue
- 移除 `<v-card>`, `<v-list>`, `<v-chip>`, `<v-btn>`
- 使用 Tailwind 卡片樣式
- 使用 SVG 圖標替代 `mdi-*`
- Hover 效果和過渡動畫
- 標籤使用自定義 badge 樣式

#### ✅ EquipmentList.vue
- 同上，移除所有 Vuetify 組件
- 使用 Tailwind 完整重寫
- 保持相同功能和 UX

#### ✅ EquipmentForm.vue
- 移除 `<v-card>`, `<v-form>`, `<v-text-field>`, `<v-textarea>`
- 使用原生 HTML 表單元素
- 添加自定義驗證邏輯
- Loading 狀態使用動畫 SVG spinner
- 錯誤提示樣式化

#### ✅ ExerciseTypeForm.vue
- 移除所有 Vuetify 表單組件
- 使用原生 HTML 表單
- 自定義多選 checkbox 列表
- 驗證邏輯（name, description, defaultMET）
- 已選器材 badge 顯示
- 系統預設項目提示

#### ✅ Settings.vue (主頁面)
- 移除 `<v-container>`, `<v-row>`, `<v-col>`
- 移除 `<v-tabs>`, `<v-window>`, `<v-window-item>`
- 移除 `<v-dialog>`, `<v-snackbar>`
- 使用自定義 Tab 切換（按鈕樣式）
- 使用自定義 Modal 組件
- 使用自定義 Toast 通知（Transition）
- 自動關閉功能

### 4. 創建通用組件

#### ✅ Modal.vue
- `<Teleport>` 到 body
- 支援多種尺寸（sm, md, lg, xl, 2xl, 4xl, 6xl）
- Backdrop 點擊關閉（可選 persistent 模式）
- 進入/退出過渡動畫
- 可重用於所有對話框

### 5. 建構測試
- ✅ 建構成功（0 錯誤）
- ✅ Bundle 大小優化：
  - CSS: 811KB (Vuetify) → 32KB (Tailwind) **減少 96%**
  - Settings.js: 18KB → 34KB（因為增加了更多自定義邏輯）
- ✅ 開發伺服器運行正常（http://localhost:5177）

---

## ⚠️ 未完成工作

由於專案中使用 Vuetify 的地方非常多（**152 個使用實例**），以下頁面/組件仍需要重寫：

### 待重寫頁面

#### 1. WorkoutDetail.vue (高優先級)
- 使用 `<v-container>`, `<v-row>`, `<v-col>`
- 使用 `<v-btn>`, `<v-icon>`, `<v-card>`
- 使用 `<v-skeleton-loader>`
- **預估工作量**: 2-3 小時

#### 2. Charts 組件 (4 個)
- **LineChart.vue**: `<v-card>`, `<v-card-item>`
- **BarChart.vue**: `<v-card>`, `<v-card-item>`
- **PieChart.vue**: `<v-card>`, `<v-card-item>`
- **DataPointDetailDialog.vue**: `<v-dialog>`, `<v-btn>`
- **預估工作量**: 3-4 小時

#### 3. Workout 組件
- **WeeklyCalendar.vue**: `<v-card>`, `<v-btn>`
- **DailyWorkoutCard.vue**: `<v-card>`
- **AddWorkoutDialog.vue**: Dialog + Form
- **EditWorkoutDialog.vue**: Dialog + Form
- **DeleteWorkoutDialog.vue**: Dialog
- **預估工作量**: 4-5 小時

#### 4. Goals 頁面
- 部分使用 Vuetify 指令（`v-if`, `v-for`）- 這些不需要改
- 可能需要調整某些樣式
- **預估工作量**: 1-2 小時

#### 5. Trends 頁面
- 較少使用 Vuetify 組件
- 主要是 Charts 組件
- **預估工作量**: 1-2 小時

#### 6. Common 組件
- **ErrorMessage.vue**: `<v-alert>`
- **SuccessMessage.vue**: `<v-icon>`, `<v-btn>`
- **NotificationPanel.vue**: 可能使用 Vuetify
- **預估工作量**: 2-3 小時

---

## 📊 統計數據

### 已完成
- **組件重寫**: 8 個（Settings 相關）
- **代碼行數**: ~1,500 行重寫
- **Vuetify 使用實例移除**: ~80 個（估計）
- **完成度**: Settings 頁面 100%

### 待完成
- **剩餘組件**: ~15-20 個
- **剩餘 Vuetify 實例**: ~70 個
- **預估總工作量**: 15-20 小時
- **整體完成度**: ~35%

---

## 🎯 建議的下一步

### 選項 A: 繼續完成所有頁面（推薦）
1. 重寫 WorkoutDetail 頁面
2. 重寫 Charts 組件
3. 重寫 Workout 相關組件
4. 重寫 Goals 和 Trends 頁面
5. 重寫 Common 組件
6. 完整測試所有功能

**優點**: 完全移除 Vuetify，純 Tailwind 方案  
**缺點**: 工作量大（15-20 小時）

### 選項 B: 分階段遷移
1. 保持當前 Settings 頁面的 Tailwind 實現
2. 其他頁面暫時保留 Vuetify（需要重新安裝）
3. 逐步遷移其他頁面

**優點**: 彈性大，可以逐步完成  
**缺點**: 需要同時維護兩套 UI 框架

### 選項 C: 使用 Headless UI 庫加速
安裝 `@headlessui/vue` 來快速獲得無樣式的組件：
- Dialog/Modal
- Listbox (Select)
- Combobox (Autocomplete)
- Transitions

**優點**: 加速開發，減少自定義代碼  
**缺點**: 增加新依賴

---

## 💡 技術筆記

### Tailwind CSS v4 配置
```javascript
// postcss.config.js
export default {
  plugins: {
    '@tailwindcss/postcss': {},
  },
}

// global.css
@import 'tailwindcss';
```

### 自定義主題
```javascript
// tailwind.config.js
theme: {
  extend: {
    colors: {
      primary: {
        DEFAULT: '#2563eb',
        50: '#eff6ff',
        // ... 完整色階
      }
    }
  }
}
```

### Modal 組件模式
```vue
<Modal v-model="isOpen" max-width="lg">
  <YourContent />
</Modal>
```

### Toast 通知模式
使用 `<Transition>` + fixed positioning + 自動計時器

---

## 🚀 當前可用功能

### ✅ 完全可用
- Settings 頁面
  - 運動類型管理（新增/編輯/刪除/查看）
  - 器材管理（新增/編輯/刪除）
  - 搜尋功能
  - 系統預設項目保護
  - Toast 通知
  - 表單驗證

### ⚠️ 需要 Vuetify（暫時不可用）
- WorkoutDetail 頁面
- Charts 視覺化
- Workout 相關操作
- Goals 頁面
- Trends 頁面
- 部分 Common 組件

---

## 📦 依賴變化

### 移除
```json
{
  "vuetify": "^3.7.4",
  "@mdi/font": "^7.4.47",
  "vite-plugin-vuetify": "^2.0.4"
}
```

### 保留/添加
```json
{
  "tailwindcss": "^4.1.16",
  "@tailwindcss/postcss": "^4.0.0"
}
```

### Bundle 大小影響
- **總 CSS**: 811KB → 32KB (**-96%**)
- **Vuetify 字體**: ~2MB → 0KB
- **JS Bundle**: 略微增加（自定義邏輯）

---

## ✨ 下一個里程碑

**目標**: 完成 WorkoutDetail 和 Charts 組件遷移  
**預估時間**: 5-7 小時  
**優先級**: 高（這兩個頁面使用頻率最高）

---

**報告生成時間**: 2025-11-06  
**開發伺服器**: http://localhost:5177  
**分支**: 001-fitness-tracking
