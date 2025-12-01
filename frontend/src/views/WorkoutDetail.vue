<template>
  <div class="min-h-screen py-8">
    <div class="max-w-7xl mx-auto px-6">
      <!-- Header -->
      <div class="mb-8">
        <button
          @click="$router.back()"
          class="flex items-center text-gray-600 hover:text-primary mb-4 transition-colors"
        >
          <svg class="w-5 h-5 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          {{ $t('common.back') }}
        </button>
        <h1 class="text-3xl font-bold gradient-text mb-1">
          {{ selectedDate }}
        </h1>
        <div class="text-sm text-gray-500">
          {{ dayName }}
        </div>
      </div>

      <!-- Weekly Calendar Selector -->
      <div class="mb-8">
        <WeeklyCalendar
          :selected-date="selectedDate"
          :start-date="weekStartDate"
          @select-date="onSelectDate"
        />
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="mb-8">
        <div class="animate-pulse space-y-4">
          <div class="h-32 bg-gray-200 rounded-2xl"></div>
          <div class="h-24 bg-gray-200 rounded-2xl"></div>
          <div class="h-24 bg-gray-200 rounded-2xl"></div>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="mb-8">
        <ErrorMessage :message="error" />
      </div>

      <!-- Empty State -->
      <div v-else-if="!dailyWorkout || dailyWorkout.records.length === 0" class="mb-8">
        <div class="glass rounded-2xl p-12 text-center border border-dashed border-gray-300">
          <div class="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg class="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 10h4.764a2 2 0 011.789 2.894l-3.5 7A2 2 0 0115.263 21h-4.017c-.163 0-.326-.02-.485-.06L7 20m7-10V5a2 2 0 00-2-2h-.095c-.5 0-.905.405-.905.905 0 .714-.211 1.412-.608 2.006L7 11v9m7-10h-2M7 20H5a2 2 0 01-2-2v-6a2 2 0 012-2h2.5" />
            </svg>
          </div>
          <div class="text-lg font-medium text-gray-900 mb-1">{{ $t('workout.noRecords') }}</div>
          <div class="text-sm text-gray-500">
            {{ $t('workout.addFirstWorkout') }}
          </div>
        </div>
      </div>

      <!-- Daily Total Summary -->
      <div v-else class="mb-8">
        <DailyTotalCard
          :total-duration-minutes="dailyWorkout.totalDurationMinutes"
          :total-calories-burned="dailyWorkout.totalCaloriesBurned"
          :record-count="dailyWorkout.recordCount"
        />
      </div>

      <!-- Workout Records List -->
      <div v-if="dailyWorkout && dailyWorkout.records.length > 0" class="mb-8">
        <div class="text-sm font-semibold text-gray-500 mb-4 uppercase tracking-wider">{{ $t('workout.records') }}</div>
        <div class="space-y-4">
          <DailyWorkoutCard
            v-for="(record, index) in dailyWorkout.records"
            :key="`${record.id}-${index}`"
            :record="record"
            show-notes
            show-actions
            @edit="onEditRecord(record)"
            @delete="onDeleteRecord(record)"
          />
        </div>
      </div>

      <!-- Add Workout Button -->
      <div class="mb-8">
        <button
          class="w-full bg-gradient-primary text-white font-semibold py-4 rounded-2xl shadow-lg hover:shadow-xl transition-all duration-300 flex items-center justify-center gap-2 hover:scale-[1.02]"
          @click="onAddWorkout"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          {{ $t('workout.addWorkout') }}
        </button>
      </div>
    </div>

    <!-- Edit Dialog -->
    <EditWorkoutDialog
      v-if="editingRecord"
      :open="showEditDialog"
      :record="editingRecord"
      @close="showEditDialog = false"
      @save="onEditSave"
    />

    <!-- Delete Dialog -->
    <DeleteWorkoutDialog
      v-if="deletingRecord"
      :open="showDeleteDialog"
      :record="deletingRecord"
      @close="showDeleteDialog = false"
      @confirm="onDeleteConfirm"
    />

    <!-- Add Dialog -->
    <AddWorkoutDialog
      :open="showAddDialog"
      @close="showAddDialog = false"
      @save="onAddSave"
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

function parseRouteDate(dateParam: string): string {
  // 處理特殊的 'today' 參數
  if (dateParam === 'today') {
    return formatDate(new Date())
  }
  // 驗證日期格式是否有效
  const parsed = new Date(dateParam)
  if (isNaN(parsed.getTime())) {
    return formatDate(new Date())
  }
  return dateParam
}

onMounted(async () => {
  if (route.params.date) {
    selectedDate.value = parseRouteDate(String(route.params.date))
  }
  weekStartDate.value = getWeekStart(new Date(selectedDate.value))
  await loadDailyWorkout()
})

watch(selectedDate, async () => {
  await loadDailyWorkout()
})
</script>

<style scoped>
/* Removed Vuetify specific styles */
</style>
