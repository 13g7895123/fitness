<template>
  <v-form @submit.prevent="submitForm" class="workout-record-form">
    <v-row>
      <v-col cols="12">
        <v-text-field
          v-model="form.date"
          type="date"
          :label="$t('workout.date')"
          :rules="dateRules"
          required
          variant="outlined"
          density="comfortable"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="6">
        <ExerciseTypeSelector
          v-model="form.exerciseTypeId"
          @select="handleExerciseSelect"
        />
      </v-col>

      <v-col cols="12" sm="6">
        <EquipmentSelector
          v-model="form.equipmentId"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="6">
        <v-text-field
          v-model.number="form.durationMinutes"
          type="number"
          :label="$t('workout.duration')"
          :hint="$t('workout.durationHint')"
          :rules="durationRules"
          min="1"
          max="480"
          required
          variant="outlined"
          density="comfortable"
        />
      </v-col>

      <v-col cols="12" sm="6">
        <v-text-field
          v-model.number="form.caloriesBurned"
          type="number"
          :label="$t('workout.calories')"
          :hint="$t('common.optional')"
          :rules="caloriesRules"
          min="1"
          max="5000"
          variant="outlined"
          density="comfortable"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="6">
        <v-text-field
          v-model.number="form.weight"
          type="number"
          :label="$t('workout.weight')"
          :hint="$t('common.optional')"
          :rules="weightRules"
          min="0.1"
          step="0.1"
          variant="outlined"
          density="comfortable"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <v-textarea
          v-model="form.notes"
          :label="$t('workout.notes')"
          :hint="$t('common.optional')"
          :rules="notesRules"
          counter
          maxlength="500"
          variant="outlined"
          density="comfortable"
          rows="3"
        />
      </v-col>
    </v-row>

    <v-row class="mt-4">
      <v-col cols="12" class="d-flex gap-2">
        <v-btn
          type="submit"
          color="primary"
          :loading="isSubmitting"
        >
          {{ submitButtonText }}
        </v-btn>
        <v-btn
          type="button"
          variant="outlined"
          @click="resetForm"
        >
          {{ $t('common.reset') }}
        </v-btn>
        <v-spacer />
        <v-btn
          type="button"
          variant="text"
          @click="$emit('cancel')"
        >
          {{ $t('common.cancel') }}
        </v-btn>
      </v-col>
    </v-row>
  </v-form>
</template>

<script setup lang="ts">
import { ref, computed, watch, defineProps, defineEmits, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
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

const form = ref<CreateWorkoutRecordDto | UpdateWorkoutRecordDto>({
  date: new Date().toISOString().split('T')[0],
  exerciseTypeId: '',
  equipmentId: undefined,
  durationMinutes: 30,
  caloriesBurned: undefined,
  weight: undefined,
  notes: ''
})

const isSubmitting = ref(false)
const selectedExerciseName = ref('')

const submitButtonText = computed(() => {
  return props.isEditing ? t('common.edit') : t('workout.addRecord')
})

// 驗證規則
const dateRules = computed(() => [
  (v: any) => !!v || t('validation.requiredField'),
  (v: any) => new Date(v) <= new Date() || t('validation.dateNotFuture')
])

const durationRules = computed(() => [
  (v: any) => !!v || t('validation.requiredField'),
  (v: any) => v >= 1 || t('workout.durationMinError'),
  (v: any) => v <= 480 || t('workout.durationMaxError')
])

const caloriesRules = computed(() => [
  (v: any) => !v || v >= 1 || t('workout.caloriesMinError'),
  (v: any) => !v || v <= 5000 || t('workout.caloriesMaxError')
])

const weightRules = computed(() => [
  (v: any) => !v || v > 0 || t('validation.weightPositive')
])

const notesRules = computed(() => [
  (v: any) => !v || v.length <= 500 || t('common.maxLength', { max: 500 })
])

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
    date: new Date().toISOString().split('T')[0],
    exerciseTypeId: '',
    equipmentId: undefined,
    durationMinutes: 30,
    caloriesBurned: undefined,
    weight: undefined,
    notes: ''
  }
  selectedExerciseName.value = ''
}

onMounted(() => {
  if (props.initialData && props.isEditing) {
    form.value = {
      date: props.initialData.date,
      exerciseTypeId: props.initialData.exerciseTypeId,
      equipmentId: props.initialData.equipmentId,
      durationMinutes: props.initialData.durationMinutes,
      caloriesBurned: props.initialData.caloriesBurned,
      weight: props.initialData.weight,
      notes: props.initialData.notes
    }
  }
})
</script>

<style scoped>
.workout-record-form {
  width: 100%;
}

.gap-2 {
  gap: 8px;
}
</style>
