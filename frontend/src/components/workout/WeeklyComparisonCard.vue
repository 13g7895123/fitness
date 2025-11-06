<template>
  <Card :title="$t('statistics.comparison')">
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div class="p-3 bg-gray-50 rounded-lg text-center">
        <div class="text-sm text-gray-600 mb-2">{{ $t('statistics.totalTime') }}</div>
        <div class="text-2xl font-bold" :class="getClass(summary?.durationChangePercent)">
          {{ summary?.durationChangePercent || 0 }}%
        </div>
      </div>
      <div class="p-3 bg-gray-50 rounded-lg text-center">
        <div class="text-sm text-gray-600 mb-2">{{ $t('statistics.totalCalories') }}</div>
        <div class="text-2xl font-bold" :class="getClass(summary?.caloriesChangePercent)">
          {{ summary?.caloriesChangePercent || 0 }}%
        </div>
      </div>
      <div class="p-3 bg-gray-50 rounded-lg text-center">
        <div class="text-sm text-gray-600 mb-2">{{ $t('statistics.workoutDays') }}</div>
        <div class="text-2xl font-bold" :class="getClass(summary?.workoutDaysChangePercent)">
          {{ summary?.workoutDaysChangePercent || 0 }}%
        </div>
      </div>
    </div>
  </Card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useStatisticsStore } from '@/stores/statistics'
import Card from '@/components/common/Card.vue'

const statisticsStore = useStatisticsStore()
const summary = computed(() => statisticsStore.weeklySummary)

const getClass = (value?: number) => {
  if (!value) return 'text-gray-600'
  return value > 0 ? 'text-green-600' : value < 0 ? 'text-red-600' : 'text-gray-600'
}
</script>

<style scoped>
/* Tailwind handles all styling */
</style>
