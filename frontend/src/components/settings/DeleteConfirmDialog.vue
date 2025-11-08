<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div v-if="modelValue" class="fixed inset-0 z-50 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4 text-center">
          <!-- Backdrop -->
          <div class="fixed inset-0 bg-white/10 backdrop-blur-sm" @click="$emit('update:modelValue', false)"></div>

          <!-- Dialog -->
          <Transition
            enter-active-class="transition duration-300 ease-out"
            enter-from-class="opacity-0 scale-95"
            enter-to-class="opacity-100 scale-100"
            leave-active-class="transition duration-200 ease-in"
            leave-from-class="opacity-100 scale-100"
            leave-to-class="opacity-0 scale-95"
          >
            <div v-if="modelValue" class="relative bg-white rounded-xl shadow-2xl transform transition-all w-full max-w-md p-6">
              <h3 class="text-lg font-semibold text-gray-900 mb-4">
                確認刪除
              </h3>
              
              <div class="mb-6">
                <p class="text-gray-700">確定要刪除「{{ itemName }}」嗎？</p>
                <p v-if="warningMessage" class="text-red-600 mt-2">{{ warningMessage }}</p>
              </div>
              
              <div class="flex justify-end gap-3">
                <Button
                  variant="outlined"
                  size="medium"
                  @click="$emit('update:modelValue', false)"
                >
                  取消
                </Button>
                <Button
                  variant="danger"
                  size="medium"
                  :disabled="!!warningMessage"
                  :loading="loading"
                  @click="$emit('confirm')"
                >
                  刪除
                </Button>
              </div>
            </div>
          </Transition>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import Button from '@/components/common/Button.vue'

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
