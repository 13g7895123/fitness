import { ref } from 'vue'

export interface ErrorNotification {
  id: string
  message: string
  type: 'error' | 'warning' | 'info' | 'success'
  duration: number
  timestamp: number
}

const notifications = ref<ErrorNotification[]>([])
const nextId = ref(0)

/**
 * 顯示錯誤通知
 */
export const showError = (message: string, duration = 5000): string => {
  return addNotification(message, 'error', duration)
}

/**
 * 顯示成功通知
 */
export const showSuccess = (message: string, duration = 3000): string => {
  return addNotification(message, 'success', duration)
}

/**
 * 顯示警告通知
 */
export const showWarning = (message: string, duration = 4000): string => {
  return addNotification(message, 'warning', duration)
}

/**
 * 顯示資訊通知
 */
export const showInfo = (message: string, duration = 3000): string => {
  return addNotification(message, 'info', duration)
}

/**
 * 新增通知
 */
const addNotification = (
  message: string,
  type: 'error' | 'warning' | 'info' | 'success',
  duration: number
): string => {
  const id = `notification-${nextId.value++}`
  const notification: ErrorNotification = {
    id,
    message,
    type,
    duration,
    timestamp: Date.now()
  }

  notifications.value.push(notification)

  // 自動移除通知
  if (duration > 0) {
    setTimeout(() => {
      removeNotification(id)
    }, duration)
  }

  return id
}

/**
 * 移除通知
 */
export const removeNotification = (id: string): void => {
  const index = notifications.value.findIndex((n) => n.id === id)
  if (index > -1) {
    notifications.value.splice(index, 1)
  }
}

/**
 * 清除所有通知
 */
export const clearNotifications = (): void => {
  notifications.value = []
}

/**
 * 取得所有通知
 */
export const getNotifications = () => {
  return notifications
}

/**
 * useErrorHandler Composable
 */
export const useErrorHandler = () => {
  return {
    showError,
    showSuccess,
    showWarning,
    showInfo,
    removeNotification,
    clearNotifications,
    getNotifications
  }
}

/**
 * 處理 API 錯誤
 */
export const handleApiError = (error: any): string => {
  let message = '發生錯誤'

  if (error.response) {
    // 伺服器回應了錯誤狀態
    const status = error.response.status
    const data = error.response.data

    if (status === 400) {
      message = data?.message || '請求無效'
    } else if (status === 401) {
      message = '您的登入已過期，請重新登入'
    } else if (status === 403) {
      message = '您無權執行此操作'
    } else if (status === 404) {
      message = '要求的資源不存在'
    } else if (status === 409) {
      message = data?.message || '資料衝突'
    } else if (status >= 500) {
      message = '伺服器錯誤，請稍後重試'
    } else {
      message = data?.message || `錯誤 (${status})`
    }
  } else if (error.request) {
    // 請求已發出但沒有收到回應
    message = '網路連接失敗，請檢查您的連接'
  } else {
    // 其他錯誤
    message = error.message || '發生未知錯誤'
  }

  showError(message)
  return message
}
