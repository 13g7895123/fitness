<template>
  <v-card class="weekly-summary-card">
    <v-card-title>{{ $t('statistics.weekly') }}</v-card-title>
    <v-card-text>
      <div class="summary-grid">
        <div class="summary-item">
          <div class="label">{{ $t('statistics.totalTime') }}</div>
          <div class="value">{{ summary?.totalDurationMinutes || 0 }} <span class="unit">分鐘</span></div>
        </div>
        <div class="summary-item">
          <div class="label">{{ $t('statistics.totalCalories') }}</div>
          <div class="value">{{ Math.round(summary?.totalCaloriesBurned || 0) }} <span class="unit">卡</span></div>
        </div>
        <div class="summary-item">
          <div class="label">{{ $t('statistics.workoutDays') }}</div>
          <div class="value">{{ summary?.workoutDays || 0 }} <span class="unit">天</span></div>
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useStatisticsStore } from '@/stores/statistics'

const statisticsStore = useStatisticsStore()
const summary = computed(() => statisticsStore.weeklySummary)
</script>

<style scoped>
.weekly-summary-card {
  margin-bottom: 16px;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 16px;
}

.summary-item {
  text-align: center;
  padding: 12px;
  background: #f5f5f5;
  border-radius: 8px;
}

.label {
  font-size: 12px;
  color: #999;
  margin-bottom: 8px;
}

.value {
  font-size: 24px;
  font-weight: bold;
  color: #1976d2;
}

.unit {
  font-size: 12px;
  color: #999;
  margin-left: 4px;
}
</style>
