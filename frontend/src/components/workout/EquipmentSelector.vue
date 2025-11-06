<template>
  <div class="equipment-selector">
    <v-select
      v-model="selectedEquipment"
      :items="activeEquipment"
      :label="$t('workout.equipment')"
      item-title="name"
      item-value="id"
      clearable
      variant="outlined"
      density="comfortable"
      rounded="lg"
      @update:model-value="handleSelect"
    ></v-select>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, defineProps, defineEmits } from 'vue'
import { api } from '@/services/api'

interface Equipment {
  id: string
  name: string
  isActive: boolean
}

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:modelValue', 'select'])

const equipmentList = ref<Equipment[]>([])
const selectedEquipment = ref<string>('')
const isLoading = ref(false)

const activeEquipment = computed(() => {
  return equipmentList.value.filter(eq => eq.isActive)
})

const handleSelect = (id: string | null) => {
  if (id) {
    const selected = activeEquipment.value.find(eq => eq.id === id)
    emit('select', selected)
  }
  emit('update:modelValue', id || '')
}

const loadEquipment = async () => {
  try {
    isLoading.value = true
    const response = await api.get('/equipment')
    equipmentList.value = response.data?.data || []
  } catch (error) {
    console.error('Failed to load equipment:', error)
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadEquipment()
})
</script>

<style scoped>
.equipment-selector {
  width: 100%;
}
</style>
