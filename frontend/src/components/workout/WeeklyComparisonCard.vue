<template>
  <v-card class="weekly-comparison-card">
    <v-card-title>{{ $t('statistics.comparison') }}</v-card-title>
    <v-card-text>
      <div class="comparison-list">
        <div class="comparison-item">
          <div class="item-label">{{ $t('statistics.totalTime') }}</div>
          <div class="item-value" :class="getClass(summary?.durationChangePercent)">
            {{ summary?.durationChangePercent || 0 }}%
          </div>
        </div>
        <div class="comparison-item">
          <div class="item-label">{{ $t('statistics.totalCalories') }}</div>
          <div class="item-value" :class="getClass(summary?.caloriesChangePercent)">
            {{ summary?.caloriesChangePercent || 0 }}%
          </div>
        </div>
        <div class="comparison-item">
          <div class="item-label">{{ $t('statistics.workoutDays') }}</div>
          <div class="item-value" :class="getClass(summary?.workoutDaysChangePercent)">
            {{ summary?.workoutDaysChangePercent || 0 }}%
          </div>
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

const getClass = (value?: number) => {
  if (!value) return ''
  return value > 0 ? 'positive' : value < 0 ? 'negative' : 'neutral'
}
</script>

<style scoped>
.weekly-comparison-card {
  margin-bottom: 16px;
}

.comparison-list {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 16px;
}

.comparison-item {
  padding: 12px;
  background: #f5f5f5;
  border-radius: 8px;
  text-align: center;
}

.item-label {
  font-size: 12px;
  color: #999;
  margin-bottom: 8px;
}

.item-value {
  font-size: 20px;
  font-weight: bold;
}

.item-value.positive {
  color: #4caf50;
}

.item-value.negative {
  color: #f44336;
}

.item-value.neutral {
  color: #999;
}
</style>
