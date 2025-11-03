<template>
  <v-card class="goal-progress-bar">
    <v-card-item>
      <div class="progress-container">
        <div class="progress-label">
          <span class="label-text">{{ label }}</span>
          <span class="progress-value">{{ currentValue }} / {{ goalValue }}</span>
        </div>
        <v-progress-linear
          :value="achievementPercent"
          :color="getColor(achievementPercent)"
          height="24"
          rounded
          class="progress-bar"
        >
          <template #default="{ value }">
            <span class="progress-text">{{ Math.round(value) }}%</span>
          </template>
        </v-progress-linear>
        <div v-if="isAchieved" class="achievement-badge">
          <v-icon small color="success">mdi-check-circle</v-icon>
          <span class="badge-text">{{ $t('goals.achieved') }}</span>
        </div>
      </div>
    </v-card-item>
  </v-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

interface Props {
  label: string
  currentValue: number
  goalValue: number
  isAchieved?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isAchieved: false
})

const { t } = useI18n()

const achievementPercent = computed(() => {
  if (!props.goalValue || props.goalValue <= 0) return 0
  return Math.min((props.currentValue / props.goalValue) * 100, 100)
})

const getColor = (percent: number): string => {
  if (percent >= 100) return 'success'
  if (percent >= 75) return 'info'
  if (percent >= 50) return 'warning'
  return 'error'
}
</script>

<style scoped>
.goal-progress-bar {
  margin-bottom: 16px;
}

.progress-container {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.progress-label {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.label-text {
  font-size: 14px;
  font-weight: 500;
  color: rgba(0, 0, 0, 0.87);
}

.progress-value {
  font-size: 12px;
  color: #999;
}

.progress-bar {
  position: relative;
  min-height: 24px;
}

.progress-text {
  font-size: 12px;
  font-weight: 600;
  color: white;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
}

.achievement-badge {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background: rgba(76, 175, 80, 0.1);
  border-radius: 8px;
  color: #4caf50;
  font-size: 12px;
  font-weight: 600;
}

.badge-text {
  margin-left: 4px;
}
</style>
