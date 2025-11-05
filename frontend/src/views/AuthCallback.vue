<template>
  <v-container class="auth-callback-page fill-height" fluid>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="6" lg="4" class="text-center">
        <v-progress-circular
          v-if="isProcessing"
          :size="70"
          :width="7"
          color="primary"
          indeterminate
        />
        <v-icon v-else-if="hasError" size="70" color="error">mdi-alert-circle</v-icon>
        <v-icon v-else size="70" color="success">mdi-check-circle</v-icon>

        <div class="mt-6">
          <h2 v-if="isProcessing">{{ $t('auth.processing') }}</h2>
          <h2 v-else-if="hasError">{{ $t('common.error') }}</h2>
          <h2 v-else>{{ $t('auth.success') }}</h2>

          <p v-if="errorMessage" class="mt-4 text-error">{{ errorMessage }}</p>
          <p v-else-if="!isProcessing" class="mt-4">{{ $t('auth.redirecting') }}</p>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthService } from '@/services/authService'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const authService = useAuthService()
const authStore = useAuthStore()

const isProcessing = ref(true)
const hasError = ref(false)
const errorMessage = ref('')

onMounted(async () => {
  try {
    const code = route.query.code as string
    const state = route.query.state as string

    if (!code || !state) {
      throw new Error('Missing authorization code or state')
    }

    await authService.handleCallback(code, state)

    isProcessing.value = false

    setTimeout(() => {
      router.push('/')
    }, 2000)
  } catch (error) {
    isProcessing.value = false
    hasError.value = true
    errorMessage.value = error instanceof Error ? error.message : 'Authentication failed'

    setTimeout(() => {
      router.push('/login')
    }, 3000)
  }
})
</script>

<style scoped>
.auth-callback-page {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  min-height: 100vh;
}
</style>
