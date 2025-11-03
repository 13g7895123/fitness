export interface WorkoutRecordResponseDto {
  id: string
  userId: string
  date: string
  exerciseTypeId: string
  exerciseTypeName: string
  equipmentId?: string
  equipmentName?: string
  durationMinutes: number
  caloriesBurned: number
  weight?: number
  notes?: string
  createdAt: string
  updatedAt?: string
}

export interface CreateWorkoutRecordDto {
  date: string
  exerciseTypeId: string
  equipmentId?: string
  durationMinutes: number
  caloriesBurned?: number
  weight?: number
  notes?: string
}

export interface UpdateWorkoutRecordDto {
  date?: string
  exerciseTypeId?: string
  equipmentId?: string
  durationMinutes?: number
  caloriesBurned?: number
  weight?: number
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
