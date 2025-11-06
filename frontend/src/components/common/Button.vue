<template>
  <button
    :class="buttonClasses"
    :disabled="disabled || loading"
    @click="$emit('click', $event)"
  >
    <span v-if="loading" class="inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></span>
    <slot></slot>
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps({
  variant: {
    type: String as () => 'primary' | 'secondary' | 'outlined' | 'text',
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
  const base = 'btn inline-flex items-center justify-center'
  const variants = {
    primary: 'btn-primary',
    secondary: 'btn-secondary',
    outlined: 'btn-outlined',
    text: 'btn-text'
  }
  const sizes = {
    small: 'text-sm px-3 py-1.5',
    medium: 'text-base px-4 py-2',
    large: 'text-lg px-6 py-3'
  }
  const block = props.block ? 'w-full' : ''
  const disabled = props.disabled || props.loading ? 'opacity-50 cursor-not-allowed' : ''
  
  return [base, variants[props.variant], sizes[props.size], block, disabled].join(' ')
})
</script>
