<template>
  <div class="trends-container">
    <v-container fluid style="max-width: 1280px; margin: 0 auto;">
      <h1 class="page-title">{{ $t('navigation.trends') }}</h1>

      <v-row class="mb-4">
        <v-col cols="12">
          <TimeRangeSelector v-model="selectedTimeRange" />
        </v-col>
      </v-row>

      <v-row class="mb-4">
        <v-col cols="12">
          <ChartTypeToggle v-model="selectedChartType" />
        </v-col>
      </v-row>

      <!-- 資料不足提示 -->
      <v-row v-if="isDataInsufficient" class="mb-4">
        <v-col cols="12">
          <v-alert
            type="warning"
            variant="tonal"
            :title="$t('statistics.insufficientData')"
            :text="$t('statistics.insufficientDataMessage')"
            closable
          ></v-alert>
        </v-col>
      </v-row>

      <!-- 趨勢圖表 -->
      <v-row v-if="!isLoading && !isDataInsufficient" class="mb-4">
        <v-col cols="12">
          <component
            :is="getCurrentChartComponent"
            :data="currentChartData"
            :title="currentChartTitle"
            :dataType="currentDataType"
          />
        </v-col>
      </v-row>

      <!-- 加載狀態 -->
      <v-row v-if="isLoading">
        <v-col cols="12" class="text-center">
          <v-progress-circular indeterminate color="primary"></v-progress-circular>
        </v-col>
      </v-row>

      <!-- 每週摘要 -->
      <v-row v-if="!isLoading && weeklySummary">
        <v-col cols="12" md="6">
          <v-card class="summary-card">
            <v-card-item>
              <div class="summary-header">{{ $t('statistics.weeklySummary') }}</div>
              <div class="summary-content">
                <div class="summary-row">
                  <span class="summary-label">{{ $t('statistics.totalDuration') }}</span>
                  <span class="summary-value">{{ weeklySummary.totalDurationMinutes }} 分鐘</span>
                </div>
                <div class="summary-row">
                  <span class="summary-label">{{ $t('statistics.totalCalories') }}</span>
                  <span class="summary-value">{{ weeklySummary.totalCaloriesBurned.toFixed(0) }} kcal</span>
                </div>
                <div class="summary-row">
                  <span class="summary-label">{{ $t('statistics.workoutDays') }}</span>
                  <span class="summary-value">{{ weeklySummary.workoutDays }} 天</span>
                </div>
              </div>
            </v-card-item>
          </v-card>
        </v-col>

        <v-col cols="12" md="6">
          <v-card class="summary-card">
            <v-card-item>
              <div class="summary-header">{{ $t('statistics.comparison') }}</div>
              <div class="summary-content">
                <div class="summary-row">
                  <span class="summary-label">{{ $t('statistics.durationChange') }}</span>
                  <span class="summary-value" :class="getDurationChangeClass">
                    {{ weeklySummary.durationChangePercentage > 0 ? '+' : '' }}
                    {{ weeklySummary.durationChangePercentage.toFixed(1) }}%
                  </span>
                </div>
                <div class="summary-row">
                  <span class="summary-label">{{ $t('statistics.caloriesChange') }}</span>
                  <span class="summary-value" :class="getCaloriesChangeClass">
                    {{ weeklySummary.caloriesChangePercentage > 0 ? '+' : '' }}
                    {{ weeklySummary.caloriesChangePercentage.toFixed(1) }}%
                  </span>
                </div>
              </div>
            </v-card-item>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
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

const getDurationChangeClass = computed(() => {
  if (!weeklySummary.value) return ''
  return weeklySummary.value.durationChangePercentage >= 0 ? 'positive' : 'negative'
})

const getCaloriesChangeClass = computed(() => {
  if (!weeklySummary.value) return ''
  return weeklySummary.value.caloriesChangePercentage >= 0 ? 'positive' : 'negative'
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
.trends-container {
  padding-top: 20px;
  padding-bottom: 40px;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: 100vh;
}

.page-title {
  font-size: 28px;
  font-weight: 700;
  margin-bottom: 24px;
  color: #1f2937;
}

.summary-card {
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.summary-header {
  font-size: 16px;
  font-weight: 600;
  margin-bottom: 16px;
  color: #1f2937;
}

.summary-content {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
  border-bottom: 1px solid rgba(0, 0, 0, 0.06);
}

.summary-row:last-child {
  border-bottom: none;
}

.summary-label {
  font-size: 14px;
  color: rgba(0, 0, 0, 0.6);
}

.summary-value {
  font-size: 16px;
  font-weight: 600;
  color: #1f2937;
}

.summary-value.positive {
  color: #10b981;
}

.summary-value.negative {
  color: #ef4444;
}
</style>
