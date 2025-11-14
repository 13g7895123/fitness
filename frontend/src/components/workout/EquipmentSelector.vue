<template>
  <div class="equipment-selector">
    <Select
      v-model="selectedEquipment"
      :items="activeEquipment"
      :label="$t('workout.equipment')"
      item-title="name"
      item-value="id"
      :placeholder="$t('workout.selectEquipment')"
      @update:model-value="handleSelect"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, defineProps, defineEmits } from 'vue'
import { api } from '@/services/api'
import Select from '@/components/common/Select.vue'

interface Equipment {
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

const equipmentList = ref<Equipment[]>([])
const selectedEquipment = ref<number | string>('')
const isLoading = ref(false)

const activeEquipment = computed(() => {
  return equipmentList.value.filter(eq => !eq.isDisabled)
})

const handleSelect = (id: number | string | null) => {
  if (id) {
    const selected = activeEquipment.value.find(eq => eq.id === id)
    emit('select', selected)
  }
  emit('update:modelValue', id || '')
}

const loadEquipment = async () => {
  try {
    isLoading.value = true
    const response = await api.get('/equipments')
    equipmentList.value = response.data?.data || []
  } catch (error) {
    console.error('Failed to load equipment:', error)
  } finally {
    isLoading.value = false
  }
}

// 監聽 modelValue 變化以同步選中狀態
watch(() => props.modelValue, (newValue) => {
  if (newValue !== selectedEquipment.value) {
    selectedEquipment.value = newValue || ''
  }
}, { immediate: true })

onMounted(() => {
  loadEquipment()
})
</script>

<style scoped>
.equipment-selector {
  width: 100%;
}
</style>
