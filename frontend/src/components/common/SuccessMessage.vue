<template>
  <transition name="slide-fade">
    <div
      v-if="visible"
      class="success-message"
      :class="`success-${type}`"
    >
      <div class="success-content">
        <v-icon class="success-icon">{{ icon }}</v-icon>
        <div class="success-text">
          <p v-if="title" class="success-title">{{ title }}</p>
          <p class="success-body">{{ message }}</p>
        </div>
        <v-btn
          icon="mdi-close"
          variant="text"
          size="small"
          @click="handleClose"
        ></v-btn>
      </div>
      <div v-if="duration > 0" class="success-progress">
        <div class="progress-bar" :style="{ animationDuration: `${duration}ms` }"></div>
      </div>
    </div>
  </transition>
</template>

<script setup lang="ts">
import { computed, ref, watch, onMounted } from 'vue'

interface Props {
  visible: boolean
  message: string
  title?: string
  type?: 'success' | 'info' | 'warning'
  duration?: number
  icon?: string
}

const props = withDefaults(defineProps<Props>(), {
  visible: false,
  title: undefined,
  type: 'success',
  duration: 3000,
  icon: 'mdi-check-circle'
})

const emit = defineEmits<{
  close: []
}>()

const handleClose = () => {
  emit('close')
}

watch(() => props.visible, (newVal) => {
  if (newVal && props.duration > 0) {
    setTimeout(() => {
      handleClose()
    }, props.duration)
  }
})
</script>

<style scoped>
.success-message {
  position: fixed;
  top: 20px;
  right: 20px;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  max-width: 400px;
  z-index: 9998;
}

.success-content {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 16px;
}

.success-icon {
  flex-shrink: 0;
  font-size: 24px;
}

.success-success .success-icon {
  color: #4caf50;
}

.success-info .success-icon {
  color: #2196f3;
}

.success-warning .success-icon {
  color: #ff9800;
}

.success-text {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.success-title {
  font-weight: 500;
  margin: 0;
}

.success-body {
  margin: 0;
  font-size: 14px;
  color: #666;
}

.success-progress {
  height: 3px;
  background-color: #f0f0f0;
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #4caf50, #45a049);
  animation: progress linear forwards;
}

@keyframes progress {
  from {
    width: 100%;
  }
  to {
    width: 0%;
  }
}

.slide-fade-enter-active {
  transition: all 0.3s ease;
}

.slide-fade-leave-active {
  transition: all 0.3s ease;
}

.slide-fade-enter-from {
  transform: translateX(400px);
  opacity: 0;
}

.slide-fade-leave-to {
  transform: translateX(400px);
  opacity: 0;
}

@media (max-width: 600px) {
  .success-message {
    left: 20px;
    right: 20px;
    max-width: none;
  }
}
</style>
