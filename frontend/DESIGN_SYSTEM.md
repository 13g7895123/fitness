# UI/UX 設計系統 2.0

## 🎨 設計理念

這個專案採用**現代漸層美學**與**玻璃擬態風格**，打造出視覺衝擊力強、用戶體驗優秀的健身追蹤應用。

### 核心設計原則
- ✨ **視覺深度** - 使用漸層、陰影和玻璃效果創造層次感
- 🎯 **直觀導航** - 清晰的資訊架構和流暢的互動動畫
- 🎨 **活力色彩** - 充滿能量的漸層配色激勵用戶
- 📱 **響應式優先** - 完美適配所有裝置尺寸
- 🌈 **品牌一致性** - 統一的視覺語言和組件系統

## 🎨 色彩系統

### 主要色彩
- **Primary**: `#6366f1` - 靛藍色（主要品牌色）
  - 用途：主要按鈕、強調元素、導航激活狀態
  - 漸層：`linear-gradient(135deg, #667eea 0%, #764ba2 100%)`

- **Secondary**: `#ec4899` - 粉紅色（次要品牌色）
  - 用途：次要操作、標籤、裝飾元素
  - 漸層：`linear-gradient(135deg, #f093fb 0%, #f5576c 100%)`

- **Accent**: `#14b8a6` - 青綠色（強調色）
  - 用途：成功狀態、進度指示、成就徽章
  - 漸層：`linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)`

### 狀態色彩
- **Success**: `#10b981` - 綠色（成功、完成）
- **Warning**: `#f59e0b` - 橙色（警告、注意）
- **Error**: `#ef4444` - 紅色（錯誤、危險）
- **Info**: `#3b82f6` - 藍色（資訊提示）

### 背景與表面
- **Background**: 漸層背景 `from-gray-50 via-blue-50/30 to-purple-50/30`
- **Glass Surface**: `rgba(255, 255, 255, 0.7)` + `backdrop-filter: blur(10px)`
- **Dark Glass**: `rgba(15, 23, 42, 0.7)` + `backdrop-filter: blur(10px)`

### 預設漸層組合
- **gradient-primary**: 紫藍漸層 `#667eea → #764ba2`
- **gradient-secondary**: 粉紅漸層 `#f093fb → #f5576c`
- **gradient-accent**: 藍綠漸層 `#4facfe → #00f2fe`
- **gradient-success**: 綠色漸層 `#43e97b → #38f9d7`
- **gradient-warm**: 暖色漸層 `#fa709a → #fee140`
- **gradient-cool**: 冷色漸層 `#30cfd0 → #330867`

## 📦 組件系統

### GradientButton 漸層按鈕
```vue
<GradientButton variant="primary" size="lg">
  點擊我
</GradientButton>
```

**變體 (Variants)**
- `primary` - 主要按鈕（紫藍漸層）
- `secondary` - 次要按鈕（粉紅漸層）
- `accent` - 強調按鈕（藍綠漸層）
- `success` - 成功按鈕（綠色漸層）
- `danger` - 危險按鈕（紅色漸層）
- `outline` - 外框按鈕（白底藍框）
- `ghost` - 幽靈按鈕（透明底）

**尺寸 (Sizes)**
- `sm` - 小 `px-4 py-2 text-sm`
- `md` - 中 `px-6 py-3 text-base`
- `lg` - 大 `px-8 py-4 text-lg`
- `xl` - 超大 `px-10 py-5 text-xl`

**屬性 (Props)**
- `fullWidth` - 全寬按鈕
- `rounded` - 完全圓角
- `loading` - 載入狀態
- `disabled` - 禁用狀態

---

### GradientCard 漸層卡片
```vue
<GradientCard 
  title="卡片標題" 
  subtitle="副標題"
  gradient="primary"
  hover
>
  內容區域
</GradientCard>
```

**屬性**
- `title` - 標題文字
- `subtitle` - 副標題文字
- `gradient` - 漸層類型（primary/secondary/accent/success/warm/cool）
- `hover` - 啟用懸停效果
- `className` - 自訂類別

**插槽 (Slots)**
- `default` - 主要內容
- `header-action` - 標題右側操作區
- `footer` - 底部區域

---

### StatsCard 統計卡片
```vue
<StatsCard
  value="1,234"
  label="總訓練次數"
  unit="次"
  :trend="12"
  gradient="primary"
/>
```

**屬性**
- `value` - 數值
- `label` - 標籤文字
- `unit` - 單位
- `trend` - 趨勢百分比（正負數）
- `icon` - 圖標組件
- `gradient` - 漸層背景

