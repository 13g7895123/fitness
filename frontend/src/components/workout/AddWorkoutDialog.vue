<template>
  <v-dialog v-model="isOpen" width="600">
    <v-card>
      <v-card-title>
        {{ $t('workout.addRecord') }}
        <v-spacer />
        <v-btn icon size="small" @click="closeDialog">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-card-title>

      <v-divider />

      <v-card-text class="pa-6">
        <WorkoutRecordForm
          @submit="handleSubmit"
          @cancel="closeDialog"
        />
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits } from 'vue'
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
  emit('update:modelValue', false)
}

// 監聽 modelValue prop 變化
defineExpose({
  isOpen
})
</script>

<style scoped>
v-divider {
  margin-bottom: 16px;
}
</style>
