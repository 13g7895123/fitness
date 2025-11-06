<template>
  <v-container class="login-page fill-height" fluid>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="5" lg="4">
        <div class="login-card">
          <div class="text-center mb-10">
            <h1 class="text-h3 font-weight-bold mb-2">{{ $t('app.title') }}</h1>
            <p class="text-body-1 text-medium-emphasis">{{ $t('auth.welcomeMessage') }}</p>
          </div>

          <v-btn
            color="#00B900"
            size="x-large"
            variant="flat"
            block
            rounded="lg"
            class="text-none font-weight-medium"
            @click="handleLineLogin"
          >
            {{ $t('auth.loginWithLine') }}
          </v-btn>
        </div>
      </v-col>
    </v-row>
  </v-container>
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
.login-page {
  background-color: #fafafa;
  min-height: 100vh;
}

.login-card {
  background: white;
  border-radius: 24px;
  padding: 48px 32px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}
</style>
