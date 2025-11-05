<template>
  <v-card class="daily-bar-chart-card">
    <v-card-title>{{ $t('statistics.dailyBreakdown') }}</v-card-title>
    <v-card-text>
      <div class="chart-container">
        <div v-if="chartData && chartData.length" class="bar-chart">
          <div v-for="(day, index) in chartData" :key="index" class="bar-item">
            <div class="bar-wrapper">
              <div v-if="maxDuration > 0" class="duration-bar" :style="{ height: (day.durationMinutes / maxDuration * 100) + '%' }">
                <span v-if="day.durationMinutes > 0" class="bar-label">{{ day.durationMinutes }}</span>
              </div>
              <div class="day-label">{{ getDayName(day.dayOfWeek) }}</div>
            </div>
          </div>
        </div>
        <div v-else class="no-data">
          {{ $t('statistics.noData') }}
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useStatisticsStore } from '@/stores/statistics'

const statisticsStore = useStatisticsStore()

const chartData = computed(() => {
  return statisticsStore.weeklySummary?.dailyBreakdown || []
})

const maxDuration = computed(() => {
  return Math.max(0, ...chartData.value.map(d => d.durationMinutes))
})

const getDayName = (dayOfWeek: number): string => {
  const days = ['日', '一', '二', '三', '四', '五', '六']
  return days[dayOfWeek] || ''
}
</script>

<style scoped>
.daily-bar-chart-card {
  margin-bottom: 16px;
}

.chart-container {
  height: 250px;
}

.bar-chart {
  display: flex;
  align-items: flex-end;
  justify-content: space-around;
  height: 100%;
  padding: 12px 0;
}

.bar-item {
  flex: 1;
  max-width: 50px;
  margin: 0 4px;
}

.bar-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  height: 100%;
}

.duration-bar {
  width: 100%;
  background: linear-gradient(135deg, #1976d2, #2196f3);
  border-radius: 4px 4px 0 0;
  display: flex;
  align-items: flex-end;
  justify-content: center;
  min-height: 4px;
  position: relative;
}

.bar-label {
  color: white;
  font-size: 10px;
  font-weight: bold;
  padding-bottom: 2px;
}

.day-label {
  font-size: 12px;
  margin-top: 8px;
  color: #666;
}

.no-data {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #999;
}
</style>
