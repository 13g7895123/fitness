<template>
  <div class="min-h-screen py-8">
    <div class="max-w-7xl mx-auto px-6">
      <!-- Header -->
      <div class="mb-8 animate-fade-in-up">
        <button
          @click="$router.back()"
          class="flex items-center space-x-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"/>
          </svg>
          <span class="font-medium">{{ $t('common.back') }}</span>
        </button>
        <div class="flex items-center space-x-3 mb-2">
          <div class="w-10 h-10 rounded-xl bg-gradient-primary flex items-center justify-center shadow-glow">
            <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 10h4.764a2 2 0 011.789 2.894l-3.5 7A2 2 0 0115.263 21h-4.017c-.163 0-.326-.02-.485-.06L7 20m7-10V5a2 2 0 00-2-2h-.095c-.5 0-.905.405-.905.905 0 .714-.211 1.412-.608 2.006L7 11v9m7-10h-2M7 20H5a2 2 0 01-2-2v-6a2 2 0 012-2h2.5"/>
            </svg>
          </div>
          <div>
            <h1 class="text-3xl font-bold gradient-text">{{ formattedDate }}</h1>
            <p class="text-sm text-gray-600 mt-0.5">{{ dayName }}</p>
          </div>
        </div>
      </div>

      <!-- Weekly Calendar Selector -->
      <div class="mb-6 animate-fade-in">
        <WeeklyCalendar
          :selected-date="selectedDate"
          :start-date="weekStartDate"
          @select-date="onSelectDate"
        />
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="space-y-6 animate-fade-in">
        <div class="glass rounded-3xl p-6">
          <div class="animate-pulse space-y-4">
            <div class="h-4 bg-gray-200 rounded w-1/4"></div>
            <div class="h-8 bg-gray-200 rounded w-1/2"></div>
            <div class="h-4 bg-gray-200 rounded w-3/4"></div>
          </div>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="animate-fade-in">
        <div class="glass border border-error/20 rounded-2xl p-6">
          <div class="flex items-start space-x-3">
            <svg class="w-6 h-6 text-error flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
            <div>
              <h3 class="font-semibold text-error mb-1">發生錯誤</h3>
              <p class="text-gray-600 text-sm">{{ error }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else-if="!dailyWorkout || dailyWorkout.records.length === 0" class="text-center py-20 animate-fade-in-up">
        <div class="inline-flex items-center justify-center w-24 h-24 bg-gradient-primary rounded-3xl mb-6 shadow-glow">
          <svg class="w-12 h-12 text-[#555]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 10h4.764a2 2 0 011.789 2.894l-3.5 7A2 2 0 0115.263 21h-4.017c-.163 0-.326-.02-.485-.06L7 20m7-10V5a2 2 0 00-2-2h-.095c-.5 0-.905.405-.905.905 0 .714-.211 1.412-.608 2.006L7 11v9m7-10h-2M7 20H5a2 2 0 01-2-2v-6a2 2 0 012-2h2.5"/>
          </svg>
        </div>
        <h2 class="text-3xl font-bold mb-3 gradient-text">今天還沒有訓練記錄</h2>
        <p class="text-gray-600 mb-8 text-lg max-w-md mx-auto">
          開始記錄你的第一次訓練，追蹤進度達成目標
        </p>
        <Button
          variant="primary"
          size="large"
          @click="onAddWorkout"
        >
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
          </svg>
          <span>{{ $t('workout.addWorkout') }}</span>
        </Button>
      </div>

      <!-- Content -->
      <div v-else class="space-y-6 animate-fade-in">
        <!-- Daily Total Summary -->
        <div class="glass rounded-3xl p-6 shadow-soft card-hover">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-bold text-gray-900">今日總計</h3>
            <svg class="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
            </svg>
          </div>
          <div class="grid grid-cols-3 gap-4">
            <div class="text-center">
              <div class="text-3xl font-bold text-primary mb-1">{{ dailyWorkout.totalDurationMinutes }}</div>
              <div class="text-sm text-gray-600">分鐘</div>
            </div>
            <div class="text-center">
              <div class="text-3xl font-bold text-secondary mb-1">{{ dailyWorkout.totalCaloriesBurned }}</div>
              <div class="text-sm text-gray-600">卡路里</div>
            </div>
            <div class="text-center">
              <div class="text-3xl font-bold text-accent mb-1">{{ dailyWorkout.recordCount }}</div>
              <div class="text-sm text-gray-600">項目</div>
            </div>
          </div>
        </div>

        <!-- Workout Records List -->
        <div>
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-bold text-gray-900">訓練記錄</h3>
            <span class="text-sm text-gray-500">共 {{ dailyWorkout.records.length }} 筆</span>
          </div>
          <div class="space-y-4">
            <div
              v-for="(record, index) in dailyWorkout.records"
              :key="`${record.id}-${index}`"
              class="glass rounded-2xl p-5 shadow-soft card-hover"
            >
              <div class="flex items-start justify-between mb-3">
                <div class="flex-1">
                  <h4 class="text-lg font-semibold text-gray-900 mb-1">{{ record.exerciseTypeName }}</h4>
                  <div class="flex items-center space-x-4 text-sm text-gray-600">
                    <span class="flex items-center space-x-1">
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                      </svg>
                      <span>{{ record.durationMinutes }} 分鐘</span>
                    </span>
                    <span class="flex items-center space-x-1">
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 18.657A8 8 0 016.343 7.343S7 9 9 10c0-2 .5-5 2.986-7C14 5 16.09 5.777 17.656 7.343A7.975 7.975 0 0120 13a7.975 7.975 0 01-2.343 5.657z"/>
                      </svg>
                      <span>{{ record.caloriesBurned }} 卡</span>
                    </span>
                  </div>
                </div>
                <div class="flex space-x-2">
                  <button
                    @click="onEditRecord(record)"
                    class="p-2 rounded-lg hover:bg-gray-100 transition-colors"
                    title="編輯"
                  >
                    <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                    </svg>
                  </button>
                  <button
                    @click="onDeleteRecord(record)"
                    class="p-2 rounded-lg hover:bg-red-50 transition-colors"
                    title="刪除"
                  >
                    <svg class="w-5 h-5 text-error" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                    </svg>
                  </button>
                </div>
              </div>
              <div v-if="record.notes" class="text-sm text-gray-600 bg-gray-50 rounded-lg p-3">
                {{ record.notes }}
              </div>
            </div>
          </div>
        </div>

        <!-- Add Workout Button -->
        <Button variant="primary" size="large" block @click="onAddWorkout">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
          </svg>
          <span>新增訓練記錄</span>
        </Button>
      </div>
    </div>

    <!-- Edit Dialog -->
    <EditWorkoutDialog
      v-if="editingRecord"
      v-model="showEditDialog"
      :record="editingRecord"
      @success="onEditSave"
    />

    <!-- Delete Dialog -->
    <DeleteWorkoutDialog
      v-if="deletingRecord"
      v-model="showDeleteDialog"
      :record="deletingRecord"
      @success="onDeleteConfirm"
    />

    <!-- Add Dialog -->
    <AddWorkoutDialog
      v-model="showAddDialog"
      @success="onAddSave"
    />

    <!-- Notifications -->
    <NotificationPanel />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useWorkoutsStore } from '@/stores/workouts'
