<template>
  <v-card class="chart-type-toggle">
    <v-card-item>
      <div class="toggle-label">{{ $t('statistics.chartType') }}</div>
      <div class="toggle-buttons">
        <v-btn
          v-for="option in chartTypes"
          :key="option.value"
          :variant="selectedType === option.value ? 'elevated' : 'outlined'"
          :color="selectedType === option.value ? 'primary' : 'default'"
          :icon="option.icon"
          size="small"
          @click="handleSelectType(option.value)"
        >
          {{ option.label }}
        </v-btn>
      </div>
    </v-card-item>
  </v-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'

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
.chart-type-toggle {
  margin-bottom: 16px;
}

.toggle-label {
  font-size: 12px;
  font-weight: 600;
  color: rgba(0, 0, 0, 0.6);
  margin-bottom: 8px;
  text-transform: uppercase;
}

.toggle-buttons {
  display: flex;
  gap: 8px;
}
</style>
