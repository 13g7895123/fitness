# Vuetify 到 Tailwind CSS 遷移指南

## 概述
本專案正在從 Vuetify 3 遷移到 Tailwind CSS，以獲得更輕量和靈活的樣式系統。

## 已完成的工作

### 1. 安裝和配置
- ✅ 安裝 Tailwind CSS, PostCSS, Autoprefixer
- ✅ 創建 `tailwind.config.js` 配置
- ✅ 創建 `postcss.config.js` 配置
- ✅ 更新 `global.css` 引入 Tailwind 指令
- ✅ 移除 Vuetify 套件
- ✅ 從 `main.ts` 移除 Vuetify 引用

### 2. 通用元件庫
已創建以下 Tailwind 版本的通用元件：

#### Button (`/components/common/Button.vue`)
- 支援 variant: primary, secondary, outlined, text
- 支援 size: small, medium, large
- 支援 loading 狀態和 disabled 狀態
- 支援 block 模式

```vue
<Button variant="primary" size="large" @click="handleClick">
  點擊我
</Button>
```

#### Input (`/components/common/Input.vue`)
- 支援所有標準 input 類型
- 內建錯誤和提示訊息顯示
- 支援 required 和 disabled 狀態

```vue
<Input
  v-model="form.name"
  label="名稱"
  placeholder="請輸入名稱"
  :error="errors.name"
  required
/>
```

#### Select (`/components/common/Select.vue`)
- 支援動態選項列表
- 支援 itemTitle 和 itemValue 配置
- 錯誤和提示訊息

```vue
<Select
  v-model="form.type"
  label="類型"
  :items="types"
  item-title="name"
  item-value="id"
/>
```

#### Card (`/components/common/Card.vue`)
- 支援 header, body, footer 插槽
- hoverable 效果
- 可選的 padding

```vue
<Card title="標題" hoverable>
  <template #header>
    <h3>自訂標題</h3>
  </template>
  內容區域
  <template #footer>
    <Button>操作</Button>
  </template>
</Card>
```

#### Dialog (`/components/common/Dialog.vue`)
- 可關閉/persistent 模式
- 支援不同的 maxWidth
- header, body, footer 插槽
- Teleport 到 body

```vue
<Dialog v-model="isOpen" title="對話框" max-width="lg">
  對話框內容
  <template #footer>
    <div class="flex gap-2 justify-end">
      <Button variant="outlined" @click="isOpen = false">取消</Button>
      <Button variant="primary" @click="handleSave">儲存</Button>
    </div>
  </template>
</Dialog>
```

### 3. 已遷移的頁面
- ✅ App.vue - 主應用佈局
- ✅ Login.vue - 登入頁面

## Vuetify 到 Tailwind 對應表

### 佈局
| Vuetify | Tailwind |
|---------|----------|
| `<v-container>` | `<div class="container mx-auto px-4">` |
| `<v-row>` | `<div class="grid grid-cols-12 gap-4">` |
| `<v-col cols="6">` | `<div class="col-span-6">` |
| `class="d-flex"` | `class="flex"` |
| `class="align-center"` | `class="items-center"` |
| `class="justify-center"` | `class="justify-center"` |

### 間距
| Vuetify | Tailwind |
|---------|----------|
| `class="pa-4"` | `class="p-4"` |
| `class="px-4"` | `class="px-4"` |
| `class="py-4"` | `class="py-4"` |
| `class="ma-4"` | `class="m-4"` |
| `class="mb-4"` | `class="mb-4"` |

### 文字
| Vuetify | Tailwind |
|---------|----------|
| `class="text-h1"` | `class="text-6xl font-bold"` |
| `class="text-h4"` | `class="text-3xl font-bold"` |
| `class="text-body-1"` | `class="text-base"` |
| `class="font-weight-bold"` | `class="font-bold"` |
| `class="text-center"` | `class="text-center"` |

### 顏色
| Vuetify | Tailwind |
|---------|----------|
| `color="primary"` | `class="text-primary"` 或 `class="bg-primary"` |
| `color="secondary"` | `class="text-secondary"` 或 `class="bg-secondary"` |
| `color="error"` | `class="text-red-600"` 或 `class="bg-red-600"` |

### 元件

#### 按鈕
```vue
<!-- Vuetify -->
<v-btn color="primary" size="large" variant="flat">
  按鈕
</v-btn>

<!-- Tailwind -->
<Button variant="primary" size="large">
  按鈕
</Button>
```

#### 輸入框
```vue
<!-- Vuetify -->
<v-text-field
  v-model="value"
  label="標籤"
  variant="outlined"
/>

<!-- Tailwind -->
<Input
  v-model="value"
  label="標籤"
/>
```

