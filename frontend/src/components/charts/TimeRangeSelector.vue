<template>
  <v-card class="time-range-selector">
    <v-card-item>
      <div class="selector-label">{{ $t('statistics.timeRange') }}</div>
      <div class="selector-buttons">
        <v-btn
          v-for="option in options"
          :key="option.value"
          :variant="selectedRange === option.value ? 'elevated' : 'outlined'"
          :color="selectedRange === option.value ? 'primary' : 'default'"
          size="small"
          @click="handleSelectRange(option.value)"
        >
          {{ option.label }}
        </v-btn>
      </div>
    </v-card-item>
  </v-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'

export type TimeRange = '4weeks' | '12months' | 'alltime'

interface TimeRangeOption {
  value: TimeRange
  label: string
}

const props = withDefaults(
  defineProps<{
    modelValue?: TimeRange
  }>(),
  {
    modelValue: '4weeks'
  }
)

const emit = defineEmits<{
  'update:modelValue': [value: TimeRange]
}>()

const selectedRange = ref<TimeRange>(props.modelValue)

const options: TimeRangeOption[] = [
  { value: '4weeks', label: '4 週' },
  { value: '12months', label: '12 個月' },
  { value: 'alltime', label: '全部' }
]

const handleSelectRange = (range: TimeRange) => {
  selectedRange.value = range
  emit('update:modelValue', range)
}
</script>

<style scoped>
.time-range-selector {
  margin-bottom: 16px;
}

.selector-label {
  font-size: 12px;
  font-weight: 600;
  color: rgba(0, 0, 0, 0.6);
  margin-bottom: 8px;
  text-transform: uppercase;
}

.selector-buttons {
  display: flex;
  gap: 8px;
}
</style>
