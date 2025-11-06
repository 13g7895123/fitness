<template>
  <div class="bg-white rounded-xl shadow-lg p-6 max-h-[90vh] overflow-y-auto">
    <h3 class="text-xl font-semibold text-gray-900 mb-6">
      {{ mode === 'create' ? '新增運動類型' : mode === 'edit' ? '編輯運動類型' : '運動類型詳情' }}
    </h3>

    <form @submit.prevent="handleSubmit" class="space-y-6">
      <!-- 運動名稱 -->
      <div>
        <label for="name" class="block text-sm font-medium text-gray-700 mb-2">
          運動名稱 <span class="text-red-500">*</span>
        </label>
        <input
          id="name"
          v-model="formData.name"
          type="text"
          :readonly="mode === 'view' || isSystemDefault"
          :class="[
            'block w-full px-4 py-3 border rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary sm:text-sm',
            errors.name ? 'border-red-300' : 'border-gray-300',
            (mode === 'view' || isSystemDefault) ? 'bg-gray-100 cursor-not-allowed' : 'bg-white'
          ]"
          placeholder="輸入運動名稱"
          @blur="validateField('name')"
        />
        <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
      </div>

      <!-- 描述 -->
      <div>
        <label for="description" class="block text-sm font-medium text-gray-700 mb-2">
          描述
        </label>
        <textarea
          id="description"
          v-model="formData.description"
          rows="3"
          :readonly="mode === 'view' || isSystemDefault"
          :class="[
            'block w-full px-4 py-3 border rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary sm:text-sm',
            errors.description ? 'border-red-300' : 'border-gray-300',
            (mode === 'view' || isSystemDefault) ? 'bg-gray-100 cursor-not-allowed' : 'bg-white'
          ]"
          placeholder="輸入運動描述（選填）"
          @blur="validateField('description')"
        ></textarea>
        <p v-if="errors.description" class="mt-1 text-sm text-red-600">{{ errors.description }}</p>
      </div>

      <!-- MET 值 -->
      <div>
        <label for="defaultMET" class="block text-sm font-medium text-gray-700 mb-2">
          預設 MET 值 <span class="text-red-500">*</span>
        </label>
        <input
          id="defaultMET"
          v-model.number="formData.defaultMET"
          type="number"
          step="0.1"
          min="1"
          max="20"
          :readonly="mode === 'view' || isSystemDefault"
          :class="[
            'block w-full px-4 py-3 border rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary sm:text-sm',
            errors.defaultMET ? 'border-red-300' : 'border-gray-300',
            (mode === 'view' || isSystemDefault) ? 'bg-gray-100 cursor-not-allowed' : 'bg-white'
          ]"
          @blur="validateField('defaultMET')"
        />
        <p class="mt-1 text-xs text-gray-500">代謝當量 (Metabolic Equivalent of Task)，範圍: 1-20</p>
        <p v-if="errors.defaultMET" class="mt-1 text-sm text-red-600">{{ errors.defaultMET }}</p>
      </div>

      <!-- 相關器材 -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          相關器材
        </label>
        <div class="border border-gray-300 rounded-lg p-3 space-y-2 max-h-60 overflow-y-auto">
          <label
            v-for="equipment in availableEquipments"
            :key="equipment.id"
            class="flex items-center space-x-3 p-2 hover:bg-gray-50 rounded cursor-pointer"
            :class="{ 'opacity-50 cursor-not-allowed': mode === 'view' || isSystemDefault }"
          >
            <input
              type="checkbox"
              :value="equipment.id"
              v-model="formData.equipmentIds"
              :disabled="mode === 'view' || isSystemDefault"
              class="h-4 w-4 text-primary focus:ring-primary border-gray-300 rounded"
            />
            <span class="text-sm text-gray-900">{{ equipment.name }}</span>
          </label>
          <p v-if="availableEquipments.length === 0" class="text-sm text-gray-500 text-center py-4">
            尚無可用器材
          </p>
        </div>
        <p class="mt-1 text-xs text-gray-500">選擇此運動類型可能使用的器材</p>
      </div>

      <!-- 已選擇的器材標籤 -->
      <div v-if="selectedEquipmentNames.length > 0" class="flex flex-wrap gap-2">
        <span
          v-for="name in selectedEquipmentNames"
          :key="name"
          class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-primary-100 text-primary-800"
        >
          {{ name }}
        </span>
      </div>

      <!-- 系統預設提示 -->
      <div v-if="isSystemDefault" class="rounded-lg bg-blue-50 p-4 flex">
        <svg class="h-5 w-5 text-blue-400 mt-0.5 mr-3 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
        </svg>
        <p class="text-sm text-blue-700">系統預設項目無法編輯或刪除</p>
      </div>

      <!-- 按鈕 -->
      <div class="flex justify-end gap-3 pt-4 border-t border-gray-200">
        <button
          type="button"
          @click="$emit('cancel')"
          class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary"
        >
          {{ mode === 'view' ? '關閉' : '取消' }}
        </button>
        <button
          v-if="mode !== 'view' && !isSystemDefault"
          type="submit"
          :disabled="!isValid || loading"
          class="px-6 py-2 text-sm font-medium text-white bg-primary border border-transparent rounded-lg hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary disabled:opacity-50 disabled:cursor-not-allowed inline-flex items-center"
        >
          <svg v-if="loading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          {{ mode === 'create' ? '建立' : '更新' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { reactive, computed, watch } from 'vue'
