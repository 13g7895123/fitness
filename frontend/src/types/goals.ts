export interface WorkoutGoalDto {
  Id: string
  UserId: string
  WeeklyMinutesGoal?: number
  WeeklyCaloriesGoal?: number
  StartDate: string
  EndDate?: string
  IsActive: boolean
  CurrentWeekMinutes: number
  CurrentWeekCalories: number
  MinutesAchievementPercent: number
  CaloriesAchievementPercent: number
  IsMinutesAchieved: boolean
  IsCaloriesAchieved: boolean
  CreatedAt: string
  UpdatedAt?: string
}

export interface CreateWorkoutGoalDto {
  WeeklyMinutesGoal?: number
  WeeklyCaloriesGoal?: number
  StartDate: string
  EndDate?: string
}

export interface UpdateWorkoutGoalDto extends CreateWorkoutGoalDto {}