---

### Badge 標籤
```vue
<Badge variant="success" size="md" rounded>
  完成
</Badge>
```

**變體**
- `default` - 預設灰色
- `primary` - 主要色
- `secondary` - 次要色
- `success` - 成功綠色
- `warning` - 警告橙色
- `error` - 錯誤紅色
- `info` - 資訊藍色

**尺寸**
- `sm` - 小 `px-2 py-0.5 text-xs`
- `md` - 中 `px-3 py-1 text-sm`
- `lg` - 大 `px-4 py-1.5 text-base`

**屬性**
- `rounded` - 完全圓角
- `outline` - 外框樣式

---

## 🎭 視覺效果

### 玻璃擬態 (Glassmorphism)
```css
.glass {
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.18);
}

.glass-dark {
  background: rgba(15, 23, 42, 0.7);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
}
```

### 陰影系統
- **shadow-soft**: `0 2px 8px 0 rgba(0, 0, 0, 0.05)` - 輕微陰影
- **shadow-medium**: `0 4px 16px 0 rgba(0, 0, 0, 0.08)` - 中等陰影
- **shadow-hard**: `0 8px 24px 0 rgba(0, 0, 0, 0.12)` - 深度陰影
- **shadow-glow**: `0 0 20px rgba(99, 102, 241, 0.3)` - 發光效果
- **shadow-glow-pink**: `0 0 20px rgba(236, 72, 153, 0.3)` - 粉紅發光

### 圓角系統
- **rounded-xl**: `1rem` (16px)
- **rounded-2xl**: `1.25rem` (20px)
- **rounded-3xl**: `1.5rem` (24px)
- **rounded-4xl**: `2rem` (32px)

---

## 🎬 動畫系統

### 淡入動畫
```css
.fade-enter-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}
.fade-enter-from {
  opacity: 0;
  transform: translateY(10px);
}
```

### 預設動畫類別
- `animate-fade-in` - 淡入
- `animate-fade-in-up` - 向上淡入
- `animate-fade-in-down` - 向下淡入
- `animate-slide-in-right` - 從右滑入
- `animate-slide-in-left` - 從左滑入
- `animate-scale-in` - 縮放淡入
- `animate-pulse-soft` - 柔和脈衝
- `animate-bounce-soft` - 柔和彈跳

### 過渡時間
- 快速：`0.2s`
- 標準：`0.3s`
- 慢速：`0.5s`

### 緩動函數
- 標準：`cubic-bezier(0.4, 0, 0.2, 1)`
- 進入：`cubic-bezier(0, 0, 0.2, 1)`
- 離開：`cubic-bezier(0.4, 0, 1, 1)`

---

## 間距系統

Tailwind 預設間距 + 自訂：
- **0-96**: 標準 Tailwind 間距（0px - 384px）
- 推薦使用：`2` `3` `4` `6` `8` `12` `16` `20` `24`

## 📝 字體系統

### 字體家族
```css
font-family: 'Inter', 'Noto Sans TC', sans-serif;  /* 內文字體 */
font-family: 'Poppins', 'Inter', sans-serif;       /* 標題字體 */
```

### 字體大小
- **display-1**: `3.5rem` (56px) - 超大標題
- **display-2**: `3rem` (48px) - 大標題
- **display-3**: `2.5rem` (40px) - 中標題
- **3xl**: `1.875rem` (30px) - 頁面標題
- **2xl**: `1.5rem` (24px) - 區塊標題
- **xl**: `1.25rem` (20px) - 小標題
- **lg**: `1.125rem` (18px) - 大內文
- **base**: `1rem` (16px) - 標準內文
- **sm**: `0.875rem` (14px) - 小字
- **xs**: `0.75rem` (12px) - 極小字

### 字重
- **Light**: `300` - 輕盈文字
- **Regular**: `400` - 一般文字
- **Medium**: `500` - 中等強調
- **Semibold**: `600` - 次標題
- **Bold**: `700` - 強調標題
- **Extrabold**: `800` - 超級強調

### 行高
- `leading-tight`: `1.25` - 緊湊
- `leading-snug`: `1.375` - 舒適
- `leading-normal`: `1.5` - 標準
- `leading-relaxed`: `1.625` - 放鬆
- `leading-loose`: `2` - 寬鬆

---

## 📱 響應式設計

### 斷點 (Breakpoints)
```javascript
screens: {
  'sm': '640px',   // 手機橫屏
  'md': '768px',   // 平板直向
  'lg': '1024px',  // 平板橫向 / 小筆電
  'xl': '1280px',  // 桌面
  '2xl': '1536px'  // 大螢幕
}
```

