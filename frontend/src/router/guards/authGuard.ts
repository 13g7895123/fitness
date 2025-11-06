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

    console.log('[Auth Guard] Navigating to:', to.path, 'from:', from.path)

    // 恢復已保存的 token
    if (!authStore.token) {
      authStore.restoreToken()
    }

    console.log('[Auth Guard] Auth state:', {
      token: authStore.token ? (authStore.token.substring(0, Math.min(20, authStore.token.length)) + '...') : null,
      isAuthenticated: authStore.isAuthenticated,
      user: authStore.user?.displayName
    })

    // 檢查是否為公開路由
    if (PUBLIC_ROUTES.includes(to.path)) {
      console.log('[Auth Guard] Public route detected')
      // 如果已登入且訪問登入頁，重定向到首頁
      if (authStore.isAuthenticated && to.path === '/login') {
        console.log('[Auth Guard] Already authenticated, redirecting to home')
        next('/')
      } else {
        next()
      }
      return
    }

    // 受保護路由 - 檢查認證狀態
    // 檢查是否有 token（直接檢查 token，而不是 isAuthenticated）
    if (!authStore.token) {
      console.log('[Auth Guard] No token, redirecting to login')
      next('/login')
      return
    }

    // 有 token，檢查是否需要驗證
    const isMockToken = authStore.token.startsWith('mock-jwt-token-')
    console.log('[Auth Guard] Token found, isMock:', isMockToken)
    
    if (!isMockToken) {
      // 真實 token，需要驗證
      const isValid = await lineLoginService.validateToken(authStore.token)
      if (!isValid) {
        console.log('[Auth Guard] Token validation failed')
        authStore.clearAuth()
        next('/login')
        return
      }
      console.log('[Auth Guard] Token validated successfully')
    } else {
      console.log('[Auth Guard] Mock token, skipping validation')
    }

    // Token 有效，允許訪問
    console.log('[Auth Guard] Access granted to:', to.path)
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
