import { ref } from 'vue'
import { api } from '@/services/api'
import { useGoalsStore } from '@/stores/goals'
import type { WorkoutGoalDto, CreateWorkoutGoalDto } from '@/types/goals'

export const useGoalService = () => {
  const goalsStore = useGoalsStore()
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const createGoal = async (data: CreateWorkoutGoalDto): Promise<WorkoutGoalDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.post<{ data: WorkoutGoalDto }>('/goals', data)
      const goal = response.data?.data
      if (goal) {
        goalsStore.addGoal(goal)
      }
      return goal || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '建立目標失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getGoal = async (id: string): Promise<WorkoutGoalDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.get<{ data: WorkoutGoalDto }>(`/goals/${id}`)
      return response.data?.data || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '查詢目標失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getAllGoals = async (): Promise<WorkoutGoalDto[]> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.get<{ data: WorkoutGoalDto[] }>('/goals')
      const goals = response.data?.data || []
      goalsStore.setGoals(goals)
      return goals
    } catch (err: any) {
      error.value = err.response?.data?.message || '查詢目標失敗'
      return []
    } finally {
      isLoading.value = false
    }
  }

  const getActiveGoal = async (): Promise<WorkoutGoalDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.get<{ data: WorkoutGoalDto }>('/goals/active')
      const goal = response.data?.data || null
      goalsStore.setActiveGoal(goal)
      return goal
    } catch (err: any) {
      error.value = err.response?.data?.message || '查詢活動目標失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const updateGoal = async (id: string, data: CreateWorkoutGoalDto): Promise<WorkoutGoalDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.put<{ data: WorkoutGoalDto }>(`/goals/${id}`, data)
      const goal = response.data?.data
      if (goal) {
        goalsStore.updateGoal(goal)
      }
      return goal || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '更新目標失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const deleteGoal = async (id: string): Promise<boolean> => {
    try {
      isLoading.value = true
      error.value = null
      await api.delete(`/goals/${id}`)
      goalsStore.removeGoal(id)
      return true
    } catch (err: any) {
      error.value = err.response?.data?.message || '刪除目標失敗'
      return false
    } finally {
      isLoading.value = false
    }
  }

  const deactivateGoal = async (id: string): Promise<WorkoutGoalDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.patch<{ data: WorkoutGoalDto }>(`/goals/${id}/deactivate`, {})
      const goal = response.data?.data
      if (goal) {
        goalsStore.updateGoal(goal)
        goalsStore.setActiveGoal(null)
      }
      return goal || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '停用目標失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    createGoal,
    getGoal,
    getAllGoals,
    getActiveGoal,
    updateGoal,
    deleteGoal,
    deactivateGoal
  }
}
