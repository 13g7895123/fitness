<template>
  <form @submit.prevent="submitForm" class="workout-record-form space-y-4">
    <div>
      <Input
        v-model="form.exerciseDate"
        type="date"
        :label="$t('workout.date')"
        :error="dateError"
        required
      />
    </div>

    <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
      <ExerciseTypeSelector
        v-model="form.exerciseTypeId"
        @select="handleExerciseSelect"
      />

      <EquipmentSelector
        v-model="form.equipmentId"
      />
    </div>

    <div>
      <Textarea
        v-model="form.notes"
        :label="$t('workout.notes')"
        :hint="$t('common.optional')"
        :error="notesError"
        counter
        :maxlength="500"
        :rows="3"
      />
    </div>

    <div class="flex items-center gap-2 mt-6">
      <Button
        type="submit"
        variant="primary"
        :loading="isSubmitting"
      >
        {{ submitButtonText }}
      </Button>
      <Button
        type="button"
        variant="outlined"
        @click="resetForm"
      >
        {{ $t('common.reset') }}
      </Button>
      <div class="flex-1"></div>
      <Button
        type="button"
        variant="text"
        @click="$emit('cancel')"
      >
        {{ $t('common.cancel') }}
      </Button>
    </div>
  </form>
</template>

<script setup lang="ts">
import { ref, computed, watch, defineProps, defineEmits, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import Button from '@/components/common/Button.vue'
import Input from '@/components/common/Input.vue'
import Textarea from '@/components/common/Textarea.vue'
import ExerciseTypeSelector from './ExerciseTypeSelector.vue'
import EquipmentSelector from './EquipmentSelector.vue'
import type { CreateWorkoutRecordDto, UpdateWorkoutRecordDto, WorkoutRecordResponseDto } from '@/types/workout'

const { t } = useI18n()

const props = defineProps({
  initialData: {
    type: Object as any,
    default: null
  },
  isEditing: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['submit', 'cancel'])

const form = ref<CreateWorkoutRecordDto>({
  exerciseDate: new Date().toISOString().split('T')[0],
  exerciseTypeId: '',
  equipmentId: undefined,
  notes: ''
})

const isSubmitting = ref(false)
const selectedExerciseName = ref('')

const submitButtonText = computed(() => {
  return props.isEditing ? t('common.edit') : t('workout.addRecord')
})

// 驗證錯誤訊息
const dateError = computed(() => {
  if (!form.value.exerciseDate) return t('validation.requiredField')
  if (new Date(form.value.exerciseDate) > new Date()) return t('validation.dateNotFuture')
  return ''
})

const notesError = computed(() => {
  if (form.value.notes && form.value.notes.length > 500) {
    return t('common.maxLength', { max: 500 })
  }
  return ''
})

const handleExerciseSelect = (exercise: any) => {
  selectedExerciseName.value = exercise?.name || ''
}

const submitForm = async () => {
  isSubmitting.value = true
  try {
    emit('submit', form.value)
  } finally {
    isSubmitting.value = false
  }
}

const resetForm = () => {
  form.value = {
    exerciseDate: new Date().toISOString().split('T')[0],
    exerciseTypeId: '',
    equipmentId: undefined,
    notes: ''
  }
  selectedExerciseName.value = ''
}

onMounted(() => {
  if (props.initialData && props.isEditing) {
    form.value = {
      exerciseDate: props.initialData.exerciseDate,
      exerciseTypeId: props.initialData.exerciseTypeId,
      equipmentId: props.initialData.equipmentId,
      notes: props.initialData.notes
    }
  }
})
</script>

<style scoped>
.workout-record-form {
  width: 100%;
}
</style>
