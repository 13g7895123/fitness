<template>
  <div class="w-full">
    <label v-if="label" :for="id" class="block text-sm font-medium text-gray-700 mb-1">
      {{ label }}
      <span v-if="required" class="text-red-500">*</span>
    </label>
    <select
      :id="id"
      :value="modelValue"
      :disabled="disabled"
      :required="required"
      :class="selectClasses"
      @change="$emit('update:modelValue', ($event.target as HTMLSelectElement).value)"
    >
      <option v-if="placeholder" value="" disabled>{{ placeholder }}</option>
      <option
        v-for="item in items"
        :key="itemValue ? item[itemValue] : item"
        :value="itemValue ? item[itemValue] : item"
      >
        {{ itemTitle ? item[itemTitle] : item }}
      </option>
    </select>
    <p v-if="error" class="mt-1 text-sm text-red-600">{{ error }}</p>
    <p v-else-if="hint" class="mt-1 text-sm text-gray-500">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps({
  modelValue: {
    type: [String, Number],
    default: ''
  },
  label: {
    type: String,
    default: ''
  },
  placeholder: {
    type: String,
    default: ''
  },
  hint: {
    type: String,
    default: ''
  },
  error: {
    type: String,
    default: ''
  },
  disabled: {
    type: Boolean,
    default: false
  },
  required: {
    type: Boolean,
    default: false
  },
  items: {
    type: Array,
    default: () => []
  },
  itemTitle: {
    type: String,
    default: ''
  },
  itemValue: {
    type: String,
    default: ''
  },
  id: {
    type: String,
    default: () => `select-${Math.random().toString(36).substr(2, 9)}`
  }
})

defineEmits(['update:modelValue'])

const selectClasses = computed(() => {
  const base = 'input appearance-none bg-white'
  const error = props.error ? 'input-error' : ''
  const disabled = props.disabled ? 'bg-gray-100 cursor-not-allowed' : ''
  
  return [base, error, disabled].filter(Boolean).join(' ')
})
</script>
