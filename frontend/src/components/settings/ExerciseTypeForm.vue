<template>
  <v-card>
    <v-card-title>
      {{ mode === 'create' ? '新增運動類型' : mode === 'edit' ? '編輯運動類型' : '運動類型詳情' }}
    </v-card-title>

    <v-divider></v-divider>

    <v-card-text>
      <v-form ref="formRef" v-model="valid">
        <v-text-field
          v-model="formData.name"
          label="運動名稱"
          :rules="[rules.required, rules.maxLength(100)]"
          :readonly="mode === 'view' || isSystemDefault"
          required
        ></v-text-field>

        <v-textarea
          v-model="formData.description"
          label="描述"
          :rules="[rules.maxLength(500)]"
          :readonly="mode === 'view' || isSystemDefault"
          rows="3"
        ></v-textarea>

        <v-text-field
          v-model.number="formData.defaultMET"
          label="預設 MET 值"
          type="number"
          :rules="[rules.required, rules.minValue(1), rules.maxValue(20)]"
          :readonly="mode === 'view' || isSystemDefault"
          hint="代謝當量 (Metabolic Equivalent of Task)"
          persistent-hint
          required
        ></v-text-field>

        <v-select
          v-model="formData.equipmentIds"
          :items="availableEquipments"
          item-title="name"
          item-value="id"
          label="相關器材"
          multiple
          chips
          :readonly="mode === 'view' || isSystemDefault"
          hint="選擇此運動類型可能使用的器材"
          persistent-hint
        ></v-select>

        <v-alert v-if="isSystemDefault" type="info" text class="mt-4">
          系統預設項目無法編輯或刪除
        </v-alert>
      </v-form>
    </v-card-text>

    <v-divider></v-divider>

    <v-card-actions>
      <v-spacer></v-spacer>
      <v-btn text @click="$emit('cancel')">
        {{ mode === 'view' ? '關閉' : '取消' }}
      </v-btn>
      <v-btn
        v-if="mode !== 'view' && !isSystemDefault"
        color="primary"
        :disabled="!valid || loading"
        :loading="loading"
        @click="handleSubmit"
      >
        {{ mode === 'create' ? '建立' : '更新' }}
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import type { ExerciseType, Equipment, CreateExerciseTypeDto, UpdateExerciseTypeDto } from '@/types/exercise-type'

const props = defineProps<{
  mode: 'create' | 'edit' | 'view'
  exerciseType?: ExerciseType
  availableEquipments: Equipment[]
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'submit', data: CreateExerciseTypeDto | UpdateExerciseTypeDto): void
  (e: 'cancel'): void
}>()

const formRef = ref()
const valid = ref(false)

const isSystemDefault = computed(() => props.exerciseType?.isSystemDefault || false)

const formData = reactive({
  name: '',
  description: '',
  defaultMET: 5.0,
  equipmentIds: [] as number[]
})

const rules = {
  required: (v: any) => !!v || '此欄位為必填',
  maxLength: (max: number) => (v: string) => !v || v.length <= max || `最多 ${max} 個字元`,
  minValue: (min: number) => (v: number) => v >= min || `最小值為 ${min}`,
  maxValue: (max: number) => (v: number) => v <= max || `最大值為 ${max}`
}

watch(() => props.exerciseType, (newVal) => {
  if (newVal) {
    formData.name = newVal.name
    formData.description = newVal.description || ''
    formData.defaultMET = newVal.defaultMET
    formData.equipmentIds = newVal.equipments.map(e => e.id)
  }
}, { immediate: true })

const handleSubmit = async () => {
  const { valid: isValid } = await formRef.value.validate()
  if (isValid) {
    emit('submit', {
      name: formData.name,
      description: formData.description || undefined,
      defaultMET: formData.defaultMET,
      equipmentIds: formData.equipmentIds
    })
  }
}
</script>
