# 研究文件 - 健身紀錄追蹤功能

**專案**: 健身紀錄追蹤系統  
**日期**: 2025-11-02  
**分支**: 001-fitness-tracking  
**階段**: Phase 0 - 技術研究

## 研究目標

本文件針對技術背景中的未知項目進行研究，確保實作前所有技術決策皆有充分依據。

---

## 1. LINE Login SDK 整合（.NET Core 8）

### 研究問題
- 如何在 ASP.NET Core 8 整合 LINE Login OAuth 2.0？
- 身份驗證流程與 Token 管理最佳實踐？

### 研究結果

**官方套件**: 使用 `LINE.Messaging` NuGet 套件（LINE 官方維護）

**OAuth 2.0 流程**:
1. 前端導向 LINE 授權頁面：`https://access.line.me/oauth2/v2.1/authorize`
2. 使用者授權後，LINE 重導向至 callback URL 並附帶 `code`
3. 後端用 `code` 交換 `access_token` 與 `id_token`
4. 驗證 `id_token`（JWT）並提取使用者資訊（userId, displayName, pictureUrl）
5. 將使用者資訊存入資料庫，建立應用程式內部 session/JWT

**實作範例**:
```csharp
// appsettings.json
"LineLogin": {
  "ChannelId": "YOUR_CHANNEL_ID",
  "ChannelSecret": "YOUR_CHANNEL_SECRET",
  "CallbackUrl": "https://yourdomain.com/api/auth/line/callback"
}

// Controllers/AuthController.cs
[HttpGet("line/callback")]
public async Task<IActionResult> LineCallback([FromQuery] string code)
{
    var tokenResponse = await _httpClient.PostAsync(
        "https://api.line.me/oauth2/v2.1/token",
        new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = _config["LineLogin:CallbackUrl"],
            ["client_id"] = _config["LineLogin:ChannelId"],
            ["client_secret"] = _config["LineLogin:ChannelSecret"]
        })
    );
    
    var tokenData = await tokenResponse.Content.ReadFromJsonAsync<LineTokenResponse>();
    
    // 驗證 id_token 並提取使用者資訊
    var profile = await GetLineProfile(tokenData.AccessToken);
    
    // 建立或更新使用者
    var user = await _userService.GetOrCreateUser(profile.UserId, profile.DisplayName);
    
    // 發放應用程式 JWT
    var appToken = _jwtService.GenerateToken(user);
    
    return Ok(new { token = appToken });
}
```

**安全性考量**:
- 驗證 `state` 參數防止 CSRF 攻擊
- 使用 HTTPS 確保通訊加密
- 將 Channel Secret 存於環境變數，不寫入程式碼
- Token 過期處理：access_token 有效期 30 天，需實作 refresh_token 機制

