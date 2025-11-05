<template>
  <v-card>
    <v-card-title>
      {{ mode === 'create' ? '新增器材' : '編輯器材' }}
    </v-card-title>

    <v-divider></v-divider>

    <v-card-text>
      <v-form ref="formRef" v-model="valid">
        <v-text-field
          v-model="formData.name"
          label="器材名稱"
          :rules="[rules.required, rules.maxLength(100)]"
          :readonly="isSystemDefault"
          required
        ></v-text-field>

        <v-textarea
          v-model="formData.description"
          label="描述"
          :rules="[rules.maxLength(500)]"
          :readonly="isSystemDefault"
          rows="3"
        ></v-textarea>

        <v-alert v-if="isSystemDefault" type="info" text class="mt-4">
          系統預設項目無法編輯或刪除
        </v-alert>
      </v-form>
    </v-card-text>

    <v-divider></v-divider>

    <v-card-actions>
      <v-spacer></v-spacer>
      <v-btn text @click="$emit('cancel')">取消</v-btn>
      <v-btn
        v-if="!isSystemDefault"
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
import type { Equipment, CreateEquipmentDto, UpdateEquipmentDto } from '@/types/exercise-type'

const props = defineProps<{
  mode: 'create' | 'edit'
  equipment?: Equipment
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'submit', data: CreateEquipmentDto | UpdateEquipmentDto): void
  (e: 'cancel'): void
}>()

const formRef = ref()
const valid = ref(false)

const isSystemDefault = computed(() => props.equipment?.isSystemDefault || false)

const formData = reactive({
  name: '',
  description: ''
})

const rules = {
  required: (v: any) => !!v || '此欄位為必填',
  maxLength: (max: number) => (v: string) => !v || v.length <= max || `最多 ${max} 個字元`
}

watch(() => props.equipment, (newVal) => {
  if (newVal) {
    formData.name = newVal.name
    formData.description = newVal.description || ''
  }
}, { immediate: true })

const handleSubmit = async () => {
  const { valid: isValid } = await formRef.value.validate()
  if (isValid) {
    emit('submit', {
      name: formData.name,
      description: formData.description || undefined
    })
  }
}
</script>
