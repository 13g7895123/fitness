<template>
  <div class="exercise-type-selector">
    <Input
      v-model="searchQuery"
      :label="$t('workout.exercise')"
      placeholder="搜尋運動項目..."
      @update:model-value="handleSearch"
    />

    <div
      v-if="showDropdown"
      class="card mt-2 max-h-[300px] overflow-y-auto"
    >
      <div class="space-y-1 p-2">
        <div
          v-for="exercise in filteredExercises"
          :key="exercise.id"
          class="list-item flex items-center justify-between"
          @click="selectExercise(exercise)"
        >
          <span>{{ exercise.name }}</span>
          <span
            v-if="!exercise.isActive"
            class="chip chip-warning text-xs"
          >
            {{ $t('common.inactive') }}
          </span>
        </div>

        <div v-if="filteredExercises.length === 0" class="p-4 text-center text-gray-500">
          {{ $t('workout.noExerciseFound') }}
        </div>
      </div>
    </div>

    <!-- 顯示已選擇項目 -->
    <div
      v-if="selectedExercise"
      class="chip chip-primary mt-2 inline-flex items-center gap-2"
    >
      <span>{{ selectedExercise.name }}</span>
      <button
        type="button"
        class="hover:bg-black/10 rounded-full p-0.5"
        @click="clearSelection"
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, defineProps, defineEmits } from 'vue'
import { api } from '@/services/api'
import Input from '@/components/common/Input.vue'

interface ExerciseType {
  id: string
  name: string
  category: string
  isCustom: boolean
  isActive: boolean
}

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:modelValue', 'select'])

const exercises = ref<ExerciseType[]>([])
const selectedExercise = ref<ExerciseType | null>(null)
const searchQuery = ref('')
const showDropdown = ref(false)
const isLoading = ref(false)

const filteredExercises = computed(() => {
  if (!searchQuery.value.trim()) {
    return exercises.value
  }

  const query = searchQuery.value.toLowerCase()
  return exercises.value.filter(ex =>
    ex.name.toLowerCase().includes(query) ||
    ex.category.toLowerCase().includes(query)
  )
})

const handleSearch = () => {
  showDropdown.value = searchQuery.value.length > 0 || filteredExercises.value.length > 0
}

const selectExercise = (exercise: ExerciseType) => {
  selectedExercise.value = exercise
  emit('update:modelValue', exercise.id)
  emit('select', exercise)
  searchQuery.value = ''
  showDropdown.value = false
}

const clearSelection = () => {
  selectedExercise.value = null
  searchQuery.value = ''
  showDropdown.value = false
  emit('update:modelValue', '')
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
  position: relative;
  width: 100%;
}
</style>
