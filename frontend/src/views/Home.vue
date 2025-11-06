<template>
  <div class="min-h-screen p-6 bg-gray-50">
    <div class="max-w-content mx-auto">
      <div class="mb-8">
        <h1 class="text-3xl font-bold mb-2">{{ $t('statistics.weekly') }}</h1>
        <p class="text-gray-600">{{ new Date().toLocaleDateString('zh-TW', { year: 'numeric', month: 'long', day: 'numeric' }) }}</p>
      </div>

      <Loading :visible="isLoading" :text="$t('common.loading')" />

      <div v-if="!isLoading">
        <div
          v-if="hasError"
          class="alert alert-error mb-4"
        >
          {{ statisticsStore.error }}
        </div>

        <div
          v-if="!hasData && !isLoading"
          class="text-center py-12"
        >
          <svg class="w-16 h-16 mx-auto mb-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
          </svg>
          <p class="text-xl font-semibold mb-2">{{ $t('statistics.noData') }}</p>
          <p class="text-gray-600 mb-6">{{ $t('workout.addRecord') }}</p>
          <Button variant="primary" @click="showAddDialog">
            {{ $t('workout.addRecord') }}
          </Button>
        </div>

        <div v-if="hasData" class="space-y-6">
          <div class="grid grid-cols-1 gap-6">
            <WeeklySummaryCard />
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <WeeklyComparisonCard />
            <Card :title="$t('common.quickAction')">
              <div class="space-y-3">
                <Button 
                  variant="primary" 
                  block 
                  size="large"
                  @click="showAddDialog"
                >
                  {{ $t('workout.addRecord') }}
                </Button>
                <Button 
                  variant="outlined" 
                  block 
                  size="large"
                  @click="handleViewDetail"
                >
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
    </div>

    <!-- 新增紀錄對話框 -->
    <AddWorkoutDialog
      v-model="isAddDialogOpen"
      @success="refreshData"
    />

    <!-- 編輯紀錄對話框 -->
    <EditWorkoutDialog
      v-if="selectedRecord"
      v-model="isEditDialogOpen"
      :record="selectedRecord"
      @success="refreshData"
    />

    <!-- 刪除確認對話框 -->
    <DeleteWorkoutDialog
      v-if="selectedRecord"
      v-model="isDeleteDialogOpen"
      :record="selectedRecord"
      @success="refreshData"
    />

    <!-- 通知面板 -->
    <NotificationPanel />
  </v-container>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStatisticsStore } from '@/stores/statistics'
import { useStatisticsService } from '@/services/statisticsService'
import { useWorkoutsStore } from '@/stores/workouts'
import Button from '@/components/common/Button.vue'
import Card from '@/components/common/Card.vue'
import WeeklySummaryCard from '@/components/workout/WeeklySummaryCard.vue'
import WeeklyComparisonCard from '@/components/workout/WeeklyComparisonCard.vue'
import DailyBarChart from '@/components/charts/DailyBarChart.vue'
import Loading from '@/components/common/Loading.vue'
import AddWorkoutDialog from '@/components/workout/AddWorkoutDialog.vue'
import EditWorkoutDialog from '@/components/workout/EditWorkoutDialog.vue'
import DeleteWorkoutDialog from '@/components/workout/DeleteWorkoutDialog.vue'
import NotificationPanel from '@/components/common/NotificationPanel.vue'

const router = useRouter()
const statisticsStore = useStatisticsStore()
const workoutsStore = useWorkoutsStore()
const { getWeeklySummary } = useStatisticsService()

const isLoading = computed(() => statisticsStore.isLoading)
const hasData = computed(() => statisticsStore.hasData)
const hasError = computed(() => !!statisticsStore.error)

const isAddDialogOpen = ref(false)
const isEditDialogOpen = ref(false)
const isDeleteDialogOpen = ref(false)
const selectedRecord = ref(null)

const showAddDialog = () => {
  isAddDialogOpen.value = true
}

const handleViewDetail = () => {
  const today = new Date()
  const dateStr = today.toISOString().split('T')[0]
  router.push(`/workouts/detail/${dateStr}`)
}

const refreshData = async () => {
  await getWeeklySummary()
}

onMounted(async () => {
  await getWeeklySummary()
})
</script>

<style scoped>
/* Tailwind handles all styling */
</style>
