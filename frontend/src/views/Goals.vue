<template>
  <div class="min-h-screen p-6 bg-gray-50">
    <div class="max-w-content mx-auto">
      <div class="mb-8">
        <h1 class="text-3xl font-bold">{{ $t('goals.weeklyGoal') }}</h1>
        <p class="text-gray-600">{{ $t('goals.progress') }}</p>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="mb-6">
        <div class="card p-6 animate-pulse">
          <div class="h-4 bg-gray-200 rounded w-3/4 mb-4"></div>
          <div class="h-32 bg-gray-200 rounded"></div>
        </div>
      </div>

      <!-- Error State -->
      <Alert
        v-else-if="error"
        type="error"
        :title="$t('common.error')"
        :message="error"
        class="mb-6"
      />

      <!-- Active Goal Display -->
      <div v-else-if="activeGoal" class="mb-6">
        <Card class="bg-gradient-to-br from-blue-50 to-green-50">
          <div class="flex justify-between items-start mb-4">
            <div>
              <h2 class="text-xl font-bold text-gray-900">{{ $t('goals.progress') }}</h2>
              <p class="text-xs text-gray-500 mt-1">
                {{ formatDate(activeGoal.StartDate) }} ~ 
                {{ activeGoal.EndDate ? formatDate(activeGoal.EndDate) : '無結束日期' }}
              </p>
            </div>
            <button
              @click="showEditDialog = true"
              class="p-2 hover:bg-white/50 rounded-lg transition-colors"
            >
              <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
              </svg>
            </button>
          </div>

          <div class="border-t border-gray-200 pt-4">
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
          </div>

          <div class="flex justify-end gap-2 mt-4 pt-4 border-t border-gray-200">
            <Button
              variant="text"
              @click="showEditDialog = true"
            >
              {{ $t('goals.editGoal') }}
            </Button>
            <Button
              class="bg-red-600 hover:bg-red-700 text-white"
              @click="handleDeactivate"
            >
              {{ $t('common.close') }}
            </Button>
          </div>
        </Card>
      </div>

      <!-- No Active Goal -->
      <div v-else class="mb-6">
        <Card class="text-center py-12">
          <svg class="w-16 h-16 mx-auto mb-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
          </svg>
          <div class="text-base font-medium text-gray-700">{{ $t('goals.notAchieved') }}</div>
          <div class="text-sm text-gray-500 mt-1">
            {{ $t('goals.setGoal') }}
          </div>
        </Card>
      </div>

      <!-- Create New Goal Button -->
      <div v-if="!activeGoal" class="mb-6">
        <Button
          variant="primary"
          block
          class="py-3"
          @click="showCreateDialog = true"
        >
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          {{ $t('goals.setGoal') }}
        </Button>
      </div>

      <!-- Past Goals List -->
      <div v-if="goals.length > 0" class="mb-6">
        <h3 class="text-base font-semibold text-gray-900 mb-4">{{ $t('goals.weeklyGoal') }} 歷史</h3>
        <div class="space-y-4">
          <div
            v-for="goal in goals"
            :key="goal.Id"
            class="relative pl-6 pb-4 border-l-2"
            :class="goal.IsActive ? 'border-primary' : 'border-gray-300'"
          >
            <div
              class="absolute left-0 top-0 w-3 h-3 rounded-full border-2 border-white -translate-x-[7px]"
              :class="goal.IsActive ? 'bg-primary' : 'bg-gray-400'"
            ></div>
            <Card>
              <div class="font-semibold text-gray-900 mb-2">
                {{ formatDate(goal.StartDate) }} ~ 
                {{ goal.EndDate ? formatDate(goal.EndDate) : '無結束日期' }}
              </div>
              <div class="flex gap-4 text-xs text-gray-600">
                <span v-if="goal.WeeklyMinutesGoal">
                  時間: {{ goal.CurrentWeekMinutes }} / {{ goal.WeeklyMinutesGoal }} 分
                </span>
                <span v-if="goal.WeeklyCaloriesGoal">
                  卡路里: {{ goal.CurrentWeekCalories }} / {{ goal.WeeklyCaloriesGoal }} 卡
                </span>
              </div>
            </Card>
          </div>
        </div>
      </div>

      <!-- Create Goal Dialog -->
      <Dialog
        v-model="showCreateDialog"
        :title="$t('goals.setGoal')"
        max-width="md"
      >
        <GoalForm
          @save="handleCreateGoal"
          @cancel="showCreateDialog = false"
        />
      </Dialog>

      <!-- Edit Goal Dialog -->
      <Dialog
        v-model="showEditDialog"
        :title="$t('goals.editGoal')"
        max-width="md"
      >
        <GoalForm
          v-if="activeGoal"
          :initial-data="getInitialData(activeGoal)"
          @save="handleUpdateGoal"
          @cancel="showEditDialog = false"
        />
      </Dialog>

      <!-- Notifications -->
      <NotificationPanel />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useGoalsStore } from '@/stores/goals'
import { useGoalService } from '@/services/goalService'
import Card from '@/components/common/Card.vue'
import Button from '@/components/common/Button.vue'
import Dialog from '@/components/common/Dialog.vue'
import Alert from '@/components/common/Alert.vue'
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
/* Tailwind handles all styling */
</style>
