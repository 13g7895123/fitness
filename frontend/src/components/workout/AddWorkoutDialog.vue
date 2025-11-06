<template>
  <Dialog
    v-model="isOpen"
    :title="$t('workout.addRecord')"
    max-width="lg"
    @close="closeDialog"
  >
    <WorkoutRecordForm
      @submit="handleSubmit"
      @cancel="closeDialog"
    />
  </Dialog>
</template>

<script setup lang="ts">
import { ref, watch, defineProps, defineEmits } from 'vue'
import Dialog from '@/components/common/Dialog.vue'
import WorkoutRecordForm from './WorkoutRecordForm.vue'
import { useWorkoutService } from '@/services/workoutService'
import { useWorkoutsStore } from '@/stores/workouts'
import { useErrorHandler } from '@/utils/errorHandler'
import type { CreateWorkoutRecordDto } from '@/types/workout'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:modelValue', 'success'])

const { createRecord, isLoading } = useWorkoutService()
const workoutsStore = useWorkoutsStore()
const { showSuccess, showError } = useErrorHandler()

const isOpen = ref(props.modelValue)

// 同步 props 變化
watch(() => props.modelValue, (newVal) => {
  isOpen.value = newVal
})

// 同步 isOpen 變化
watch(isOpen, (newVal) => {
  emit('update:modelValue', newVal)
})

const handleSubmit = async (formData: CreateWorkoutRecordDto) => {
  try {
    const result = await createRecord(formData)
    if (result) {
      workoutsStore.addRecord(result)
      showSuccess('新增健身紀錄成功')
      closeDialog()
      emit('success')
    } else {
      showError('新增健身紀錄失敗')
    }
  } catch (error: any) {
    showError(error.message || '新增健身紀錄失敗')
  }
}

const closeDialog = () => {
  isOpen.value = false
}
</script>

<style scoped>
/* Styles handled by Tailwind */
</style>
