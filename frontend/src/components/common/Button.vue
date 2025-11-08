<template>
  <button
    :class="buttonClasses"
    :disabled="disabled || loading"
    @click="$emit('click', $event)"
  >
    <span v-if="loading" class="inline-block w-4 h-4 border-2 border-current border-t-transparent rounded-full animate-spin mr-2"></span>
    <slot></slot>
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps({
  variant: {
    type: String as () => 'primary' | 'secondary' | 'danger' | 'success' | 'outlined' | 'text',
    default: 'primary'
  },
  size: {
    type: String as () => 'small' | 'medium' | 'large',
    default: 'medium'
  },
  disabled: {
    type: Boolean,
    default: false
  },
  loading: {
    type: Boolean,
    default: false
  },
  block: {
    type: Boolean,
    default: false
  }
})

defineEmits(['click'])

const buttonClasses = computed(() => {
  // 基礎樣式 - 所有按鈕共用
  const base = [
    'inline-flex items-center justify-center',
    'font-medium',
    'rounded-lg',
    'transition-all duration-200',
    'focus:outline-none focus:ring-2 focus:ring-offset-2',
    'disabled:opacity-50 disabled:cursor-not-allowed disabled:shadow-none'
  ]

  // 尺寸樣式
  const sizeClasses = {
    small: 'text-sm px-3 py-1.5 min-h-[32px]',
    medium: 'text-base px-4 py-2.5 min-h-[40px]',
    large: 'text-lg px-6 py-3 min-h-[48px]'
  }

  // 變體樣式 - 應用專業的配色和陰影
  const variantClasses = {
    primary: [
      'bg-gradient-to-r from-blue-600 to-indigo-600',
      'text-white',
      'shadow-md hover:shadow-lg',
      'hover:from-blue-700 hover:to-indigo-700',
      'active:from-blue-800 active:to-indigo-800',
      'focus:ring-blue-500'
    ],
    secondary: [
      'bg-gradient-to-r from-gray-600 to-gray-700',
      'text-white',
      'shadow-md hover:shadow-lg',
      'hover:from-gray-700 hover:to-gray-800',
      'active:from-gray-800 active:to-gray-900',
      'focus:ring-gray-500'
    ],
    danger: [
      'bg-gradient-to-r from-red-600 to-red-700',
      'text-white',
      'shadow-md hover:shadow-lg',
      'hover:from-red-700 hover:to-red-800',
      'active:from-red-800 active:to-red-900',
      'focus:ring-red-500'
    ],
    success: [
      'bg-gradient-to-r from-green-600 to-emerald-600',
      'text-white',
      'shadow-md hover:shadow-lg',
      'hover:from-green-700 hover:to-emerald-700',
      'active:from-green-800 active:to-emerald-800',
      'focus:ring-green-500'
    ],
    outlined: [
      'bg-white',
      'text-gray-700',
      'border-2 border-gray-300',
      'shadow-sm hover:shadow',
      'hover:border-gray-400 hover:bg-gray-50',
      'active:border-gray-500 active:bg-gray-100',
      'focus:ring-gray-400'
    ],
    text: [
      'bg-transparent',
      'text-gray-700',
      'hover:bg-gray-100',
      'active:bg-gray-200',
      'focus:ring-gray-400'
    ]
  }

  // 區塊樣式
  const blockClass = props.block ? 'w-full' : ''

  return [
    ...base,
    sizeClasses[props.size],
    ...variantClasses[props.variant],
    blockClass
  ].join(' ')
})
</script>
