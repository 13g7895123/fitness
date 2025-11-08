<template>
  <div v-if="visible" class="fixed inset-0 bg-white/10 backdrop-blur-sm flex items-center justify-center z-[9999]">
    <div class="flex flex-col items-center gap-4">
      <div
        :class="spinnerClasses"
        class="border-primary border-t-transparent rounded-full animate-spin"
      ></div>
      <p v-if="text" class="text-white text-sm">{{ text }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  visible: boolean
  text?: string
  size?: number
}

const props = withDefaults(defineProps<Props>(), {
  visible: false,
  text: undefined,
  size: 60
})

const spinnerClasses = computed(() => {
  const sizeMap: Record<number, string> = {
    40: 'w-10 h-10 border-2',
    60: 'w-16 h-16 border-4',
    80: 'w-20 h-20 border-[5px]'
  }
  return sizeMap[props.size] || 'w-16 h-16 border-4'
})
</script>

<style scoped>
/* Tailwind handles all styling */
</style>
