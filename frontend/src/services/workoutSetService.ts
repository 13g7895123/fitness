import { ref } from 'vue'
import { api } from '@/services/api'
import type { WorkoutSetDto, CreateWorkoutSetDto, UpdateWorkoutSetDto } from '@/types/workout-set'

export const useWorkoutSetService = () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const createSet = async (data: CreateWorkoutSetDto): Promise<WorkoutSetDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.post<{ data: WorkoutSetDto }>('/workout-sets', data)
      return response.data?.data || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '新增組數失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const updateSet = async (id: number, data: UpdateWorkoutSetDto): Promise<WorkoutSetDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.put<{ data: WorkoutSetDto }>(`/workout-sets/${id}`, data)
      return response.data?.data || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '更新組數失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const deleteSet = async (id: number): Promise<boolean> => {
    try {
      isLoading.value = true
      error.value = null
      await api.delete(`/workout-sets/${id}`)
      return true
    } catch (err: any) {
      error.value = err.response?.data?.message || '刪除組數失敗'
      return false
    } finally {
      isLoading.value = false
    }
  }

  const getSetsByWorkoutRecord = async (workoutRecordId: number): Promise<WorkoutSetDto[]> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.get<{ data: WorkoutSetDto[] }>(`/workout-sets/by-record/${workoutRecordId}`)
      return response.data?.data || []
    } catch (err: any) {
      error.value = err.response?.data?.message || '查詢組數失敗'
      return []
    } finally {
      isLoading.value = false
    }
  }

  const getLastSet = async (workoutRecordId: number): Promise<WorkoutSetDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.get<{ data: WorkoutSetDto | null }>(`/workout-sets/last-set/${workoutRecordId}`)
      return response.data?.data || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '查詢上次組數失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    createSet,
    updateSet,
    deleteSet,
    getSetsByWorkoutRecord,
    getLastSet
  }
}
