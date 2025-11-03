<template>
  <v-card class="daily-total-card">
    <v-card-item>
      <div class="text-center">
        <div class="text-h6 mb-4">{{ $t('workout.dayTotal') }}</div>
        <v-row dense>
          <v-col cols="6">
            <div class="stat-item">
              <div class="stat-icon text-primary">
                <v-icon size="large">mdi-clock</v-icon>
              </div>
              <div class="stat-value font-weight-bold">
                {{ totalDurationMinutes }}
              </div>
              <div class="stat-label text-caption">
                {{ $t('common.minuteAbbr') }}
              </div>
            </div>
          </v-col>
          <v-col cols="6">
            <div class="stat-item">
              <div class="stat-icon text-success">
                <v-icon size="large">mdi-fire</v-icon>
              </div>
              <div class="stat-value font-weight-bold">
                {{ totalCaloriesBurned }}
              </div>
              <div class="stat-label text-caption">
                {{ $t('workout.caloriesShort') }}
              </div>
            </div>
          </v-col>
        </v-row>
        <v-row dense class="mt-2">
          <v-col cols="12">
            <div class="text-caption text-medium-emphasis">
              {{ recordCount }} {{ $t('workout.records') }}
            </div>
          </v-col>
        </v-row>
      </div>
    </v-card-item>
  </v-card>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

interface Props {
  totalDurationMinutes: number
  totalCaloriesBurned: number
  recordCount: number
}

defineProps<Props>()

const { t } = useI18n()
</script>

<style scoped>
.daily-total-card {
  background: linear-gradient(135deg, rgba(var(--v-theme-primary), 0.05) 0%, rgba(var(--v-theme-success), 0.05) 100%);
  border: 1px solid rgba(var(--v-theme-primary), 0.2);
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.stat-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  border-radius: 8px;
  background-color: rgba(var(--v-theme-primary), 0.1);
}

.stat-icon.text-success {
  background-color: rgba(var(--v-theme-success), 0.1);
}

.stat-value {
  font-size: 1.5rem;
  line-height: 1;
}

.stat-label {
  color: rgb(var(--v-theme-on-surface-variant));
}
</style>
