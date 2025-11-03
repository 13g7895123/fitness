<template>
  <v-form @submit.prevent="handleSubmit">
    <div class="form-fields">
      <!-- 每週運動時間目標 -->
      <v-text-field
        v-model.number="form.WeeklyMinutesGoal"
        :label="$t('goals.weeklyMinutes')"
        type="number"
        min="0"
        max="10080"
        :hint="$t('validation.durationBetween')"
        outlined
        class="mb-3"
      />

      <!-- 每週卡路里目標 -->
      <v-text-field
        v-model.number="form.WeeklyCaloriesGoal"
        :label="$t('goals.weeklyCalories')"
        type="number"
        min="0"
        max="100000"
        hint="最多 100000 卡路里"
        outlined
        class="mb-3"
      />

      <!-- 開始日期 -->
      <v-text-field
        v-model="form.StartDate"
        :label="$t('goals.startDate')"
        type="date"
        outlined
        class="mb-3"
        required
      />

      <!-- 結束日期 -->
      <v-text-field
        v-model="form.EndDate"
        :label="$t('goals.endDate') + ' (' + $t('common.optional') + ')'"
        type="date"
        outlined
        class="mb-3"
      />

      <!-- 驗證錯誤 -->
      <v-alert v-if="validationError" type="error" class="mb-3">
        {{ validationError }}
      </v-alert>
    </div>

    <div class="form-actions">
      <v-btn variant="text" @click="$emit('cancel')">
        {{ $t('common.cancel') }}
      </v-btn>
      <v-btn color="primary" type="submit" :loading="isSubmitting">
        {{ $t('common.save') }}
      </v-btn>
    </div>
  </v-form>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useI18n } from 'vue-i18n'
import type { CreateWorkoutGoalDto } from '@/types/goals'

interface Props {
  initialData?: CreateWorkoutGoalDto
}

const props = withDefaults(defineProps<Props>(), {
  initialData: undefined
})

const emit = defineEmits<{
  save: [data: CreateWorkoutGoalDto]
  cancel: []
}>()

const { t } = useI18n()
const isSubmitting = ref(false)
const validationError = ref<string | null>(null)

const form = reactive<CreateWorkoutGoalDto>({
  WeeklyMinutesGoal: props.initialData?.WeeklyMinutesGoal,
  WeeklyCaloriesGoal: props.initialData?.WeeklyCaloriesGoal,
  StartDate: props.initialData?.StartDate || new Date().toISOString().split('T')[0],
  EndDate: props.initialData?.EndDate
})

const validateForm = (): boolean => {
  validationError.value = null

  // 至少設定一項目標
  if (!form.WeeklyMinutesGoal && !form.WeeklyCaloriesGoal) {
    validationError.value = t('validation.atLeastOne')
    return false
  }

  // 時間驗證
  if (form.WeeklyMinutesGoal && (form.WeeklyMinutesGoal <= 0 || form.WeeklyMinutesGoal > 10080)) {
    validationError.value = t('validation.durationBetween')
    return false
  }

  // 卡路里驗證
  if (form.WeeklyCaloriesGoal && (form.WeeklyCaloriesGoal <= 0 || form.WeeklyCaloriesGoal > 100000)) {
    validationError.value = '卡路里必須在 1 至 100000 之間'
    return false
  }

  // 開始日期驗證
  if (!form.StartDate) {
    validationError.value = t('validation.requiredField')
    return false
  }

  // 結束日期驗證
  if (form.EndDate && new Date(form.EndDate) <= new Date(form.StartDate)) {
    validationError.value = '結束日期必須晚於開始日期'
    return false
  }

  return true
}

const handleSubmit = async () => {
  if (!validateForm()) {
    return
  }

  isSubmitting.value = true
  try {
    emit('save', form)
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
.form-fields {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1.5rem;
}

.mb-3 {
  margin-bottom: 1rem;
}
</style>
