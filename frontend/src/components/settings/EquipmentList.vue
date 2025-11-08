<template>
  <div class="bg-white rounded-lg shadow">
    <div class="flex justify-between items-center p-6 border-b border-gray-200">
      <h3 class="text-lg font-semibold text-gray-900">器材列表</h3>
      <Button variant="primary" @click="$emit('create')">
        <svg class="h-5 w-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        新增器材
      </Button>
    </div>

    <div class="p-6">
      <div v-if="equipments.length > 0" class="divide-y divide-gray-200">
        <div
          v-for="equipment in equipments"
          :key="equipment.id"
          class="py-4 hover:bg-gray-50 transition-colors"
          :class="{ 'bg-gray-50': equipment.isSystemDefault }"
        >
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <div class="flex items-center gap-2 mb-1">
                <h4 class="text-base font-medium text-gray-900">
                  {{ equipment.name }}
                </h4>
                <span
                  v-if="equipment.isSystemDefault"
                  class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800"
                >
                  系統預設
                </span>
              </div>
              
              <p v-if="equipment.description" class="text-sm text-gray-600 mb-2">
                {{ equipment.description }}
              </p>
              
              <p class="text-xs text-gray-500">
                建立時間: {{ formatDate(equipment.createdAt) }}
              </p>
            </div>

            <div class="flex items-center gap-2 ml-4">
              <button
                v-if="!equipment.isSystemDefault"
                @click="$emit('edit', equipment)"
                class="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                title="編輯"
              >
                <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                </svg>
              </button>
              <button
                v-if="!equipment.isSystemDefault"
                @click="$emit('delete', equipment)"
                class="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                title="刪除"
              >
                <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      <div v-else class="py-12 text-center">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
        </svg>
        <p class="mt-2 text-sm text-gray-600">尚無器材資料</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Equipment } from '@/types/exercise-type'
import Button from '@/components/common/Button.vue'

defineProps<{
  equipments: Equipment[]
}>()

defineEmits<{
  (e: 'create'): void
  (e: 'edit', equipment: Equipment): void
  (e: 'delete', equipment: Equipment): void
}>()

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-TW')
}
</script>
