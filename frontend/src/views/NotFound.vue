<template>
  <div class="min-h-screen flex items-center justify-center px-4">
    <div class="text-center max-w-md">
      <svg class="w-32 h-32 mx-auto mb-6 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>

      <h1 class="text-6xl font-bold mb-4">404</h1>
      <h2 class="text-2xl mb-6">{{ $t('error.pageNotFound') }}</h2>

      <p class="text-gray-600 mb-8">
        {{ $t('error.pageNotFoundMessage') }}
      </p>

      <Button
        variant="primary"
        size="large"
        @click="goHome"
      >
        {{ $t('common.backToHome') }}
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import Button from '@/components/common/Button.vue'

const router = useRouter()

const goHome = () => {
  router.push('/')
}
</script>

<style scoped>
/* Tailwind handles all styling */
</style>
