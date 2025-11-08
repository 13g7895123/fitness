export interface DailyBreakdownDto {
  date: string
  durationMinutes: number
  caloriesBurned: number
  workoutCount: number
  hasWorkout: boolean
}

export interface WeeklySummaryDto {
  weekStartDate: string
  weekEndDate: string
  totalDurationMinutes: number
  totalCaloriesBurned: number
  workoutDays: number
  totalWorkoutCount: number
  durationChangePercentage: number
  caloriesChangePercentage: number
  dailyBreakdown: DailyBreakdownDto[]
}

export interface TrendDataDto {
  date: string
  durationMinutes: number
  caloriesBurned: number
  recordCount: number
  periodType: string
}

export interface MonthlySummaryDto {
  month: string
  totalDurationMinutes: number
  totalCaloriesBurned: number
  workoutDays: number
  totalRecords: number
  averageDailyDuration: number
  averageDailyCalories: number
}

export interface ExerciseDistributionDto {
  exerciseName: string
  totalDurationMinutes: number
  totalCaloriesBurned: number
  recordCount: number
  percentageOfTotal: number
}
