<template>
  <div class="glass rounded-2xl p-6 border border-primary/20 bg-gradient-to-br from-primary-50/50 to-success-50/50">
    <div class="text-center">
      <div class="text-lg font-semibold mb-4 text-gray-800">{{ $t('workout.dayTotal') }}</div>
      <div class="grid grid-cols-2 gap-4">
        <div class="flex flex-col items-center gap-2">
          <div class="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center text-primary">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <div class="text-2xl font-bold text-gray-900">
            {{ totalDurationMinutes }}
          </div>
          <div class="text-xs text-gray-500">
            {{ $t('common.minuteAbbr') }}
          </div>
        </div>
        <div class="flex flex-col items-center gap-2">
          <div class="w-12 h-12 rounded-xl bg-success/10 flex items-center justify-center text-success">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 18.657A8 8 0 016.343 7.343S7 9 9 10c0-2 .5-5 2.986-7C14 5 16.09 5.777 17.656 7.343A7.975 7.975 0 0120 13a7.975 7.975 0 01-2.343 5.657z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.879 16.121A3 3 0 1012.015 11L11 14H9c0 .768.293 1.536.879 2.121z" />
            </svg>
          </div>
          <div class="text-2xl font-bold text-gray-900">
            {{ totalCaloriesBurned }}
          </div>
          <div class="text-xs text-gray-500">
            {{ $t('workout.caloriesShort') }}
          </div>
        </div>
      </div>
      <div class="mt-4 pt-4 border-t border-gray-200/50">
        <div class="text-xs text-gray-500">
          {{ recordCount }} {{ $t('workout.records') }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

interface Props {
  totalDurationMinutes: number
  totalCaloriesBurned: number
  recordCount: number
}

defineProps<Props>()

const { t } = useI18n()
</script>
