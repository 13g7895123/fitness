<template>
  <v-card>
    <v-card-title class="d-flex justify-space-between align-center">
      <span>運動類型列表</span>
      <v-btn color="primary" @click="$emit('create')">
        <v-icon left>mdi-plus</v-icon>
        新增運動類型
      </v-btn>
    </v-card-title>

    <v-divider></v-divider>

    <v-card-text>
      <v-list v-if="exerciseTypes.length > 0">
        <v-list-item
          v-for="exerciseType in exerciseTypes"
          :key="exerciseType.id"
          :class="{ 'grey lighten-4': exerciseType.isSystemDefault }"
        >
          <v-list-item-content>
            <v-list-item-title>
              {{ exerciseType.name }}
              <v-chip
                v-if="exerciseType.isSystemDefault"
                x-small
                color="blue"
                text-color="white"
                class="ml-2"
              >
                系統預設
              </v-chip>
            </v-list-item-title>
            <v-list-item-subtitle v-if="exerciseType.description">
              {{ exerciseType.description }}
            </v-list-item-subtitle>
            <v-list-item-subtitle class="mt-1">
              <v-chip x-small class="mr-1">
                MET: {{ exerciseType.defaultMET }}
              </v-chip>
              <v-chip
                v-for="equipment in exerciseType.equipments"
                :key="equipment.id"
                x-small
                class="mr-1"
              >
                {{ equipment.name }}
              </v-chip>
            </v-list-item-subtitle>
            <v-list-item-subtitle v-if="exerciseType.workoutRecordCount > 0" class="mt-1">
              已使用 {{ exerciseType.workoutRecordCount }} 次
            </v-list-item-subtitle>
          </v-list-item-content>

          <v-list-item-action>
            <div class="d-flex gap-2">
              <v-btn
                icon
                small
                color="primary"
                @click="$emit('view', exerciseType)"
                title="查看詳情"
              >
                <v-icon>mdi-eye</v-icon>
              </v-btn>
              <v-btn
                v-if="!exerciseType.isSystemDefault"
                icon
                small
                color="info"
                @click="$emit('edit', exerciseType)"
                title="編輯"
              >
                <v-icon>mdi-pencil</v-icon>
              </v-btn>
              <v-btn
                v-if="!exerciseType.isSystemDefault"
                icon
                small
                color="error"
                :disabled="exerciseType.workoutRecordCount > 0"
                @click="$emit('delete', exerciseType)"
                title="刪除"
              >
                <v-icon>mdi-delete</v-icon>
              </v-btn>
            </div>
          </v-list-item-action>
        </v-list-item>
      </v-list>

      <v-alert v-else type="info" text>
        尚無運動類型資料
      </v-alert>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import type { ExerciseType } from '@/types/exercise-type'

defineProps<{
  exerciseTypes: ExerciseType[]
}>()

defineEmits<{
  (e: 'create'): void
  (e: 'view', exerciseType: ExerciseType): void
  (e: 'edit', exerciseType: ExerciseType): void
  (e: 'delete', exerciseType: ExerciseType): void
}>()
</script>

<style scoped>
.gap-2 {
  gap: 8px;
}
</style>
