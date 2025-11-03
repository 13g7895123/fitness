import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { WorkoutGoalDto } from '@/types/goals'

export const useGoalsStore = defineStore('goals', () => {
  const goals = ref<WorkoutGoalDto[]>([])
  const activeGoal = ref<WorkoutGoalDto | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const hasGoals = computed(() => goals.value.length > 0)
  const hasActiveGoal = computed(() => activeGoal.value !== null)

  const setGoals = (data: WorkoutGoalDto[]) => {
    goals.value = data
    error.value = null
  }

  const setActiveGoal = (goal: WorkoutGoalDto | null) => {
    activeGoal.value = goal
  }

  const setLoading = (loading: boolean) => {
    isLoading.value = loading
  }

  const setError = (errorMsg: string | null) => {
    error.value = errorMsg
  }

  const addGoal = (goal: WorkoutGoalDto) => {
    goals.value.push(goal)
    if (goal.IsActive) {
      activeGoal.value = goal
    }
  }

  const updateGoal = (goal: WorkoutGoalDto) => {
    const index = goals.value.findIndex(g => g.Id === goal.Id)
    if (index !== -1) {
      goals.value[index] = goal
      if (goal.IsActive) {
        activeGoal.value = goal
      }
    }
  }

  const removeGoal = (goalId: string) => {
    goals.value = goals.value.filter(g => g.Id !== goalId)
    if (activeGoal.value?.Id === goalId) {
      activeGoal.value = null
    }
  }

  const clearData = () => {
    goals.value = []
    activeGoal.value = null
    error.value = null
  }

  return {
    goals,
    activeGoal,
    isLoading,
    error,
    hasGoals,
    hasActiveGoal,
    setGoals,
    setActiveGoal,
    setLoading,
    setError,
    addGoal,
    updateGoal,
    removeGoal,
    clearData
  }
})
