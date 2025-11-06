<template>
  <Card class="mb-4">
    <div class="space-y-3">
      <div class="flex justify-between items-center">
        <span class="text-sm font-medium text-gray-900">{{ label }}</span>
        <span class="text-xs text-gray-500">{{ currentValue }} / {{ goalValue }}</span>
      </div>
      
      <div class="relative h-6 bg-gray-200 rounded-full overflow-hidden">
        <div
          class="absolute top-0 left-0 h-full transition-all duration-300"
          :class="getProgressColor(achievementPercent)"
          :style="{ width: achievementPercent + '%' }"
        ></div>
        <span class="absolute inset-0 flex items-center justify-center text-xs font-semibold text-white drop-shadow">
          {{ Math.round(achievementPercent) }}%
        </span>
      </div>
      
      <div v-if="isAchieved" class="flex items-center gap-2 p-2 bg-green-50 rounded-lg">
        <svg class="w-4 h-4 text-green-600" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
        </svg>
        <span class="text-xs font-semibold text-green-600">{{ $t('goals.achieved') }}</span>
      </div>
    </div>
  </Card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import Card from '@/components/common/Card.vue'

interface Props {
  label: string
  currentValue: number
  goalValue: number
  isAchieved?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isAchieved: false
})

const { t } = useI18n()

const achievementPercent = computed(() => {
  if (!props.goalValue || props.goalValue <= 0) return 0
  return Math.min((props.currentValue / props.goalValue) * 100, 100)
})

const getProgressColor = (percent: number): string => {
  if (percent >= 100) return 'bg-green-500'
  if (percent >= 75) return 'bg-blue-500'
  if (percent >= 50) return 'bg-yellow-500'
  return 'bg-red-500'
}
</script>

<style scoped>
/* Tailwind handles all styling */
</style>
