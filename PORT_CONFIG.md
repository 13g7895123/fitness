# Port 設定說明

本專案的所有 Port 設定都已統一使用環境變數管理，方便調整和部署。

## 📁 環境變數檔案結構

```
fitness/
├── .env                               # Docker Compose 使用的 Port 設定
├── .env.example                       # Docker Compose 環境變數範本
├── backend/
│   ├── .env                          # 後端環境變數
│   └── .env.example                  # 後端環境變數範本
└── frontend/
    ├── .env.development              # 前端開發環境變數
    ├── .env.development.example      # 前端開發環境變數範本
    └── .env.production.example       # 前端生產環境變數範本
```

---

## 🔧 Port 設定變數

### 根目錄 `.env` (Docker Compose 用)
```bash
API_PORT=9102           # 後端 API Port
FRONTEND_PORT=9202      # 前端 Port
POSTGRES_PORT=9302      # PostgreSQL 資料庫 Port (對外)
```

### 後端 `backend/.env`
```bash
API_PORT=9102                                    # 後端 API Port
POSTGRES_PORT=9302                               # PostgreSQL Port (對外)
FRONTEND_PORT=9202                               # 前端 Port (用於 CORS)
ASPNETCORE_URLS=http://+:9102                   # ASP.NET Core 監聽 URL
Cors__AllowedOrigins=http://localhost:9202      # CORS 允許來源
EnableTestLogin=true                             # 啟用測試登入功能
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=fitness_tracker;Username=postgres;Password=postgres
```

### 前端 `frontend/.env.development`
```bash
VITE_API_PORT=9102                               # 後端 API Port
VITE_FRONTEND_PORT=9202                          # 前端 Port
VITE_API_BASE_URL=http://localhost:9102/api/v1  # API Base URL
VITE_APP_URL=http://localhost:9202              # 前端應用程式 URL
VITE_ENABLE_TEST_LOGIN=true                      # 啟用測試登入按鈕
```

---

## 📝 使用的檔案清單

以下檔案會讀取並使用這些環境變數：

### 後端
1. **`backend/.env`** - 後端主要環境變數檔
2. **`backend/src/FitnessTracker.Api/Program.cs:14`** - 讀取資料庫 Port 環境變數
3. **`backend/src/FitnessTracker.Api/Program.cs:96-97`** - 讀取 CORS 設定
4. **`backend/src/FitnessTracker.Api/Controllers/AuthController.cs:140`** - 讀取測試登入開關
5. **`backend/src/FitnessTracker.Api/appsettings.json:14`** - EnableTestLogin 設定

### 前端
1. **`frontend/.env.development`** - 前端開發環境變數檔
2. **`frontend/vite.config.ts:9-10`** - 讀取 Port 設定
3. **`frontend/src/services/authService.ts:154`** - 測試登入 API URL
4. **`frontend/src/views/Login.vue:51,54`** - 檢查是否啟用測試登入按鈕

### Docker Compose
1. **`.env`** - Docker Compose 環境變數檔（包含資料庫 Port）
2. **`docker-compose.dev.yml:13`** - PostgreSQL Port mapping
3. **`docker-compose.dev.yml:35`** - 後端 API Port mapping
4. **`docker-compose.dev.yml:61`** - 前端 Port mapping
5. **`docker-compose.dev.yml:41-42`** - 後端容器環境變數
6. **`docker-compose.dev.yml:63-65`** - 前端容器環境變數

---

## 🚀 如何修改 Port

### 方法 1: 直接修改環境變數檔案

1. **修改根目錄 `.env`**（Docker 用）
   ```bash
   API_PORT=8080
   FRONTEND_PORT=3000
   ```

2. **修改後端 `backend/.env`**
   ```bash
   API_PORT=8080
   ASPNETCORE_URLS=http://+:8080
   Cors__AllowedOrigins=http://localhost:3000
   ```

3. **修改前端 `frontend/.env.development`**
   ```bash
   VITE_API_PORT=8080
   VITE_FRONTEND_PORT=3000
   VITE_API_BASE_URL=http://localhost:8080/api/v1
   VITE_APP_URL=http://localhost:3000
   VITE_LINE_CALLBACK_URL=http://localhost:3000/auth/callback
   ```

### 方法 2: 使用環境變數覆蓋

在啟動時覆蓋環境變數：

```bash
# 後端
cd backend/src/FitnessTracker.Api
API_PORT=8080 dotnet run

# 前端
cd frontend
VITE_FRONTEND_PORT=3000 npm run dev

# Docker Compose
API_PORT=8080 FRONTEND_PORT=3000 docker compose -f docker-compose.dev.yml up
```

---

## ⚠️ 注意事項

1. **同步修改**: 修改 Port 時，需要同時修改：
   - 根目錄 `.env`（如果使用 Docker）
   - `backend/.env` 中的 `API_PORT` 和 `ASPNETCORE_URLS`
   - `backend/.env` 中的 `Cors__AllowedOrigins`
   - `frontend/.env.development` 中的所有相關 URL

2. **CORS 設定**: 修改前端 Port 後，必須更新後端 CORS 設定，否則會出現跨域錯誤

3. **LINE Login**: 如果使用 LINE 登入，需要在 LINE Developers Console 更新 Callback URL

4. **重新啟動**: 修改環境變數後需要重新啟動應用程式才會生效

---

## 🔍 檢查設定是否正確

### 檢查後端
```bash
# 檢查後端是否在正確的 Port 運行
lsof -i :9102

# 測試 API 連線
curl http://localhost:9102/api/v1/auth/test-login -X POST
```

### 檢查前端
```bash
# 檢查前端是否在正確的 Port 運行
lsof -i :9202

# 瀏覽器訪問
# http://localhost:9202
```

### 檢查資料庫
```bash
# 檢查 PostgreSQL 是否在正確的 Port 運行
lsof -i :9302

# 測試資料庫連線
psql -h localhost -p 9302 -U postgres -d fitness_tracker
```

### 檢查 Docker
```bash
# 檢查 Docker 容器 Port mapping
docker compose -f docker-compose.dev.yml ps

# 查看容器日誌
docker compose -f docker-compose.dev.yml logs backend-dev
docker compose -f docker-compose.dev.yml logs frontend-dev
```

---

## 📚 相關設定檔案

- `backend/src/FitnessTracker.Api/Properties/launchSettings.json` - Visual Studio/Rider 啟動設定
- `frontend/vite.config.ts` - Vite 開發伺服器設定
- `docker-compose.dev.yml` - Docker Compose 開發環境設定

---

## 🆘 常見問題

### Q: 修改 Port 後出現 CORS 錯誤
**A:** 確保後端 `backend/.env` 中的 `Cors__AllowedOrigins` 包含新的前端 URL

### Q: Docker 容器無法啟動
**A:** 檢查 Port 是否被其他程式佔用：`lsof -i :<port>`

### Q: 前端無法連接後端
**A:** 確認以下項目：
1. 後端是否正常運行
2. `VITE_API_BASE_URL` 是否正確
3. 防火牆是否阻擋連線

---

最後更新: 2025-11-12
