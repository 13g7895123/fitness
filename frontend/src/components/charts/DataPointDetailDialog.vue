<template>
  <v-dialog v-model="isOpen" max-width="400px">
    <v-card>
      <v-card-title class="d-flex justify-space-between align-center">
        {{ title }}
        <v-btn icon="mdi-close" variant="text" @click="isOpen = false"></v-btn>
      </v-card-title>

      <v-card-text>
        <div class="detail-content">
          <div class="detail-row">
            <span class="label">{{ dateLabel }}</span>
            <span class="value">{{ formatDate(data?.date) }}</span>
          </div>
          <div class="detail-row">
            <span class="label">{{ durationLabel }}</span>
            <span class="value">{{ data?.duration }} 分鐘</span>
          </div>
          <div class="detail-row">
            <span class="label">{{ caloriesLabel }}</span>
            <span class="value">{{ data?.calories }} kcal</span>
          </div>
          <div v-if="data?.count" class="detail-row">
            <span class="label">運動項目數</span>
            <span class="value">{{ data.count }} 項</span>
          </div>
          <div v-if="data?.periodType" class="detail-row">
            <span class="label">統計週期</span>
            <span class="value">{{ getPeriodTypeLabel(data.periodType) }}</span>
          </div>
        </div>
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn text @click="isOpen = false">關閉</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import type { TrendDataDto } from '@/types/statistics'

interface Props {
  modelValue: boolean
  data: TrendDataDto | null
  title?: string
}

const props = withDefaults(defineProps<Props>(), {
  title: '詳細資料'
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const isOpen = ref(props.modelValue)

watch(() => props.modelValue, (newVal) => {
  isOpen.value = newVal
})

watch(isOpen, (newVal) => {
  emit('update:modelValue', newVal)
})

const dateLabel = '日期'
const durationLabel = '運動時間'
const caloriesLabel = '消耗卡路里'

const formatDate = (date?: string) => {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  })
}

const getPeriodTypeLabel = (type?: string) => {
  switch (type) {
    case 'day':
      return '每日'
    case 'week':
      return '每週'
    case 'month':
      return '每月'
    default:
      return '-'
  }
}
</script>

<style scoped>
.detail-content {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 8px 0;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
  border-bottom: 1px solid rgba(0, 0, 0, 0.06);
}

.detail-row:last-child {
  border-bottom: none;
}

.label {
  font-size: 14px;
  color: rgba(0, 0, 0, 0.6);
  font-weight: 500;
}

.value {
  font-size: 14px;
  color: #1f2937;
  font-weight: 600;
}
</style>
