<template>
  <div class="w-full">
    <label v-if="label" :for="id" class="block text-sm font-semibold text-gray-700 mb-2">
      {{ label }}
      <span v-if="required" class="text-red-500 ml-0.5">*</span>
    </label>
    <div class="relative">
      <select
        :id="id"
        :value="modelValue"
        :disabled="disabled"
        :required="required"
        :class="selectClasses"
        @change="$emit('update:modelValue', ($event.target as HTMLSelectElement).value)"
      >
        <option v-if="placeholder" value="" disabled>{{ placeholder }}</option>
        <option
          v-for="item in items"
          :key="itemValue ? item[itemValue] : item"
          :value="itemValue ? item[itemValue] : item"
        >
          {{ itemTitle ? item[itemTitle] : item }}
        </option>
      </select>
      <!-- 下拉箭頭圖標 -->
      <div class="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
        <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
        </svg>
      </div>
      <!-- 錯誤圖標（當有錯誤時顯示在箭頭左側） -->
      <div v-if="error" class="absolute inset-y-0 right-10 flex items-center pr-3 pointer-events-none">
        <svg class="h-5 w-5 text-red-500" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd"/>
        </svg>
      </div>
    </div>
    <p v-if="error" class="mt-1.5 text-sm text-red-600 flex items-center">
      <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
      </svg>
      {{ error }}
    </p>
    <p v-else-if="hint" class="mt-1.5 text-sm text-gray-500 flex items-center">
      <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
      </svg>
      {{ hint }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps({
  modelValue: {
    type: [String, Number],
    default: ''
  },
  label: {
    type: String,
    default: ''
  },
  placeholder: {
    type: String,
    default: ''
  },
  hint: {
    type: String,
    default: ''
  },
  error: {
    type: String,
    default: ''
  },
  disabled: {
    type: Boolean,
    default: false
  },
  required: {
    type: Boolean,
    default: false
  },
  items: {
    type: Array as () => any[],
    default: () => []
  },
  itemTitle: {
    type: String,
    default: ''
  },
  itemValue: {
    type: String,
    default: ''
  },
  id: {
    type: String,
    default: () => `select-${Math.random().toString(36).substr(2, 9)}`
  }
})

defineEmits(['update:modelValue'])

const selectClasses = computed(() => {
  const baseClasses = [
    'w-full',
    'px-4 py-2.5',
    'pr-10', // 為下拉箭頭留空間
    'text-base',
    'bg-white',
    'border-2',
    'rounded-lg',
    'appearance-none', // 移除預設樣式
    'transition-all duration-200',
    'focus:outline-none',
    'cursor-pointer'
  ]

  const stateClasses = props.error
    ? [
        'border-red-300',
        'text-red-900',
        'focus:border-red-500',
        'pr-20' // 錯誤時需要更多空間（錯誤圖標 + 下拉箭頭）
      ]
    : [
        'border-gray-300',
        'text-gray-900',
        'hover:border-gray-400',
        'focus:border-blue-500'
      ]

  const disabledClasses = props.disabled
    ? ['bg-gray-50', 'text-gray-500', 'cursor-not-allowed', 'border-gray-200']
    : []

  return [...baseClasses, ...stateClasses, ...disabledClasses].join(' ')
})
</script>
