<template>
  <button
    :type="type"
    :disabled="disabled || loading"
    :class="buttonClasses"
    @click="$emit('click', $event)"
  >
    <!-- Loading Spinner -->
    <svg
      v-if="loading"
      class="animate-spin h-5 w-5 mr-2"
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      viewBox="0 0 24 24"
    >
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
    </svg>

    <!-- Icon (Left) -->
    <slot name="icon-left" />

    <!-- Content -->
    <span class="relative z-10 font-medium">
      <slot />
    </span>

    <!-- Icon (Right) -->
    <slot name="icon-right" />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  variant?: 'primary' | 'secondary' | 'accent' | 'success' | 'danger' | 'outline' | 'ghost'
  size?: 'sm' | 'md' | 'lg' | 'xl'
  fullWidth?: boolean
  disabled?: boolean
  loading?: boolean
  type?: 'button' | 'submit' | 'reset'
  rounded?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  size: 'md',
  fullWidth: false,
  disabled: false,
  loading: false,
  type: 'button',
  rounded: false
})

defineEmits<{
  click: [event: MouseEvent]
}>()

const buttonClasses = computed(() => {
  const baseClasses = [
    'inline-flex items-center justify-center font-medium transition-all duration-300',
    'focus:outline-none focus:ring-2 focus:ring-offset-2',
    'disabled:opacity-50 disabled:cursor-not-allowed',
    'relative overflow-hidden btn-hover'
  ]

  // Variant styles
  const variantClasses = {
    primary: 'bg-gradient-primary text-white shadow-medium hover:shadow-hard focus:ring-primary-500',
    secondary: 'bg-gradient-secondary text-white shadow-medium hover:shadow-hard focus:ring-secondary-500',
    accent: 'bg-gradient-accent text-white shadow-medium hover:shadow-hard focus:ring-accent-500',
    success: 'bg-gradient-success text-white shadow-medium hover:shadow-hard focus:ring-success',
    danger: 'bg-gradient-to-r from-error to-error-light text-white shadow-medium hover:shadow-hard focus:ring-error',
    outline: 'bg-white border-2 border-primary text-primary hover:bg-primary hover:text-white focus:ring-primary-500',
    ghost: 'bg-transparent text-gray-700 hover:bg-gray-100 focus:ring-gray-500'
  }

  // Size styles
  const sizeClasses = {
    sm: 'px-4 py-2 text-sm rounded-lg',
    md: 'px-6 py-3 text-base rounded-xl',
    lg: 'px-8 py-4 text-lg rounded-2xl',
    xl: 'px-10 py-5 text-xl rounded-2xl'
  }

  const classes = [
    ...baseClasses,
    variantClasses[props.variant],
    sizeClasses[props.size]
  ]

  if (props.fullWidth) {
    classes.push('w-full')
  }

  if (props.rounded) {
    classes.push('rounded-full')
  }

  return classes.join(' ')
})
</script>
