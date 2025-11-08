<template>
  <div class="bg-white rounded-xl shadow-lg p-6">
    <h3 class="text-xl font-semibold text-gray-900 mb-6">
      {{ mode === 'create' ? '新增器材' : '編輯器材' }}
    </h3>

    <form @submit.prevent="handleSubmit" class="space-y-6">
      <!-- 器材名稱 -->
      <div>
        <label for="name" class="block text-sm font-medium text-gray-700 mb-2">
          器材名稱 <span class="text-red-500">*</span>
        </label>
        <input
          id="name"
          v-model="formData.name"
          type="text"
          :readonly="isSystemDefault"
          :class="[
            'block w-full px-4 py-3 border rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary sm:text-sm',
            errors.name ? 'border-red-300' : 'border-gray-300',
            isSystemDefault ? 'bg-gray-100 cursor-not-allowed' : 'bg-white'
          ]"
          placeholder="輸入器材名稱"
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
          :readonly="isSystemDefault"
          :class="[
            'block w-full px-4 py-3 border rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary sm:text-sm',
            errors.description ? 'border-red-300' : 'border-gray-300',
            isSystemDefault ? 'bg-gray-100 cursor-not-allowed' : 'bg-white'
          ]"
          placeholder="輸入器材描述（選填）"
          @blur="validateField('description')"
        ></textarea>
        <p v-if="errors.description" class="mt-1 text-sm text-red-600">{{ errors.description }}</p>
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
        <Button
          type="button"
          variant="outlined"
          size="medium"
          @click="$emit('cancel')"
        >
          取消
        </Button>
        <Button
          v-if="!isSystemDefault"
          type="submit"
          variant="primary"
          size="medium"
          :disabled="!isValid"
          :loading="loading"
        >
          {{ mode === 'create' ? '建立' : '更新' }}
        </Button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { reactive, computed, watch } from 'vue'
import type { Equipment, CreateEquipmentDto, UpdateEquipmentDto } from '@/types/exercise-type'
import Button from '@/components/common/Button.vue'

const props = defineProps<{
  mode: 'create' | 'edit'
  equipment?: Equipment
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'submit', data: CreateEquipmentDto | UpdateEquipmentDto): void
  (e: 'cancel'): void
}>()

const isSystemDefault = computed(() => props.equipment?.isSystemDefault || false)

const formData = reactive({
  name: '',
  description: ''
})

const errors = reactive<Record<string, string>>({
  name: '',
  description: ''
})

watch(() => props.equipment, (newVal) => {
  if (newVal) {
    formData.name = newVal.name
    formData.description = newVal.description || ''
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
}

const isValid = computed(() => {
  return formData.name && 
         formData.name.length <= 100 && 
         (!formData.description || formData.description.length <= 500) &&
         !errors.name &&
         !errors.description
})

const handleSubmit = async () => {
  // Validate all fields
  validateField('name')
  validateField('description')
  
  if (isValid.value) {
    emit('submit', {
      name: formData.name,
      description: formData.description || undefined
    })
  }
}
</script>
