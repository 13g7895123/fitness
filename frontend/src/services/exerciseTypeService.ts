import api from './api'
import type { ExerciseType, CreateExerciseTypeDto, UpdateExerciseTypeDto, Equipment, CreateEquipmentDto, UpdateEquipmentDto } from '@/types/exercise-type'

export interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
}

// Exercise Type APIs
export const getAllExerciseTypes = async (): Promise<ExerciseType[]> => {
  const response = await api.get<ApiResponse<ExerciseType[]>>('/exercise-types')
  return response.data.data
}

export const getExerciseTypeById = async (id: number): Promise<ExerciseType> => {
  const response = await api.get<ApiResponse<ExerciseType>>(`/exercise-types/${id}`)
  return response.data.data
}

export const searchExerciseTypes = async (query: string): Promise<ExerciseType[]> => {
  const response = await api.get<ApiResponse<ExerciseType[]>>('/exercise-types/search', {
    params: { query }
  })
  return response.data.data
}

export const createExerciseType = async (dto: CreateExerciseTypeDto): Promise<ExerciseType> => {
  const response = await api.post<ApiResponse<ExerciseType>>('/exercise-types', dto)
  return response.data.data
}

export const updateExerciseType = async (id: number, dto: UpdateExerciseTypeDto): Promise<ExerciseType> => {
  const response = await api.patch<ApiResponse<ExerciseType>>(`/exercise-types/${id}`, dto)
  return response.data.data
}

export const deleteExerciseType = async (id: number): Promise<void> => {
  await api.delete(`/exercise-types/${id}`)
}

// Equipment APIs
export const getAllEquipments = async (): Promise<Equipment[]> => {
  const response = await api.get<ApiResponse<Equipment[]>>('/equipments')
  return response.data.data
}

export const getEquipmentById = async (id: number): Promise<Equipment> => {
  const response = await api.get<ApiResponse<Equipment>>(`/equipments/${id}`)
  return response.data.data
}

export const createEquipment = async (dto: CreateEquipmentDto): Promise<Equipment> => {
  const response = await api.post<ApiResponse<Equipment>>('/equipments', dto)
  return response.data.data
}

export const updateEquipment = async (id: number, dto: UpdateEquipmentDto): Promise<Equipment> => {
  const response = await api.patch<ApiResponse<Equipment>>(`/equipments/${id}`, dto)
  return response.data.data
}

export const deleteEquipment = async (id: number): Promise<void> => {
  await api.delete(`/equipments/${id}`)
}
