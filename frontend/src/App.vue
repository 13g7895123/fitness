<template>
  <v-app>
    <v-navigation-drawer v-model="drawer" :rail="rail" @mouseenter="rail = false" @mouseleave="rail = true">
      <v-list>
        <v-list-item :prepend-avatar="userAvatar" :title="userName" subtitle="健身追蹤">
          <template v-slot:append>
            <v-btn icon="mdi-logout" @click="handleLogout" size="small"></v-btn>
          </template>
        </v-list-item>
      </v-list>

      <v-divider></v-divider>

      <v-list nav>
        <v-list-item
          v-for="item in navItems"
          :key="item.path"
          :to="item.path"
          :prepend-icon="item.icon"
          :title="item.label"
          exact
        ></v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-app-bar>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title>{{ $t('app.title') }}</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn icon="mdi-bell" @click="showNotifications"></v-btn>
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
  return authStore.user?.name || 'User'
})

const userAvatar = computed(() => {
  return authStore.user?.picture || 'https://cdn.vuetifyjs.com/images/avatars/1.jpg'
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
