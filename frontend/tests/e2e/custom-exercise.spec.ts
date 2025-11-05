import { test, expect } from '@playwright/test'

test.describe('自訂運動項目功能', () => {
  test.beforeEach(async ({ page }) => {
    // 登入
    await page.goto('/login')
    
    // 填入登入信息（實際應使用測試帳號）
    await page.fill('input[placeholder="帳號"]', 'testuser@example.com')
    await page.fill('input[placeholder="密碼"]', 'testpassword')
    await page.click('button:has-text("登入")')
    
    // 等待重定向到首頁
    await page.waitForURL('/')
  })

  test('應該能夠導航到設定頁面', async ({ page }) => {
    // 點擊導航菜單中的設定
    await page.click('button:has-text("設定")')
    
    // 驗證已導航到設定頁面
    await expect(page).toHaveURL('/settings')
    await expect(page.locator('h1')).toContainText('設定')
  })

  test('應該能夠新增自訂運動項目', async ({ page }) => {
    // 導航到設定頁面
    await page.goto('/settings')
    
    // 等待表單加載
    await page.waitForSelector('form')
    
    // 填入運動項目名稱
    await page.fill('input[placeholder="運動項目名稱"]', '自訂瑜伽')
    
    // 填入說明
    await page.fill('textarea[placeholder="說明"]', '個人化瑜伽課程')
    
    // 點擊提交按鈕
    await page.click('button:has-text("新增")')
    
    // 驗證成功訊息
    await expect(page.locator('text=運動項目建立成功')).toBeVisible()
    
    // 驗證新項目出現在清單中
    await expect(page.locator('text=自訂瑜伽')).toBeVisible()
  })

  test('應該能夠編輯自訂運動項目', async ({ page }) => {
    // 導航到設定頁面
    await page.goto('/settings')
    
    // 先新增一個項目
    await page.fill('input[placeholder="運動項目名稱"]', '原始名稱')
    await page.fill('textarea[placeholder="說明"]', '原始說明')
    await page.click('button:has-text("新增")')
    
    // 等待項目出現
    await page.waitForSelector('text=原始名稱')
    
    // 點擊編輯按鈕
    await page.click('button[data-test="edit-btn"]:first-of-type')
    
    // 更新名稱
    await page.fill('input[placeholder="運動項目名稱"]', '更新名稱')
    
    // 點擊保存按鈕
    await page.click('button:has-text("保存")')
    
    // 驗證成功訊息
    await expect(page.locator('text=更新成功')).toBeVisible()
    
    // 驗證清單已更新
    await expect(page.locator('text=更新名稱')).toBeVisible()
  })

  test('應該能夠刪除自訂運動項目', async ({ page }) => {
    // 導航到設定頁面
    await page.goto('/settings')
    
    // 先新增一個項目
    await page.fill('input[placeholder="運動項目名稱"]', '待刪除項目')
    await page.fill('textarea[placeholder="說明"]', '說明')
    await page.click('button:has-text("新增")')
    
    // 等待項目出現
    await page.waitForSelector('text=待刪除項目')
    
    // 點擊刪除按鈕
    await page.click('button[data-test="delete-btn"]:first-of-type')
    
    // 確認刪除對話框
    await page.click('button:has-text("確認")')
    
    // 驗證成功訊息
    await expect(page.locator('text=刪除成功')).toBeVisible()
    
    // 驗證項目已被移除
    await expect(page.locator('text=待刪除項目')).not.toBeVisible()
  })

  test('應該能夠搜尋運動項目', async ({ page }) => {
    // 導航到設定頁面
    await page.goto('/settings')
    
    // 在搜尋欄位輸入查詢詞
    await page.fill('input[placeholder="搜尋運動項目"]', '瑜伽')
    
    // 等待搜尋結果更新
    await page.waitForTimeout(500)
    
    // 驗證只顯示包含'瑜伽'的項目
    const items = page.locator('[data-test="exercise-item"]')
    const count = await items.count()
    
    for (let i = 0; i < count; i++) {
      const text = await items.nth(i).textContent()
      expect(text).toContain('瑜伽')
    }
  })

  test('應該能夠設定運動項目的器材', async ({ page }) => {
    // 導航到設定頁面
    await page.goto('/settings')
    
    // 填入運動項目名稱
    await page.fill('input[placeholder="運動項目名稱"]', '舉重')
    
    // 選擇器材
    await page.click('[data-test="equipment-selector"]')
    await page.click('text=啞鈴')
    await page.click('text=槓鈴')
    
    // 點擊提交
    await page.click('button:has-text("新增")')
    
    // 驗證成功
    await expect(page.locator('text=建立成功')).toBeVisible()
    
    // 驗證器材已關聯
    const exerciseItem = page.locator('text=舉重').first()
    await expect(exerciseItem).toContainText('啞鈴')
  })

  test('應該能夠管理器材', async ({ page }) => {
    // 導航到設定頁面
    await page.goto('/settings')
    
    // 點擊器材標籤頁
    await page.click('button:has-text("器材")')
    
    // 等待器材表單加載
    await page.waitForSelector('[data-test="equipment-form"]')
    
    // 填入器材名稱
    await page.fill('input[placeholder="器材名稱"]', '瑜伽墊')
    await page.fill('textarea[placeholder="說明"]', '軟質墊子')
    
    // 點擊新增按鈕
    await page.click('button:has-text("新增器材")')
    
    // 驗證成功訊息
    await expect(page.locator('text=器材新增成功')).toBeVisible()
    
    // 驗證器材出現在清單中
    await expect(page.locator('text=瑜伽墊')).toBeVisible()
  })

  test('應該禁止刪除系統運動項目', async ({ page }) => {
    // 導航到設定頁面
    await page.goto('/settings')
    
    // 查找系統項目的刪除按鈕
    const deleteButtons = page.locator('button[data-test="delete-btn"][disabled]')
    
    // 驗證系統項目的刪除按鈕已禁用
    const count = await deleteButtons.count()
    expect(count).toBeGreaterThan(0)
  })

  test('應該顯示運動項目的詳細信息', async ({ page }) => {
    // 導航到設定頁面
    await page.goto('/settings')
    
    // 點擊一個項目查看詳細信息
    await page.click('[data-test="exercise-item"]:first-of-type')
    
    // 驗證詳細資訊對話框已顯示
    await expect(page.locator('[data-test="detail-dialog"]')).toBeVisible()
    
    // 驗證包含項目詳細信息
    await expect(page.locator('[data-test="detail-name"]')).toBeVisible()
    await expect(page.locator('[data-test="detail-description"]')).toBeVisible()
  })
})
