<template>
  <v-dialog :model-value="modelValue" @update:model-value="$emit('update:modelValue', $event)" max-width="500">
    <v-card>
      <v-card-title class="text-h5">
        確認刪除
      </v-card-title>

      <v-card-text>
        <p>確定要刪除「{{ itemName }}」嗎？</p>
        <p v-if="warningMessage" class="text-error">{{ warningMessage }}</p>
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn text @click="$emit('update:modelValue', false)">取消</v-btn>
        <v-btn color="error" :loading="loading" @click="$emit('confirm')">刪除</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
defineProps<{
  modelValue: boolean
  itemName: string
  warningMessage?: string
  loading?: boolean
}>()

defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'confirm'): void
}>()
</script>
