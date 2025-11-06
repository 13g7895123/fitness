<template>
  <v-app>
    <v-navigation-drawer v-model="drawer" width="280" elevation="0" class="border-e">
      <div class="pa-6">
        <h2 class="text-h5 font-weight-bold mb-2">{{ $t('app.title') }}</h2>
        <div class="d-flex align-center mb-6">
          <v-avatar :image="userAvatar" size="40" class="mr-3"></v-avatar>
          <div class="flex-grow-1">
            <div class="text-body-2 font-weight-medium">{{ userName }}</div>
            <div class="text-caption text-medium-emphasis">健身追蹤</div>
          </div>
        </div>
      </div>

      <v-list class="px-3" density="compact" nav>
        <v-list-item
          v-for="item in navItems"
          :key="item.path"
          :to="item.path"
          :title="item.label"
          rounded="lg"
          class="mb-1"
        ></v-list-item>
      </v-list>

      <template v-slot:append>
        <div class="pa-4">
          <v-btn
            variant="text"
            block
            class="text-none"
            @click="handleLogout"
          >
            登出
          </v-btn>
        </div>
      </template>
    </v-navigation-drawer>

    <v-app-bar elevation="0" class="border-b" color="white">
      <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title class="font-weight-bold">{{ $t('app.title') }}</v-toolbar-title>
    </v-app-bar>

    <v-main class="pa-0">
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" :key="$route.path" />
        </transition>
      </router-view>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/auth'

const { t } = useI18n()
const router = useRouter()
const authStore = useAuthStore()

const drawer = ref(false)
const rail = ref(true)

const userName = computed(() => {
  return authStore.user?.displayName || 'User'
})

const userAvatar = computed(() => {
  return authStore.user?.pictureUrl || 'https://cdn.vuetifyjs.com/images/avatars/1.jpg'
})

const navItems = [
  { path: '/', label: t('navigation.home'), icon: 'mdi-home' },
  { path: '/workouts/detail/today', label: t('navigation.workouts'), icon: 'mdi-dumbbell' },
  { path: '/goals', label: t('navigation.goals'), icon: 'mdi-target' },
  { path: '/trends', label: t('navigation.trends'), icon: 'mdi-chart-line' },
  { path: '/settings', label: t('navigation.settings'), icon: 'mdi-cog' }
]

const handleLogout = async () => {
  await authStore.logout()
  router.push('/login')
}

const showNotifications = () => {
  // TODO: Implement notification panel
}
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.border-e {
  border-right: 1px solid rgba(0, 0, 0, 0.05);
}

.border-b {
  border-bottom: 1px solid rgba(0, 0, 0, 0.05);
}
</style>
