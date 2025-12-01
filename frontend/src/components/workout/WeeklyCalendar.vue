<template>
  <div class="glass rounded-2xl p-4 border border-primary/10">
    <div class="text-center">
      <div class="text-sm font-medium text-gray-500 mb-3">
        {{ $t('workout.selectDate') }}
      </div>
      <div class="flex gap-2 justify-center flex-wrap">
        <button
          v-for="(day, index) in weekDays"
          :key="index"
          :class="[
            'flex flex-col items-center justify-center w-12 h-14 rounded-xl transition-all duration-200',
            isSelectedDay(day)
              ? 'bg-gradient-primary text-white shadow-lg scale-105'
              : 'bg-white/50 text-gray-600 hover:bg-white hover:shadow-md'
          ]"
          @click="$emit('select-date', day.date)"
        >
          <span class="text-xs opacity-80">{{ day.shortName }}</span>
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

const { t } = useI18n()

const weekDays = computed<DayInfo[]>(() => {
  let start: Date
  
  if (props.startDate) {
    start = new Date(props.startDate)
  } else {
    start = new Date()
    start.setDate(start.getDate() - start.getDay() + 1)
  }

  const days: DayInfo[] = []
  // 索引 0-6 對應週日-週六 (與 getDay() 返回值一致)
  const shortNames = ['日', '一', '二', '三', '四', '五', '六']

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
