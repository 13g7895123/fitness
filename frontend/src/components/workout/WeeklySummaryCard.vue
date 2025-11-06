<template>
  <Card :title="$t('statistics.weekly')">
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div class="text-center p-3 bg-gray-50 rounded-lg">
        <div class="text-sm text-gray-600 mb-2">{{ $t('statistics.totalTime') }}</div>
        <div class="text-2xl font-bold text-primary">{{ summary?.totalDurationMinutes || 0 }} <span class="text-sm text-gray-500">分鐘</span></div>
      </div>
      <div class="text-center p-3 bg-gray-50 rounded-lg">
        <div class="text-sm text-gray-600 mb-2">{{ $t('statistics.totalCalories') }}</div>
        <div class="text-2xl font-bold text-primary">{{ Math.round(summary?.totalCaloriesBurned || 0) }} <span class="text-sm text-gray-500">卡</span></div>
      </div>
      <div class="text-center p-3 bg-gray-50 rounded-lg">
        <div class="text-sm text-gray-600 mb-2">{{ $t('statistics.workoutDays') }}</div>
        <div class="text-2xl font-bold text-primary">{{ summary?.workoutDays || 0 }} <span class="text-sm text-gray-500">天</span></div>
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
</script>

<style scoped>
/* Tailwind handles all styling */
</style>
