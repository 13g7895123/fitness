<template>
  <div class="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50/30 to-purple-50/30 relative overflow-hidden">
    <!-- 背景裝飾性漸層 -->
    <div class="fixed inset-0 bg-mesh-gradient opacity-30 pointer-events-none"></div>
    
    <!-- Sidebar Backdrop -->
    <Transition
      enter-active-class="transition-opacity duration-300"
      leave-active-class="transition-opacity duration-200"
      enter-from-class="opacity-0"
      leave-to-class="opacity-0"
    >
      <div
        v-if="drawer"
        class="fixed inset-0 bg-white/10 backdrop-blur-sm z-40 lg:hidden"
        @click="drawer = false"
      ></div>
    </Transition>

    <!-- Sidebar -->
    <aside
      :class="[
        'fixed top-0 left-0 h-full w-80 z-50 transform transition-all duration-300 ease-out',
        drawer ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'
      ]"
    >
      <!-- 白底背景 -->
      <div class="absolute inset-0 bg-white shadow-2xl border-r border-gray-200"></div>
      
      <div class="relative h-full flex flex-col">
        <!-- Logo & User Profile -->
        <div class="p-6 space-y-6">
          <!-- Logo -->
          <div class="flex items-center space-x-3">
            <div class="w-12 h-12 rounded-2xl bg-gradient-primary flex items-center justify-center shadow-glow">
              <svg class="w-7 h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"/>
              </svg>
            </div>
            <div>
              <h2 class="text-xl font-bold text-gray-900">{{ $t('app.title') }}</h2>
              <p class="text-xs text-gray-600">健身生活，從這開始</p>
            </div>
          </div>

          <!-- User Profile Card -->
          <div class="bg-gradient-to-br from-primary-50 to-secondary-50 rounded-2xl p-4 shadow-soft">
            <div class="flex items-center space-x-3">
              <div class="relative">
                <img :src="userAvatar" alt="avatar" class="w-14 h-14 rounded-xl shadow-medium object-cover ring-2 ring-white" />
                <div class="absolute -bottom-1 -right-1 w-5 h-5 bg-success rounded-full border-2 border-white"></div>
              </div>
              <div class="flex-1 min-w-0">
                <div class="text-sm font-semibold text-gray-900 truncate">{{ userName }}</div>
                <div class="text-xs text-gray-600">進階會員</div>
                <div class="flex items-center mt-1 space-x-1">
                  <svg class="w-3 h-3 text-yellow-500" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
                  </svg>
                  <span class="text-xs text-gray-600">Level 12</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Navigation -->
        <nav class="flex-1 px-4 pb-4 space-y-1 overflow-y-auto">
          <router-link
            v-for="item in navItems"
            :key="item.path"
            :to="item.path"
            class="group flex items-center space-x-3 px-4 py-3.5 rounded-xl transition-all duration-200 relative overflow-hidden"
            active-class="nav-active"
            exact-active-class="nav-active"
          >
            <span class="absolute inset-0 bg-gradient-primary opacity-0 group-[.nav-active]:opacity-100 transition-opacity duration-200"></span>
            <component :is="item.icon" class="w-5 h-5 relative z-10 text-gray-700 group-[.nav-active]:text-[#555] group-hover:scale-110 transition-transform" />
            <span class="relative z-10 font-medium text-gray-900 group-[.nav-active]:text-[#555] group-hover:text-gray-900">
              {{ item.label }}
            </span>
            <div class="absolute right-3 w-1.5 h-8 bg-white rounded-full opacity-0 group-[.nav-active]:opacity-100 transition-opacity"></div>
          </router-link>
        </nav>

        <!-- Bottom Actions -->
        <div class="p-4 space-y-3 border-t border-gray-200/50">
          <button
            class="w-full flex items-center space-x-3 px-4 py-3 rounded-xl hover:bg-gray-100/80 transition-all duration-200 group"
            @click="handleLogout"
          >
            <svg class="w-5 h-5 text-gray-600 group-hover:text-error transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/>
            </svg>
            <span class="font-medium text-gray-700 group-hover:text-error transition-colors">登出</span>
          </button>
        </div>
      </div>
    </aside>

    <!-- Main Content -->
    <div class="lg:ml-80 relative">
      <!-- Top Bar -->
      <header class="sticky top-0 z-30 glass-dark shadow-soft">
        <div class="max-w-8xl mx-auto">
          <div class="flex items-center justify-between px-6 py-4">
            <div class="flex items-center space-x-4">
              <button
                class="lg:hidden p-2.5 rounded-xl hover:bg-white/10 transition-colors"
                @click="drawer = !drawer"
              >
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"/>
                </svg>
              </button>
              <div class="hidden lg:block">
                <h1 class="text-xl font-bold text-white">{{ currentPageTitle }}</h1>
                <p class="text-xs text-gray-300 mt-0.5">{{ currentDate }}</p>
              </div>
            </div>

            <div class="flex items-center space-x-3">
              <!-- Search -->
              <button class="p-2.5 rounded-xl hover:bg-white/10 transition-colors relative group">
                <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
                </svg>
              </button>

              <!-- Notifications -->
              <button class="p-2.5 rounded-xl hover:bg-white/10 transition-colors relative group" @click="showNotifications">
                <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"/>
                </svg>
                <span class="absolute top-1 right-1 w-2 h-2 bg-error rounded-full animate-pulse"></span>
              </button>

              <!-- Settings -->
              <button class="p-2.5 rounded-xl hover:bg-white/10 transition-colors">
                <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/>
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                </svg>
              </button>
            </div>
          </div>
        </div>
      </header>

      <!-- Page Content -->
      <main class="relative min-h-[calc(100vh-73px)]">
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
import { ref, computed, h } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/auth'

