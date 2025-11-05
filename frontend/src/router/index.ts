import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import { setupAuthGuard } from './guards/authGuard'

const Home = () => import('@/views/Home.vue')
const WorkoutDetail = () => import('@/views/WorkoutDetail.vue')
const Goals = () => import('@/views/Goals.vue')
const Trends = () => import('@/views/Trends.vue')
const Settings = () => import('@/views/Settings.vue')

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Home',
    component: Home,
    meta: {
      title: '首頁',
      requiresAuth: true
    }
  },
  {
    path: '/workouts/detail/:date',
    name: 'WorkoutDetail',
    component: WorkoutDetail,
    meta: {
      title: '每日訓練詳情',
      requiresAuth: true
    }
  },
  {
    path: '/goals',
    name: 'Goals',
    component: Goals,
    meta: {
      title: '運動目標',
      requiresAuth: true
    }
  },
  {
    path: '/trends',
    name: 'Trends',
    component: Trends,
    meta: {
      title: '歷史趨勢',
      requiresAuth: true
    }
  },
  {
    path: '/settings',
    name: 'Settings',
    component: Settings,
    meta: {
      title: '設定',
      requiresAuth: true
    }
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue'),
    meta: {
      title: '登入'
    }
  },
  {
    path: '/auth/callback',
    name: 'AuthCallback',
    component: () => import('@/views/AuthCallback.vue'),
    meta: {
      title: '認證回調'
    }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('@/views/NotFound.vue'),
    meta: {
      title: '找不到頁面'
    }
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

// 設置認證守衛
setupAuthGuard(router)

export default router
