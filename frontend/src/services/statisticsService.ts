import { api } from './api'
import { useStatisticsStore } from '@/stores/statistics'
import { useErrorHandler } from '@/utils/errorHandler'
import type {
  TrendDataDto,
  WeeklySummaryDto,
  MonthlySummaryDto,
  ExerciseDistributionDto
} from '@/types/statistics'

export const useStatisticsService = () => {
  const statisticsStore = useStatisticsStore()
  const { showError } = useErrorHandler()

  const getWeeklySummary = async (date?: string): Promise<WeeklySummaryDto | null> => {
    try {
      statisticsStore.setLoading(true)
      statisticsStore.setError(null)

      const params = new URLSearchParams()
      if (date) params.append('date', date)

      const response = await api.get<{ success: boolean; message: string; data: WeeklySummaryDto }>(
        `/statistics/weekly?${params.toString()}`
      )

      if (response.data.success && response.data.data) {
        const weeklySummary = response.data.data
        statisticsStore.setWeeklySummary(weeklySummary)
        return weeklySummary
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

  const getTrends = async (periodType: 'day' | 'week' | 'month' = 'day'): Promise<TrendDataDto[]> => {
    try {
      const response = await api.get<{ success: boolean; data: TrendDataDto[] }>(
        `/statistics/trends?periodType=${periodType}`
      )
      if (response.data.success) {
        return response.data.data || []
      } else {
        throw new Error('取得趨勢資料失敗')
      }
    } catch (error) {
      const message = error instanceof Error ? error.message : '取得趨勢資料失敗'
      showError(message)
      return []
    }
  }

  const getMonthlySummary = async (): Promise<MonthlySummaryDto | null> => {
    try {
      const response = await api.get<{ success: boolean; data: MonthlySummaryDto }>(
        '/statistics/monthly'
      )
      if (response.data.success) {
        return response.data.data
      } else {
        throw new Error('取得月度摘要失敗')
      }
    } catch (error) {
      const message = error instanceof Error ? error.message : '取得月度摘要失敗'
      showError(message)
      return null
    }
  }

  const getExerciseDistribution = async (): Promise<ExerciseDistributionDto[]> => {
    try {
      const response = await api.get<{ success: boolean; data: ExerciseDistributionDto[] }>(
        '/statistics/exercise-distribution'
      )
      if (response.data.success) {
        return response.data.data || []
      } else {
        throw new Error('取得運動分布失敗')
      }
    } catch (error) {
      const message = error instanceof Error ? error.message : '取得運動分布失敗'
      showError(message)
      return []
    }
  }

  return { getWeeklySummary, getDailyBreakdown, getTrends, getMonthlySummary, getExerciseDistribution }
}
