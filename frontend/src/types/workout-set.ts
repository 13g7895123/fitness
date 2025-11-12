export interface WorkoutSetDto {
  id: number
  workoutRecordId: number
  setNumber: number
  repetitions: number
  weight?: number
  notes?: string
  createdAt: string
  updatedAt: string
}

export interface CreateWorkoutSetDto {
  workoutRecordId: number
  repetitions: number
  weight?: number
  notes?: string
}

export interface UpdateWorkoutSetDto {
  repetitions: number
  weight?: number
  notes?: string
}
