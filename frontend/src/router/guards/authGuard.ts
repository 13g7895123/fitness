import { Router, RouteLocationNormalized } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useLineLoginService } from '@/services/authService'

/**
 * 公開路由列表 - 不需要登入即可訪問
 */
const PUBLIC_ROUTES = ['/login', '/auth/callback']

/**
 * 安裝 Auth 導航守衛
 */
export const setupAuthGuard = (router: Router): void => {
  router.beforeEach(async (to: RouteLocationNormalized, from: RouteLocationNormalized, next) => {
    const authStore = useAuthStore()
    const lineLoginService = useLineLoginService()

    // 恢復已保存的 token
    if (!authStore.token) {
      authStore.restoreToken()
    }

    // 檢查是否為公開路由
    if (PUBLIC_ROUTES.includes(to.path)) {
      // 如果已登入且訪問登入頁，重定向到首頁
      if (authStore.isAuthenticated && to.path === '/login') {
        next('/')
      } else {
        next()
      }
      return
    }

    // 受保護路由 - 檢查認證狀態
    if (!authStore.isAuthenticated) {
      // 如果有 token，驗證是否仍有效
      if (authStore.token) {
        const isValid = await lineLoginService.validateToken(authStore.token)
        if (!isValid) {
          authStore.clearAuth()
          next('/login')
          return
        }
      } else {
        // 沒有 token，重定向到登入
        next('/login')
        return
      }
    }

    next()
  })

  /**
   * 處理導航完成
   */
  router.afterEach((to: RouteLocationNormalized) => {
    // 更新頁面標題
    const title = to.meta?.title || '健身追蹤系統'
    document.title = `${title} - 健身追蹤系統`
  })
}

/**
 * 檢查用戶是否有特定角色
 */
export const hasRole = (roles: string[]): boolean => {
  const authStore = useAuthStore()
  if (!authStore.user) return false

  // 可以根據需要擴展角色檢查邏輯
  // 目前使用者沒有明確的角色字段，如需可在 User 實體中添加
  return true
}

/**
 * 檢查用戶是否已登入
 */
export const isAuthenticated = (): boolean => {
  const authStore = useAuthStore()
  return authStore.isAuthenticated
}
