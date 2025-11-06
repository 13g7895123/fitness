<template>
  <div class="min-h-screen flex items-center justify-center px-4 bg-gradient-to-br from-primary-600 to-purple-700">
    <div class="text-center max-w-md text-white">
      <div v-if="isProcessing" class="w-20 h-20 mx-auto mb-6 border-4 border-white border-t-transparent rounded-full animate-spin"></div>
      <svg v-else-if="hasError" class="w-20 h-20 mx-auto mb-6 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>
      <svg v-else class="w-20 h-20 mx-auto mb-6 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>

      <div class="mt-6">
        <h2 v-if="isProcessing" class="text-2xl font-bold">{{ $t('auth.processing') }}</h2>
        <h2 v-else-if="hasError" class="text-2xl font-bold">{{ $t('common.error') }}</h2>
        <h2 v-else class="text-2xl font-bold">{{ $t('auth.success') }}</h2>

        <p v-if="errorMessage" class="mt-4 text-red-200">{{ errorMessage }}</p>
        <p v-else-if="!isProcessing" class="mt-4">{{ $t('auth.redirecting') }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useLineLoginService } from '@/services/authService'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const authService = useLineLoginService()
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
/* Tailwind handles all styling */
</style>
