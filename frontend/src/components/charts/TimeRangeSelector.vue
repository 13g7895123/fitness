<template>
  <Card>
    <div class="text-xs font-semibold text-gray-600 uppercase mb-2">{{ $t('statistics.timeRange') }}</div>
    <div class="flex gap-2">
      <button
        v-for="option in options"
        :key="option.value"
        :class="[
          'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
          selectedRange === option.value
            ? 'bg-primary text-white'
            : 'border border-gray-300 text-gray-700 hover:bg-gray-50'
        ]"
        @click="handleSelectRange(option.value)"
      >
        {{ option.label }}
      </button>
    </div>
  </Card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import Card from '@/components/common/Card.vue'

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
/* Tailwind handles all styling */
</style>
