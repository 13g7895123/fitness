import { useAuthStore } from '@/stores/auth'
import { useErrorHandler } from '@/utils/errorHandler'

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

      // 向後端交換 token
      const response = await fetch('/api/v1/auth/login', {
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
      const response = await fetch('/api/v1/auth/validate', {
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
    validateToken
  }
}
