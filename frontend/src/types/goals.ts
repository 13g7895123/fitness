export interface WorkoutGoalDto {
  id: string
  userId: string
  weeklyMinutes?: number
  weeklyCalories?: number
  startDate: string
  endDate?: string
  isActive: boolean
  currentWeekMinutes: number
  currentWeekCalories: number
  minutesAchievementPercent: number
  caloriesAchievementPercent: number
  isMinutesAchieved: boolean
  isCaloriesAchieved: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateWorkoutGoalDto {
  weeklyMinutes?: number
  weeklyCalories?: number
  startDate: string
  endDate?: string
}

export interface UpdateWorkoutGoalDto {
  weeklyMinutes?: number
  weeklyCalories?: number
  endDate?: string
  isActive?: boolean
}
