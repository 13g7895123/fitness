<template>
  <div class="workout-set-list">
    <!-- 標題和新增按鈕 -->
    <div class="flex items-center justify-between mb-4">
      <h3 class="text-lg font-bold text-gray-900">訓練組數 <span class="text-xs text-gray-500">(重量單位: lbs)</span></h3>
      <button
        @click="onAddSet"
        class="p-2 rounded-lg bg-gradient-primary text-white hover:shadow-glow transition-all"
        title="新增組數"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
        </svg>
      </button>
    </div>

    <!-- 組數列表 -->
    <div v-if="sets.length > 0" class="space-y-2">
      <div
        v-for="set in sets"
        :key="set.id"
        class="flex items-center justify-between glass rounded-xl p-3"
      >
        <div class="flex items-center space-x-3">
          <div class="flex items-center justify-center w-8 h-8 rounded-lg bg-gradient-primary text-white font-bold text-sm">
            {{ set.setNumber }}
          </div>
          <div>
            <div class="font-semibold text-gray-900">
              {{ set.repetitions }} 次
              <span v-if="set.weight" class="text-gray-600 text-sm ml-2">
                {{ set.weight }} lbs
              </span>
            </div>
            <div v-if="set.notes" class="text-xs text-gray-500">
              {{ set.notes }}
            </div>
          </div>
        </div>
        <div class="flex space-x-1">
          <button
            @click="onEditSet(set)"
            class="p-2 rounded-lg hover:bg-gray-100 transition-colors"
            title="編輯"
          >
            <svg class="w-4 h-4 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
            </svg>
          </button>
          <button
            @click="onDeleteSet(set)"
            class="p-2 rounded-lg hover:bg-red-50 transition-colors"
            title="刪除"
          >
            <svg class="w-4 h-4 text-error" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- 空狀態 -->
    <div v-else class="text-center py-8 glass rounded-xl">
      <svg class="w-12 h-12 text-gray-400 mx-auto mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
      </svg>
      <p class="text-gray-500 text-sm">尚未記錄組數</p>
      <p class="text-gray-400 text-xs mt-1">點擊上方 + 按鈕新增第一組</p>
    </div>

    <!-- 新增/編輯對話框 -->
    <AddWorkoutSetDialog
      v-model="showDialog"
      :workout-record-id="workoutRecordId"
      :initial-data="editingSet"
      :is-editing="isEditing"
      @success="onSetChanged"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits, onMounted } from 'vue'
import { useWorkoutSetService } from '@/services/workoutSetService'
import { useErrorHandler } from '@/utils/errorHandler'
import AddWorkoutSetDialog from './AddWorkoutSetDialog.vue'
import type { WorkoutSetDto } from '@/types/workout-set'

const props = defineProps<{
  workoutRecordId: number
}>()

const emit = defineEmits(['changed'])

const { getSetsByWorkoutRecord, deleteSet } = useWorkoutSetService()
const { showSuccess, showError } = useErrorHandler()

const sets = ref<WorkoutSetDto[]>([])
const showDialog = ref(false)
const editingSet = ref<WorkoutSetDto | null>(null)
const isEditing = ref(false)

const loadSets = async () => {
  sets.value = await getSetsByWorkoutRecord(props.workoutRecordId)
}

const onAddSet = () => {
  editingSet.value = null
  isEditing.value = false
  showDialog.value = true
}

const onEditSet = (set: WorkoutSetDto) => {
  editingSet.value = set
  isEditing.value = true
  showDialog.value = true
}

const onDeleteSet = async (set: WorkoutSetDto) => {
  if (!confirm(`確定要刪除第 ${set.setNumber} 組嗎？`)) {
    return
  }

  const success = await deleteSet(set.id)
  if (success) {
    showSuccess('組數刪除成功')
    await loadSets()
    emit('changed')
  } else {
    showError('組數刪除失敗')
  }
}

const onSetChanged = async () => {
  await loadSets()
  emit('changed')
}

onMounted(() => {
  console.log('[WorkoutSetList] Component mounted, workoutRecordId:', props.workoutRecordId)
  loadSets()
})
</script>

<style scoped>
.workout-set-list {
  width: 100%;
}
</style>