#### 卡片
```vue
<!-- Vuetify -->
<v-card>
  <v-card-title>標題</v-card-title>
  <v-card-text>內容</v-card-text>
</v-card>

<!-- Tailwind -->
<Card title="標題">
  內容
</Card>
```

## 需要遷移的頁面

### Views (8個)
- ❌ Home.vue - 首頁儀表板
- ❌ Goals.vue - 目標頁面
- ❌ Trends.vue - 趨勢頁面
- ❌ Settings.vue - 設定頁面
- ❌ WorkoutDetail.vue - 運動詳情
- ❌ NotFound.vue - 404頁面
- ❌ AuthCallback.vue - 認證回調

### Components
需要重構的元件類別：

1. **Charts 元件** (3-4個)
   - WeeklySummaryCard.vue
   - WeeklyComparisonCard.vue
   - DailyBarChart.vue

2. **Workout 元件** (5-6個)
   - AddWorkoutDialog.vue
   - EditWorkoutDialog.vue
   - DeleteWorkoutDialog.vue
   - WorkoutRecordForm.vue
   - ExerciseTypeSelector.vue
   - EquipmentSelector.vue

3. **Goals 元件** (2-3個)
   - GoalCard.vue
   - GoalProgress.vue

4. **Settings 元件** (1-2個)
   - SettingsPanel.vue

## 遷移步驟建議

### 階段 1: 核心佈局 (已完成)
- [x] App.vue
- [x] Login.vue
- [x] 通用元件庫

### 階段 2: 簡單頁面
1. NotFound.vue
2. AuthCallback.vue
3. Settings.vue

### 階段 3: 複雜頁面
1. Home.vue (需要先遷移 Charts 元件)
2. WorkoutDetail.vue (需要先遷移 Workout 元件)
3. Goals.vue (需要先遷移 Goals 元件)
4. Trends.vue (需要先遷移 Charts 元件)

### 階段 4: 子元件
按照依賴順序逐一遷移各個子元件

## 通用遷移模式

### 表單元件遷移
```vue
<!-- 之前 -->
<v-form @submit.prevent="onSubmit">
  <v-text-field v-model="form.name" label="名稱" />
  <v-btn type="submit">提交</v-btn>
</v-form>

<!-- 之後 -->
<form @submit.prevent="onSubmit" class="space-y-4">
  <Input v-model="form.name" label="名稱" />
  <Button type="submit">提交</Button>
</form>
```

### 對話框遷移
```vue
<!-- 之前 -->
<v-dialog v-model="isOpen">
  <v-card>
    <v-card-title>標題</v-card-title>
    <v-card-text>內容</v-card-text>
    <v-card-actions>
      <v-btn @click="isOpen = false">取消</v-btn>
      <v-btn @click="save">儲存</v-btn>
    </v-card-actions>
  </v-card>
</v-dialog>

<!-- 之後 -->
<Dialog v-model="isOpen" title="標題">
  內容
  <template #footer>
    <div class="flex gap-2 justify-end">
      <Button variant="outlined" @click="isOpen = false">取消</Button>
      <Button variant="primary" @click="save">儲存</Button>
    </div>
  </template>
</Dialog>
```

### 列表遷移
```vue
<!-- 之前 -->
<v-list>
  <v-list-item v-for="item in items" :key="item.id">
    <v-list-item-title>{{ item.name }}</v-list-item-title>
  </v-list-item>
</v-list>

<!-- 之後 -->
<div class="space-y-1">
  <div
    v-for="item in items"
    :key="item.id"
    class="list-item"
  >
    {{ item.name }}
  </div>
</div>
```

## 注意事項

1. **響應式設計**: Tailwind 使用 `sm:`, `md:`, `lg:` 等前綴，而非 Vuetify 的 breakpoint props
2. **圖標**: Vuetify 使用 MDI 圖標，需要改用 SVG 或其他圖標方案
3. **過渡動畫**: 改用 Vue 的 `<Transition>` 元件配合 Tailwind 類別
4. **主題**: 在 `tailwind.config.js` 中配置顏色而非 Vuetify 主題
5. **驗證**: 需要手動實現或使用其他驗證庫 (如 VeeValidate)

## 下一步
1. 決定是否繼續完整遷移或採用混合方案
2. 如果繼續，建議按照上述階段逐步進行
3. 每個階段完成後進行測試確保功能正常

## 資源
- [Tailwind CSS 文檔](https://tailwindcss.com/docs)
- [Headless UI](https://headlessui.com/) - 無樣式 UI 元件
- [Vue 3 文檔](https://vuejs.org/)
