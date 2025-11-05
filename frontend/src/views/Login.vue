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
import { useAuthService } from '@/services/authService'

const authService = useAuthService()

const handleLineLogin = () => {
  authService.redirectToLineLogin()
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
