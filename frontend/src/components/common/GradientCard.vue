<template>
  <div 
    :class="[
      'glass rounded-3xl p-6 transition-all duration-300 relative overflow-hidden',
      hover && 'card-hover cursor-pointer',
      gradient && 'border-0',
      className
    ]"
    :style="gradient && { background: gradientStyle }"
    @click="$emit('click', $event)"
  >
    <!-- 背景裝飾 -->
    <div v-if="gradient" class="absolute inset-0 opacity-10 pointer-events-none">
      <div class="absolute -top-24 -right-24 w-48 h-48 bg-white rounded-full blur-3xl"></div>
      <div class="absolute -bottom-24 -left-24 w-48 h-48 bg-white rounded-full blur-3xl"></div>
    </div>

    <!-- 內容 -->
    <div class="relative z-10">
      <!-- Header -->
      <div v-if="title || $slots.header" class="mb-4 flex items-center justify-between">
        <div>
          <h3 v-if="title" :class="['text-lg font-bold', gradient ? 'text-white' : 'text-gray-900']">
            {{ title }}
          </h3>
          <p v-if="subtitle" :class="['text-sm mt-1', gradient ? 'text-white/80' : 'text-gray-600']">
            {{ subtitle }}
          </p>
        </div>
        <slot name="header-action" />
      </div>

      <!-- Body -->
      <div>
        <slot />
      </div>

      <!-- Footer -->
      <div v-if="$slots.footer" class="mt-4 pt-4 border-t" :class="gradient ? 'border-white/20' : 'border-gray-200'">
        <slot name="footer" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  title?: string
  subtitle?: string
  hover?: boolean
  gradient?: 'primary' | 'secondary' | 'accent' | 'success' | 'warm' | 'cool' | null
  className?: string
}

const props = withDefaults(defineProps<Props>(), {
  title: '',
  subtitle: '',
  hover: false,
  gradient: null,
  className: ''
})

defineEmits<{
  click: [event: MouseEvent]
}>()

const gradientStyle = computed(() => {
  const gradients = {
    primary: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    secondary: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    accent: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
    success: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
    warm: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
    cool: 'linear-gradient(135deg, #30cfd0 0%, #330867 100%)',
  }
  return props.gradient ? gradients[props.gradient] : ''
})
</script>
