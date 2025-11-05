<template>
  <div v-if="visible" class="loading-overlay">
    <div class="loading-container">
      <v-progress-circular
        :size="size"
        :width="width"
        color="primary"
        indeterminate
      ></v-progress-circular>
      <p v-if="text" class="loading-text">{{ text }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  visible: boolean
  text?: string
  size?: number
  width?: number
}

withDefaults(defineProps<Props>(), {
  visible: false,
  text: undefined,
  size: 60,
  width: 4
})
</script>

<style scoped>
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
}

.loading-text {
  color: white;
  font-size: 14px;
  margin: 0;
}
</style>
