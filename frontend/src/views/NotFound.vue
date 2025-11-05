<template>
  <v-container class="not-found-page fill-height" fluid>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="6" class="text-center">
        <v-icon size="120" color="grey-lighten-1">mdi-emoticon-sad-outline</v-icon>

        <h1 class="text-h2 mt-6 mb-4">404</h1>
        <h2 class="text-h5 mb-6">{{ $t('error.pageNotFound') }}</h2>

        <p class="text-body-1 mb-8">
          {{ $t('error.pageNotFoundMessage') }}
        </p>

        <v-btn
          color="primary"
          size="large"
          variant="flat"
          prepend-icon="mdi-home"
          @click="goHome"
        >
          {{ $t('common.backToHome') }}
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const goHome = () => {
  router.push('/')
}
</script>

<style scoped>
.not-found-page {
  min-height: calc(100vh - 64px);
}
</style>
