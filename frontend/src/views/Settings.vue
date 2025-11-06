<template>
  <div class="min-h-screen bg-gray-50 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <h1 class="text-3xl font-bold text-gray-900 mb-6">設定</h1>

      <!-- Tabs -->
      <div class="border-b border-gray-200 mb-6">
        <nav class="-mb-px flex space-x-8">
          <button
            @click="activeTab = 'exerciseTypes'"
            :class="[
              activeTab === 'exerciseTypes'
                ? 'border-primary text-primary'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300',
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors'
            ]"
          >
            運動類型
          </button>
          <button
            @click="activeTab = 'equipments'"
            :class="[
              activeTab === 'equipments'
                ? 'border-primary text-primary'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300',
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors'
            ]"
          >
            器材設備
          </button>
        </nav>
      </div>

      <!-- Tab Content -->
      <div class="space-y-6">
        <!-- 運動類型 Tab -->
        <div v-if="activeTab === 'exerciseTypes'">
          <div class="mb-6">
            <ExerciseTypeSearchBar v-model="searchQuery" />
          </div>

          <ExerciseTypeList
            :exercise-types="filteredExerciseTypes"
            @create="openExerciseTypeDialog('create')"
            @view="openExerciseTypeDialog('view', $event)"
            @edit="openExerciseTypeDialog('edit', $event)"
            @delete="openDeleteDialog('exerciseType', $event)"
          />
        </div>

        <!-- 器材設備 Tab -->
        <div v-if="activeTab === 'equipments'">
          <EquipmentList
            :equipments="exerciseTypesStore.customEquipments"
            @create="openEquipmentDialog('create')"
            @edit="openEquipmentDialog('edit', $event)"
            @delete="openDeleteDialog('equipment', $event)"
          />
        </div>
      </div>
    </div>

    <!-- 運動類型表單對話框 -->
    <Modal v-model="exerciseTypeDialogOpen" max-width="4xl">
      <ExerciseTypeForm
        v-if="exerciseTypeDialogOpen"
        :mode="exerciseTypeFormMode"
        :exercise-type="selectedExerciseType"
        :available-equipments="exerciseTypesStore.equipments"
        :loading="exerciseTypesStore.loading"
        @submit="handleExerciseTypeSubmit"
        @cancel="closeExerciseTypeDialog"
      />
    </Modal>

    <!-- 器材表單對話框 -->
    <Modal v-model="equipmentDialogOpen" max-width="lg">
      <EquipmentForm
        v-if="equipmentDialogOpen"
        :mode="equipmentFormMode"
        :equipment="selectedEquipment"
        :loading="exerciseTypesStore.loading"
        @submit="handleEquipmentSubmit"
        @cancel="closeEquipmentDialog"
      />
    </Modal>

    <!-- 刪除確認對話框 -->
    <DeleteConfirmDialog
      v-model="deleteDialogOpen"
      :item-name="deleteItemName"
      :warning-message="deleteWarningMessage"
      :loading="exerciseTypesStore.loading"
      @confirm="handleDeleteConfirm"
    />

    <!-- Toast Notifications -->
    <Transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="translate-y-2 opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="translate-y-2 opacity-0"
    >
      <div v-if="successSnackbar" class="fixed bottom-4 right-4 z-50">
        <div class="bg-green-600 text-white px-6 py-3 rounded-lg shadow-lg flex items-center space-x-3">
          <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
          </svg>
          <span>{{ successMessage }}</span>
        </div>
      </div>
    </Transition>

    <Transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="translate-y-2 opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="translate-y-2 opacity-0"
    >
      <div v-if="errorSnackbar" class="fixed bottom-4 right-4 z-50">
        <div class="bg-red-600 text-white px-6 py-3 rounded-lg shadow-lg flex items-center justify-between space-x-3">
          <div class="flex items-center space-x-3">
            <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
            <span>{{ exerciseTypesStore.error }}</span>
          </div>
          <button @click="errorSnackbar = false" class="ml-4 hover:text-gray-200">
            <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useExerciseTypesStore } from '@/stores/exerciseTypes'
import Modal from '@/components/common/Modal.vue'
import ExerciseTypeList from '@/components/settings/ExerciseTypeList.vue'
import ExerciseTypeForm from '@/components/settings/ExerciseTypeForm.vue'
import ExerciseTypeSearchBar from '@/components/settings/ExerciseTypeSearchBar.vue'
import EquipmentList from '@/components/settings/EquipmentList.vue'
import EquipmentForm from '@/components/settings/EquipmentForm.vue'
import DeleteConfirmDialog from '@/components/settings/DeleteConfirmDialog.vue'
import type { ExerciseType, Equipment, CreateExerciseTypeDto, UpdateExerciseTypeDto, CreateEquipmentDto, UpdateEquipmentDto } from '@/types/exercise-type'

const exerciseTypesStore = useExerciseTypesStore()

const activeTab = ref<'exerciseTypes' | 'equipments'>('exerciseTypes')
const searchQuery = ref('')

// 運動類型對話框狀態
const exerciseTypeDialogOpen = ref(false)
const exerciseTypeFormMode = ref<'create' | 'edit' | 'view'>('create')
const selectedExerciseType = ref<ExerciseType | undefined>()

