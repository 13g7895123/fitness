<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Sidebar -->
    <div
      v-if="drawer"
      class="fixed inset-0 bg-black bg-opacity-50 z-40 lg:hidden"
      @click="drawer = false"
    ></div>

    <aside
      :class="[
        'fixed top-0 left-0 h-full w-72 bg-white border-r border-gray-200 z-50 transform transition-transform duration-200',
        drawer ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'
      ]"
    >
      <div class="p-6">
        <h2 class="text-xl font-bold mb-2">{{ $t('app.title') }}</h2>
        <div class="flex items-center mb-6">
          <img :src="userAvatar" alt="avatar" class="w-10 h-10 rounded-full mr-3" />
          <div class="flex-1">
            <div class="text-sm font-medium">{{ userName }}</div>
            <div class="text-xs text-gray-500">健身追蹤</div>
          </div>
        </div>
      </div>

      <nav class="px-3">
        <router-link
          v-for="item in navItems"
          :key="item.path"
          :to="item.path"
          class="list-item block"
          active-class="list-item-active"
        >
          {{ item.label }}
        </router-link>
      </nav>

      <div class="absolute bottom-0 left-0 right-0 p-4">
        <button
          class="btn-text w-full text-left"
          @click="handleLogout"
        >
          登出
        </button>
      </div>
    </aside>

    <!-- Main Content -->
    <div class="lg:ml-72">
      <!-- Top Bar -->
      <header class="bg-white border-b border-gray-200 sticky top-0 z-30">
        <div class="flex items-center px-4 py-3">
          <button
            class="lg:hidden p-2 rounded-lg hover:bg-gray-100 mr-2"
            @click="drawer = !drawer"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"/>
            </svg>
          </button>
          <h1 class="text-lg font-bold">{{ $t('app.title') }}</h1>
        </div>
      </header>

      <!-- Page Content -->
      <main>
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" :key="$route.path" />
          </transition>
        </router-view>
      </main>
    </div>
  </div>
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
</style>
