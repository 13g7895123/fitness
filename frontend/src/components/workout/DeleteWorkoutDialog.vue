<template>
  <v-dialog v-model="isOpen" width="400">
    <v-card>
      <v-card-title class="text-error">
        {{ $t('workout.deleteRecord') }}
      </v-card-title>

      <v-divider />

      <v-card-text class="pa-6">
        <p class="mb-4">{{ $t('common.confirmDelete') }}</p>
        
        <v-list class="pa-0 mb-4">
          <v-list-item>
            <template #prepend>
              <v-icon color="info">mdi-calendar</v-icon>
            </template>
            <v-list-item-title>
              {{ formatDate(record?.date) }}
            </v-list-item-title>
          </v-list-item>
          <v-list-item>
            <template #prepend>
              <v-icon color="info">mdi-dumbbell</v-icon>
            </template>
            <v-list-item-title>
              {{ record?.exerciseTypeName }}
            </v-list-item-title>
          </v-list-item>
          <v-list-item>
            <template #prepend>
              <v-icon color="info">mdi-timer</v-icon>
            </template>
            <v-list-item-title>
              {{ record?.durationMinutes }} {{ $t('workout.minutes') }}
            </v-list-item-title>
          </v-list-item>
        </v-list>
      </v-card-text>

      <v-divider />

      <v-card-actions class="pa-4">
        <v-spacer />
        <v-btn
          variant="text"
          @click="closeDialog"
        >
          {{ $t('common.cancel') }}
        </v-btn>
        <v-btn
          color="error"
          @click="handleDelete"
          :loading="isDeleting"
        >
          {{ $t('common.delete') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits } from 'vue'
import { useWorkoutService } from '@/services/workoutService'
import { useWorkoutsStore } from '@/stores/workouts'
import { useErrorHandler } from '@/utils/errorHandler'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  record: {
    type: Object as any,
    required: true
  }
})

const emit = defineEmits(['update:modelValue', 'success'])

const { deleteRecord, isLoading } = useWorkoutService()
const workoutsStore = useWorkoutsStore()
const { showSuccess, showError } = useErrorHandler()

const isOpen = ref(props.modelValue)
const isDeleting = ref(false)

const formatDate = (date: string) => {
  if (!date) return ''
  const d = new Date(date)
  return d.toLocaleDateString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  })
}

const handleDelete = async () => {
  try {
    isDeleting.value = true
    const success = await deleteRecord(props.record.id)
    if (success) {
      workoutsStore.deleteRecord(props.record.id)
      showSuccess('刪除健身紀錄成功')
      closeDialog()
      emit('success')
    } else {
      showError('刪除健身紀錄失敗')
    }
  } catch (error: any) {
    showError(error.message || '刪除健身紀錄失敗')
  } finally {
    isDeleting.value = false
  }
}

const closeDialog = () => {
  isOpen.value = false
  emit('update:modelValue', false)
}

defineExpose({
  isOpen
})
</script>

<style scoped>
v-divider {
  margin: 16px 0;
}

.text-error {
  color: #f44336;
}
</style>
