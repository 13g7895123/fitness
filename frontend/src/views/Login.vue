<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 px-4">
    <div class="w-full max-w-md">
      <div class="card p-12">
        <div class="text-center mb-10">
          <h1 class="text-3xl font-bold mb-2">{{ $t('app.title') }}</h1>
          <p class="text-gray-600">{{ $t('auth.welcomeMessage') }}</p>
        </div>

        <button
          class="w-full py-3 px-4 bg-[#00B900] text-white rounded-lg font-medium hover:bg-[#009900] transition-colors shadow-sm hover:shadow-md"
          @click="handleLineLogin"
        >
          {{ $t('auth.loginWithLine') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useLineLoginService } from '@/services/authService'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const { getLineLoginUrl } = useLineLoginService()

const useMockAuth = import.meta.env.VITE_USE_MOCK_AUTH === 'true'

const handleLineLogin = async () => {
  if (useMockAuth) {
    // 使用 Mock 資料登入
    const mockToken = 'mock-jwt-token-' + Date.now()
    const mockUser = {
      id: '00000000-0000-0000-0000-000000000001',
      lineUserId: 'U1234567890abcdef',
      displayName: '測試使用者',
      pictureUrl: 'https://cdn.vuetifyjs.com/images/avatars/1.jpg'
    }
    authStore.setAuth(mockToken, mockUser)
    
    // 導航到首頁
    await router.push('/')
  } else {
    // 真實 LINE 登入
    const loginUrl = getLineLoginUrl()
    window.location.href = loginUrl
  }
}
</script>

<style scoped>
/* Tailwind handles all styling */
</style>
