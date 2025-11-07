<template>
  <div class="min-h-screen">
    <!-- Hero Section -->
    <div class="relative overflow-hidden bg-gradient-to-br from-primary-600 via-purple-600 to-secondary-600 text-white">
      <div class="absolute inset-0 bg-mesh-gradient opacity-20"></div>
      <div class="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDQyYzMuMzEgMCA2IDIuNjkgNiA2cy0yLjY5IDYtNiA2LTYtMi42OS02LTYgMi42OS02IDYtNnoiIHN0cm9rZT0iI2ZmZiIgc3Ryb2tlLW9wYWNpdHk9Ii4wNSIvPjwvZz48L3N2Zz4=')] opacity-30"></div>
      
      <div class="relative max-w-7xl mx-auto px-6 py-12">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
          <div class="flex-1 animate-fade-in-up">
            <div class="inline-flex items-center space-x-2 bg-white/20 backdrop-blur-sm px-4 py-2 rounded-full mb-4">
              <span class="w-2 h-2 bg-success rounded-full animate-pulse-soft"></span>
              <span class="text-sm font-medium">本週進度追蹤</span>
            </div>
            <h1 class="text-4xl md:text-5xl font-display font-bold mb-3 leading-tight">
              繼續保持，<br class="md:hidden"/>你做得很棒！
            </h1>
            <p class="text-white/90 text-lg max-w-xl">
              {{ new Date().toLocaleDateString('zh-TW', { year: 'numeric', month: 'long', day: 'numeric', weekday: 'long' }) }}
            </p>
          </div>

          <!-- 快速統計卡片 -->
          <div class="grid grid-cols-3 gap-4 md:gap-6 animate-fade-in-down">
            <div class="glass text-center p-4 rounded-2xl">
              <div class="text-3xl font-bold mb-1">12</div>
              <div class="text-xs text-white/80">本週訓練</div>
            </div>
            <div class="glass text-center p-4 rounded-2xl">
              <div class="text-3xl font-bold mb-1">485</div>
              <div class="text-xs text-white/80">消耗卡路里</div>
            </div>
            <div class="glass text-center p-4 rounded-2xl">
              <div class="text-3xl font-bold mb-1">2.5</div>
              <div class="text-xs text-white/80">運動時數</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-6 py-8 -mt-8 relative z-10">
      <Loading :visible="isLoading" :text="$t('common.loading')" />

      <div v-if="!isLoading">
        <!-- Error State -->
        <div
          v-if="hasError"
          class="glass border border-error/20 rounded-2xl p-6 mb-6 animate-fade-in"
        >
          <div class="flex items-start space-x-3">
            <svg class="w-6 h-6 text-error flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
            <div>
              <h3 class="font-semibold text-error mb-1">發生錯誤</h3>
              <p class="text-gray-600 text-sm">{{ statisticsStore.error }}</p>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div
          v-if="!hasData && !isLoading"
          class="text-center py-20 animate-fade-in-up"
        >
          <div class="inline-flex items-center justify-center w-24 h-24 bg-gradient-primary rounded-3xl mb-6 shadow-glow">
            <svg class="w-12 h-12 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
          </div>
          <h2 class="text-3xl font-bold mb-3 gradient-text">開始你的健身之旅</h2>
          <p class="text-gray-600 mb-8 text-lg max-w-md mx-auto">
            記錄你的第一次訓練，追蹤進度，達成目標
          </p>
          <button
            @click="showAddDialog"
            class="inline-flex items-center space-x-2 bg-gradient-primary text-white px-8 py-4 rounded-2xl font-semibold shadow-glow hover:shadow-2xl transition-all duration-300 hover:scale-105 btn-hover"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
            </svg>
            <span>{{ $t('workout.addRecord') }}</span>
          </button>
        </div>

        <!-- Data View -->
        <div v-if="hasData" class="space-y-6 animate-fade-in">
          <!-- 週摘要卡片 -->
          <div class="glass rounded-3xl p-6 shadow-soft card-hover">
            <WeeklySummaryCard />
          </div>

          <!-- 雙欄佈局 -->
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <!-- 週比較卡片 -->
            <div class="glass rounded-3xl p-6 shadow-soft card-hover">
              <WeeklyComparisonCard />
            </div>

            <!-- 快速操作卡片 -->
            <div class="glass rounded-3xl p-6 shadow-soft">
              <div class="flex items-center justify-between mb-6">
                <h3 class="text-xl font-bold text-gray-900">快速操作</h3>
                <svg class="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"/>
                </svg>
              </div>
              
              <div class="space-y-3">
                <button
                  @click="showAddDialog"
                  class="w-full flex items-center justify-between bg-gradient-primary text-white p-5 rounded-2xl font-semibold shadow-medium hover:shadow-hard transition-all duration-300 group btn-hover"
                >
                  <span class="flex items-center space-x-3">
                    <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
                    </svg>
                    <span>新增訓練記錄</span>
                  </span>
                  <svg class="w-5 h-5 group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
                  </svg>
                </button>

                <button
                  @click="handleViewDetail"
                  class="w-full flex items-center justify-between bg-white border-2 border-gray-200 text-gray-700 p-5 rounded-2xl font-semibold hover:border-primary hover:text-primary transition-all duration-300 group"
                >
                  <span class="flex items-center space-x-3">
                    <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"/>
                    </svg>
                    <span>查看詳細記錄</span>
                  </span>
                  <svg class="w-5 h-5 group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
                  </svg>
                </button>

                <!-- 成就展示 -->
                <div class="bg-gradient-to-br from-yellow-50 to-orange-50 border border-yellow-200 p-4 rounded-2xl mt-4">
                  <div class="flex items-center space-x-3">
                    <div class="w-12 h-12 bg-gradient-warm rounded-xl flex items-center justify-center">
                      <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 20 20">
                        <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
                      </svg>
                    </div>
                    <div class="flex-1">
                      <div class="text-sm font-semibold text-gray-900">連續訓練 7 天！</div>
                      <div class="text-xs text-gray-600">繼續保持，再堅持 3 天解鎖新成就</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- 每日條形圖 -->
          <div class="glass rounded-3xl p-6 shadow-soft card-hover">
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
  </div>
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