const { t } = useI18n()
const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const drawer = ref(false)

const userName = computed(() => {
  return authStore.user?.displayName || 'User'
})

const userAvatar = computed(() => {
  return authStore.user?.pictureUrl || 'https://cdn.vuetifyjs.com/images/avatars/1.jpg'
})

const currentPageTitle = computed(() => {
  const titleMap: Record<string, string> = {
    '/': t('navigation.home'),
    '/goals': t('navigation.goals'),
    '/trends': t('navigation.trends'),
    '/settings': t('navigation.settings'),
  }

  // 檢查是否為運動紀錄詳情頁面（動態日期路徑）
  if (route.path.startsWith('/workouts/detail/')) {
    return t('navigation.workouts')
  }

  return titleMap[route.path] || t('app.title')
})

const currentDate = computed(() => {
  return new Date().toLocaleDateString('zh-TW', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    weekday: 'long'
  })
})

// 動態計算今日運動紀錄路徑
const todayWorkoutPath = computed(() => {
  const today = new Date()
  const dateStr = today.toISOString().split('T')[0]
  return `/workouts/detail/${dateStr}`
})

// 導航圖標組件
const HomeIcon = () => h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, 
  h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6' })
)

const DumbbellIcon = () => h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' },
  h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M14 10h4.764a2 2 0 011.789 2.894l-3.5 7A2 2 0 0115.263 21h-4.017c-.163 0-.326-.02-.485-.06L7 20m7-10V5a2 2 0 00-2-2h-.095c-.5 0-.905.405-.905.905 0 .714-.211 1.412-.608 2.006L7 11v9m7-10h-2M7 20H5a2 2 0 01-2-2v-6a2 2 0 012-2h2.5' })
)

const TargetIcon = () => h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' },
  h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z' })
)

const ChartIcon = () => h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' },
  h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z' })
)

const SettingsIcon = () => h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' },
  h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z' })
)

const navItems = computed(() => [
  { path: '/', label: t('navigation.home'), icon: HomeIcon },
  { path: todayWorkoutPath.value, label: t('navigation.workouts'), icon: DumbbellIcon },
  { path: '/goals', label: t('navigation.goals'), icon: TargetIcon },
  { path: '/trends', label: t('navigation.trends'), icon: ChartIcon },
  { path: '/settings', label: t('navigation.settings'), icon: SettingsIcon }
])

const handleLogout = async () => {
  authStore.clearAuth()
  router.push('/login')
}

const showNotifications = () => {
  // TODO: Implement notification panel
  console.log('Show notifications')
}
</script>

<style scoped>
/* 導航激活狀態 */
.nav-active {
  box-shadow: 0 4px 16px 0 rgba(0, 0, 0, 0.08);
}

/* 頁面過渡動畫 */
.fade-enter-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.fade-leave-active {
  transition: all 0.2s cubic-bezier(0.4, 0, 1, 1);
}

.fade-enter-from {
  opacity: 0;
  transform: translateY(10px) scale(0.98);
}

.fade-leave-to {
  opacity: 0;
  transform: translateY(-10px) scale(0.98);
}

/* 響應式優化 */
@media (max-width: 1024px) {
  aside {
    box-shadow: 2px 0 24px rgba(0, 0, 0, 0.15);
  }
}
</style>
