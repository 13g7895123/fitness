import { api } from './api'
import { useStatisticsStore } from '@/stores/statistics'
import { useErrorHandler } from '@/utils/errorHandler'

interface WeeklySummaryResponse {
  success: boolean
  message: string
  data?: {
    weekStartDate: string
    weekEndDate: string
    totalDurationMinutes: number
    totalCaloriesBurned: number
    workoutDays: number
    totalWorkoutCount: number
    dailyBreakdown: Array<{
      date: string
      dayOfWeek: number
      durationMinutes: number
      caloriesBurned: number
      workoutCount: number
    }>
    durationChangePercent: number
    caloriesChangePercent: number
    workoutDaysChangePercent: number
  }
}

export const useStatisticsService = () => {
  const statisticsStore = useStatisticsStore()
  const { showError } = useErrorHandler()

  const getWeeklySummary = async (date?: string) => {
    try {
      statisticsStore.setLoading(true)
      statisticsStore.setError(null)

      const params = new URLSearchParams()
      if (date) params.append('date', date)

      const response = await api.get<WeeklySummaryResponse>(
        `/statistics/weekly?${params.toString()}`
      )

      if (response.data.success && response.data.data) {
        statisticsStore.setWeeklySummary(response.data.data)
        return response.data.data
      } else {
        throw new Error(response.data.message || '取得週統計失敗')
      }
    } catch (error) {
      const message = error instanceof Error ? error.message : '取得週統計失敗'
      statisticsStore.setError(message)
      showError(message)
      return null
    } finally {
      statisticsStore.setLoading(false)
    }
  }

  const getDailyBreakdown = async (date: string) => {
    try {
      const response = await api.get(`/statistics/daily?date=${encodeURIComponent(date)}`)
      if (response.data.success) {
        return response.data.data
      } else {
        throw new Error(response.data.message || '取得每日明細失敗')
      }
    } catch (error) {
      const message = error instanceof Error ? error.message : '取得每日明細失敗'
      showError(message)
      return null
    }
  }

  return { getWeeklySummary, getDailyBreakdown }
}