**參考資源**:
- [LINE Login v2.1 官方文件](https://developers.line.biz/en/docs/line-login/integrate-line-login/)
- [ASP.NET Core OAuth 2.0 整合](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/)

---

## 2. EF Core Code First 最佳實踐（Azure SQL）

### 研究問題
- Code First Migration 管理策略？
- Azure SQL 效能最佳化設定？

### 研究結果

**Migration 策略**:
- 開發環境：使用 `dotnet ef migrations add` 與 `dotnet ef database update`
- CI/CD：在部署 pipeline 自動執行 Migration（使用 `dotnet ef database update --connection "ConnectionString"`）
- 生產環境：使用 SQL 腳本部署（`dotnet ef migrations script`），經 DBA 審核後執行

**DbContext 設定**:
```csharp
// FitnessTrackerDbContext.cs
public class FitnessTrackerDbContext : DbContext
{
    public DbSet<WorkoutRecord> WorkoutRecords { get; set; }
    public DbSet<ExerciseType> ExerciseTypes { get; set; }
    // ...其他 DbSet
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 使用 Fluent API 設定
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitnessTrackerDbContext).Assembly);
        
        // 全域查詢過濾器（軟刪除）
        modelBuilder.Entity<WorkoutRecord>().HasQueryFilter(w => !w.IsDeleted);
    }
}

// Configurations/WorkoutRecordConfiguration.cs
public class WorkoutRecordConfiguration : IEntityTypeConfiguration<WorkoutRecord>
{
    public void Configure(EntityTypeBuilder<WorkoutRecord> builder)
    {
        builder.ToTable("WorkoutRecords");
        builder.HasKey(w => w.Id);
        
        // 索引最佳化
        builder.HasIndex(w => new { w.UserId, w.Date }).IsUnique();
        builder.HasIndex(w => w.Date);
        
        // 屬性設定
        builder.Property(w => w.DurationMinutes).IsRequired();
        builder.Property(w => w.CaloriesBurned).HasPrecision(18, 2);
        
        // 關聯
        builder.HasOne(w => w.ExerciseType)
               .WithMany()
               .HasForeignKey(w => w.ExerciseTypeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

**Azure SQL 效能最佳化**:
1. **連線池**: 設定 `Max Pool Size=100` 與 `Min Pool Size=10`
2. **索引策略**:
   - Composite Index: `(UserId, Date)` 用於查詢特定使用者的日期範圍
   - Covering Index: 包含常查詢欄位（如 `DurationMinutes`, `CaloriesBurned`）
3. **查詢最佳化**:
   - 使用 `AsNoTracking()` 於唯讀查詢
   - 避免 N+1 查詢，使用 `Include()` 預載關聯
   - 分頁查詢使用 `Skip().Take()`，避免全表掃描
4. **Connection Resiliency**: 設定 `EnableRetryOnFailure(maxRetryCount: 3)`

**範例查詢**:
```csharp
// 取得使用者本週紀錄（最佳化）
var records = await _context.WorkoutRecords
    .AsNoTracking()
    .Where(w => w.UserId == userId && w.Date >= startDate && w.Date <= endDate)
    .Include(w => w.ExerciseType)
    .OrderBy(w => w.Date)
    .ToListAsync();
```

**參考資源**:
- [EF Core Code First Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Azure SQL 效能最佳實踐](https://learn.microsoft.com/en-us/azure/azure-sql/database/performance-guidance)

---

## 3. Vue.js 3 + Vuetify 3 繁體中文設定

### 研究問題
- Vuetify 3 如何支援繁體中文 locale？
- 國際化（i18n）最佳實踐？

### 研究結果

**Vuetify 3 繁體中文設定**:
```typescript
// main.ts
import { createApp } from 'vue'
import { createVuetify } from 'vuetify'
import { zhHant } from 'vuetify/locale'
import 'vuetify/styles'

const vuetify = createVuetify({
  locale: {
    locale: 'zhHant',
    messages: { zhHant }
  },
  theme: {
    defaultTheme: 'light',
    themes: {
      light: {
        colors: {
          primary: '#00B900', // LINE 品牌色
          secondary: '#424242',
        }
      }
    }
  }
})

const app = createApp(App)
app.use(vuetify)
app.mount('#app')
```

**Vue I18n 整合**:
```typescript
// i18n/zh-TW.json
{
  "workout": {
    "title": "健身紀錄",
    "add": "新增紀錄",
    "edit": "編輯紀錄",
    "delete": "刪除紀錄",
    "duration": "運動時長（分鐘）",
    "calories": "消耗熱量（大卡）",
    "date": "日期"
  },
  "validation": {
    "required": "{field}為必填欄位",
    "min": "{field}最小值為 {min}",
    "max": "{field}最大值為 {max}"
  }
}

// main.ts
import { createI18n } from 'vue-i18n'
import zhTW from './i18n/zh-TW.json'

const i18n = createI18n({
  legacy: false,
  locale: 'zh-TW',
  fallbackLocale: 'zh-TW',
  messages: {
    'zh-TW': zhTW
  }
})

app.use(i18n)
```

**元件使用範例**:
```vue
<template>
  <v-card>
    <v-card-title>{{ t('workout.title') }}</v-card-title>
    <v-card-text>
      <v-text-field
        v-model="duration"
        :label="t('workout.duration')"
        :rules="[required, minValue(1), maxValue(480)]"
      />
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const required = (value: any) => !!value || t('validation.required', { field: t('workout.duration') })
const minValue = (min: number) => (value: number) => value >= min || t('validation.min', { field: t('workout.duration'), min })
</script>
```

**日期格式化（繁體中文）**:
```typescript
// utils/date.ts
import { format } from 'date-fns'
import { zhTW } from 'date-fns/locale'

export function formatDate(date: Date): string {
  return format(date, 'yyyy年MM月dd日', { locale: zhTW })
}

export function formatWeekday(date: Date): string {
  return format(date, 'EEEE', { locale: zhTW }) // 星期一、星期二...
}
```

**參考資源**:
- [Vuetify 3 Locale 設定](https://vuetifyjs.com/en/features/internationalization/)
- [Vue I18n](https://vue-i18n.intlify.dev/)

---

## 4. Docker 多階段建置（.NET + Vue.js）

### 研究問題
- 如何最佳化 Docker 映像大小？
- 如何設定開發環境與生產環境？

### 研究結果

**後端 Dockerfile（多階段建置）**:
```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製 .csproj 並還原相依性（利用快取層）
COPY ["src/FitnessTracker.Api/FitnessTracker.Api.csproj", "src/FitnessTracker.Api/"]
COPY ["src/FitnessTracker.Core/FitnessTracker.Core.csproj", "src/FitnessTracker.Core/"]
COPY ["src/FitnessTracker.Infrastructure/FitnessTracker.Infrastructure.csproj", "src/FitnessTracker.Infrastructure/"]
RUN dotnet restore "src/FitnessTracker.Api/FitnessTracker.Api.csproj"

# 複製所有程式碼並建置
COPY . .
WORKDIR "/src/src/FitnessTracker.Api"
RUN dotnet build "FitnessTracker.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "FitnessTracker.Api.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "FitnessTracker.Api.dll"]
```

**前端 Dockerfile（多階段建置）**:
```dockerfile
# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app

