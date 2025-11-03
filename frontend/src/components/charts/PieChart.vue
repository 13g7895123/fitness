<template>
  <v-card class="pie-chart-card">
    <v-card-item>
      <div class="chart-header">
        <h3 class="chart-title">{{ title }}</h3>
      </div>
      <div class="chart-container">
        <canvas ref="chartCanvas" class="chart-canvas"></canvas>
        <div class="legend">
          <div v-for="(item, index) in data" :key="index" class="legend-item">
            <span class="legend-color" :style="{ backgroundColor: colors[index % colors.length] }"></span>
            <span class="legend-text">{{ item.exerciseName }}: {{ item.percentageOfTotal.toFixed(1) }}%</span>
          </div>
        </div>
      </div>
    </v-card-item>
  </v-card>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import type { ExerciseDistributionDto } from '@/types/statistics'

interface Props {
  data: ExerciseDistributionDto[]
  title?: string
}

const props = withDefaults(defineProps<Props>(), {
  title: '運動項目分布'
})

const chartCanvas = ref<HTMLCanvasElement | null>(null)

const colors = [
  '#1976d2',
  '#388e3c',
  '#d32f2f',
  '#f57c00',
  '#7b1fa2',
  '#0097a7',
  '#c2185b',
  '#fbc02d'
]

const drawChart = () => {
  if (!chartCanvas.value || props.data.length === 0) return

  const ctx = chartCanvas.value.getContext('2d')
  if (!ctx) return

  const canvas = chartCanvas.value
  const rect = canvas.parentElement?.getBoundingClientRect()
  
  canvas.width = (rect?.width || 400) - 32
  canvas.height = 250

  const centerX = canvas.width / 2
  const centerY = canvas.height / 2
  const radius = Math.min(canvas.width, canvas.height) / 2 - 50

  let currentAngle = -Math.PI / 2

  props.data.forEach((item, index) => {
    const sliceAngle = (item.percentageOfTotal / 100) * Math.PI * 2

    // 繪製扇形
    ctx.fillStyle = colors[index % colors.length]
    ctx.beginPath()
    ctx.moveTo(centerX, centerY)
    ctx.arc(centerX, centerY, radius, currentAngle, currentAngle + sliceAngle)
    ctx.lineTo(centerX, centerY)
    ctx.fill()

    // 繪製邊框
    ctx.strokeStyle = '#fff'
    ctx.lineWidth = 2
    ctx.beginPath()
    ctx.arc(centerX, centerY, radius, currentAngle, currentAngle + sliceAngle)
    ctx.stroke()

    currentAngle += sliceAngle
  })
}

onMounted(() => {
  drawChart()
})

watch(() => props.data, () => {
  drawChart()
})
</script>

<style scoped>
.pie-chart-card {
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

.chart-container {
  display: flex;
  gap: 24px;
  align-items: center;
}

.chart-canvas {
  flex-shrink: 0;
  max-width: 250px;
  height: auto;
}

.legend {
  flex: 1;
  display: grid;
  gap: 8px;
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
  flex-shrink: 0;
}

.legend-text {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
</style>
