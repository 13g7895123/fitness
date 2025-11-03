<template>
  <div class="notification-container">
    <transition-group name="list" tag="div">
      <v-alert
        v-for="notification in notifications"
        :key="notification.id"
        :type="notification.type"
        :icon="`mdi-${getIcon(notification.type)}`"
        closable
        class="notification-item"
        @click:close="removeNotification(notification.id)"
      >
        {{ notification.message }}
      </v-alert>
    </transition-group>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { getNotifications, removeNotification } from '@/utils/errorHandler'

const notifications = computed(() => {
  return getNotifications().value
})

const getIcon = (type: string): string => {
  const icons: Record<string, string> = {
    error: 'alert-circle',
    warning: 'alert',
    info: 'information',
    success: 'check-circle'
  }
  return icons[type] || 'information'
}
</script>

<style scoped>
.notification-container {
  position: fixed;
  top: 20px;
  right: 20px;
  max-width: 400px;
  z-index: 9999;
}

.notification-item {
  margin-bottom: 12px;
}

.list-enter-active,
.list-leave-active {
  transition: all 0.3s ease;
}

.list-enter-from {
  opacity: 0;
  transform: translateX(30px);
}

.list-leave-to {
  opacity: 0;
  transform: translateX(30px);
}

@media (max-width: 600px) {
  .notification-container {
    left: 20px;
    right: 20px;
    max-width: none;
  }
}
</style>
