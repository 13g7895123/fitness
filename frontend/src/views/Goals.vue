<template>
  <v-container class="goals-page">
    <div class="page-header">
      <h1>{{ $t('goals.weeklyGoal') }}</h1>
      <p>{{ $t('goals.progress') }}</p>
    </div>

    <!-- Loading State -->
    <v-row v-if="isLoading" class="mb-6">
      <v-col cols="12">
        <v-skeleton-loader type="card-heading, image" />
      </v-col>
    </v-row>

    <!-- Error State -->
    <v-row v-else-if="error" class="mb-6">
      <v-col cols="12">
        <v-alert type="error" :title="$t('common.error')">
          {{ error }}
        </v-alert>
      </v-col>
    </v-row>

    <!-- Active Goal Display -->
    <v-row v-else-if="activeGoal" class="mb-6">
      <v-col cols="12">
        <v-card class="active-goal-card">
          <v-card-item>
            <div class="active-goal-header">
              <div>
                <h2>{{ $t('goals.progress') }}</h2>
                <p class="goal-date">
                  {{ formatDate(activeGoal.StartDate) }} ~ 
                  {{ activeGoal.EndDate ? formatDate(activeGoal.EndDate) : '無結束日期' }}
                </p>
              </div>
              <v-btn
                icon
                variant="text"
                @click="showEditDialog = true"
              >
                <v-icon>mdi-pencil</v-icon>
              </v-btn>
            </div>
          </v-card-item>

          <v-divider></v-divider>

          <v-card-text>
            <!-- Achievement Badges -->
            <AchievementBadge
              v-if="activeGoal.IsMinutesAchieved"
              :is-achieved="true"
              goal-label="運動時間目標"
            />
            <AchievementBadge
              v-if="activeGoal.IsCaloriesAchieved"
              :is-achieved="true"
              goal-label="卡路里目標"
            />

            <!-- Time Goal Progress -->
            <GoalProgressBar
              v-if="activeGoal.WeeklyMinutesGoal"
              :label="$t('goals.weeklyMinutes')"
              :current-value="activeGoal.CurrentWeekMinutes"
              :goal-value="activeGoal.WeeklyMinutesGoal"
              :is-achieved="activeGoal.IsMinutesAchieved"
            />

            <!-- Calories Goal Progress -->
            <GoalProgressBar
              v-if="activeGoal.WeeklyCaloriesGoal"
              :label="$t('goals.weeklyCalories')"
              :current-value="activeGoal.CurrentWeekCalories"
              :goal-value="activeGoal.WeeklyCaloriesGoal"
              :is-achieved="activeGoal.IsCaloriesAchieved"
            />
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn
              color="primary"
              variant="text"
              @click="showEditDialog = true"
            >
              {{ $t('goals.editGoal') }}
            </v-btn>
            <v-btn
              color="error"
              variant="text"
              @click="handleDeactivate"
            >
              {{ $t('common.close') }}
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <!-- No Active Goal -->
    <v-row v-else class="mb-6">
      <v-col cols="12">
        <v-card class="text-center py-8">
          <v-icon size="x-large" class="mb-2 text-medium-emphasis">mdi-target</v-icon>
          <div class="text-subtitle1">{{ $t('goals.notAchieved') }}</div>
          <div class="text-caption text-medium-emphasis">
            {{ $t('goals.setGoal') }}
          </div>
        </v-card>
      </v-col>
    </v-row>

    <!-- Create New Goal Button -->
    <v-row v-if="!activeGoal" class="mb-6">
      <v-col cols="12">
        <v-btn
          block
          color="primary"
          prepend-icon="mdi-plus"
          size="large"
          @click="showCreateDialog = true"
        >
          {{ $t('goals.setGoal') }}
        </v-btn>
      </v-col>
    </v-row>

    <!-- Past Goals List -->
    <v-row v-if="goals.length > 0" class="mb-6">
      <v-col cols="12">
        <h3 class="text-subtitle2 mb-3">{{ $t('goals.weeklyGoal') }} 歷史</h3>
        <v-timeline>
          <v-timeline-item
            v-for="goal in goals"
            :key="goal.Id"
            :dot-color="goal.IsActive ? 'primary' : 'grey'"
          >
            <div class="goal-item">
              <div class="goal-title">
                {{ formatDate(goal.StartDate) }} ~ 
                {{ goal.EndDate ? formatDate(goal.EndDate) : '無結束日期' }}
              </div>
              <div class="goal-stats">
                <span v-if="goal.WeeklyMinutesGoal">
                  時間: {{ goal.CurrentWeekMinutes }} / {{ goal.WeeklyMinutesGoal }} 分
                </span>
                <span v-if="goal.WeeklyCaloriesGoal">
                  卡路里: {{ goal.CurrentWeekCalories }} / {{ goal.WeeklyCaloriesGoal }} 卡
                </span>
              </div>
            </div>
          </v-timeline-item>
        </v-timeline>
      </v-col>
    </v-row>

    <!-- Create Goal Dialog -->
    <v-dialog v-model="showCreateDialog" max-width="500">
      <v-card>
        <v-card-title>{{ $t('goals.setGoal') }}</v-card-title>
        <v-card-text>
          <GoalForm
            @save="handleCreateGoal"
            @cancel="showCreateDialog = false"
          />
        </v-card-text>
      </v-card>
    </v-dialog>

    <!-- Edit Goal Dialog -->
    <v-dialog v-model="showEditDialog" max-width="500">
      <v-card>
        <v-card-title>{{ $t('goals.editGoal') }}</v-card-title>
        <v-card-text>
          <GoalForm
            v-if="activeGoal"
            :initial-data="getInitialData(activeGoal)"
            @save="handleUpdateGoal"
            @cancel="showEditDialog = false"
          />
        </v-card-text>
      </v-card>
    </v-dialog>

    <!-- Notifications -->
    <NotificationPanel />
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useGoalsStore } from '@/stores/goals'
import { useGoalService } from '@/services/goalService'
import GoalForm from '@/components/goals/GoalForm.vue'
import GoalProgressBar from '@/components/goals/GoalProgressBar.vue'
import AchievementBadge from '@/components/goals/AchievementBadge.vue'
import NotificationPanel from '@/components/common/NotificationPanel.vue'
import type { WorkoutGoalDto, CreateWorkoutGoalDto } from '@/types/goals'

