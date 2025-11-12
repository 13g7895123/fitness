import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

interface User {
  id: string
  lineUserId: string
  displayName: string
  pictureUrl: string
}

// 初始化時從 localStorage 恢復資料
const initUser = (): User | null => {
  const savedUser = localStorage.getItem('authUser')
  if (savedUser) {
    try {
      return JSON.parse(savedUser)
    } catch (e) {
      console.error('Failed to parse saved user data:', e)
      return null
    }
  }
  return null
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(initUser())
  const token = ref<string | null>(localStorage.getItem('authToken'))
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value && !!user.value)

  // 從 localStorage 恢復 token 和 user
  const restoreToken = () => {
    const savedToken = localStorage.getItem('authToken')
    const savedUser = localStorage.getItem('authUser')
    if (savedToken) {
      token.value = savedToken
    }
    if (savedUser) {
      try {
        user.value = JSON.parse(savedUser)
      } catch (e) {
        console.error('Failed to parse saved user data:', e)
      }
    }
  }

  // 設定登入資訊
  const setAuth = (newToken: string, userData: User) => {
    token.value = newToken
    user.value = userData
    localStorage.setItem('authToken', newToken)
    localStorage.setItem('authUser', JSON.stringify(userData))
  }

  // 清除登入資訊
  const clearAuth = () => {
    user.value = null
    token.value = null
    localStorage.removeItem('authToken')
    localStorage.removeItem('authUser')
    error.value = null
  }

  // 設定用戶資料
  const setUser = (userData: User) => {
    user.value = userData
  }

  // 設定 Token
  const setToken = (newToken: string) => {
    token.value = newToken
    localStorage.setItem('authToken', newToken)
  }

  // 設定加載狀態
  const setLoading = (loading: boolean) => {
    isLoading.value = loading
  }

  // 設定錯誤訊息
  const setError = (errorMsg: string | null) => {
    error.value = errorMsg
  }

  return {
    user,
    token,
    loading: isLoading,  // 將 isLoading 導出為 loading
    error,
    isAuthenticated,
    restoreToken,
    setAuth,
    clearAuth,
    setUser,
    setToken,
    setLoading,
    setError
  }
})
