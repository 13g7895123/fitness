<template>
  <div class="w-full">
    <label v-if="label" :for="id" class="block text-sm font-medium text-gray-700 mb-1">
      {{ label }}
      <span v-if="required" class="text-red-500">*</span>
    </label>
    <textarea
      :id="id"
      :value="modelValue"
      :placeholder="placeholder"
      :disabled="disabled"
      :required="required"
      :rows="rows"
      :maxlength="maxlength"
      :class="textareaClasses"
      @input="$emit('update:modelValue', ($event.target as HTMLTextAreaElement).value)"
      @blur="$emit('blur')"
      @focus="$emit('focus')"
    ></textarea>
    <div v-if="counter && maxlength" class="mt-1 text-sm text-gray-500 text-right">
      {{ modelValue?.length || 0 }} / {{ maxlength }}
    </div>
    <p v-if="error" class="mt-1 text-sm text-red-600">{{ error }}</p>
    <p v-else-if="hint" class="mt-1 text-sm text-gray-500">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps({
  modelValue: {
    type: String,
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
  rows: {
    type: Number,
    default: 3
  },
  maxlength: {
    type: Number,
    default: undefined
  },
  counter: {
    type: Boolean,
    default: false
  },
  id: {
    type: String,
    default: () => `textarea-${Math.random().toString(36).substr(2, 9)}`
  }
})

defineEmits(['update:modelValue', 'blur', 'focus'])

const textareaClasses = computed(() => {
  const base = 'input resize-y min-h-[80px]'
  const error = props.error ? 'input-error' : ''
  const disabled = props.disabled ? 'bg-gray-100 cursor-not-allowed' : ''
  
  return [base, error, disabled].filter(Boolean).join(' ')
})
</script>
