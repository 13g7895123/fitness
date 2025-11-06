<template>
  <div class="fixed top-5 right-5 max-w-md z-[9999] md:right-5 md:left-auto left-5">
    <transition-group name="list" tag="div" class="space-y-3">
      <Alert
        v-for="notification in notifications"
        :key="notification.id"
        :type="notification.type"
        :message="notification.message"
        closable
        @close="removeNotification(notification.id)"
      />
    </transition-group>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { getNotifications, removeNotification } from '@/utils/errorHandler'
import Alert from '@/components/common/Alert.vue'

const notifications = computed(() => {
  return getNotifications().value
})
</script>

<style scoped>
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
</style>
