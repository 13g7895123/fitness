<template>
  <div :class="cardClasses">
    <!-- 背景裝飾 -->
    <div v-if="gradient" class="absolute inset-0 opacity-10 pointer-events-none">
      <div class="absolute top-0 right-0 w-32 h-32 bg-white rounded-full blur-2xl transform translate-x-1/2 -translate-y-1/2"></div>
    </div>

    <div class="relative z-10">
      <!-- Icon -->
      <div v-if="icon || $slots.icon" class="mb-4">
        <div :class="iconWrapperClasses">
          <slot name="icon">
            <component :is="icon" class="w-6 h-6" />
          </slot>
        </div>
      </div>

      <!-- Value -->
      <div class="mb-2">
        <div :class="['text-3xl md:text-4xl font-bold', gradient ? 'text-white' : 'text-gray-900']">
          {{ value }}
          <span v-if="unit" :class="['text-lg font-medium ml-1', gradient ? 'text-white/80' : 'text-gray-600']">
            {{ unit }}
          </span>
        </div>
      </div>

      <!-- Label -->
      <div :class="['text-sm font-medium', gradient ? 'text-white/90' : 'text-gray-600']">
        {{ label }}
      </div>

      <!-- Trend -->
      <div v-if="trend !== undefined" class="mt-3 flex items-center space-x-1">
        <svg 
          v-if="trend > 0"
          class="w-4 h-4 text-success"
          fill="none" 
          stroke="currentColor" 
          viewBox="0 0 24 24"
        >
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
        </svg>
        <svg 
          v-else-if="trend < 0"
          class="w-4 h-4 text-error"
          fill="none" 
          stroke="currentColor" 
          viewBox="0 0 24 24"
        >
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 17h8m0 0V9m0 8l-8-8-4 4-6-6"/>
        </svg>
        <span :class="[
          'text-xs font-semibold',
          trend > 0 ? 'text-success' : trend < 0 ? 'text-error' : 'text-gray-500'
        ]">
          {{ trend > 0 ? '+' : '' }}{{ trend }}%
        </span>
        <span :class="['text-xs', gradient ? 'text-white/70' : 'text-gray-500']">
          vs 上週
        </span>
      </div>

      <!-- Extra Info -->
      <div v-if="$slots.extra" class="mt-3">
        <slot name="extra" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  value: string | number
  label: string
  unit?: string
  trend?: number
  icon?: any
  gradient?: 'primary' | 'secondary' | 'accent' | 'success' | 'warm' | null
  hover?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  unit: '',
  trend: undefined,
  icon: undefined,
  gradient: null,
  hover: true
})

const cardClasses = computed(() => {
  const classes = [
    'relative rounded-3xl p-6 transition-all duration-300 overflow-hidden'
  ]

  if (props.gradient) {
    const gradients = {
      primary: 'bg-gradient-primary text-white shadow-glow',
      secondary: 'bg-gradient-secondary text-white shadow-glow-pink',
      accent: 'bg-gradient-accent text-white shadow-medium',
      success: 'bg-gradient-success text-white shadow-medium',
      warm: 'bg-gradient-warm text-white shadow-medium',
    }
    classes.push(gradients[props.gradient] || gradients.primary)
  } else {
    classes.push('glass shadow-soft')
  }

  if (props.hover) {
    classes.push('card-hover')
  }

  return classes.join(' ')
})

const iconWrapperClasses = computed(() => {
  const classes = [
    'inline-flex items-center justify-center w-12 h-12 rounded-2xl'
  ]

  if (props.gradient) {
    classes.push('bg-white/20 backdrop-blur-sm text-white')
  } else {
    classes.push('bg-gradient-primary text-white shadow-glow')
  }

  return classes.join(' ')
})
</script>
