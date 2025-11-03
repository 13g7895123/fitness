import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosError } from 'axios'
import { useAuthStore } from '@/stores/auth'
import { useErrorHandler } from '@/utils/errorHandler'

interface CustomAxiosRequestConfig extends InternalAxiosRequestConfig {
  skipErrorHandler?: boolean
}

/**
 * 建立 Axios 實例，自動帶入 JWT Token
 */
export const createApiClient = (): AxiosInstance => {
  const authStore = useAuthStore()
  const { showError } = useErrorHandler()

  const apiClient = axios.create({
    baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000',
    timeout: 10000,
    headers: {
      'Content-Type': 'application/json'
    }
  })

  /**
   * 請求攔截器 - 加入 JWT Token
   */
  apiClient.interceptors.request.use(
    (config: CustomAxiosRequestConfig) => {
      const token = authStore.token
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    },
    (error) => {
      return Promise.reject(error)
    }
  )

  /**
   * 回應攔截器 - 處理錯誤
   */
  apiClient.interceptors.response.use(
    (response) => {
      return response
    },
    async (error: AxiosError) => {
      const config = error.config as CustomAxiosRequestConfig

      // 401 Unauthorized - Token 過期或無效
      if (error.response?.status === 401) {
        authStore.clearAuth()
        window.location.href = '/login'
        return Promise.reject(error)
      }

      // 403 Forbidden - 無權限
      if (error.response?.status === 403) {
        if (!config?.skipErrorHandler) {
          showError('您無權執行此操作')
        }
        return Promise.reject(error)
      }

      // 404 Not Found
      if (error.response?.status === 404) {
        if (!config?.skipErrorHandler) {
          showError('要求的資源不存在')
        }
        return Promise.reject(error)
      }

      // 500+ Server Error
      if (error.response?.status && error.response.status >= 500) {
        if (!config?.skipErrorHandler) {
          showError('伺服器錯誤，請稍後重試')
        }
        return Promise.reject(error)
      }

      // 其他錯誤
      if (!config?.skipErrorHandler) {
        const message =
          (error.response?.data as any)?.message ||
          error.message ||
          '網路請求失敗'
        showError(message)
      }

      return Promise.reject(error)
    }
  )

  return apiClient
}

export const api = createApiClient()

export default api
