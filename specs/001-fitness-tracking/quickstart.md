# 快速開始指南 - 健身紀錄追蹤系統

**專案**: 健身紀錄追蹤系統  
**日期**: 2025-11-02  
**分支**: 001-fitness-tracking  
**目標**: 建立本地開發環境並執行完整應用程式

---

## 目錄

1. [系統需求](#1-系統需求)
2. [環境設定](#2-環境設定)
3. [資料庫設定](#3-資料庫設定)
4. [後端設定](#4-後端設定)
5. [前端設定](#5-前端設定)
6. [Docker 設定](#6-docker-設定)
7. [LINE Login 設定](#7-line-login-設定)
8. [執行測試](#8-執行測試)
9. [常見問題](#9-常見問題)

---

## 1. 系統需求

### 軟體需求

| 軟體 | 最低版本 | 說明 |
|------|---------|------|
| Docker Desktop | 4.0+ | **必要**。用於容器化開發環境，無需本地安裝 .NET/Node.js |
| Git | 2.30+ | 版本控制 |
| VS Code | 最新版 | 推薦搭配 Dev Containers 擴充套件 |

**重要**: 本專案採用**完全容器化開發環境**，所有開發工具（.NET SDK 8.0、Node.js 20）都在 Docker 容器內，無需在本機安裝。

### 推薦開發工具

- **IDE**: VS Code + Dev Containers 擴充套件（可直接附加到容器開發）
- **資料庫管理**: Azure Data Studio、DBeaver 或任何支援 SQL Server 的客戶端
- **API 測試**: Postman 或 Thunder Client (VS Code 擴充套件)

### 硬體需求

- **CPU**: 4 核心以上
- **記憶體**: 16GB RAM（容器化環境建議）
- **硬碟**: 至少 30GB 可用空間（含 Docker 映像）

---

## 2. 快速啟動（容器化開發環境）

### 2.1 啟動所有開發容器

```bash
# 啟動 SQL Server、後端開發容器、前端開發容器
docker-compose -f docker-compose.dev.yml up -d

# 查看容器狀態
docker-compose -f docker-compose.dev.yml ps
```

預期輸出：
```
NAME                    STATUS              PORTS
fitness-sqlserver       Up (healthy)        0.0.0.0:1433->1433/tcp
fitness-backend-dev     Up                  0.0.0.0:5000-5001->5000-5001/tcp
fitness-frontend-dev    Up                  0.0.0.0:5173->5173/tcp
```

### 2.2 環境變數（已預設於 docker-compose）

**後端** (`backend/.env`):
```bash
# 資料庫連線
ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=FitnessTracker;User Id=sa;Password=YourStrong@Password;TrustServerCertificate=True"

# LINE Login 設定（請至 LINE Developers Console 取得）
LineLogin__ChannelId=YOUR_CHANNEL_ID
LineLogin__ChannelSecret=YOUR_CHANNEL_SECRET
LineLogin__CallbackUrl=http://localhost:5000/api/v1/auth/line/callback

# JWT 設定
Jwt__SecretKey=YOUR_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG
Jwt__Issuer=FitnessTrackerApi
Jwt__Audience=FitnessTrackerClient
Jwt__ExpiresInMinutes=1440

# Logging
Serilog__MinimumLevel__Default=Information
Serilog__MinimumLevel__Override__Microsoft=Warning

# CORS
Cors__AllowedOrigins=http://localhost:3000,http://localhost:5173
```

**前端** (`frontend/.env.development`):
```bash
# API Base URL
VITE_API_BASE_URL=http://localhost:5000/api/v1

# LINE Login
VITE_LINE_CHANNEL_ID=YOUR_CHANNEL_ID
VITE_LINE_CALLBACK_URL=http://localhost:3000/auth/callback

# 環境
VITE_ENV=development
```

**前端** (`frontend/.env.production`):
```bash
VITE_API_BASE_URL=https://api.yourdomain.com/api/v1
VITE_LINE_CHANNEL_ID=YOUR_PRODUCTION_CHANNEL_ID
VITE_LINE_CALLBACK_URL=https://yourdomain.com/auth/callback
VITE_ENV=production
```

---

## 3. 後端開發（容器內執行）

### 3.1 進入後端開發容器

```bash
docker exec -it fitness-backend-dev bash
```

### 3.2 初始化 .NET 專案（首次執行）

在容器內執行：
```bash
# 建立專案結構
cd /workspace/src
dotnet new webapi -n FitnessTracker.Api -f net8.0
dotnet new classlib -n FitnessTracker.Core -f net8.0
dotnet new classlib -n FitnessTracker.Infrastructure -f net8.0
dotnet new classlib -n FitnessTracker.Shared -f net8.0

# 建立測試專案
cd /workspace/tests
dotnet new xunit -n FitnessTracker.UnitTests -f net8.0
dotnet new xunit -n FitnessTracker.IntegrationTests -f net8.0
dotnet new xunit -n FitnessTracker.ContractTests -f net8.0

# 建立方案檔
cd /workspace
dotnet new sln -n FitnessTracker
dotnet sln add src/**/*.csproj
dotnet sln add tests/**/*.csproj
```

### 3.3 執行 Migration（稍後階段）

```bash
cd /workspace/src/FitnessTracker.Api
dotnet ef database update --project ../FitnessTracker.Infrastructure
```

### 3.4 執行後端開發伺服器

```bash
cd /workspace/src/FitnessTracker.Api
dotnet watch run
```

後端 API 將於 http://localhost:5000 啟動。

## 4. 前端開發（容器內執行）

### 4.1 進入前端開發容器

```bash
docker exec -it fitness-frontend-dev sh
```

### 4.2 初始化 Vue 專案（首次執行）

在容器內執行：
```bash
cd /workspace
npm create vite@latest . -- --template vue-ts
npm install

# 安裝依賴套件
npm install vue-router@4 pinia axios vuetify@3 chart.js
npm install -D vitest @vue/test-utils happy-dom
npm install -D @playwright/test
npm install -D eslint prettier @typescript-eslint/parser
```

### 4.3 執行前端開發伺服器

```bash
cd /workspace
npm run dev -- --host
```

前端應用將於 http://localhost:5173 啟動。

## 5. 資料庫管理

### 5.1 連線資訊

使用任何 SQL 客戶端連線：
- **伺服器**: `localhost:1433` 或 `localhost,1433`
- **使用者**: `sa`
- **密碼**: `FitnessTracker@2025`
- **資料庫**: `FitnessTracker`（Migration 後才會建立）

### 5.2 驗證資料表建立

連線至資料庫並執行：
```sql
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

應該看到以下資料表：
- `Users`
- `ExerciseTypes`
- `Equipments`
- `WorkoutRecords`
- `WorkoutGoals`

### 5.3 驗證 Seed Data（稍後階段）

```sql
SELECT COUNT(*) FROM ExerciseTypes;  -- 應回傳 7（系統預設項目）
SELECT COUNT(*) FROM Equipments;     -- 應回傳 6（預設器材）
```

---

## 6. VS Code Dev Containers（推薦開發方式）

### 6.1 安裝擴充套件

在 VS Code 安裝：
- **Dev Containers** (ms-vscode-remote.remote-containers)
- **Docker** (ms-azuretools.vscode-docker)

### 6.2 附加到容器

1. 按 `F1` 開啟命令面板
2. 輸入 "Dev Containers: Attach to Running Container..."
3. 選擇 `fitness-backend-dev`（後端）或 `fitness-frontend-dev`（前端）
4. VS Code 會在新視窗中開啟容器環境
5. 在容器內享受完整 IntelliSense、偵錯、終端機支援

### 6.3 容器內開發優勢

- ✅ 無需本地安裝 .NET SDK 或 Node.js
- ✅ 完整的 IntelliSense 和程式碼補全
- ✅ 可直接在容器內偵錯
- ✅ 終端機直接在容器環境執行
- ✅ 跨平台一致性（Windows/Mac/Linux）

---

## 7. 停止與管理容器

### 7.1 停止所有容器（保留資料）

```bash
docker-compose -f docker-compose.dev.yml stop
```

### 7.2 重新啟動

```bash
docker-compose -f docker-compose.dev.yml start
```

### 7.3 停止並移除容器（保留資料庫 Volume）

```bash
docker-compose -f docker-compose.dev.yml down
```

### 7.4 完全清除（包含資料庫資料）

```bash
docker-compose -f docker-compose.dev.yml down -v
```

### 7.5 查看容器日誌

```bash
# 所有容器
docker-compose -f docker-compose.dev.yml logs -f

# 特定容器
docker-compose -f docker-compose.dev.yml logs -f backend-dev
docker-compose -f docker-compose.dev.yml logs -f sqlserver
```

---

## 8. 執行後端 API（容器內）

```bash
cd src/FitnessTracker.Api
dotnet run
```

預期輸出：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 4.4 驗證 API 運作

開啟瀏覽器訪問 Swagger UI：
```
http://localhost:5000/swagger
```

或使用 curl 測試健康檢查端點：
```bash
curl http://localhost:5000/api/health
```

預期回應：
```json
{
  "status": "Healthy",
  "timestamp": "2025-11-02T10:30:00Z"
}
```

---

## 5. 前端設定

### 5.1 安裝 npm 套件

```bash
cd frontend
npm install
```

### 5.2 執行開發伺服器

```bash
npm run dev
```

預期輸出：
```
  VITE v5.0.0  ready in 1200 ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
  ➜  press h to show help
```

### 5.3 驗證前端運作

開啟瀏覽器訪問：
```
http://localhost:5173
```

應該看到登入頁面。

### 5.4 建置生產版本

```bash
npm run build
```

建置產物將位於 `frontend/dist` 目錄。

### 5.5 預覽生產版本

```bash
npm run preview
```

---

## 6. Docker 設定

### 6.1 建置所有容器

```bash
# 在專案根目錄執行
docker-compose build
```

### 6.2 啟動完整環境

```bash
docker-compose up -d
```

這會啟動：
- SQL Server (port 1433)
- 後端 API (port 5000)
- 前端應用程式 (port 3000)

### 6.3 查看容器狀態

```bash
docker-compose ps
```

預期輸出：
```
NAME                SERVICE    STATUS    PORTS
fitness-backend     backend    running   0.0.0.0:5000->80/tcp
fitness-frontend    frontend   running   0.0.0.0:3000->80/tcp
fitness-db          db         running   0.0.0.0:1433->1433/tcp
```

### 6.4 查看容器日誌

```bash
# 查看所有服務日誌
docker-compose logs -f

# 查看特定服務日誌
docker-compose logs -f backend
docker-compose logs -f frontend
```

### 6.5 停止所有容器

```bash
docker-compose down
```

### 6.6 清除資料庫資料（重置）

```bash
docker-compose down -v  # -v 會刪除 volumes
docker-compose up -d
```

---

## 7. LINE Login 設定

### 7.1 註冊 LINE Developers 帳號

1. 前往 [LINE Developers Console](https://developers.line.biz/)
2. 使用 LINE 帳號登入
3. 建立新的 Provider（例如：FitnessTracker）

### 7.2 建立 Channel

1. 點選 "Create a new channel"
2. 選擇 "LINE Login"
3. 填寫必要資訊：
   - **Channel name**: 健身紀錄追蹤
   - **Channel description**: 追蹤健身紀錄與目標的應用程式
   - **App types**: Web app
4. 提交並建立

### 7.3 設定 Callback URL

在 Channel 設定頁面：
1. 找到 "LINE Login" 分頁
2. 在 "Callback URL" 欄位填寫：
   ```
   http://localhost:5000/api/v1/auth/line/callback
   ```
3. 儲存設定

### 7.4 取得 Credentials

在 "Basic settings" 分頁複製：
- **Channel ID**
- **Channel secret**

將這兩個值填入 `backend/.env` 與 `frontend/.env.development`。

### 7.5 測試 LINE Login

1. 啟動後端與前端
2. 訪問 http://localhost:5173
3. 點擊「使用 LINE 登入」按鈕
4. 使用 LINE 帳號授權
5. 應成功重導向回應用程式並顯示使用者資訊

---

## 8. 執行測試

### 8.1 後端測試

**執行所有測試**:
```bash
cd backend
dotnet test
```

**執行特定測試專案**:
```bash
dotnet test tests/FitnessTracker.UnitTests
dotnet test tests/FitnessTracker.IntegrationTests
dotnet test tests/FitnessTracker.ContractTests
```

**產生測試覆蓋率報告**:
```bash
dotnet test --collect:"XPlat Code Coverage"
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

覆蓋率報告位於 `coverage-report/index.html`。

### 8.2 前端測試

**執行單元測試**:
```bash
cd frontend
npm run test:unit
```

**執行元件測試**:
```bash
npm run test:component
```

**執行 E2E 測試** (需先啟動後端):
```bash
npm run test:e2e
```

**產生測試覆蓋率報告**:
```bash
npm run test:coverage
```

覆蓋率報告位於 `frontend/coverage/index.html`。

### 8.3 API Contract 測試

使用 Postman Collection（位於 `specs/001-fitness-tracking/contracts/postman/`）：

1. 匯入 Collection 至 Postman
2. 設定環境變數：
   - `baseUrl`: http://localhost:5000/api/v1
   - `accessToken`: [從 LINE Login 取得]
3. 執行整個 Collection

或使用命令列（需安裝 Newman）：
```bash
npm install -g newman
newman run specs/001-fitness-tracking/contracts/postman/fitness-tracker.postman_collection.json \
  --environment specs/001-fitness-tracking/contracts/postman/local.postman_environment.json
```

---

## 9. 常見問題

### 9.1 資料庫連線失敗

**錯誤訊息**:
```
Microsoft.Data.SqlClient.SqlException: A network-related or instance-specific error occurred
```

**解決方案**:
1. 確認 SQL Server 容器正在執行：
   ```bash
   docker ps | grep sql
   ```
2. 檢查防火牆是否阻擋 port 1433
3. 驗證 `appsettings.json` 連線字串是否正確
4. 等待 30 秒讓 SQL Server 完全啟動

### 9.2 Migration 執行失敗

**錯誤訊息**:
```
Login failed for user 'sa'
```

**解決方案**:
1. 確認密碼符合 SQL Server 複雜度要求（至少 8 字元，包含大小寫、數字、特殊符號）
2. 重新建立容器：
   ```bash
   docker-compose down -v
   docker-compose up -d db
   ```
3. 等待 30 秒後重新執行 Migration

### 9.3 前端無法連線至後端 API

**錯誤訊息**:
```
Network Error: Failed to fetch
```

**解決方案**:
1. 確認後端 API 正在執行：
   ```bash
   curl http://localhost:5000/api/health
   ```
2. 檢查 `frontend/.env.development` 中的 `VITE_API_BASE_URL` 是否正確
3. 確認 CORS 設定允許前端來源（`http://localhost:5173`）
4. 檢查瀏覽器控制台是否有 CORS 錯誤

### 9.4 LINE Login 失敗

**錯誤訊息**:
```
invalid_request: redirect_uri is invalid
```

**解決方案**:
1. 確認 LINE Developers Console 中的 Callback URL 與程式碼一致
2. Callback URL 必須使用 HTTP/HTTPS，不可使用 IP
3. 本地開發使用 `http://localhost`，不要使用 `http://127.0.0.1`
4. 確認 Channel ID 與 Channel Secret 正確

### 9.5 Docker 容器無法啟動

**錯誤訊息**:
```
Error response from daemon: Ports are not available
```

**解決方案**:
1. 檢查 port 是否被其他程式佔用：
   ```bash
   # Windows
   netstat -ano | findstr :5000
   
   # Linux/Mac
   lsof -i :5000
   ```
2. 修改 `docker-compose.yml` 使用不同 port
3. 停止佔用 port 的程式

### 9.6 測試覆蓋率未達標

**問題**: 測試覆蓋率低於 80%

**解決方案**:
1. 執行覆蓋率報告找出未測試的程式碼：
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```
2. 針對低覆蓋率檔案新增測試
3. 確保至少涵蓋：
   - 所有 public 方法
   - 錯誤處理邏輯
   - 驗證規則
   - 業務邏輯

### 9.7 Vuetify 元件顯示異常

**問題**: 元件沒有樣式或顯示錯誤

**解決方案**:
1. 確認已安裝 Vuetify：
   ```bash
   npm list vuetify
   ```
2. 檢查 `main.ts` 中是否正確匯入樣式：
   ```typescript
   import 'vuetify/styles'
   ```
3. 清除 node_modules 重新安裝：
   ```bash
   rm -rf node_modules package-lock.json
   npm install
   ```

---

## 附錄：開發工作流程

### 日常開發流程

1. **啟動開發環境**:
   ```bash
   docker-compose up -d db  # 只啟動資料庫
   cd backend/src/FitnessTracker.Api && dotnet watch run  # 後端 hot reload
   cd frontend && npm run dev  # 前端 hot reload
   ```

2. **進行程式碼變更**

3. **執行測試**:
   ```bash
   # 後端
   dotnet test --filter Category=Unit
   
   # 前端
   npm run test:unit
   ```

4. **Commit 前檢查**:
   ```bash
   # 後端格式化
   dotnet format
   
   # 前端 Lint
   npm run lint
   
   # 執行完整測試
   dotnet test
   npm run test:all
   ```

5. **提交變更**:
   ```bash
   git add .
   git commit -m "feat: 新增健身紀錄查詢功能"
   git push origin 001-fitness-tracking
   ```

### 新增 Migration

當修改實體定義後：
```bash
cd backend/src/FitnessTracker.Api
dotnet ef migrations add [MigrationName] --project ../FitnessTracker.Infrastructure
dotnet ef database update --project ../FitnessTracker.Infrastructure
```

### 產生 API 文件

Swagger JSON：
```bash
cd backend/src/FitnessTracker.Api
dotnet run
# 訪問 http://localhost:5000/swagger/v1/swagger.json
```

### 效能監控

使用 Application Insights（生產環境）或本地 Prometheus + Grafana（開發環境）監控：
- API 回應時間
- 資料庫查詢效能
- 記憶體使用量
- 錯誤率

---

## 取得協助

- **文件**: 參考 `specs/001-fitness-tracking/` 目錄下的規格文件
- **API 規格**: 查看 `specs/001-fitness-tracking/contracts/api-spec.md`
- **資料模型**: 查看 `specs/001-fitness-tracking/data-model.md`
- **技術研究**: 查看 `specs/001-fitness-tracking/research.md`

**下一步**: 查看 `tasks.md`（由 `/speckit.tasks` 命令產生）以開始實作任務