# 複製 package.json 並安裝相依性（利用快取層）
COPY package*.json ./
RUN npm ci

# 複製程式碼並建置
COPY . .
RUN npm run build

# Stage 2: Production Server
FROM nginx:alpine AS final
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

**docker-compose.yml（開發環境）**:
```yaml
version: '3.8'

services:
  backend:
    build:
      context: ./backend
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=FitnessTracker;User Id=sa;Password=YourStrong@Password;TrustServerCertificate=True
      - LineLogin__ChannelId=${LINE_CHANNEL_ID}
      - LineLogin__ChannelSecret=${LINE_CHANNEL_SECRET}
    depends_on:
      - db
    volumes:
      - ./backend/src:/src:ro  # 開發時即時載入
  
  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - "3000:80"
    environment:
      - VITE_API_BASE_URL=http://localhost:5000
    depends_on:
      - backend
  
  db:
    image: mcr.microsoft.com/azure-sql-edge:latest
    ports:
      - "1433:1433"
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Password
    volumes:
      - sqldata:/var/opt/mssql

volumes:
  sqldata:
```

**最佳化技巧**:
- 使用 Alpine Linux 基礎映像（減少 70% 大小）
- 多階段建置分離 SDK 與 Runtime
- 利用 Docker 快取層（先複製 package.json / .csproj）
- 使用 `.dockerignore` 排除不必要檔案（`node_modules`, `bin`, `obj`）