### 容器寬度
- **max-w-content**: `1280px` - 標準內容寬度
- **max-w-8xl**: `88rem` (1408px)
- **max-w-9xl**: `96rem` (1536px)

### 響應式原則
1. **Mobile First** - 從手機版開始設計
2. **流式佈局** - 使用 flex 和 grid
3. **彈性圖片** - 使用 max-w-full
4. **適當留白** - 不同尺寸不同間距
5. **可觸控目標** - 至少 44x44px

---

## ♿ 無障礙設計 (Accessibility)

### 顏色對比度
- 標準文字：至少 4.5:1 (WCAG AA)
- 大文字（18px+）：至少 3:1
- UI 組件：至少 3:1

### 鍵盤導航
```css
button:focus-visible,
a:focus-visible,
input:focus-visible {
  outline: 2px solid #6366f1;
  outline-offset: 2px;
}
```

### 語義化 HTML
- 使用正確的標籤（`<button>`, `<nav>`, `<main>`）
- 圖片添加 `alt` 屬性
- 表單添加 `<label>`
- 使用 ARIA 標籤（`aria-label`, `aria-describedby`）

### 可觸控目標
- 最小尺寸：44x44px
- 按鈕間距：至少 8px
- 手勢操作提供替代方案

---

## 🎨 圖標系統

### 圖標來源
- 內建 SVG 圖標（優先使用）
- Heroicons（備選）
- 自訂 SVG 圖標

### 圖標尺寸
- `w-4 h-4` - 16px（小圖標）
- `w-5 h-5` - 20px（標準圖標）
- `w-6 h-6` - 24px（大圖標）
- `w-8 h-8` - 32px（超大圖標）

### 使用原則
- 保持風格一致（統一線寬）
- 適當的間距和對齊
- 使用 `currentColor` 繼承顏色
- 提供替代文字

---

## 🎯 設計原則

### 1. 視覺層次 (Visual Hierarchy)
- 使用大小、顏色、間距創造層次
- 重要元素使用漸層和陰影強調
- 適當的留白引導視線

### 2. 一致性 (Consistency)
- 統一的組件庫
- 一致的顏色和字體
- 標準化的間距系統

### 3. 回饋與互動 (Feedback)
- 懸停效果（hover）
- 點擊效果（active）
- 載入狀態（loading）
- 錯誤提示（error）

### 4. 性能優化 (Performance)
- 使用 CSS 動畫（避免 JS）
- 圖片懶加載
- 適當的過渡時間
- 避免過度動畫

### 5. 品牌個性 (Brand Personality)
- **活力** - 使用鮮豔漸層和動畫
- **專業** - 保持乾淨的佈局
- **友善** - 圓角和柔和的陰影
- **激勵** - 正面的文案和視覺元素

---

## 📚 設計資源

### 字體
- [Google Fonts - Inter](https://fonts.google.com/specimen/Inter)
- [Google Fonts - Poppins](https://fonts.google.com/specimen/Poppins)
- [Google Fonts - Noto Sans TC](https://fonts.google.com/specimen/Noto+Sans+TC)

### 工具
- [Tailwind CSS](https://tailwindcss.com)
- [Hero Icons](https://heroicons.com)
- [CSS Gradient Generator](https://cssgradient.io)
- [Glassmorphism Generator](https://glassmorphism.com)

### 靈感
- [Dribbble - Fitness App](https://dribbble.com/tags/fitness-app)
- [Behance - Health & Fitness](https://www.behance.net/search/projects?search=health+fitness)

---

## 🚀 快速開始

### 使用設計組件
```vue
<script setup>
import GradientButton from '@/components/common/GradientButton.vue'
import GradientCard from '@/components/common/GradientCard.vue'
import StatsCard from '@/components/common/StatsCard.vue'
import Badge from '@/components/common/Badge.vue'
</script>

<template>
  <div class="space-y-6">
    <!-- 統計卡片 -->
    <StatsCard
      value="1,234"
      label="總訓練次數"
      :trend="12"
      gradient="primary"
    />

    <!-- 內容卡片 -->
    <GradientCard title="本週訓練" hover>
      <p>這裡是內容...</p>
    </GradientCard>

    <!-- 按鈕 -->
    <GradientButton variant="primary" size="lg">
      開始訓練
    </GradientButton>

    <!-- 標籤 -->
    <Badge variant="success" rounded>完成</Badge>
  </div>
</template>
```

---

**最後更新**: 2025-11-06  
**版本**: 2.0  
**維護者**: UI/UX 設計團隊