import type { ExerciseType, Equipment, CreateExerciseTypeDto, UpdateExerciseTypeDto } from '@/types/exercise-type'

const props = defineProps<{
  mode: 'create' | 'edit' | 'view'
  exerciseType?: ExerciseType
  availableEquipments: Equipment[]
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'submit', data: CreateExerciseTypeDto | UpdateExerciseTypeDto): void
  (e: 'cancel'): void
}>()

const isSystemDefault = computed(() => props.exerciseType?.isSystemDefault || false)

const formData = reactive({
  name: '',
  description: '',
  defaultMET: 5.0,
  equipmentIds: [] as number[]
})

const errors = reactive<Record<string, string>>({
  name: '',
  description: '',
  defaultMET: ''
})

const selectedEquipmentNames = computed(() => {
  return props.availableEquipments
    .filter(e => formData.equipmentIds.includes(e.id))
    .map(e => e.name)
})

watch(() => props.exerciseType, (newVal) => {
  if (newVal) {
    formData.name = newVal.name
    formData.description = newVal.description || ''
    formData.defaultMET = newVal.defaultMET
    formData.equipmentIds = newVal.equipments.map(e => e.id)
  }
}, { immediate: true })

const validateField = (field: string) => {
  if (field === 'name') {
    if (!formData.name) {
      errors.name = '此欄位為必填'
    } else if (formData.name.length > 100) {
      errors.name = '最多 100 個字元'
    } else {
      errors.name = ''
    }
  }
  
  if (field === 'description') {
    if (formData.description && formData.description.length > 500) {
      errors.description = '最多 500 個字元'
    } else {
      errors.description = ''
    }
  }
  
  if (field === 'defaultMET') {
    if (!formData.defaultMET) {
      errors.defaultMET = '此欄位為必填'
    } else if (formData.defaultMET < 1) {
      errors.defaultMET = '最小值為 1'
    } else if (formData.defaultMET > 20) {
      errors.defaultMET = '最大值為 20'
    } else {
      errors.defaultMET = ''
    }
  }
}

const isValid = computed(() => {
  return formData.name && 
         formData.name.length <= 100 && 
         (!formData.description || formData.description.length <= 500) &&
         formData.defaultMET >= 1 &&
         formData.defaultMET <= 20 &&
         !errors.name &&
         !errors.description &&
         !errors.defaultMET
})

const handleSubmit = async () => {
  // Validate all fields
  validateField('name')
  validateField('description')
  validateField('defaultMET')
  
  if (isValid.value) {
    emit('submit', {
      name: formData.name,
      description: formData.description || undefined,
      defaultMET: formData.defaultMET,
      equipmentIds: formData.equipmentIds
    })
  }
}
</script>
