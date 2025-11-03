import { createI18n } from 'vue-i18n'
import zhTW from './zh-TW.json'

export type MessageSchema = typeof zhTW

/**
 * 建立 i18n 實例
 */
const i18n = createI18n<{ message: MessageSchema }, 'zhTW'>({
  legacy: false,
  locale: 'zhTW',
  fallbackLocale: 'zhTW',
  messages: {
    zhTW
  },
  globalInjection: true,
  missingWarn: false,
  fallbackWarn: false
})

export default i18n
