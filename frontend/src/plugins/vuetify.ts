import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { zhHant } from 'vuetify/locale'

/**
 * 自訂色彩主題 - 簡約風格
 */
const customTheme = {
  colors: {
    primary: '#2563eb',
    secondary: '#64748b',
    accent: '#f59e0b',
    error: '#ef4444',
    warning: '#f59e0b',
    info: '#3b82f6',
    success: '#10b981',
    surface: '#ffffff',
    background: '#fafafa'
  }
}

/**
 * 建立 Vuetify 實例
 */
export default createVuetify({
  components,
  directives,
  locale: {
    locale: 'zhHant',
    messages: {
      zhHant: {
        ...zhHant,
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
  },
  defaults: {
    VCard: {
      elevation: 0,
      rounded: 'lg',
    },
    VBtn: {
      rounded: 'lg',
    },
    VTextField: {
      variant: 'outlined',
      density: 'comfortable',
      rounded: 'lg',
    },
    VSelect: {
      variant: 'outlined',
      density: 'comfortable',
      rounded: 'lg',
    },
    VTextarea: {
      variant: 'outlined',
      density: 'comfortable',
      rounded: 'lg',
    },
    VAutocomplete: {
      variant: 'outlined',
      density: 'comfortable',
      rounded: 'lg',
    },
    VDialog: {
      rounded: 'xl',
    },
  },
})
