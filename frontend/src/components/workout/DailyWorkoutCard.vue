<template>
  <div class="glass rounded-2xl p-4 mb-3 border-l-4 border-l-primary hover:shadow-lg transition-all duration-200">
    <div class="flex justify-between items-center">
      <div class="flex-grow">
        <div class="flex items-center mb-1">
          <!-- Icon placeholder based on exercise type -->
          <div class="mr-2 text-primary">
            <svg v-if="record.exerciseTypeName.includes('跑')" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
            <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 10h4.764a2 2 0 011.789 2.894l-3.5 7A2 2 0 0115.263 21h-4.017c-.163 0-.326-.02-.485-.06L7 20m7-10V5a2 2 0 00-2-2h-.095c-.5 0-.905.405-.905.905 0 .714-.211 1.412-.608 2.006L7 11v9m7-10h-2M7 20H5a2 2 0 01-2-2v-6a2 2 0 012-2h2.5" />
            </svg>
          </div>
          <span class="font-medium text-gray-900">{{ record.exerciseTypeName }}</span>
        </div>
        <div class="text-xs text-gray-500">
          {{ record.equipmentName || $t('workout.noEquipment') }}
        </div>
      </div>
      <div class="text-right">
        <div class="font-bold text-primary text-lg">
          {{ record.caloriesBurned }}<span class="text-xs ml-0.5">kcal</span>
        </div>
        <div class="text-xs text-gray-500">
          {{ record.durationMinutes }}{{ $t('common.minuteAbbr') }}
        </div>
      </div>
    </div>
    
    <div v-if="showNotes && record.notes" class="my-3 border-t border-gray-100 pt-2">
      <div class="text-sm text-gray-600">{{ record.notes }}</div>
    </div>
    
    <div v-if="showActions" class="flex justify-end gap-2 mt-2 pt-2 border-t border-gray-100">
      <button
        class="text-sm text-primary hover:text-primary-700 font-medium px-2 py-1 rounded hover:bg-primary-50 transition-colors"
        @click="$emit('edit')"
      >
        {{ $t('common.edit') }}
      </button>
      <button
        class="text-sm text-error hover:text-error-700 font-medium px-2 py-1 rounded hover:bg-error-50 transition-colors"
        @click="$emit('delete')"
      >
        {{ $t('common.delete') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { WorkoutRecordResponseDto } from '@/types/workout'

interface Props {
  record: WorkoutRecordResponseDto
  showNotes?: boolean
  showActions?: boolean
}

withDefaults(defineProps<Props>(), {
  showNotes: true,
  showActions: false
})

defineEmits<{
  edit: []
  delete: []
}>()

const { t } = useI18n()
</script>
