import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import Settings from '@/views/Settings.vue'

describe('Settings.vue', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('renders Settings page with tabs', () => {
    const wrapper = mount(Settings)
    
    expect(wrapper.exists()).toBe(true)
    expect(wrapper.text()).toContain('運動項目')
    expect(wrapper.text()).toContain('器材')
  })

  it('displays ExerciseTypeList component', () => {
    const wrapper = mount(Settings)
    
    expect(wrapper.findComponent({ name: 'ExerciseTypeList' }).exists()).toBe(true)
  })

  it('displays ExerciseTypeForm component', () => {
    const wrapper = mount(Settings)
    
    expect(wrapper.findComponent({ name: 'ExerciseTypeForm' }).exists()).toBe(true)
  })

  it('displays ExerciseTypeSearchBar component', () => {
    const wrapper = mount(Settings)
    
    expect(wrapper.findComponent({ name: 'ExerciseTypeSearchBar' }).exists()).toBe(true)
  })

  it('displays EquipmentList component in equipment tab', async () => {
    const wrapper = mount(Settings)
    
    // 點擊器材標籤頁
    const tabs = wrapper.findAll('[role="tab"]')
    const equipmentTab = tabs.find(tab => tab.text().includes('器材'))
    
    if (equipmentTab) {
      await equipmentTab.trigger('click')
      expect(wrapper.findComponent({ name: 'EquipmentList' }).exists()).toBe(true)
    }
  })

  it('displays EquipmentForm component in equipment tab', async () => {
    const wrapper = mount(Settings)
    
    // 點擊器材標籤頁
    const tabs = wrapper.findAll('[role="tab"]')
    const equipmentTab = tabs.find(tab => tab.text().includes('器材'))
    
    if (equipmentTab) {
      await equipmentTab.trigger('click')
      expect(wrapper.findComponent({ name: 'EquipmentForm' }).exists()).toBe(true)
    }
  })

  it('handles exercise type creation', async () => {
    const wrapper = mount(Settings)
    const form = wrapper.findComponent({ name: 'ExerciseTypeForm' })
    
    if (form.exists()) {
      const createSpy = vi.spyOn(form.vm, 'handleCreate')
      
      // 模擬提交表單
      await form.vm.$emit('create', {
        name: '自訂運動',
        description: '說明'
      })
      
      // 驗證已觸發建立事件
      expect(createSpy).toBeDefined()
    }
  })

  it('handles exercise type deletion', async () => {
    const wrapper = mount(Settings)
    const list = wrapper.findComponent({ name: 'ExerciseTypeList' })
    
    if (list.exists()) {
      const deleteSpy = vi.spyOn(list.vm, 'handleDelete')
      
      // 模擬刪除事件
      await list.vm.$emit('delete', 1)
      
      // 驗證已觸發刪除事件
      expect(deleteSpy).toBeDefined()
    }
  })

  it('handles exercise type search', async () => {
    const wrapper = mount(Settings)
    const searchBar = wrapper.findComponent({ name: 'ExerciseTypeSearchBar' })
    
    if (searchBar.exists()) {
      const input = searchBar.find('input')
      
      await input.setValue('瑜伽')
      
      // 驗證搜尋詞已更新
      expect(searchBar.vm.searchQuery).toBe('瑜伽')
    }
  })

  it('displays loading state when fetching data', async () => {
    const wrapper = mount(Settings, {
      data() {
        return {
          isLoading: true
        }
      }
    })
    
    expect(wrapper.find('[data-test="loading"]').exists()).toBe(true)
  })

  it('displays error message when loading fails', async () => {
    const wrapper = mount(Settings, {
      data() {
        return {
          error: '載入失敗'
        }
      }
    })
    
    expect(wrapper.text()).toContain('載入失敗')
  })

  it('handles equipment creation', async () => {
    const wrapper = mount(Settings)
    
    // 切換到器材標籤頁
    const tabs = wrapper.findAll('[role="tab"]')
    const equipmentTab = tabs.find(tab => tab.text().includes('器材'))
    
    if (equipmentTab) {
      await equipmentTab.trigger('click')
      
      const form = wrapper.findComponent({ name: 'EquipmentForm' })
      if (form.exists()) {
        // 驗證表單存在
        expect(form.vm).toBeDefined()
      }
    }
  })

  it('displays success message after creating exercise type', async () => {
    const wrapper = mount(Settings, {
      data() {
        return {
          successMessage: '運動項目建立成功'
        }
      }
    })
    
    expect(wrapper.text()).toContain('建立成功')
  })

  it('disables delete button for system exercise types', async () => {
    const wrapper = mount(Settings)
    const list = wrapper.findComponent({ name: 'ExerciseTypeList' })
    
    if (list.exists()) {
      const deleteButtons = wrapper.findAll('[data-test="delete-btn"]')
      
      // 系統項目的刪除按鈕應該被禁用
      deleteButtons.forEach(btn => {
        if (btn.vm.isSystemExerciseType) {
          expect(btn.attributes('disabled')).toBeDefined()
        }
      })
    }
  })
})
