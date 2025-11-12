export interface WorkoutRecordResponseDto {
  id: number
  userId: string
  exerciseDate: string
  exerciseTypeId: number
  exerciseTypeName: string
  equipmentId?: number
  equipmentName?: string
  durationMinutes: number
  caloriesBurned: number
  weight?: number
  notes?: string
  createdAt: string
  updatedAt?: string
}

export interface CreateWorkoutRecordDto {
  exerciseDate: string
  exerciseTypeId: number | string
  equipmentId?: number | string
  notes?: string
}

export interface UpdateWorkoutRecordDto {
  exerciseDate?: string
  exerciseTypeId?: number | string
  equipmentId?: number | string
  notes?: string
}

export interface PaginatedResponse<T> {
  items: T[]
  total: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}
