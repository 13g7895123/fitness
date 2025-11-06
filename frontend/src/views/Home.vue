<template>
  <v-container class="home-page" fluid>
    <div class="page-header">
      <h1>{{ $t('app.title') }}</h1>
      <p>{{ $t('statistics.weekly') }}</p>
    </div>

    <Loading :visible="isLoading" :text="$t('common.loading')" />

    <div v-if="!isLoading" class="content">
      <v-alert
        v-if="hasError"
        type="error"
        :title="$t('common.error')"
        closable
        class="mb-4"
      >
        {{ statisticsStore.error }}
      </v-alert>

      <v-empty-state
        v-if="!hasData && !isLoading"
        :headline="$t('statistics.noData')"
      >
        <template #media>
          <v-icon size="64" color="disabled">mdi-chart-box-outline</v-icon>
        </template>
        <p class="mt-4">{{ $t('workout.addRecord') }}</p>
        <v-btn color="primary" @click="showAddDialog">
          <v-icon start>mdi-plus</v-icon>
          {{ $t('workout.addRecord') }}
        </v-btn>
      </v-empty-state>

      <div v-if="hasData">
        <v-row>
          <v-col cols="12">
            <WeeklySummaryCard />
          </v-col>
        </v-row>

        <v-row>
          <v-col cols="12" md="6">
            <WeeklyComparisonCard />
          </v-col>
          <v-col cols="12" md="6">
            <v-card class="action-card">
              <v-card-title>{{ $t('common.quickAction') }}</v-card-title>
              <v-card-text>
                <v-btn color="primary" block class="mb-2" @click="showAddDialog">
                  <v-icon start>mdi-plus</v-icon>
                  {{ $t('workout.addRecord') }}
                </v-btn>
                <v-btn color="secondary" block @click="handleViewDetail">
                  <v-icon start>mdi-eye</v-icon>
                  {{ $t('statistics.dailyBreakdown') }}
                </v-btn>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>

        <v-row>
          <v-col cols="12">
            <DailyBarChart />
          </v-col>
        </v-row>
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
.home-page {
  padding: 24px;
}

.page-header {
  margin-bottom: 32px;
}

.page-header h1 {
  margin: 0 0 8px 0;
  font-size: 28px;
  font-weight: bold;
}

.page-header p {
  margin: 0;
  color: #999;
}

.content {
  width: 100%;
}

.action-card {
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.mb-2 {
  margin-bottom: 8px !important;
}

.mb-4 {
  margin-bottom: 16px !important;
}

.mt-4 {
  margin-top: 16px !important;
}
</style>
