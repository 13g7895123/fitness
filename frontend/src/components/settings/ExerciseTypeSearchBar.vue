<template>
  <v-text-field
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    label="搜尋運動類型"
    prepend-inner-icon="mdi-magnify"
    clearable
    hide-details
    @click:clear="$emit('update:modelValue', '')"
  ></v-text-field>
</template>

<script setup lang="ts">
defineProps<{
  modelValue: string
}>()

defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()
</script>
