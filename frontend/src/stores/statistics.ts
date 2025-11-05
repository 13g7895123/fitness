import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

interface WeeklySummary {
  weekStartDate: string
  weekEndDate: string
  totalDurationMinutes: number
  totalCaloriesBurned: number
  workoutDays: number
  totalWorkoutCount: number
  dailyBreakdown: DailyBreakdown[]
  durationChangePercent: number
  caloriesChangePercent: number
  workoutDaysChangePercent: number
}

interface DailyBreakdown {
  date: string
  dayOfWeek: number
  durationMinutes: number
  caloriesBurned: number
  workoutCount: number
}

export const useStatisticsStore = defineStore('statistics', () => {
  const weeklySummary = ref<WeeklySummary | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const hasData = computed(() => !!weeklySummary.value)

  const setWeeklySummary = (data: WeeklySummary) => {
    weeklySummary.value = data
    error.value = null
  }

  const setLoading = (loading: boolean) => {
    isLoading.value = loading
  }

  const setError = (errorMsg: string | null) => {
    error.value = errorMsg
  }

  const clearData = () => {
    weeklySummary.value = null
    error.value = null
  }

  return {
    weeklySummary,
    isLoading,
    error,
    hasData,
    setWeeklySummary,
    setLoading,
    setError,
    clearData
  }
})
