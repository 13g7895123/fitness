<template>
  <div>
    <v-card class="line-chart-card">
      <v-card-item>
        <div class="chart-header">
          <h3 class="chart-title">{{ title }}</h3>
          <div class="chart-legend">
            <div class="legend-item">
              <span class="legend-color" style="background-color: #1976d2;"></span>
              <span>{{ $t('statistics.totalTime') }}</span>
            </div>
          </div>
        </div>
        <canvas ref="chartCanvas" class="chart-canvas" @click="handleCanvasClick"></canvas>
        <div class="chart-hint">點擊數據點查看詳細資訊</div>
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
  title: '趨勢圖表',
  dataType: 'duration'
})

const chartCanvas = ref<HTMLCanvasElement | null>(null)
const showDetailDialog = ref(false)
const selectedDataPoint = ref<TrendDataDto | null>(null)
const dialogTitle = ref('詳細資料')
const dataPointPositions = ref<Array<{ x: number; y: number; data: TrendDataDto }>>([])

const drawChart = () => {
  if (!chartCanvas.value || props.data.length === 0) return

  const ctx = chartCanvas.value.getContext('2d')
  if (!ctx) return

  const canvas = chartCanvas.value
  const rect = canvas.parentElement?.getBoundingClientRect()
  
  canvas.width = (rect?.width || 600) - 32
  canvas.height = 300

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

  // 計算數據點
  const values = props.data.map(d => props.dataType === 'duration' ? d.durationMinutes : d.caloriesBurned)
  const maxValue = Math.max(...values)
  const padding = 40
  const pointSpacing = (canvas.width - padding * 2) / Math.max(props.data.length - 1, 1)

  // 清除舊的數據點位置
  dataPointPositions.value = []

  // 繪製折線
  ctx.strokeStyle = '#1976d2'
  ctx.lineWidth = 2
  ctx.beginPath()

  props.data.forEach((d, i) => {
    const value = props.dataType === 'duration' ? d.durationMinutes : d.caloriesBurned
    const x = padding + i * pointSpacing
    const y = canvas.height - padding - (value / maxValue) * (canvas.height - padding * 2)

    if (i === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })

  ctx.stroke()

  // 繪製數據點並記錄位置（用於互動）
  ctx.fillStyle = '#1976d2'
  props.data.forEach((d, i) => {
    const value = props.dataType === 'duration' ? d.durationMinutes : d.caloriesBurned
    const x = padding + i * pointSpacing
    const y = canvas.height - padding - (value / maxValue) * (canvas.height - padding * 2)

    ctx.beginPath()
    ctx.arc(x, y, 4, 0, Math.PI * 2)
    ctx.fill()

    dataPointPositions.value.push({ x, y, data: d })
  })

  // 繪製 X 軸標籤
  ctx.fillStyle = '#666'
  ctx.font = '12px Arial'
  ctx.textAlign = 'center'
  
  const labelStep = Math.max(1, Math.floor(props.data.length / 5))
  props.data.forEach((d, i) => {
    if (i % labelStep === 0 || i === props.data.length - 1) {
      const x = padding + i * pointSpacing
      ctx.fillText(d.date.substring(5), x, canvas.height - 10)
    }
  })
}

const handleCanvasClick = (event: MouseEvent) => {
  if (!chartCanvas.value) return

  const rect = chartCanvas.value.getBoundingClientRect()
  const clickX = event.clientX - rect.left
  const clickY = event.clientY - rect.top

  // 查找最近的數據點
  const tolerance = 10
  for (const point of dataPointPositions.value) {
    const distance = Math.sqrt(Math.pow(clickX - point.x, 2) + Math.pow(clickY - point.y, 2))
    if (distance <= tolerance) {
      selectedDataPoint.value = point.data
      dialogTitle.value = `${point.data.date} - 詳細資料`
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
.line-chart-card {
  margin-bottom: 16px;
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.chart-title {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
}

.chart-legend {
  display: flex;
  gap: 16px;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
}

.legend-color {
  width: 12px;
  height: 12px;
  border-radius: 2px;
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
