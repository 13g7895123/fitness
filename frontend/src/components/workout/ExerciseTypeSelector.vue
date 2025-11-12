<template>
  <div class="exercise-type-selector">
    <Select
      v-model="selectedExerciseId"
      :items="activeExercises"
      :label="$t('workout.exercise')"
      item-title="name"
      item-value="id"
      :placeholder="$t('workout.selectExercise')"
      :required="true"
      @update:model-value="handleSelect"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, defineProps, defineEmits } from 'vue'
import { api } from '@/services/api'
import Select from '@/components/common/Select.vue'

interface ExerciseType {
  id: number
  name: string
  description?: string
  isSystemDefault: boolean
  isDisabled: boolean
}

const props = defineProps({
  modelValue: {
    type: [String, Number],
    default: ''
  }
})

const emit = defineEmits(['update:modelValue', 'select'])

const exercises = ref<ExerciseType[]>([])
const selectedExerciseId = ref<number | string>('')
const isLoading = ref(false)

const activeExercises = computed(() => {
  return exercises.value.filter(ex => !ex.isDisabled)
})

const handleSelect = (id: number | string | null) => {
  if (id) {
    const selected = activeExercises.value.find(ex => ex.id === id)
    emit('select', selected)
  }
  emit('update:modelValue', id || '')
}

const loadExercises = async () => {
  try {
    isLoading.value = true
    const response = await api.get('/exercise-types')
    exercises.value = response.data?.data || []
  } catch (error) {
    console.error('Failed to load exercise types:', error)
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadExercises()
})
</script>

<style scoped>
.exercise-type-selector {
  width: 100%;
}
</style>