**參考資源**:
- [Docker 多階段建置](https://docs.docker.com/build/building/multi-stage/)
- [.NET Docker 最佳實踐](https://learn.microsoft.com/en-us/dotnet/core/docker/build-container)

---

## 5. 效能監控與最佳化策略

### 研究問題
- 如何達成 P95 < 500ms API 回應時間？
- 如何監控前端頁面載入效能？

### 研究結果

**後端效能最佳化**:

1. **資料庫查詢最佳化**:
   ```csharp
   // 使用 AsNoTracking 減少記憶體開銷
   var records = await _context.WorkoutRecords
       .AsNoTracking()
       .Where(w => w.UserId == userId)
       .ToListAsync();
   
   // 使用 Select 投影，避免查詢不必要欄位
   var summary = await _context.WorkoutRecords
       .Where(w => w.UserId == userId && w.Date >= startDate)
       .Select(w => new { w.DurationMinutes, w.CaloriesBurned })
       .ToListAsync();
   ```

2. **回應壓縮**:
   ```csharp
   // Program.cs
   builder.Services.AddResponseCompression(options =>
   {
       options.EnableForHttps = true;
       options.Providers.Add<GzipCompressionProvider>();
   });
   ```

3. **效能監控（Serilog + Application Insights）**:
   ```csharp
   // appsettings.json
   "Serilog": {
     "WriteTo": [
       { "Name": "Console" },
       {
         "Name": "ApplicationInsights",
         "Args": {
           "instrumentationKey": "YOUR_KEY",
           "telemetryConverter": "Serilog.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter, Serilog.Sinks.ApplicationInsights"
         }
       }
     ]
   }
   
   // Middleware 記錄 API 回應時間
   app.Use(async (context, next) =>
   {
       var sw = Stopwatch.StartNew();
       await next();
       sw.Stop();
       Log.Information("Request {Method} {Path} completed in {ElapsedMilliseconds}ms", 
           context.Request.Method, context.Request.Path, sw.ElapsedMilliseconds);
   });
   ```

**前端效能最佳化**:

1. **程式碼分割（Code Splitting）**:
   ```typescript
   // router/index.ts
   const routes = [
     {
       path: '/',
       component: () => import('../views/Home.vue')  // 動態載入
     },
     {
       path: '/trends',
       component: () => import('../views/Trends.vue')
     }
   ]
   ```

2. **圖片最佳化**:
   - 使用 WebP 格式
   - Lazy loading: `<img loading="lazy" />`
   - 壓縮圖片（使用 `vite-plugin-image-optimizer`）

3. **效能監控（Web Vitals）**:
   ```typescript
   // main.ts
   import { onCLS, onFID, onLCP } from 'web-vitals'
   
   onCLS(console.log)  // Cumulative Layout Shift
   onFID(console.log)  // First Input Delay
   onLCP(console.log)  // Largest Contentful Paint
   
   // 發送至後端
   function sendToAnalytics(metric) {
     fetch('/api/analytics', {
       method: 'POST',
       body: JSON.stringify(metric)
     })
   }
   ```

4. **Service Worker（離線支援）**:
   ```typescript
   // vite.config.ts
   import { VitePWA } from 'vite-plugin-pwa'
   
   export default defineConfig({
     plugins: [
       VitePWA({
         registerType: 'autoUpdate',
         workbox: {
           runtimeCaching: [
             {
               urlPattern: /^https:\/\/api\.yourdomain\.com\/.*/i,
               handler: 'NetworkFirst',
               options: {
                 cacheName: 'api-cache',
                 expiration: {
                   maxEntries: 50,
                   maxAgeSeconds: 60 * 60 * 24  // 1 天
                 }
               }
             }
           ]
         }
       })
     ]
   })
   ```

**效能目標驗證**:
- 使用 k6 或 Apache JMeter 進行負載測試
- 目標：1000 並發使用者，P95 < 500ms
- 前端使用 Lighthouse CI 驗證頁面載入時間 < 2s

**參考資源**:
- [ASP.NET Core 效能最佳實踐](https://learn.microsoft.com/en-us/aspnet/core/performance/performance-best-practices)
- [Vue.js 效能最佳化](https://vuejs.org/guide/best-practices/performance.html)

---

## 6. 離線支援實作策略

### 研究問題
- 如何實作 PWA 離線存取功能？
- 資料同步策略？

### 研究結果

**PWA 設定（Vite PWA Plugin）**:
```typescript
// vite.config.ts
import { VitePWA } from 'vite-plugin-pwa'

export default defineConfig({
  plugins: [
    VitePWA({
      registerType: 'autoUpdate',
      includeAssets: ['favicon.ico', 'apple-touch-icon.png', 'masked-icon.svg'],
      manifest: {
        name: '健身紀錄追蹤',
        short_name: '健身追蹤',
        description: '追蹤您的健身紀錄與目標',
        theme_color: '#00B900',
        icons: [
          {
            src: 'pwa-192x192.png',
            sizes: '192x192',
            type: 'image/png'
          },
          {
            src: 'pwa-512x512.png',
            sizes: '512x512',
            type: 'image/png'
          }
        ]
      },
      workbox: {
        runtimeCaching: [
          {
            urlPattern: /^https:\/\/api\.yourdomain\.com\/api\/workouts.*/i,
            handler: 'NetworkFirst',  // 優先網路，失敗則使用快取
            options: {
              cacheName: 'workout-api-cache',
              expiration: {
                maxEntries: 100,
                maxAgeSeconds: 60 * 60 * 24  // 1 天
              },
              cacheableResponse: {
                statuses: [0, 200]
              }
            }
          },
          {
            urlPattern: /\.(?:png|jpg|jpeg|svg|gif)$/,
            handler: 'CacheFirst',  // 優先快取
            options: {
              cacheName: 'image-cache',
              expiration: {
                maxEntries: 50,
                maxAgeSeconds: 60 * 60 * 24 * 30  // 30 天
              }
            }
          }
        ]
      }
    })
  ]
})
```

**IndexedDB 本地儲存**:
```typescript
// stores/workouts.ts
import { defineStore } from 'pinia'
import { openDB, DBSchema } from 'idb'

interface WorkoutDB extends DBSchema {
  workouts: {
    key: string
    value: WorkoutRecord
    indexes: { 'by-date': Date }
  }
}

export const useWorkoutStore = defineStore('workouts', () => {
  const db = ref<IDBPDatabase<WorkoutDB> | null>(null)
  
  async function initDB() {
    db.value = await openDB<WorkoutDB>('fitness-tracker', 1, {
      upgrade(db) {
        const store = db.createObjectStore('workouts', { keyPath: 'id' })
        store.createIndex('by-date', 'date')
      }
    })
  }
  
  async function saveWorkout(workout: WorkoutRecord) {
    if (!navigator.onLine) {
      // 離線時存入 IndexedDB
      await db.value?.put('workouts', workout)
      pendingSyncs.value.push(workout.id)
    } else {
      // 線上時直接呼叫 API
      await workoutService.create(workout)
    }
  }
  
  async function syncPendingChanges() {
    if (!navigator.onLine) return
    
    for (const id of pendingSyncs.value) {
      const workout = await db.value?.get('workouts', id)
      if (workout) {
        await workoutService.create(workout)
        await db.value?.delete('workouts', id)
      }
    }
    
    pendingSyncs.value = []
  }
  
  // 監聽網路狀態
  window.addEventListener('online', syncPendingChanges)
  
  return { initDB, saveWorkout, syncPendingChanges }
})
```

**同步策略**:
1. **寫入策略**: 
   - 線上：直接寫入後端 API
   - 離線：寫入 IndexedDB，標記為待同步
2. **同步時機**:
   - 網路恢復時自動同步
   - 使用者手動觸發同步
3. **衝突處理**:
   - 使用時間戳記（Last-Write-Wins）
   - 後端驗證避免重複紀錄（UserId + Date 唯一鍵）

**使用者體驗**:
```vue
<template>
  <v-snackbar v-model="isOffline" :timeout="-1" color="warning">
    您目前處於離線狀態，資料將在恢復連線後同步
  </v-snackbar>
  
  <v-snackbar v-model="isSyncing" :timeout="-1" color="info">
    正在同步離線資料...
  </v-snackbar>
</template>

<script setup lang="ts">
const isOffline = ref(!navigator.onLine)
const isSyncing = ref(false)

window.addEventListener('online', async () => {
  isOffline.value = false
  isSyncing.value = true
  await workoutStore.syncPendingChanges()
  isSyncing.value = false
})

window.addEventListener('offline', () => {
  isOffline.value = true
})
</script>
```

**參考資源**:
- [Vite PWA Plugin](https://vite-pwa-org.netlify.app/)
- [IndexedDB API](https://developer.mozilla.org/en-US/docs/Web/API/IndexedDB_API)
- [Workbox](https://developer.chrome.com/docs/workbox/)

---

## 研究結論

所有技術未知項目已完成研究，可進入 Phase 1 設計階段：

1. ✅ **LINE Login 整合**: 使用 OAuth 2.0 流程，整合官方 SDK
2. ✅ **EF Core Code First**: 採用 Configuration 類別管理實體，Migration 策略已定義
3. ✅ **Vue.js 3 繁體中文**: 使用 Vuetify 3 內建 locale + Vue I18n
4. ✅ **Docker 容器化**: 多階段建置最佳化映像大小，docker-compose 管理開發環境
5. ✅ **效能最佳化**: AsNoTracking、回應壓縮、程式碼分割、Web Vitals 監控
6. ✅ **離線支援**: PWA + IndexedDB，NetworkFirst 快取策略，自動同步機制

**下一步**: 產生 `data-model.md`, `contracts/`, `quickstart.md`
