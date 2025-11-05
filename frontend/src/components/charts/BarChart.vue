<template>
  <div>
    <v-card class="bar-chart-card">
      <v-card-item>
        <div class="chart-header">
          <h3 class="chart-title">{{ title }}</h3>
        </div>
        <canvas ref="chartCanvas" class="chart-canvas" @click="handleCanvasClick"></canvas>
        <div class="chart-hint">點擊柱子查看詳細資訊</div>
      </v-card-item>
    </v-card>

    <!-- 詳細資料對話框 -->
    <DataPointDetailDialog v-model="showDetailDialog" :data="selectedDataPoint" :title="dialogTitle" />
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import DataPointDetailDialog from './DataPointDetailDialog.vue'
import type { TrendDataDto } from '@/types/statistics'

interface Props {
  data: TrendDataDto[]
  title?: string
  dataType?: 'duration' | 'calories'
}

const props = withDefaults(defineProps<Props>(), {
  title: '柱狀圖',
  dataType: 'duration'
})

const chartCanvas = ref<HTMLCanvasElement | null>(null)
const showDetailDialog = ref(false)
const selectedDataPoint = ref<TrendDataDto | null>(null)
const dialogTitle = ref('詳細資料')
const barPositions = ref<Array<{ x: number; width: number; data: TrendDataDto }>>([])

const drawChart = () => {
  if (!chartCanvas.value || props.data.length === 0) return

  const ctx = chartCanvas.value.getContext('2d')
  if (!ctx) return

  const canvas = chartCanvas.value
  const rect = canvas.parentElement?.getBoundingClientRect()
  
  canvas.width = (rect?.width || 600) - 32
  canvas.height = 300

  const values = props.data.map((d: TrendDataDto) => props.dataType === 'duration' ? d.durationMinutes : d.caloriesBurned)
  const maxValue = Math.max(...values)
  
  const padding = 40
  const barWidth = Math.max(20, (canvas.width - padding * 2) / props.data.length - 8)
  const spacing = (canvas.width - padding * 2) / props.data.length

  // 清除舊的柱位置
  barPositions.value = []

  // 繪製背景網格
  ctx.strokeStyle = '#e0e0e0'
  ctx.lineWidth = 1
  const stepY = canvas.height / 5
  for (let i = 0; i <= 5; i++) {
    const y = canvas.height - stepY * i
    ctx.beginPath()
    ctx.moveTo(0, y)
    ctx.lineTo(canvas.width, y)
    ctx.stroke()
  }

  // 繪製柱子並記錄位置
  ctx.fillStyle = '#1976d2'
  props.data.forEach((d: TrendDataDto, i: number) => {
    const value = props.dataType === 'duration' ? d.durationMinutes : d.caloriesBurned
    const barHeight = (value / maxValue) * (canvas.height - padding * 2)
    const x = padding + i * spacing + (spacing - barWidth) / 2
    const y = canvas.height - padding - barHeight

    ctx.fillRect(x, y, barWidth, barHeight)
    
    // 記錄柱的位置用於互動
    barPositions.value.push({
      x,
      width: barWidth,
      data: d
    })
  })

  // 繪製 X 軸標籤
  ctx.fillStyle = '#666'
  ctx.font = '12px Arial'
  ctx.textAlign = 'center'
  
  const labelStep = Math.max(1, Math.floor(props.data.length / 5))
  props.data.forEach((d: TrendDataDto, i: number) => {
    if (i % labelStep === 0 || i === props.data.length - 1) {
      const x = padding + i * spacing + spacing / 2
      ctx.fillText(d.date.substring(5), x, canvas.height - 10)
    }
  })
}

const handleCanvasClick = (event: MouseEvent) => {
  if (!chartCanvas.value) return

  const rect = chartCanvas.value.getBoundingClientRect()
  const clickX = event.clientX - rect.left

  // 查找被點擊的柱
  for (const bar of barPositions.value) {
    if (clickX >= bar.x && clickX <= bar.x + bar.width) {
      selectedDataPoint.value = bar.data
      dialogTitle.value = `${bar.data.date} - 詳細資料`
      showDetailDialog.value = true
      break
    }
  }
}

onMounted(() => {
  drawChart()
})

watch(() => props.data, () => {
  drawChart()
})
</script>

<style scoped>
.bar-chart-card {
  margin-bottom: 16px;
}

.chart-header {
  margin-bottom: 16px;
}

.chart-title {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
}

.chart-canvas {
  display: block;
  width: 100%;
  max-height: 300px;
  cursor: pointer;
}

.chart-hint {
  font-size: 12px;
  color: rgba(0, 0, 0, 0.4);
  text-align: center;
  margin-top: 8px;
}
</style>
