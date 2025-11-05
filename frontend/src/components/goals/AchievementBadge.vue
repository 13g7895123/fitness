<template>
  <v-card v-if="isAchieved" class="achievement-badge-card">
    <v-card-item>
      <div class="badge-content">
        <v-icon size="x-large" color="success" class="badge-icon">mdi-trophy</v-icon>
        <div class="badge-text">
          <h3 class="badge-title">{{ $t('goals.achievementBadge') }}</h3>
          <p class="badge-description">{{ congratsMessage }}</p>
        </div>
      </div>
    </v-card-item>
  </v-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

interface Props {
  isAchieved: boolean
  goalLabel?: string
}

const props = withDefaults(defineProps<Props>(), {
  goalLabel: '目標'
})

const { t } = useI18n()

const congratsMessage = computed(() => {
  const messages = [
    `恭喜！您已達成 ${props.goalLabel}！`,
    `太棒了！您已完成 ${props.goalLabel}！`,
    `很好！您已達成 ${props.goalLabel}，繼續努力！`
  ]
  return messages[Math.floor(Math.random() * messages.length)]
})
</script>

<style scoped>
.achievement-badge-card {
  background: linear-gradient(135deg, rgba(76, 175, 80, 0.1) 0%, rgba(255, 193, 7, 0.1) 100%);
  border: 2px solid #4caf50;
  margin-bottom: 16px;
}

.badge-content {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 12px 0;
}

.badge-icon {
  flex-shrink: 0;
}

.badge-text {
  flex: 1;
}

.badge-title {
  margin: 0 0 4px 0;
  font-size: 16px;
  font-weight: 600;
  color: #4caf50;
}

.badge-description {
  margin: 0;
  font-size: 14px;
  color: #666;
}
</style>
