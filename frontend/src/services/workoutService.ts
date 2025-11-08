import { ref } from 'vue'
import { api } from '@/services/api'
import type { WorkoutRecordResponseDto, CreateWorkoutRecordDto, UpdateWorkoutRecordDto, PaginatedResponse } from '@/types/workout'
import type { DailyWorkoutDto } from '@/types/daily-workout'

export const useWorkoutService = () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const createRecord = async (data: CreateWorkoutRecordDto): Promise<WorkoutRecordResponseDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.post<{ data: WorkoutRecordResponseDto }>('/workout-records', data)
      return response.data?.data || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '新增紀錄失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const updateRecord = async (id: string, data: UpdateWorkoutRecordDto): Promise<WorkoutRecordResponseDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.put<{ data: WorkoutRecordResponseDto }>(`/workout-records/${id}`, data)
      return response.data?.data || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '更新紀錄失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const deleteRecord = async (id: string): Promise<boolean> => {
    try {
      isLoading.value = true
      error.value = null
      await api.delete(`/workout-records/${id}`)
      return true
    } catch (err: any) {
      error.value = err.response?.data?.message || '刪除紀錄失敗'
      return false
    } finally {
      isLoading.value = false
    }
  }

  const getRecord = async (id: string): Promise<WorkoutRecordResponseDto | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.get<{ data: WorkoutRecordResponseDto }>(`/workout-records/${id}`)
      return response.data?.data || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '查詢紀錄失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getRecordsByDate = async (date: string): Promise<WorkoutRecordResponseDto[]> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.get<{ data: WorkoutRecordResponseDto[] }>(`/workout-records/daily/${date}`)
      return response.data?.data || []
    } catch (err: any) {
      error.value = err.response?.data?.message || '查詢紀錄失敗'
      return []
    } finally {
      isLoading.value = false
    }
  }

  const getRecordsByDateRange = async (startDate: string, endDate: string): Promise<WorkoutRecordResponseDto[]> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.get<{ data: WorkoutRecordResponseDto[] }>('/workout-records/range', {
        params: { startDate, endDate }
      })
      return response.data?.data || []
    } catch (err: any) {
      error.value = err.response?.data?.message || '查詢紀錄失敗'
      return []
    } finally {
      isLoading.value = false
    }
  }

  const getAllRecords = async (pageNumber = 1, pageSize = 10): Promise<PaginatedResponse<WorkoutRecordResponseDto> | null> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.get<{ data: PaginatedResponse<WorkoutRecordResponseDto> }>('/workout-records', {
        params: { pageNumber, pageSize }
      })
      return response.data?.data || null
    } catch (err: any) {
      error.value = err.response?.data?.message || '查詢紀錄失敗'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getDailyWorkout = async (date: string): Promise<{ data: DailyWorkoutDto }> => {
    try {
      isLoading.value = true
      error.value = null
      const response = await api.get<{ data: DailyWorkoutDto }>(`/workout-records/daily/${date}`)
      return response.data || { data: null as any }
    } catch (err: any) {
      error.value = err.response?.data?.message || '查詢每日紀錄失敗'
      throw error.value
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    createRecord,
    updateRecord,
    deleteRecord,
    getRecord,
    getRecordsByDate,
    getRecordsByDateRange,
    getAllRecords,
    getDailyWorkout
  }
}
