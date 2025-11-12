import { useAuthStore } from '@/stores/auth'
import { useErrorHandler, showSuccess } from '@/utils/errorHandler'

interface LineTokenResponse {
  access_token: string
  token_type: string
  expires_in: number
}

interface LineUserInfo {
  sub: string
  name: string
  picture: string
  email?: string
}

interface AuthResponse {
  success: boolean
  message: string
  data?: {
    token: string
    user: {
      id: string
      lineUserId: string
      displayName: string
      pictureUrl: string
    }
  }
  errors?: Array<{ code: string; message: string }>
}

export const useLineLoginService = () => {
  const authStore = useAuthStore()
  const { showError } = useErrorHandler()

  const CHANNEL_ID = import.meta.env.VITE_LINE_CHANNEL_ID
  const REDIRECT_URI = `${import.meta.env.VITE_APP_URL}/auth/callback`
  const LINE_AUTH_URL = 'https://access.line.me/oauth2/v2.1/authorize'

  /**
   * 生成 LINE 登入 URL
   */
  const getLineLoginUrl = (): string => {
    const state = generateRandomState()
    sessionStorage.setItem('line_oauth_state', state)

    const params = new URLSearchParams({
      response_type: 'code',
      client_id: CHANNEL_ID,
      redirect_uri: REDIRECT_URI,
      state,
      scope: 'profile openid email'
    })

    return `${LINE_AUTH_URL}?${params.toString()}`
  }

  /**
   * 處理 LINE OAuth callback
   */
  const handleCallback = async (code: string, state: string): Promise<boolean> => {
    try {
      authStore.setLoading(true)
      authStore.setError(null)

      // 驗證 state 參數
      const savedState = sessionStorage.getItem('line_oauth_state')
      if (!savedState || savedState !== state) {
        throw new Error('State 驗證失敗，可能是跨站請求偽造攻擊')
      }

      const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

      // 向後端交換 token
      const response = await fetch(`${API_BASE_URL}/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          code,
          redirectUri: REDIRECT_URI
        })
      })

      if (!response.ok) {
        throw new Error(`登入失敗: ${response.statusText}`)
      }

      const data: AuthResponse = await response.json()

      if (!data.success) {
        throw new Error(data.message || '登入失敗')
      }

      if (!data.data) {
        throw new Error('無效的回應資料')
      }

      // 保存認證資訊
      authStore.setAuth(data.data.token, data.data.user)

      // 清除臨時 state
      sessionStorage.removeItem('line_oauth_state')

      // 顯示成功訊息
      showSuccess(`歡迎回來，${data.data.user.displayName}！`)

      return true
    } catch (error) {
      const message = error instanceof Error ? error.message : '登入過程中出現錯誤'
      authStore.setError(message)
      showError(message)
      return false
    } finally {
      authStore.setLoading(false)
    }
  }

  /**
   * 登出
   */
  const logout = async (): Promise<void> => {
    try {
      authStore.clearAuth()
    } catch (error) {
      const message = error instanceof Error ? error.message : '登出失敗'
      showError(message)
    }
  }

  /**
   * 驗證 token
   */
  const validateToken = async (token: string): Promise<boolean> => {
    try {
      const API_BASE_URL = import.meta.env.VITE_API_BASE_URL
      const response = await fetch(`${API_BASE_URL}/auth/validate`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      })

      return response.ok
    } catch (error) {
      return false
    }
  }

  /**
   * 測試帳號登入
   */
  const testLogin = async (): Promise<boolean> => {
    try {
      authStore.setLoading(true)
      authStore.setError(null)

      const API_BASE_URL = import.meta.env.VITE_API_BASE_URL
      const response = await fetch(`${API_BASE_URL}/auth/test-login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        }
      })

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}))
        throw new Error(errorData.error || `測試登入失敗: ${response.statusText}`)
      }

      const data = await response.json()

      // 保存認證資訊
      authStore.setAuth(data.accessToken, {
        id: data.user.id,
        lineUserId: data.user.lineUserId,
        displayName: data.user.displayName,
        pictureUrl: data.user.pictureUrl
      })

      // 顯示成功訊息
      showSuccess(`歡迎回來，${data.user.displayName}！`)

      return true
    } catch (error) {
      const message = error instanceof Error ? error.message : '測試登入失敗'
      authStore.setError(message)
      showError(message)
      return false
    } finally {
      authStore.setLoading(false)
    }
  }

  /**
   * 生成隨機 state 字符串
   */
  const generateRandomState = (): string => {
    return Math.random().toString(36).substring(2, 15) +
           Math.random().toString(36).substring(2, 15)
  }

  return {
    getLineLoginUrl,
    handleCallback,
    logout,
    validateToken,
    testLogin
  }
}
