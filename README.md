# 健身紀錄追蹤系統

## 容器化開發環境快速啟動

本專案採用 **完全容器化開發環境**，無需在本地安裝 .NET SDK 或 Node.js。

### 前置需求

- Docker Desktop（Windows/Mac）或 Docker Engine（Linux）
- VS Code + Dev Containers 擴充套件（選用，提升開發體驗）

### 1. 啟動開發環境容器

```bash
docker-compose -f docker-compose.dev.yml up -d
```

這會啟動三個容器：
- `fitness-sqlserver`: SQL Server 2022 資料庫
- `fitness-backend-dev`: .NET 8 SDK 開發容器
- `fitness-frontend-dev`: Node.js 20 開發容器

### 2. 後端開發

進入後端容器：
```bash
docker exec -it fitness-backend-dev bash
```

在容器內執行：
```bash
# 初始化 .NET 專案
cd /workspace/src
dotnet new webapi -n FitnessTracker.Api -f net8.0
dotnet new classlib -n FitnessTracker.Core -f net8.0
dotnet new classlib -n FitnessTracker.Infrastructure -f net8.0
dotnet new classlib -n FitnessTracker.Shared -f net8.0

# 建立方案檔
cd /workspace
dotnet new sln -n FitnessTracker
dotnet sln add src/FitnessTracker.Api
dotnet sln add src/FitnessTracker.Core
dotnet sln add src/FitnessTracker.Infrastructure
dotnet sln add src/FitnessTracker.Shared

# 執行後端
cd /workspace/src/FitnessTracker.Api
dotnet watch run
```

### 3. 前端開發

進入前端容器：
```bash
docker exec -it fitness-frontend-dev sh
```

在容器內執行：
```bash
# 初始化 Vue 3 + TypeScript 專案
cd /workspace
npm create vite@latest . -- --template vue-ts
npm install

# 啟動開發伺服器
npm run dev -- --host
```

### 4. 資料庫管理

使用任何 SQL 客戶端連線：
- **Server**: `localhost:1433`
- **User**: `sa`
- **Password**: `FitnessTracker@2025`
- **Database**: `FitnessTracker`

### 5. 停止開發環境

```bash
docker-compose -f docker-compose.dev.yml down
```

保留資料庫資料，僅停止容器：
```bash
docker-compose -f docker-compose.dev.yml stop
```

## 連線資訊

| 服務 | 網址 | 說明 |
|------|------|------|
| 後端 API | http://localhost:5000 | ASP.NET Core Web API |
| 前端應用 | http://localhost:5173 | Vue.js + Vite |
| 資料庫 | localhost:1433 | SQL Server 2022 |

## VS Code Dev Containers（推薦）

安裝 "Dev Containers" 擴充套件後：
1. 按 `F1` 開啟命令面板
2. 選擇 "Dev Containers: Attach to Running Container..."
3. 選擇 `fitness-backend-dev` 或 `fitness-frontend-dev`
4. 在容器內直接開發，享受完整 IntelliSense 支援

## 常見指令

```bash
# 查看容器狀態
docker-compose -f docker-compose.dev.yml ps

# 查看容器日誌
docker-compose -f docker-compose.dev.yml logs -f

# 重新建立容器
docker-compose -f docker-compose.dev.yml up -d --build

# 清除所有資料（包含資料庫）
docker-compose -f docker-compose.dev.yml down -v
```

## 專案結構

```
fitness/
├── backend/
│   ├── src/
│   │   ├── FitnessTracker.Api/
│   │   ├── FitnessTracker.Core/
│   │   ├── FitnessTracker.Infrastructure/
│   │   └── FitnessTracker.Shared/
│   └── tests/
├── frontend/
│   ├── src/
│   └── tests/
├── docker-compose.dev.yml      # 開發環境配置
└── README.md
```
