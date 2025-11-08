<template>
  <div class="glass rounded-2xl p-6 shadow-soft">
    <div class="text-center">
      <div class="text-sm text-gray-600 mb-4 font-medium">
        {{ $t('workout.selectDate') }}
      </div>
      <div class="flex gap-2 justify-center flex-wrap">
        <button
          v-for="(day, index) in weekDays"
          :key="index"
          :class="[
            'flex flex-col items-center px-4 py-3 rounded-xl transition-all duration-300 min-w-[60px]',
            isSelectedDay(day)
              ? 'bg-gradient-primary text-white shadow-glow scale-105'
              : 'bg-white border-2 border-gray-200 hover:border-primary hover:scale-105 text-gray-700'
          ]"
          @click="$emit('select-date', day.date)"
        >
          <span class="text-xs font-medium mb-1">{{ day.shortName }}</span>
          <span class="text-lg font-bold">{{ day.dayNum }}</span>
        </button>
      </div>
    </div>
  </div>
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
/* 所有樣式已使用 Tailwind CSS */
</style>
