<template>
  <div class="min-h-screen p-6 bg-gradient-to-br from-gray-50 to-blue-50">
    <div class="max-w-content mx-auto">
      <h1 class="text-3xl font-bold text-gray-900 mb-6">{{ $t('navigation.trends') }}</h1>

      <div class="mb-4">
        <TimeRangeSelector v-model="selectedTimeRange" />
      </div>

      <div class="mb-4">
        <ChartTypeToggle v-model="selectedChartType" />
      </div>

      <!-- 資料不足提示 -->
      <Alert
        v-if="isDataInsufficient"
        type="warning"
        :title="$t('statistics.insufficientData')"
        :message="$t('statistics.insufficientDataMessage')"
        :visible="isDataInsufficient"
        closable
        class="mb-4"
      />

      <!-- 趨勢圖表 -->
      <div v-if="!isLoading && !isDataInsufficient" class="mb-4">
        <component
          :is="getCurrentChartComponent"
          :data="currentChartData"
          :title="currentChartTitle"
          :dataType="currentDataType"
        />
      </div>

      <!-- 加載狀態 -->
      <Loading v-if="isLoading" :visible="isLoading" text="載入中..." />

      <!-- 每週摘要 -->
      <div v-if="!isLoading && weeklySummary" class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Card>
          <div class="text-base font-semibold text-gray-900 mb-4">{{ $t('statistics.weeklySummary') }}</div>
          <div class="space-y-3">
            <div class="flex justify-between items-center py-2 border-b border-gray-100 last:border-0">
              <span class="text-sm text-gray-600">{{ $t('statistics.totalDuration') }}</span>
              <span class="text-base font-semibold text-gray-900">{{ weeklySummary.totalDurationMinutes }} 分鐘</span>
            </div>
            <div class="flex justify-between items-center py-2 border-b border-gray-100 last:border-0">
              <span class="text-sm text-gray-600">{{ $t('statistics.totalCalories') }}</span>
              <span class="text-base font-semibold text-gray-900">{{ weeklySummary.totalCaloriesBurned.toFixed(0) }} kcal</span>
            </div>
            <div class="flex justify-between items-center py-2 border-b border-gray-100 last:border-0">
              <span class="text-sm text-gray-600">{{ $t('statistics.workoutDays') }}</span>
              <span class="text-base font-semibold text-gray-900">{{ weeklySummary.workoutDays }} 天</span>
            </div>
          </div>
        </Card>

        <Card>
          <div class="text-base font-semibold text-gray-900 mb-4">{{ $t('statistics.comparison') }}</div>
          <div class="space-y-3">
            <div class="flex justify-between items-center py-2 border-b border-gray-100 last:border-0">
              <span class="text-sm text-gray-600">{{ $t('statistics.durationChange') }}</span>
              <span
                class="text-base font-semibold"
                :class="weeklySummary.durationChangePercentage >= 0 ? 'text-green-600' : 'text-red-600'"
              >
                {{ weeklySummary.durationChangePercentage > 0 ? '+' : '' }}
                {{ weeklySummary.durationChangePercentage.toFixed(1) }}%
              </span>
            </div>
            <div class="flex justify-between items-center py-2 border-b border-gray-100 last:border-0">
              <span class="text-sm text-gray-600">{{ $t('statistics.caloriesChange') }}</span>
              <span
                class="text-base font-semibold"
                :class="weeklySummary.caloriesChangePercentage >= 0 ? 'text-green-600' : 'text-red-600'"
              >
                {{ weeklySummary.caloriesChangePercentage > 0 ? '+' : '' }}
                {{ weeklySummary.caloriesChangePercentage.toFixed(1) }}%
              </span>
            </div>
          </div>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import Card from '@/components/common/Card.vue'
import Alert from '@/components/common/Alert.vue'
import Loading from '@/components/common/Loading.vue'
import TimeRangeSelector from '@/components/charts/TimeRangeSelector.vue'
import ChartTypeToggle from '@/components/charts/ChartTypeToggle.vue'
import LineChart from '@/components/charts/LineChart.vue'
import BarChart from '@/components/charts/BarChart.vue'
import PieChart from '@/components/charts/PieChart.vue'
import type { TimeRange } from '@/components/charts/TimeRangeSelector.vue'
import type { ChartType } from '@/components/charts/ChartTypeToggle.vue'
import { useStatisticsService } from '@/services/statisticsService'
import type { TrendDataDto, WeeklySummaryDto, ExerciseDistributionDto } from '@/types/statistics'

const { t } = useI18n()
const statisticsService = useStatisticsService()

const selectedTimeRange = ref<TimeRange>('4weeks')
const selectedChartType = ref<ChartType>('line')
const isLoading = ref(false)

const trendData = ref<TrendDataDto[]>([])
const weeklySummary = ref<WeeklySummaryDto | null>(null)
const exerciseDistribution = ref<ExerciseDistributionDto[]>([])

const MIN_DATA_WEEKS = 2

const isDataInsufficient = computed(() => {
  const weekCount = Math.ceil((trendData.value.length || 0) / 7)
  return weekCount < MIN_DATA_WEEKS
})

const getTimeRangePeriodType = (range: TimeRange) => {
  switch (range) {
    case '4weeks':
      return 'day'
    case '12months':
      return 'week'
    case 'alltime':
      return 'month'
  }
}

const getCurrentChartComponent = computed(() => {
  switch (selectedChartType.value) {
    case 'bar':
      return BarChart
    case 'pie':
      return PieChart
    case 'line':
    default:
      return LineChart
  }
})

const currentChartData = computed(() => {
  if (selectedChartType.value === 'pie') {
    return exerciseDistribution.value
  }
  return trendData.value
})

const currentChartTitle = computed(() => {
  if (selectedChartType.value === 'pie') {
    return t('statistics.exerciseDistribution')
  } else if (selectedChartType.value === 'bar') {
    return t('statistics.caloriesTrend')
  }
  return t('statistics.durationTrend')
})

const currentDataType = computed(() => {
  return selectedChartType.value === 'bar' ? 'calories' : 'duration'
})

const loadData = async () => {
  isLoading.value = true
  try {
    const periodType = getTimeRangePeriodType(selectedTimeRange.value)
    const response = await statisticsService.getTrends(periodType)
    trendData.value = response
    
    const weeklyResponse = await statisticsService.getWeeklySummary()
    weeklySummary.value = weeklyResponse
    
    const exerciseResponse = await statisticsService.getExerciseDistribution()
    exerciseDistribution.value = exerciseResponse
  } catch (error) {
    console.error('Failed to load statistics:', error)
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
/* Tailwind handles all styling */
</style>