// 器材對話框狀態
const equipmentDialogOpen = ref(false)
const equipmentFormMode = ref<'create' | 'edit'>('create')
const selectedEquipment = ref<Equipment | undefined>()

// 刪除對話框狀態
const deleteDialogOpen = ref(false)
const deleteItemType = ref<'exerciseType' | 'equipment'>('exerciseType')
const deleteItem = ref<ExerciseType | Equipment | null>(null)

// 通知狀態
const successSnackbar = ref(false)
const successMessage = ref('')
const errorSnackbar = ref(false)

// 計算屬性
const filteredExerciseTypes = computed(() => {
  if (!searchQuery.value) {
    return exerciseTypesStore.customExerciseTypes
  }
  const query = searchQuery.value.toLowerCase()
  return exerciseTypesStore.customExerciseTypes.filter(e => 
    e.name.toLowerCase().includes(query)
  )
})

const deleteItemName = computed(() => {
  return deleteItem.value ? deleteItem.value.name : ''
})

const deleteWarningMessage = computed(() => {
  if (deleteItemType.value === 'exerciseType' && deleteItem.value) {
    const exerciseType = deleteItem.value as ExerciseType
    if (exerciseType.workoutRecordCount > 0) {
      return `此運動類型已被使用 ${exerciseType.workoutRecordCount} 次，無法刪除`
    }
  }
  return undefined
})

// 生命週期
onMounted(async () => {
  await exerciseTypesStore.fetchExerciseTypes()
  await exerciseTypesStore.fetchEquipments()
})

// 監聽錯誤
watch(() => exerciseTypesStore.error, (error) => {
  if (error) {
    errorSnackbar.value = true
    setTimeout(() => {
      errorSnackbar.value = false
    }, 5000)
  }
})

// 自動關閉成功提示
watch(successSnackbar, (value) => {
  if (value) {
    setTimeout(() => {
      successSnackbar.value = false
    }, 3000)
  }
})

// 運動類型對話框方法
const openExerciseTypeDialog = (mode: 'create' | 'edit' | 'view', exerciseType?: ExerciseType) => {
  exerciseTypeFormMode.value = mode
  selectedExerciseType.value = exerciseType
  exerciseTypeDialogOpen.value = true
}

const closeExerciseTypeDialog = () => {
  exerciseTypeDialogOpen.value = false
  selectedExerciseType.value = undefined
  exerciseTypesStore.clearError()
}

const handleExerciseTypeSubmit = async (data: CreateExerciseTypeDto | UpdateExerciseTypeDto) => {
  try {
    if (exerciseTypeFormMode.value === 'create') {
      await exerciseTypesStore.createExerciseType(data as CreateExerciseTypeDto)
      successMessage.value = '運動類型建立成功'
    } else if (exerciseTypeFormMode.value === 'edit' && selectedExerciseType.value) {
      await exerciseTypesStore.updateExerciseType(selectedExerciseType.value.id, data as UpdateExerciseTypeDto)
      successMessage.value = '運動類型更新成功'
    }
    successSnackbar.value = true
    closeExerciseTypeDialog()
  } catch (error) {
    // 錯誤已由 store 處理
  }
}

// 器材對話框方法
const openEquipmentDialog = (mode: 'create' | 'edit', equipment?: Equipment) => {
  equipmentFormMode.value = mode
  selectedEquipment.value = equipment
  equipmentDialogOpen.value = true
}

const closeEquipmentDialog = () => {
  equipmentDialogOpen.value = false
  selectedEquipment.value = undefined
  exerciseTypesStore.clearError()
}

const handleEquipmentSubmit = async (data: CreateEquipmentDto | UpdateEquipmentDto) => {
  try {
    if (equipmentFormMode.value === 'create') {
      await exerciseTypesStore.createEquipment(data as CreateEquipmentDto)
      successMessage.value = '器材建立成功'
    } else if (equipmentFormMode.value === 'edit' && selectedEquipment.value) {
      await exerciseTypesStore.updateEquipment(selectedEquipment.value.id, data as UpdateEquipmentDto)
      successMessage.value = '器材更新成功'
    }
    successSnackbar.value = true
    closeEquipmentDialog()
  } catch (error) {
    // 錯誤已由 store 處理
  }
}

// 刪除對話框方法
const openDeleteDialog = (type: 'exerciseType' | 'equipment', item: ExerciseType | Equipment) => {
  deleteItemType.value = type
  deleteItem.value = item
  deleteDialogOpen.value = true
}

const handleDeleteConfirm = async () => {
  if (!deleteItem.value) return

  try {
    if (deleteItemType.value === 'exerciseType') {
      await exerciseTypesStore.deleteExerciseType(deleteItem.value.id)
      successMessage.value = '運動類型刪除成功'
    } else {
      await exerciseTypesStore.deleteEquipment(deleteItem.value.id)
      successMessage.value = '器材刪除成功'
    }
    successSnackbar.value = true
    deleteDialogOpen.value = false
    deleteItem.value = null
  } catch (error) {
    // 錯誤已由 store 處理
  }
}
</script>
