export interface Equipment {
  id: number
  name: string
  description?: string
  isSystemDefault: boolean
  createdAt: string
  isDisabled: boolean
}

export interface ExerciseType {
  id: number
  name: string
  description?: string
  isSystemDefault: boolean
  defaultMET: number
  createdAt: string
  equipments: Equipment[]
  workoutRecordCount: number
  isDisabled: boolean
}

export interface CreateExerciseTypeDto {
  name: string
  description?: string
  defaultMET: number
  equipmentIds: number[]
}

export interface UpdateExerciseTypeDto {
  name: string
  description?: string
  defaultMET: number
  equipmentIds: number[]
}

export interface CreateEquipmentDto {
  name: string
  description?: string
}

export interface UpdateEquipmentDto {
  name: string
  description?: string
}
