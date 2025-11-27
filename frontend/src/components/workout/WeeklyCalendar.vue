<template>
  <v-card class="weekly-calendar-card">
    <v-card-item>
      <div class="text-center">
        <div class="text-subtitle2 mb-3 text-medium-emphasis">
          {{ $t('workout.selectDate') }}
        </div>
        <div class="d-flex gap-2 justify-center flex-wrap">
          <v-btn
            v-for="(day, index) in weekDays"
            :key="index"
            :variant="isSelectedDay(day) ? 'elevated' : 'outlined'"
            :color="isSelectedDay(day) ? 'primary' : undefined"
            size="small"
            @click="$emit('select-date', day.date)"
          >
            <div class="d-flex flex-column align-center">
              <span class="text-caption">{{ day.shortName }}</span>
              <span class="text-subtitle2 font-weight-bold">{{ day.dayNum }}</span>
            </div>
          </v-btn>
        </div>
      </div>
    </v-card-item>
  </v-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

interface DayInfo {
  date: string
  dayNum: number
  shortName: string
  dayOfWeek: number
}

interface Props {
  selectedDate: string
  startDate?: string
}

const props = withDefaults(defineProps<Props>(), {
  startDate: undefined
})

defineEmits<{
  'select-date': [date: string]
}>()

const { t, locale } = useI18n()

const weekDays = computed<DayInfo[]>(() => {
  let start: Date
  
  if (props.startDate) {
    start = new Date(props.startDate)
  } else {
    start = new Date()
    start.setDate(start.getDate() - start.getDay() + 1)
  }

  const days: DayInfo[] = []
  const shortNames = ['一', '二', '三', '四', '五', '六', '日']

  for (let i = 0; i < 7; i++) {
    const date = new Date(start)
    date.setDate(date.getDate() + i)
    const dateStr = date.toISOString().split('T')[0]
    
    days.push({
      date: dateStr,
      dayNum: date.getDate(),
      shortName: shortNames[date.getDay()],
      dayOfWeek: date.getDay()
    })
  }

  return days
})

const isSelectedDay = (day: DayInfo): boolean => {
  return day.date === props.selectedDate
}
</script>

<style scoped>
.weekly-calendar-card {
  background: rgba(var(--v-theme-primary), 0.02);
  border: 1px solid rgba(var(--v-theme-primary), 0.12);
}

.d-flex.gap-2 {
  gap: 0.5rem;
}
</style>
