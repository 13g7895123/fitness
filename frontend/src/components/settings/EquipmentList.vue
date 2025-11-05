<template>
  <v-card>
    <v-card-title class="d-flex justify-space-between align-center">
      <span>器材列表</span>
      <v-btn color="primary" @click="$emit('create')">
        <v-icon left>mdi-plus</v-icon>
        新增器材
      </v-btn>
    </v-card-title>

    <v-divider></v-divider>

    <v-card-text>
      <v-list v-if="equipments.length > 0">
        <v-list-item
          v-for="equipment in equipments"
          :key="equipment.id"
          :class="{ 'grey lighten-4': equipment.isSystemDefault }"
        >
          <v-list-item-content>
            <v-list-item-title>
              {{ equipment.name }}
              <v-chip
                v-if="equipment.isSystemDefault"
                x-small
                color="blue"
                text-color="white"
                class="ml-2"
              >
                系統預設
              </v-chip>
            </v-list-item-title>
            <v-list-item-subtitle v-if="equipment.description">
              {{ equipment.description }}
            </v-list-item-subtitle>
            <v-list-item-subtitle class="text-caption text-grey">
              建立時間: {{ formatDate(equipment.createdAt) }}
            </v-list-item-subtitle>
          </v-list-item-content>

          <v-list-item-action>
            <div class="d-flex gap-2">
              <v-btn
                v-if="!equipment.isSystemDefault"
                icon
                small
                color="info"
                @click="$emit('edit', equipment)"
                title="編輯"
              >
                <v-icon>mdi-pencil</v-icon>
              </v-btn>
              <v-btn
                v-if="!equipment.isSystemDefault"
                icon
                small
                color="error"
                @click="$emit('delete', equipment)"
                title="刪除"
              >
                <v-icon>mdi-delete</v-icon>
              </v-btn>
            </div>
          </v-list-item-action>
        </v-list-item>
      </v-list>

      <v-alert v-else type="info" text>
        尚無器材資料
      </v-alert>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import type { Equipment } from '@/types/exercise-type'

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

<style scoped>
.gap-2 {
  gap: 8px;
}
</style>
