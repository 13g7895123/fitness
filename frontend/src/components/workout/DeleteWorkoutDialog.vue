<template>
  <Dialog
    v-model="isOpen"
    :title="$t('workout.deleteRecord')"
    max-width="sm"
  >
    <div class="space-y-4">
      <p class="text-gray-700">{{ $t('common.confirmDelete') }}</p>
      
      <div class="space-y-2 bg-gray-50 rounded-lg p-4">
        <div class="flex items-center gap-3">
          <svg class="w-5 h-5 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          <span class="text-gray-900">{{ formatDate(record?.date) }}</span>
        </div>
        <div class="flex items-center gap-3">
          <svg class="w-5 h-5 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19.428 15.428a2 2 0 00-1.022-.547l-2.387-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z" />
          </svg>
          <span class="text-gray-900">{{ record?.exerciseTypeName }}</span>
        </div>
        <div class="flex items-center gap-3">
          <svg class="w-5 h-5 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <span class="text-gray-900">{{ record?.durationMinutes }} {{ $t('workout.minutes') }}</span>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="flex justify-end gap-2">
        <Button
          variant="text"
          @click="closeDialog"
        >
          {{ $t('common.cancel') }}
        </Button>
        <Button
          class="bg-red-600 hover:bg-red-700 text-white"
          @click="handleDelete"
          :loading="isDeleting"
        >
          {{ $t('common.delete') }}
        </Button>
      </div>
    </template>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits } from 'vue'
import Dialog from '@/components/common/Dialog.vue'
import Button from '@/components/common/Button.vue'
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
/* Tailwind handles all styling */
</style>
