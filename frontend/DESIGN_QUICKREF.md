# 🎨 設計系統快速參考

> Fitness Tracker UI/UX Design System v2.0

---

## 🚀 快速開始

### 安裝依賴
```bash
cd frontend
npm install
```

### 啟動開發伺服器
```bash
npm run dev
# 訪問: http://localhost:5178
```

### 查看設計展示
```
http://localhost:5178/design-showcase.html
```

---

## 🎨 色彩使用

### 漸層背景
```html
<!-- Primary 漸層 -->
<div class="bg-gradient-primary"></div>

<!-- Secondary 漸層 -->
<div class="bg-gradient-secondary"></div>

<!-- Accent 漸層 -->
<div class="bg-gradient-accent"></div>
```

### 漸層文字
```html
<h1 class="gradient-text">標題文字</h1>
<h2 class="gradient-text-pink">粉紅文字</h2>
```

---

## 📦 組件快速使用

### GradientButton
```vue
<GradientButton 
  variant="primary"    <!-- primary/secondary/accent/success/danger/outline/ghost -->
  size="lg"            <!-- sm/md/lg/xl -->
  fullWidth            <!-- 可選：全寬 -->
  :loading="false"     <!-- 可選：載入狀態 -->
  @click="handleClick"
>
  按鈕文字
</GradientButton>
```

### GradientCard
```vue
<GradientCard 
  title="卡片標題"
  subtitle="副標題"
  gradient="primary"   <!-- primary/secondary/accent/success/warm/cool/null -->
  hover                <!-- 可選：懸停效果 -->
>
  <template #header-action>
    <!-- 標題右側操作按鈕 -->
  </template>
  
  <!-- 主要內容 -->
  
  <template #footer>
    <!-- 底部內容 -->
  </template>
</GradientCard>
```

### StatsCard
```vue
<StatsCard
  value="1,234"
  label="總訓練次數"
  unit="次"            <!-- 可選 -->
  :trend="12"          <!-- 可選：+12% 或 -8% -->
  :icon="IconComponent" <!-- 可選 -->
  gradient="primary"   <!-- 可選 -->
/>
```

### Badge
```vue
<Badge 
  variant="success"    <!-- default/primary/secondary/success/warning/error/info -->
  size="md"            <!-- sm/md/lg -->
  rounded              <!-- 可選：完全圓角 -->
  outline              <!-- 可選：外框樣式 -->
>
  標籤文字
</Badge>
```

---

## 🎭 視覺效果

### 玻璃擬態
```html
<!-- 明亮玻璃 -->
<div class="glass rounded-3xl p-6">
  內容
</div>

<!-- 深色玻璃 -->
<div class="glass-dark rounded-3xl p-6">
  內容
</div>
```

### 懸停效果
```html
<div class="card-hover">
  <!-- 懸停時會上浮並加深陰影 -->
</div>

<button class="btn-hover">
  <!-- 按鈕懸停漣漪效果 -->
</button>
```

### 陰影
```html
<div class="shadow-soft">輕微陰影</div>
<div class="shadow-medium">中等陰影</div>
<div class="shadow-hard">深度陰影</div>
<div class="shadow-glow">藍色發光</div>
<div class="shadow-glow-pink">粉紅發光</div>
```

---

## 🎬 動畫

### Vue 過渡
```vue
<transition name="fade" mode="out-in">
  <component :is="currentComponent" />
</transition>
```

### Tailwind 動畫類別
```html
<div class="animate-fade-in">淡入</div>
<div class="animate-fade-in-up">向上淡入</div>
<div class="animate-fade-in-down">向下淡入</div>
<div class="animate-slide-in-right">從右滑入</div>
<div class="animate-scale-in">縮放淡入</div>
<div class="animate-pulse-soft">柔和脈衝</div>
```

---

## 📐 佈局

### 容器
```html
<!-- 標準內容寬度 -->
<div class="max-w-content mx-auto px-6">
  內容
</div>

<!-- 大容器 -->
<div class="max-w-7xl mx-auto px-6">
  內容
</div>
```

### 響應式網格
```html
<!-- 1-2-3 欄佈局 -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
  <div>項目 1</div>
  <div>項目 2</div>
  <div>項目 3</div>
</div>
```

---

## 📝 字體

### 標題
```html
<h1 class="text-display-1 font-display">超大標題</h1>
<h2 class="text-display-2 font-display">大標題</h2>
<h3 class="text-3xl font-bold">頁面標題</h3>
<h4 class="text-2xl font-semibold">區塊標題</h4>
```

### 內文
```html
<p class="text-base">標準內文</p>
<p class="text-sm text-gray-600">次要內文</p>
<p class="text-xs text-gray-500">小字</p>
```

---

## 🎯 常用模式

### Hero 區塊
```html
<div class="bg-gradient-to-br from-primary-600 via-purple-600 to-secondary-600 text-white">
  <div class="max-w-7xl mx-auto px-6 py-12">
    <h1 class="text-5xl font-bold mb-4">標題</h1>
    <p class="text-lg">副標題</p>
  </div>
</div>
```

### 統計卡片組
```html
<div class="grid grid-cols-3 gap-6">
  <StatsCard value="12" label="本週訓練" gradient="primary" />
  <StatsCard value="485" label="消耗卡路里" gradient="secondary" />
  <StatsCard value="2.5" label="運動時數" gradient="accent" />
</div>
```

### 操作按鈕組
```html
<div class="flex gap-3">
  <GradientButton variant="primary" size="lg">
    主要操作
  </GradientButton>
  <GradientButton variant="outline" size="lg">
    次要操作
  </GradientButton>
</div>
```

### 表單輸入
```html
<div class="space-y-4">
  <div>
    <label class="block text-sm font-medium mb-2">名稱</label>
    <input 
      type="text"
      class="w-full px-4 py-3 rounded-xl border border-gray-300 focus:ring-2 focus:ring-primary focus:border-transparent"
    />
  </div>
</div>
```

---

## 🔍 Debug 技巧

### 查看所有漸層
```
/design-showcase.html
```

### 檢查組件
```bash
# 在瀏覽器開發者工具中
# 搜尋 class 名稱看實際樣式
```

### 調整顏色
```javascript
// tailwind.config.js
theme: {
  extend: {
    colors: {
      primary: {
        DEFAULT: '#你的顏色',
        // ...
      }
    }
  }
}
```

---

## 📚 完整文檔

- **完整設計系統**: `DESIGN_SYSTEM.md`
- **重構報告**: `UI_REDESIGN_REPORT.md`
- **組件範例**: `public/design-showcase.html`
- **開發伺服器**: `http://localhost:5178`

---

## 💡 提示

### 效能最佳化
- 使用 `backdrop-filter` 需要 GPU 加速
- 避免過度使用陰影和漸層
- 大圖片使用懶加載

### 無障礙
- 確保顏色對比度 ≥ 4.5:1
- 按鈕最小尺寸 44x44px
- 提供鍵盤導航支援

### 響應式
- Mobile First 開發
- 測試常見斷點（375px, 768px, 1024px）
- 使用相對單位（rem, %, vw）

---

**快速參考更新**: 2025-11-06  
**版本**: 2.0  
**下次更新**: 待定
