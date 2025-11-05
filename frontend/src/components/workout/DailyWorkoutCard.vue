<template>
  <v-card class="daily-workout-card mb-3">
    <v-card-item>
      <div class="d-flex justify-space-between align-center">
        <div class="flex-grow-1">
          <div class="d-flex align-center mb-2">
            <v-icon size="small" class="mr-2">{{ getExerciseIcon(record.exerciseTypeName) }}</v-icon>
            <span class="font-weight-medium">{{ record.exerciseTypeName }}</span>
          </div>
          <div class="text-caption text-medium-emphasis">
            {{ record.equipmentName || $t('workout.noEquipment') }}
          </div>
        </div>
        <div class="text-right">
          <div class="font-weight-bold text-primary">
            {{ record.caloriesBurned }}<span class="text-caption">kcal</span>
          </div>
          <div class="text-caption text-medium-emphasis">
            {{ record.durationMinutes }}{{ $t('common.minuteAbbr') }}
          </div>
        </div>
      </div>
    </v-card-item>
    <v-divider v-if="showNotes && record.notes" class="my-2"></v-divider>
    <v-card-text v-if="showNotes && record.notes" class="py-2">
      <div class="text-caption">{{ record.notes }}</div>
    </v-card-text>
    <v-card-actions v-if="showActions" class="pt-0">
      <v-spacer></v-spacer>
      <v-btn
        size="small"
        variant="text"
        @click="$emit('edit')"
      >
        {{ $t('common.edit') }}
      </v-btn>
      <v-btn
        size="small"
        variant="text"
        color="error"
        @click="$emit('delete')"
      >
        {{ $t('common.delete') }}
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { WorkoutRecordResponseDto } from '@/types/workout'

interface Props {
  record: WorkoutRecordResponseDto
  showNotes?: boolean
  showActions?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  showNotes: true,
  showActions: false
})

defineEmits<{
  edit: []
  delete: []
}>()

const { t } = useI18n()

const getExerciseIcon = (exerciseType: string): string => {
  const iconMap: Record<string, string> = {
    '跑步': 'mdi-run',
    '騎車': 'mdi-bike',
    '游泳': 'mdi-pool',
    '健身': 'mdi-dumbbell',
    '瑜伽': 'mdi-yoga',
    '登山': 'mdi-mountain',
    '漫步': 'mdi-walk',
    '其他': 'mdi-dumbbell'
  }
  return iconMap[exerciseType] || 'mdi-dumbbell'
}
</script>

<style scoped>
.daily-workout-card {
  border-left: 4px solid rgb(var(--v-theme-primary));
  transition: all 0.2s ease;
}

.daily-workout-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}
</style>
