<template>
  <v-dialog v-model="isOpen" max-width="600">
    <v-card rounded="xl">
      <v-card-title class="d-flex align-center pa-6">
        <span class="text-h5 font-weight-bold">{{ $t('workout.addRecord') }}</span>
        <v-spacer />
        <v-btn 
          icon="mdi-close" 
          variant="text" 
          size="small" 
          @click="closeDialog"
        ></v-btn>
      </v-card-title>

      <v-divider></v-divider>

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
import { ref, watch, defineProps, defineEmits } from 'vue'
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
v-divider {
  margin-bottom: 16px;
}
</style>
