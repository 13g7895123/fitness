<template>
  <div class="exercise-type-selector">
    <v-text-field
      v-model="searchQuery"
      :label="$t('workout.exercise')"
      placeholder="搜尋運動項目..."
      variant="outlined"
      density="comfortable"
      clearable
      @update:model-value="handleSearch"
    >
      <template #prepend-inner>
        <v-icon>mdi-magnify</v-icon>
      </template>
    </v-text-field>

    <v-card v-if="showDropdown" class="mt-2">
      <v-list>
        <v-list-item
          v-for="exercise in filteredExercises"
          :key="exercise.id"
          @click="selectExercise(exercise)"
        >
          <template #prepend>
            <v-icon>mdi-dumbbell</v-icon>
          </template>
          <v-list-item-title>{{ exercise.name }}</v-list-item-title>
          <template v-if="!exercise.isActive" #append>
            <v-chip size="small" color="warning">{{ $t('common.inactive') }}</v-chip>
          </template>
        </v-list-item>

        <v-list-item v-if="filteredExercises.length === 0">
          <v-list-item-title class="text-center">
            {{ $t('workout.noExerciseFound') }}
          </v-list-item-title>
        </v-list-item>
      </v-list>
    </v-card>

    <!-- 顯示已選擇項目 -->
    <v-chip
      v-if="selectedExercise"
      class="mt-2"
      closable
      @click:close="clearSelection"
    >
      {{ selectedExercise.name }}
    </v-chip>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, defineProps, defineEmits } from 'vue'
import { api } from '@/services/api'

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
}

v-card {
  max-height: 300px;
  overflow-y: auto;
}
</style>
