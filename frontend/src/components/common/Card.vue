<template>
  <div :class="cardClasses">
    <div v-if="$slots.header || title" class="px-6 py-4 border-b border-gray-200">
      <slot name="header">
        <h3 class="text-lg font-bold">{{ title }}</h3>
      </slot>
    </div>
    <div :class="bodyClasses">
      <slot></slot>
    </div>
    <div v-if="$slots.footer" class="px-6 py-4 border-t border-gray-200">
      <slot name="footer"></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps({
  title: {
    type: String,
    default: ''
  },
  noPadding: {
    type: Boolean,
    default: false
  },
  hoverable: {
    type: Boolean,
    default: false
  }
})

const cardClasses = computed(() => {
  const base = 'card'
  const hoverable = props.hoverable ? 'cursor-pointer' : ''
  return [base, hoverable].filter(Boolean).join(' ')
})

const bodyClasses = computed(() => {
  return props.noPadding ? '' : 'p-6'
})
</script>
