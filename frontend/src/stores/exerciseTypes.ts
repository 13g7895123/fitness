import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { ExerciseType, CreateExerciseTypeDto, UpdateExerciseTypeDto, Equipment, CreateEquipmentDto, UpdateEquipmentDto } from '@/types/exercise-type'
import * as exerciseTypeService from '@/services/exerciseTypeService'

export const useExerciseTypesStore = defineStore('exerciseTypes', () => {
  // State
  const exerciseTypes = ref<ExerciseType[]>([])
  const equipments = ref<Equipment[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const searchQuery = ref('')

  // Getters
  const systemExerciseTypes = computed(() => 
    exerciseTypes.value.filter(e => e.isSystemDefault && !e.isDisabled)
  )

  const customExerciseTypes = computed(() => 
    exerciseTypes.value.filter(e => !e.isSystemDefault && !e.isDisabled)
  )

  const filteredExerciseTypes = computed(() => {
    if (!searchQuery.value) {
      return exerciseTypes.value.filter(e => !e.isDisabled)
    }
    const query = searchQuery.value.toLowerCase()
    return exerciseTypes.value.filter(e => 
      !e.isDisabled && e.name.toLowerCase().includes(query)
    )
  })

  const systemEquipments = computed(() => 
    equipments.value.filter(e => e.isSystemDefault && !e.isDisabled)
  )

  const customEquipments = computed(() => 
    equipments.value.filter(e => !e.isSystemDefault && !e.isDisabled)
  )

  // Actions - Exercise Types
  const fetchExerciseTypes = async () => {
    loading.value = true
    error.value = null
    try {
      exerciseTypes.value = await exerciseTypeService.getAllExerciseTypes()
    } catch (err: any) {
      error.value = err.message || '獲取運動類型失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  const searchExerciseTypes = async (query: string) => {
    loading.value = true
    error.value = null
    searchQuery.value = query
    try {
      if (query) {
        exerciseTypes.value = await exerciseTypeService.searchExerciseTypes(query)
      } else {
        await fetchExerciseTypes()
      }
    } catch (err: any) {
      error.value = err.message || '搜尋運動類型失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  const createExerciseType = async (dto: CreateExerciseTypeDto) => {
    loading.value = true
    error.value = null
    try {
      const newExerciseType = await exerciseTypeService.createExerciseType(dto)
      exerciseTypes.value.push(newExerciseType)
      return newExerciseType
    } catch (err: any) {
      error.value = err.message || '建立運動類型失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  const updateExerciseType = async (id: number, dto: UpdateExerciseTypeDto) => {
    loading.value = true
    error.value = null
    try {
      const updated = await exerciseTypeService.updateExerciseType(id, dto)
      const index = exerciseTypes.value.findIndex(e => e.id === id)
      if (index !== -1) {
        exerciseTypes.value[index] = updated
      }
      return updated
    } catch (err: any) {
      error.value = err.message || '更新運動類型失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  const deleteExerciseType = async (id: number) => {
    loading.value = true
    error.value = null
    try {
      await exerciseTypeService.deleteExerciseType(id)
      exerciseTypes.value = exerciseTypes.value.filter(e => e.id !== id)
    } catch (err: any) {
      error.value = err.message || '刪除運動類型失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Actions - Equipments
  const fetchEquipments = async () => {
    loading.value = true
    error.value = null
    try {
      equipments.value = await exerciseTypeService.getAllEquipments()
    } catch (err: any) {
      error.value = err.message || '獲取器材失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  const createEquipment = async (dto: CreateEquipmentDto) => {
    loading.value = true
    error.value = null
    try {
      const newEquipment = await exerciseTypeService.createEquipment(dto)
      equipments.value.push(newEquipment)
      return newEquipment
    } catch (err: any) {
      error.value = err.message || '建立器材失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  const updateEquipment = async (id: number, dto: UpdateEquipmentDto) => {
    loading.value = true
    error.value = null
    try {
      const updated = await exerciseTypeService.updateEquipment(id, dto)
      const index = equipments.value.findIndex(e => e.id === id)
      if (index !== -1) {
        equipments.value[index] = updated
      }
      return updated
    } catch (err: any) {
      error.value = err.message || '更新器材失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  const deleteEquipment = async (id: number) => {
    loading.value = true
    error.value = null
    try {
      await exerciseTypeService.deleteEquipment(id)
      equipments.value = equipments.value.filter(e => e.id !== id)
    } catch (err: any) {
      error.value = err.message || '刪除器材失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  const clearError = () => {
    error.value = null
  }

  return {
    // State
    exerciseTypes,
    equipments,
    loading,
    error,
    searchQuery,
    // Getters
    systemExerciseTypes,
    customExerciseTypes,
    filteredExerciseTypes,
    systemEquipments,
    customEquipments,
    // Actions
    fetchExerciseTypes,
    searchExerciseTypes,
    createExerciseType,
    updateExerciseType,
    deleteExerciseType,
    fetchEquipments,
    createEquipment,
    updateEquipment,
    deleteEquipment,
    clearError
  }
})
