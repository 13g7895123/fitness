import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { zhTW } from 'vuetify/locale'

/**
 * 自訂色彩主題
 */
const customTheme = {
  colors: {
    primary: '#1976D2',
    secondary: '#26A69A',
    accent: '#82B1FF',
    error: '#FF5252',
    warning: '#FFC107',
    info: '#2196F3',
    success: '#4CAF50',
    surface: '#FFFFFF',
    background: '#F5F5F5'
  }
}

/**
 * 建立 Vuetify 實例
 */
export default createVuetify({
  components,
  directives,
  locale: {
    locale: 'zhTW',
    messages: {
      zhTW: {
        ...zhTW,
        // 自訂繁體中文翻譯
        close: '關閉',
        edit: '編輯',
        delete: '刪除',
        save: '保存',
        cancel: '取消',
        add: '新增',
        search: '搜尋',
        loading: '加載中...',
        error: '錯誤',
        success: '成功',
        warning: '警告',
        confirm: '確認',
        yes: '是',
        no: '否'
      }
    }
  },
  theme: {
    defaultTheme: 'light',
    themes: {
      light: {
        dark: false,
        colors: customTheme.colors
      },
      dark: {
        dark: true,
        colors: {
          ...customTheme.colors,
          surface: '#1E1E1E',
          background: '#121212'
        }
      }
    }
  }
})
