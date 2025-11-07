<template>
  <span :class="badgeClasses">
    <slot name="icon" />
    <slot />
  </span>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  variant?: 'default' | 'primary' | 'secondary' | 'success' | 'warning' | 'error' | 'info'
  size?: 'sm' | 'md' | 'lg'
  rounded?: boolean
  outline?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  size: 'md',
  rounded: false,
  outline: false
})

const badgeClasses = computed(() => {
  const baseClasses = 'inline-flex items-center justify-center font-medium transition-colors'

  // Variant styles
  const variantClasses = {
    default: props.outline 
      ? 'bg-gray-100 text-gray-700 border border-gray-300'
      : 'bg-gray-100 text-gray-700',
    primary: props.outline
      ? 'bg-primary-50 text-primary-700 border border-primary-300'
      : 'bg-gradient-primary text-white shadow-glow',
    secondary: props.outline
      ? 'bg-secondary-50 text-secondary-700 border border-secondary-300'
      : 'bg-gradient-secondary text-white shadow-glow-pink',
    success: props.outline
      ? 'bg-success-50 text-success-700 border border-success-300'
      : 'bg-success text-white',
    warning: props.outline
      ? 'bg-warning-50 text-warning-700 border border-warning-300'
      : 'bg-warning text-white',
    error: props.outline
      ? 'bg-error-50 text-error-700 border border-error-300'
      : 'bg-error text-white',
    info: props.outline
      ? 'bg-info-50 text-info-700 border border-info-300'
      : 'bg-info text-white'
  }

  // Size styles
  const sizeClasses = {
    sm: 'px-2 py-0.5 text-xs gap-1',
    md: 'px-3 py-1 text-sm gap-1.5',
    lg: 'px-4 py-1.5 text-base gap-2'
  }

  // Rounded styles
  const roundedClasses = props.rounded ? 'rounded-full' : 'rounded-lg'

  return [
    baseClasses,
    variantClasses[props.variant],
    sizeClasses[props.size],
    roundedClasses
  ].join(' ')
})
</script>
