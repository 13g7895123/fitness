import type { WorkoutRecordResponseDto } from './workout'

export interface DailyWorkoutDto {
  date: string
  dayOfWeek: number
  dayName: string
  records: WorkoutRecordResponseDto[]
  totalDurationMinutes: number
  totalCaloriesBurned: number
  recordCount: number
  isToday: boolean
}