import Button from '@/components/common/Button.vue'
import DailyTotalCard from '@/components/workout/DailyTotalCard.vue'
import DailyWorkoutCard from '@/components/workout/DailyWorkoutCard.vue'
import WeeklyCalendar from '@/components/workout/WeeklyCalendar.vue'
import EditWorkoutDialog from '@/components/workout/EditWorkoutDialog.vue'
import DeleteWorkoutDialog from '@/components/workout/DeleteWorkoutDialog.vue'
import AddWorkoutDialog from '@/components/workout/AddWorkoutDialog.vue'
import ErrorMessage from '@/components/common/ErrorMessage.vue'
import NotificationPanel from '@/components/common/NotificationPanel.vue'
import { useWorkoutService } from '@/services/workoutService'
import type { WorkoutRecordResponseDto } from '@/types/workout'
import type { DailyWorkoutDto } from '@/types/daily-workout'

const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const workoutsStore = useWorkoutsStore()
const workoutService = useWorkoutService()

const selectedDate = ref<string>(formatDate(new Date()))
const weekStartDate = ref<string>(getWeekStart(new Date()))
const dailyWorkout = ref<DailyWorkoutDto | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)

const showEditDialog = ref(false)
const showDeleteDialog = ref(false)
const showAddDialog = ref(false)
const editingRecord = ref<WorkoutRecordResponseDto | null>(null)
const deletingRecord = ref<WorkoutRecordResponseDto | null>(null)

const dayName = computed(() => {
  if (!dailyWorkout.value) return ''
  return dailyWorkout.value.dayName
})

const formattedDate = computed(() => {
  const date = new Date(selectedDate.value)
  return date.toLocaleDateString('zh-TW', { 
    year: 'numeric', 
    month: 'long', 
    day: 'numeric'
  })
})

function formatDate(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

function getWeekStart(date: Date): string {
  const d = new Date(date)
  const day = d.getDay()
  const diff = d.getDate() - day + (day === 0 ? -6 : 1)
  return formatDate(new Date(d.setDate(diff)))
}

async function loadDailyWorkout() {
  try {
    loading.value = true
    error.value = null
    const response = await workoutService.getDailyWorkout(selectedDate.value)
    dailyWorkout.value = response.data
  } catch (err: any) {
    error.value = err.message || t('common.loadError')
    dailyWorkout.value = null
  } finally {
    loading.value = false
  }
}

function onSelectDate(date: string) {
  selectedDate.value = date
  weekStartDate.value = getWeekStart(new Date(date))
}

function onAddWorkout() {
  showAddDialog.value = true
}

async function onAddSave() {
  showAddDialog.value = false
  await loadDailyWorkout()
}

function onEditRecord(record: WorkoutRecordResponseDto) {
  editingRecord.value = record
  showEditDialog.value = true
}

async function onEditSave() {
  showEditDialog.value = false
  await loadDailyWorkout()
}

function onDeleteRecord(record: WorkoutRecordResponseDto) {
  deletingRecord.value = record
  showDeleteDialog.value = true
}

async function onDeleteConfirm() {
  showDeleteDialog.value = false
  await loadDailyWorkout()
}

onMounted(async () => {
  if (route.params.date) {
    let dateParam = String(route.params.date)

    // 如果是 "today" 關鍵字，轉換為當天日期
    if (dateParam.toLowerCase() === 'today') {
      const today = new Date()
      dateParam = formatDate(today)
      // 更新路由到正確的日期格式
      router.replace(`/workouts/detail/${dateParam}`)
    }

    selectedDate.value = dateParam
  }
  weekStartDate.value = getWeekStart(new Date(selectedDate.value))
  await loadDailyWorkout()
})

watch(selectedDate, async () => {
  await loadDailyWorkout()
})
</script>

<style scoped>
/* 所有樣式已使用 Tailwind CSS */
</style>
