<template>
  <form @submit.prevent="handleSubmit" class="space-y-4">
    <!-- 每週運動時間目標 -->
    <Input
      v-model.number="form.WeeklyMinutesGoal"
      :label="$t('goals.weeklyMinutes')"
      type="number"
      :hint="$t('validation.durationBetween')"
      min="0"
      max="10080"
    />

    <!-- 每週卡路里目標 -->
    <Input
      v-model.number="form.WeeklyCaloriesGoal"
      :label="$t('goals.weeklyCalories')"
      type="number"
      hint="最多 100000 卡路里"
      min="0"
      max="100000"
    />

    <!-- 開始日期 -->
    <Input
      v-model="form.StartDate"
      :label="$t('goals.startDate')"
      type="date"
      required
    />

    <!-- 結束日期 -->
    <Input
      v-model="form.EndDate"
      :label="$t('goals.endDate') + ' (' + $t('common.optional') + ')'"
      type="date"
    />

    <!-- 驗證錯誤 -->
    <Alert
      v-if="validationError"
      type="error"
      :message="validationError"
      :visible="!!validationError"
    />

    <div class="flex justify-end gap-2 pt-4">
      <Button variant="text" @click="$emit('cancel')">
        {{ $t('common.cancel') }}
      </Button>
      <Button variant="primary" type="submit" :loading="isSubmitting">
        {{ $t('common.save') }}
      </Button>
    </div>
  </form>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useI18n } from 'vue-i18n'
import Input from '@/components/common/Input.vue'
import Button from '@/components/common/Button.vue'
import Alert from '@/components/common/Alert.vue'
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
/* Tailwind handles all styling */
</style>
