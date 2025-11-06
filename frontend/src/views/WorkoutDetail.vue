<template>
  <div class="workout-detail-page">
    <v-container class="py-6" fluid style="max-width: 1280px; margin: 0 auto;">
      <!-- Header -->
      <div class="mb-6">
        <v-btn
          variant="text"
          prepend-icon="mdi-arrow-left"
          @click="$router.back()"
          class="mb-3"
        >
          {{ $t('common.back') }}
        </v-btn>
        <h1 class="text-h4 font-weight-bold">
          {{ selectedDate }}
        </h1>
        <div class="text-subtitle2 text-medium-emphasis">
          {{ dayName }}
        </div>
      </div>

      <!-- Weekly Calendar Selector -->
      <v-row class="mb-6">
        <v-col cols="12">
          <WeeklyCalendar
            :selected-date="selectedDate"
            :start-date="weekStartDate"
            @select-date="onSelectDate"
          />
        </v-col>
      </v-row>

      <!-- Loading State -->
      <v-row v-if="loading" class="mb-6">
        <v-col cols="12">
          <v-skeleton-loader type="card-heading, image, article" />
        </v-col>
      </v-row>

      <!-- Error State -->
      <v-row v-else-if="error" class="mb-6">
        <v-col cols="12">
          <ErrorMessage :message="error" />
        </v-col>
      </v-row>

      <!-- Empty State -->
      <v-row v-else-if="!dailyWorkout || dailyWorkout.records.length === 0" class="mb-6">
        <v-col cols="12">
          <v-card class="text-center py-8">
            <v-icon size="x-large" class="mb-2 text-medium-emphasis">mdi-dumbbell</v-icon>
            <div class="text-subtitle1">{{ $t('workout.noRecords') }}</div>
            <div class="text-caption text-medium-emphasis">
              {{ $t('workout.addFirstWorkout') }}
            </div>
          </v-card>
        </v-col>
      </v-row>

      <!-- Daily Total Summary -->
      <v-row v-else class="mb-6">
        <v-col cols="12">
          <DailyTotalCard
            :total-duration-minutes="dailyWorkout.totalDurationMinutes"
            :total-calories-burned="dailyWorkout.totalCaloriesBurned"
            :record-count="dailyWorkout.recordCount"
          />
        </v-col>
      </v-row>

      <!-- Workout Records List -->
      <v-row v-if="dailyWorkout && dailyWorkout.records.length > 0" class="mb-6">
        <v-col cols="12">
          <div class="text-subtitle2 mb-3">{{ $t('workout.records') }}</div>
          <DailyWorkoutCard
            v-for="(record, index) in dailyWorkout.records"
            :key="`${record.id}-${index}`"
            :record="record"
            show-notes
            show-actions
            @edit="onEditRecord(record)"
            @delete="onDeleteRecord(record)"
          />
        </v-col>
      </v-row>

      <!-- Add Workout Button -->
      <v-row class="mb-6">
        <v-col cols="12">
          <v-btn
            block
            color="primary"
            prepend-icon="mdi-plus"
            @click="onAddWorkout"
          >
            {{ $t('workout.addWorkout') }}
          </v-btn>
        </v-col>
      </v-row>
    </v-container>

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

onMounted(async () => {
  if (route.params.date) {
    selectedDate.value = String(route.params.date)
  }
  weekStartDate.value = getWeekStart(new Date(selectedDate.value))
  await loadDailyWorkout()
})

watch(selectedDate, async () => {
  await loadDailyWorkout()
})
</script>

<style scoped>
.workout-detail-page {
  background-color: rgba(var(--v-theme-surface-variant), 0.2);
  min-height: 100vh;
}
</style>
