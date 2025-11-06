<template>
  <div class="min-h-screen p-6">
    <div class="max-w-content mx-auto">
      <div class="mb-8">
        <h1 class="text-3xl font-bold mb-2">{{ $t('statistics.weekly') }}</h1>
        <p class="text-gray-600">{{ new Date().toLocaleDateString('zh-TW', { year: 'numeric', month: 'long', day: 'numeric' }) }}</p>
      </div>

      <div v-if="isLoading" class="flex justify-center py-12">
        <div class="w-12 h-12 border-4 border-primary border-t-transparent rounded-full animate-spin"></div>
      </div>

      <div v-else-if="hasError" class="alert alert-error mb-4">
        {{ statisticsStore.error }}
      </div>

      <div v-else-if="!hasData" class="text-center py-12">
        <div class="card p-12">
          <svg class="w-16 h-16 mx-auto mb-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
          </svg>
          <h2 class="text-xl font-bold mb-2">{{ $t('statistics.noData') }}</h2>
          <p class="text-gray-600 mb-6">{{ $t('workout.addRecord') }}</p>
          <Button variant="primary" @click="showAddDialog">
            {{ $t('workout.addRecord') }}
          </Button>
        </div>
      </div>

      <div v-else class="space-y-6">
        <div class="grid grid-cols-1 gap-6">
          <WeeklySummaryCard />
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <WeeklyComparisonCard />
          <Card title=" $t('common.quickAction')">
            <div class="space-y-3">
              <Button variant="primary" block @click="showAddDialog">
                {{ $t('workout.addRecord') }}
              </Button>
              <Button variant="outlined" block @click="handleViewDetail">
                {{ $t('statistics.dailyBreakdown') }}
              </Button>
            </div>
          </Card>
        </div>

        <div class="grid grid-cols-1 gap-6">
          <DailyBarChart />
        </div>
      </div>
    </div>

    <AddWorkoutDialog
      v-model="isAddDialogOpen"
      @success="refreshData"
    />

    <EditWorkoutDialog
      v-if="selectedRecord"
      v-model="isEditDialogOpen"
      :record="selectedRecord"
      @success="refreshData"
    />

    <DeleteWorkoutDialog
      v-if="selectedRecord"
      v-model="isDeleteDialogOpen"
      :record="selectedRecord"
      @success="refreshData"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useStatisticsStore } from '@/stores/statistics'
import { useWorkoutsStore } from '@/stores/workouts'
import Button from '@/components/common/Button.vue'
import Card from '@/components/common/Card.vue'
import WeeklySummaryCard from '@/components/charts/WeeklySummaryCard.vue'
import WeeklyComparisonCard from '@/components/charts/WeeklyComparisonCard.vue'
import DailyBarChart from '@/components/charts/DailyBarChart.vue'
import AddWorkoutDialog from '@/components/workout/AddWorkoutDialog.vue'
import EditWorkoutDialog from '@/components/workout/EditWorkoutDialog.vue'
import DeleteWorkoutDialog from '@/components/workout/DeleteWorkoutDialog.vue'
import type { WorkoutRecordResponseDto } from '@/types/workout'

const { t } = useI18n()
const router = useRouter()
const statisticsStore = useStatisticsStore()
const workoutsStore = useWorkoutsStore()

const isAddDialogOpen = ref(false)
const isEditDialogOpen = ref(false)
const isDeleteDialogOpen = ref(false)
const selectedRecord = ref<WorkoutRecordResponseDto | null>(null)

const isLoading = computed(() => statisticsStore.isLoading)
const hasError = computed(() => statisticsStore.hasError)
const hasData = computed(() => !!statisticsStore.weeklyStats)

onMounted(async () => {
  await refreshData()
})

const refreshData = async () => {
  await statisticsStore.fetchWeeklyStats()
}

const showAddDialog = () => {
  isAddDialogOpen.value = true
}

const handleViewDetail = () => {
  router.push('/workouts/detail/today')
}
</script>
