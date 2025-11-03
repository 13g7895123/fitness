<template>
  <v-dialog v-model="isOpen" width="600">
    <v-card>
      <v-card-title>
        {{ $t('workout.editRecord') }}
        <v-spacer />
        <v-btn icon size="small" @click="closeDialog">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </v-card-title>

      <v-divider />

      <v-card-text class="pa-6">
        <WorkoutRecordForm
          :initial-data="record"
          is-editing
          @submit="handleSubmit"
          @cancel="closeDialog"
        />
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits, computed } from 'vue'
import WorkoutRecordForm from './WorkoutRecordForm.vue'
import { useWorkoutService } from '@/services/workoutService'
import { useWorkoutsStore } from '@/stores/workouts'
import { useErrorHandler } from '@/utils/errorHandler'
import type { UpdateWorkoutRecordDto } from '@/types/workout'

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

const { updateRecord, isLoading } = useWorkoutService()
const workoutsStore = useWorkoutsStore()
const { showSuccess, showError } = useErrorHandler()

const isOpen = ref(props.modelValue)

const handleSubmit = async (formData: UpdateWorkoutRecordDto) => {
  try {
    const result = await updateRecord(props.record.id, formData)
    if (result) {
      workoutsStore.updateRecord(props.record.id, result)
      showSuccess('更新健身紀錄成功')
      closeDialog()
      emit('success')
    } else {
      showError('更新健身紀錄失敗')
    }
  } catch (error: any) {
    showError(error.message || '更新健身紀錄失敗')
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
  margin-bottom: 16px;
}
</style>
