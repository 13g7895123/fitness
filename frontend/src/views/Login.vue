<template>
  <v-container class="login-page fill-height" fluid>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="6" lg="4">
        <v-card class="login-card" elevation="4">
          <v-card-title class="text-h4 text-center py-6">
            <v-icon size="48" color="primary" class="mr-2">mdi-dumbbell</v-icon>
            {{ $t('app.title') }}
          </v-card-title>

          <v-card-text class="text-center py-8">
            <p class="text-h6 mb-6">{{ $t('auth.welcomeMessage') }}</p>

            <v-btn
              color="success"
              size="large"
              variant="flat"
              prepend-icon="mdi-line"
              block
              @click="handleLineLogin"
            >
              {{ $t('auth.loginWithLine') }}
            </v-btn>
          </v-card-text>
        </v-card>
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

const handleLineLogin = () => {
  if (useMockAuth) {
    // 使用 Mock 資料登入
    const mockToken = 'mock-jwt-token-' + Date.now()
    const mockUser = {
      id: 'mock-user-id',
      lineUserId: 'U1234567890abcdef',
      displayName: '測試使用者',
      pictureUrl: 'https://cdn.vuetifyjs.com/images/avatars/1.jpg'
    }
    authStore.setAuth(mockToken, mockUser)
    router.push('/')
  } else {
    // 真實 LINE 登入
    const loginUrl = getLineLoginUrl()
    window.location.href = loginUrl
  }
}
</script>

<style scoped>
.login-page {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  min-height: 100vh;
}

.login-card {
  border-radius: 16px;
}
</style>
