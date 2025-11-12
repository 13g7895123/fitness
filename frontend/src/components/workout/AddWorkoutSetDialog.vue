<template>
  <Dialog
    v-model="isOpen"
    :title="isEditing ? '編輯組數' : '新增組數'"
    max-width="sm"
    @close="closeDialog"
  >
    <form @submit.prevent="handleSubmit" class="space-y-4">
      <div class="grid grid-cols-2 gap-4">
        <Input
          v-model="form.repetitions"
          type="number"
          label="次數"
          placeholder="例如: 12"
          required
          min="1"
        />
        <Input
          v-model="form.weight"
          type="number"
          label="重量 (lbs)"
          placeholder="選填"
          min="0"
          step="0.5"
        />
      </div>

      <Textarea
        v-model="form.notes"
        label="備註"
        placeholder="記錄這組的感受或注意事項..."
        :rows="3"
        :maxlength="200"
        counter
      />

      <!-- 上次記錄提示 -->
      <div v-if="lastSet && !isEditing" class="glass rounded-lg p-3 flex items-start space-x-2">
        <svg class="w-5 h-5 text-blue-500 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
        </svg>
        <div class="flex-1">
          <p class="text-sm text-gray-700">上次記錄</p>
          <p class="text-xs text-gray-500">
            {{ lastSet.repetitions }} 次
            <span v-if="lastSet.weight"> · {{ lastSet.weight }} lbs</span>
          </p>
          <button
            type="button"
            @click="useLastSetData"
            class="text-xs text-primary hover:underline mt-1"
          >
            使用上次資料
          </button>
        </div>
      </div>
    </form>

    <template #footer>
      <div class="flex justify-end gap-2">
        <Button
          variant="text"
          @click="closeDialog"
        >
          取消
        </Button>
        <Button
          variant="primary"
          @click="handleSubmit"
          :loading="isSubmitting"
        >
          {{ isEditing ? '更新' : '新增' }}
        </Button>
      </div>
    </template>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, watch, defineProps, defineEmits, onMounted } from 'vue'
import Dialog from '@/components/common/Dialog.vue'
import Button from '@/components/common/Button.vue'
import Input from '@/components/common/Input.vue'
import Textarea from '@/components/common/Textarea.vue'
import { useWorkoutSetService } from '@/services/workoutSetService'
import { useErrorHandler } from '@/utils/errorHandler'
import type { WorkoutSetDto, CreateWorkoutSetDto, UpdateWorkoutSetDto } from '@/types/workout-set'

const props = defineProps<{
  modelValue: boolean
  workoutRecordId: number
  initialData?: WorkoutSetDto | null
  isEditing?: boolean
}>()

const emit = defineEmits(['update:modelValue', 'success'])

const { createSet, updateSet, getLastSet, isLoading } = useWorkoutSetService()
const { showSuccess, showError } = useErrorHandler()

const isOpen = ref(props.modelValue)
const isSubmitting = ref(false)
const lastSet = ref<WorkoutSetDto | null>(null)

interface FormData {
  repetitions: string
  weight: string
  notes: string
}

const form = ref<FormData>({
  repetitions: '',
  weight: '',
  notes: ''
})

// 同步 props 變化
watch(() => props.modelValue, (newVal) => {
  isOpen.value = newVal
  if (newVal) {
    if (props.initialData && props.isEditing) {
      // 編輯模式
      form.value = {
        repetitions: String(props.initialData.repetitions),
        weight: props.initialData.weight ? String(props.initialData.weight) : '',
        notes: props.initialData.notes || ''
      }
    } else {
      // 新增模式
      resetForm()
      loadLastSet()
    }
  }
})

// 同步 isOpen 變化
watch(isOpen, (newVal) => {
  emit('update:modelValue', newVal)
})

const loadLastSet = async () => {
  lastSet.value = await getLastSet(props.workoutRecordId)
}

const useLastSetData = () => {
  if (lastSet.value) {
    form.value.repetitions = String(lastSet.value.repetitions)
    form.value.weight = lastSet.value.weight ? String(lastSet.value.weight) : ''
  }
}

const handleSubmit = async () => {
  isSubmitting.value = true
  try {
    if (props.isEditing && props.initialData) {
      // 更新
      const updateData: UpdateWorkoutSetDto = {
        repetitions: Number(form.value.repetitions),
        weight: form.value.weight ? Number(form.value.weight) : undefined,
        notes: form.value.notes || undefined
      }
      const result = await updateSet(props.initialData.id, updateData)
      if (result) {
        showSuccess('組數更新成功')
        closeDialog()
        emit('success')
      } else {
        showError('組數更新失敗')
      }
    } else {
      // 新增
      const createData: CreateWorkoutSetDto = {
        workoutRecordId: props.workoutRecordId,
        repetitions: Number(form.value.repetitions),
        weight: form.value.weight ? Number(form.value.weight) : undefined,
        notes: form.value.notes || undefined
      }
      const result = await createSet(createData)
      if (result) {
        showSuccess('組數新增成功')
        resetForm()
        emit('success')
        // 不關閉對話框，方便連續新增
        // closeDialog()
      } else {
        showError('組數新增失敗')
      }
    }
  } finally {
    isSubmitting.value = false
  }
}

const resetForm = () => {
  form.value = {
    repetitions: '',
    weight: '',
    notes: ''
  }
}

const closeDialog = () => {
  isOpen.value = false
  resetForm()
}
</script>

<style scoped>
/* Styles handled by Tailwind */
</style>
