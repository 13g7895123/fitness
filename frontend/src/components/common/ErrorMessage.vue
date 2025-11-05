<template>
  <v-alert
    v-if="visible"
    :type="type"
    :title="title"
    closable
    class="error-message"
    @click:close="handleClose"
  >
    <div class="error-content">
      <p>{{ message }}</p>
      <ul v-if="details && details.length > 0" class="error-details">
        <li v-for="(detail, index) in details" :key="index">
          {{ detail }}
        </li>
      </ul>
    </div>
  </v-alert>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  visible: boolean
  message: string
  title?: string
  type?: 'error' | 'warning' | 'info' | 'success'
  details?: string[]
}

const props = withDefaults(defineProps<Props>(), {
  visible: false,
  title: '錯誤',
  type: 'error',
  details: undefined
})

const emit = defineEmits<{
  close: []
}>()

const handleClose = () => {
  emit('close')
}
</script>

<style scoped>
.error-message {
  margin-bottom: 16px;
}

.error-content {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.error-content p {
  margin: 0;
}

.error-details {
  margin: 8px 0 0 0;
  padding-left: 20px;
}

.error-details li {
  font-size: 12px;
  margin: 4px 0;
}
</style>
