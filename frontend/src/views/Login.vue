<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 px-4">
    <div class="w-full max-w-md">
      <div class="card p-12">
        <div class="text-center mb-10">
          <h1 class="text-3xl font-bold mb-2">{{ $t('app.title') }}</h1>
          <p class="text-gray-600">{{ $t('auth.welcomeMessage') }}</p>
        </div>

        <!-- LINE 登入按鈕 -->
        <button
          class="w-full py-3 px-4 bg-[#00B900] text-white rounded-lg font-medium hover:bg-[#009900] transition-colors shadow-sm hover:shadow-md mb-4"
          @click="handleLineLogin"
          :disabled="isLoading"
        >
          <span v-if="!isLoading">{{ $t('auth.loginWithLine') }}</span>
          <span v-else>登入中...</span>
        </button>

        <!-- 測試登入按鈕 -->
        <button
          v-if="showTestLogin"
          class="w-full py-3 px-4 bg-gradient-to-r from-purple-500 to-pink-500 text-white rounded-lg font-medium hover:from-purple-600 hover:to-pink-600 transition-all shadow-sm hover:shadow-md"
          @click="handleTestLogin"
          :disabled="isLoading"
        >
          <span v-if="!isLoading">🧪 測試帳號登入</span>
          <span v-else>登入中...</span>
        </button>

        <!-- 提示訊息 -->
        <p v-if="showTestLogin" class="text-center text-sm text-gray-500 mt-4">
          測試帳號可體驗所有功能
        </p>
      </div>
    </div>

    <!-- Notification Panel -->
    <NotificationPanel />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useLineLoginService } from '@/services/authService'
import { useAuthStore } from '@/stores/auth'
import { showSuccess } from '@/utils/errorHandler'
import NotificationPanel from '@/components/common/NotificationPanel.vue'

const router = useRouter()
const authStore = useAuthStore()
const { getLineLoginUrl, testLogin } = useLineLoginService()

const useMockAuth = import.meta.env.VITE_USE_MOCK_AUTH === 'true'
const enableTestLogin = import.meta.env.VITE_ENABLE_TEST_LOGIN === 'true'

// 顯示測試登入按鈕的條件
const showTestLogin = computed(() => enableTestLogin)

// 載入狀態
const isLoading = computed(() => authStore.loading)

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

    // 顯示成功訊息
    showSuccess(`歡迎回來，${mockUser.displayName}！`)

    // 延遲一下讓使用者看到成功訊息
    setTimeout(() => {
      router.push('/')
    }, 500)
  } else {
    // 真實 LINE 登入
    const loginUrl = getLineLoginUrl()
    window.location.href = loginUrl
  }
}

const handleTestLogin = async () => {
  const success = await testLogin()
  if (success) {
    // 延遲一下讓使用者看到成功訊息
    setTimeout(() => {
      router.push('/')
    }, 500)
  }
}
</script>

<style scoped>
/* Tailwind handles all styling */
</style>
