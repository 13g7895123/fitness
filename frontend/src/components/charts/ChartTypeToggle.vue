<template>
  <Card>
    <div class="text-xs font-semibold text-gray-600 uppercase mb-2">{{ $t('statistics.chartType') }}</div>
    <div class="flex gap-2">
      <button
        v-for="option in chartTypes"
        :key="option.value"
        :class="[
          'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
          selectedType === option.value
            ? 'bg-primary text-white'
            : 'border border-gray-300 text-gray-700 hover:bg-gray-50'
        ]"
        @click="handleSelectType(option.value)"
      >
        {{ option.label }}
      </button>
    </div>
  </Card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import Card from '@/components/common/Card.vue'

export type ChartType = 'line' | 'bar' | 'pie'

interface ChartTypeOption {
  value: ChartType
  label: string
  icon: string
}

const props = withDefaults(
  defineProps<{
    modelValue?: ChartType
  }>(),
  {
    modelValue: 'line'
  }
)

const emit = defineEmits<{
  'update:modelValue': [value: ChartType]
}>()

const selectedType = ref<ChartType>(props.modelValue)

const chartTypes: ChartTypeOption[] = [
  { value: 'line', label: '折線圖', icon: 'mdi-chart-line' },
  { value: 'bar', label: '長條圖', icon: 'mdi-chart-bar' },
  { value: 'pie', label: '圓餅圖', icon: 'mdi-chart-pie' }
]

const handleSelectType = (type: ChartType) => {
  selectedType.value = type
  emit('update:modelValue', type)
}
</script>

<style scoped>
/* Tailwind handles all styling */
</style>