const { t } = useI18n()
const goalsStore = useGoalsStore()
const {
  isLoading,
  error,
  getAllGoals,
  getActiveGoal,
  createGoal,
  updateGoal,
  deactivateGoal
} = useGoalService()

const showCreateDialog = ref(false)
const showEditDialog = ref(false)

const goals = computed(() => goalsStore.goals)
const activeGoal = computed(() => goalsStore.activeGoal)

const formatDate = (dateStr: string): string => {
  const date = new Date(dateStr)
  return date.toLocaleDateString('zh-TW')
}

const getInitialData = (goal: WorkoutGoalDto): CreateWorkoutGoalDto => {
  return {
    WeeklyMinutesGoal: goal.WeeklyMinutesGoal,
    WeeklyCaloriesGoal: goal.WeeklyCaloriesGoal,
    StartDate: goal.StartDate,
    EndDate: goal.EndDate
  }
}

const handleCreateGoal = async (data: CreateWorkoutGoalDto) => {
  const result = await createGoal(data)
  if (result) {
    showCreateDialog.value = false
    await loadData()
  }
}

const handleUpdateGoal = async (data: CreateWorkoutGoalDto) => {
  if (!activeGoal.value) return
  
  const result = await updateGoal(activeGoal.value.Id, data)
  if (result) {
    showEditDialog.value = false
    await loadData()
  }
}

const handleDeactivate = async () => {
  if (!activeGoal.value) return
  
  const result = await deactivateGoal(activeGoal.value.Id)
  if (result) {
    await loadData()
  }
}

const loadData = async () => {
  await getAllGoals()
  await getActiveGoal()
}

onMounted(async () => {
  await loadData()
})
</script>

<style scoped>
.goals-page {
  padding: 24px;
}

.page-header {
  margin-bottom: 32px;
}

.page-header h1 {
  margin: 0 0 8px 0;
  font-size: 28px;
  font-weight: bold;
}

.page-header p {
  margin: 0;
  color: #999;
}

.active-goal-card {
  background: linear-gradient(135deg, rgba(25, 118, 210, 0.05) 0%, rgba(76, 175, 80, 0.05) 100%);
}

.active-goal-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.goal-date {
  margin: 4px 0 0 0;
  font-size: 12px;
  color: #999;
}

.goal-item {
  padding: 8px 0;
}

.goal-title {
  font-weight: 600;
  margin-bottom: 4px;
}

.goal-stats {
  font-size: 12px;
  color: #999;
  display: flex;
  gap: 16px;
}

.mb-3 {
  margin-bottom: 1rem;
}

.mb-6 {
  margin-bottom: 1.5rem;
}

.py-8 {
  padding: 32px 16px;
}
</style>
