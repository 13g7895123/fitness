import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { WorkoutRecordResponseDto, CreateWorkoutRecordDto, UpdateWorkoutRecordDto } from '@/types/workout'

export const useWorkoutsStore = defineStore('workouts', () => {
  // State
  const records = ref<WorkoutRecordResponseDto[]>([])
  const currentRecord = ref<WorkoutRecordResponseDto | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const pagination = ref({
    pageNumber: 1,
    pageSize: 10,
    total: 0,
    totalPages: 0
  })

  // Computed
  const hasRecords = computed(() => records.value.length > 0)
  const isError = computed(() => !!error)
  const pageRecords = computed(() => {
    const start = (pagination.value.pageNumber - 1) * pagination.value.pageSize
    const end = start + pagination.value.pageSize
    return records.value.slice(start, end)
  })

  // Methods
  const setRecords = (newRecords: WorkoutRecordResponseDto[]) => {
    records.value = newRecords
  }

  const addRecord = (record: WorkoutRecordResponseDto) => {
    records.value.unshift(record)
    pagination.value.total += 1
  }

  const updateRecord = (id: string, updatedRecord: WorkoutRecordResponseDto) => {
    const index = records.value.findIndex(r => r.id === id)
    if (index > -1) {
      records.value[index] = updatedRecord
    }
  }

  const deleteRecord = (id: string) => {
    records.value = records.value.filter(r => r.id !== id)
    pagination.value.total -= 1
  }

  const setCurrentRecord = (record: WorkoutRecordResponseDto | null) => {
    currentRecord.value = record
  }

  const setLoading = (value: boolean) => {
    isLoading.value = value
  }

  const setError = (message: string | null) => {
    error.value = message
  }

  const setPagination = (pageNumber: number, pageSize: number, total: number) => {
    pagination.value.pageNumber = pageNumber
    pagination.value.pageSize = pageSize
    pagination.value.total = total
    pagination.value.totalPages = Math.ceil(total / pageSize)
  }

  const clearError = () => {
    error.value = null
  }

  const clearData = () => {
    records.value = []
    currentRecord.value = null
    error.value = null
    pagination.value = {
      pageNumber: 1,
      pageSize: 10,
      total: 0,
      totalPages: 0
    }
  }

  return {
    // State
    records,
    currentRecord,
    isLoading,
    error,
    pagination,
    
    // Computed
    hasRecords,
    isError,
    pageRecords,
    
    // Methods
    setRecords,
    addRecord,
    updateRecord,
    deleteRecord,
    setCurrentRecord,
    setLoading,
    setError,
    setPagination,
    clearError,
    clearData
  }
})
