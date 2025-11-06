<template>
  <v-container fluid style="max-width: 1280px; margin: 0 auto;">
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 mb-4">設定</h1>

        <v-tabs v-model="activeTab">
          <v-tab value="exerciseTypes">運動類型</v-tab>
          <v-tab value="equipments">器材設備</v-tab>
        </v-tabs>

        <v-window v-model="activeTab" class="mt-4">
          <!-- 運動類型 Tab -->
          <v-window-item value="exerciseTypes">
            <v-row>
              <v-col cols="12">
                <ExerciseTypeSearchBar v-model="searchQuery" />
              </v-col>
            </v-row>

            <v-row class="mt-4">
              <v-col cols="12">
                <ExerciseTypeList
                  :exercise-types="filteredExerciseTypes"
                  @create="openExerciseTypeDialog('create')"
                  @view="openExerciseTypeDialog('view', $event)"
                  @edit="openExerciseTypeDialog('edit', $event)"
                  @delete="openDeleteDialog('exerciseType', $event)"
                />
              </v-col>
            </v-row>
          </v-window-item>

          <!-- 器材設備 Tab -->
          <v-window-item value="equipments">
            <v-row>
              <v-col cols="12">
                <EquipmentList
                  :equipments="exerciseTypesStore.customEquipments"
                  @create="openEquipmentDialog('create')"
                  @edit="openEquipmentDialog('edit', $event)"
                  @delete="openDeleteDialog('equipment', $event)"
                />
              </v-col>
            </v-row>
          </v-window-item>
        </v-window>
      </v-col>
    </v-row>

    <!-- 運動類型表單對話框 -->
    <v-dialog v-model="exerciseTypeDialogOpen" max-width="800">
      <ExerciseTypeForm
        v-if="exerciseTypeDialogOpen"
        :mode="exerciseTypeFormMode"
        :exercise-type="selectedExerciseType"
        :available-equipments="exerciseTypesStore.equipments"
        :loading="exerciseTypesStore.loading"
        @submit="handleExerciseTypeSubmit"
        @cancel="closeExerciseTypeDialog"
      />
    </v-dialog>

    <!-- 器材表單對話框 -->
    <v-dialog v-model="equipmentDialogOpen" max-width="600">
      <EquipmentForm
        v-if="equipmentDialogOpen"
        :mode="equipmentFormMode"
        :equipment="selectedEquipment"
        :loading="exerciseTypesStore.loading"
        @submit="handleEquipmentSubmit"
        @cancel="closeEquipmentDialog"
      />
    </v-dialog>

    <!-- 刪除確認對話框 -->
    <DeleteConfirmDialog
      v-model="deleteDialogOpen"
      :item-name="deleteItemName"
      :warning-message="deleteWarningMessage"
      :loading="exerciseTypesStore.loading"
      @confirm="handleDeleteConfirm"
    />

    <!-- 成功 Snackbar -->
    <v-snackbar v-model="successSnackbar" color="success" timeout="3000">
      {{ successMessage }}
    </v-snackbar>

    <!-- 錯誤 Snackbar -->
    <v-snackbar v-model="errorSnackbar" color="error" timeout="5000">
      {{ exerciseTypesStore.error }}
      <template v-slot:actions>
        <v-btn text @click="errorSnackbar = false">關閉</v-btn>
      </template>
    </v-snackbar>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useExerciseTypesStore } from '@/stores/exerciseTypes'
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
  exerciseTypesStore.setSearchQuery(searchQuery.value)
  return exerciseTypesStore.searchResults
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
