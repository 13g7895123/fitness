# API 合約規格 - 健身紀錄追蹤功能

**專案**: 健身紀錄追蹤系統  
**日期**: 2025-11-02  
**分支**: 001-fitness-tracking  
**階段**: Phase 1 - 設計  
**API 版本**: v1  
**Base URL**: `https://api.yourdomain.com/api/v1`

---

## 目錄

1. [身份驗證 API](#1-身份驗證-api)
2. [健身紀錄 API](#2-健身紀錄-api)
3. [運動項目 API](#3-運動項目-api)
4. [運動目標 API](#4-運動目標-api)
5. [統計與趨勢 API](#5-統計與趨勢-api)
6. [共用資料型別](#6-共用資料型別)
7. [錯誤處理](#7-錯誤處理)

---

## 通用說明

### 身份驗證

所有 API（除了 `/auth/*` 端點）都需要 JWT Bearer Token：

```http
Authorization: Bearer <JWT_TOKEN>
```

### 回應格式

成功回應（2xx）:
```json
{
  "data": { /* 實際資料 */ },
  "message": "操作成功",
  "timestamp": "2025-11-02T10:30:00Z"
}
```

錯誤回應（4xx/5xx）:
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "驗證失敗",
    "details": [
      {
        "field": "durationMinutes",
        "message": "運動時長必須介於 1 至 480 分鐘"
      }
    ]
  },
  "timestamp": "2025-11-02T10:30:00Z"
}
```

### 分頁

使用 query parameters:
- `page`: 頁碼（從 1 開始，預設 1）
- `pageSize`: 每頁筆數（預設 20，最大 100）

回應包含分頁資訊:
```json
{
  "data": [ /* 資料陣列 */ ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalItems": 95
  }
}
```

---

## 1. 身份驗證 API

### 1.1 LINE Login 授權

**端點**: `GET /auth/line/authorize`

**說明**: 重導向至 LINE 授權頁面

**Request**:
```http
GET /api/v1/auth/line/authorize?redirectUrl=https://yourdomain.com/callback
```

**Query Parameters**:
| 參數 | 型別 | 必填 | 說明 |
|------|------|------|------|
| redirectUrl | string | Yes | 授權後的重導向 URL |

**Response**: `302 Found`
```http
Location: https://access.line.me/oauth2/v2.1/authorize?response_type=code&client_id=...
```

---

### 1.2 LINE Login Callback

**端點**: `GET /auth/line/callback`

**說明**: 處理 LINE OAuth callback 並發放 JWT

**Request**:
```http
GET /api/v1/auth/line/callback?code=ABC123&state=XYZ789
```

**Query Parameters**:
| 參數 | 型別 | 必填 | 說明 |
|------|------|------|------|
| code | string | Yes | LINE 授權碼 |
| state | string | Yes | CSRF 防護 token |

**Response**: `200 OK`
```json
{
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 86400,
    "tokenType": "Bearer",
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "displayName": "張小明",
      "pictureUrl": "https://profile.line-scdn.net/..."
    }
  },
  "message": "登入成功"
}
```

**Error Responses**:
- `400 Bad Request`: 無效的授權碼
- `401 Unauthorized`: LINE 授權失敗
- `500 Internal Server Error`: 伺服器錯誤

---

### 1.3 Token 刷新

**端點**: `POST /auth/refresh`

**說明**: 使用 refresh token 取得新的 access token

**Request**:
```http
POST /api/v1/auth/refresh
Content-Type: application/json

{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response**: `200 OK`
```json
{
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 86400,
    "tokenType": "Bearer"
  }
}
```

---

### 1.4 登出

**端點**: `POST /auth/logout`

**說明**: 撤銷當前 token

**Request**:
```http
POST /api/v1/auth/logout
Authorization: Bearer <JWT_TOKEN>
```

**Response**: `204 No Content`

---

## 2. 健身紀錄 API

### 2.1 取得健身紀錄清單

**端點**: `GET /workouts`

**說明**: 取得使用者的健身紀錄（支援日期範圍過濾）

**Request**:
```http
GET /api/v1/workouts?startDate=2025-10-28&endDate=2025-11-03&page=1&pageSize=20
Authorization: Bearer <JWT_TOKEN>
```

**Query Parameters**:
| 參數 | 型別 | 必填 | 說明 |
|------|------|------|------|
| startDate | date | No | 開始日期（yyyy-MM-dd） |
| endDate | date | No | 結束日期（yyyy-MM-dd） |
| page | int | No | 頁碼（預設 1） |
| pageSize | int | No | 每頁筆數（預設 20） |

**Response**: `200 OK`
```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "date": "2025-11-02",
      "exerciseType": {
        "id": "660e8400-e29b-41d4-a716-446655440000",
        "name": "跑步",
        "category": "有氧"
      },
      "equipment": {
        "id": "770e8400-e29b-41d4-a716-446655440000",
        "name": "跑步機"
      },
      "durationMinutes": 30,
      "caloriesBurned": 250.50,
      "weight": 70.5,
      "notes": "今天狀態很好",
      "createdAt": "2025-11-02T08:30:00Z",
      "updatedAt": "2025-11-02T08:30:00Z"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalPages": 3,
    "totalItems": 52
  }
}
```

---

### 2.2 取得單一健身紀錄

**端點**: `GET /workouts/{id}`

**說明**: 取得特定健身紀錄詳細資料

**Request**:
```http
GET /api/v1/workouts/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer <JWT_TOKEN>
```

**Response**: `200 OK`
```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "date": "2025-11-02",
    "exerciseType": {
      "id": "660e8400-e29b-41d4-a716-446655440000",
      "name": "跑步",
      "category": "有氧",
      "isCustom": false
    },
    "equipment": {
      "id": "770e8400-e29b-41d4-a716-446655440000",
      "name": "跑步機"
    },
    "durationMinutes": 30,
    "caloriesBurned": 250.50,
    "weight": 70.5,
    "notes": "今天狀態很好",
    "createdAt": "2025-11-02T08:30:00Z",
    "updatedAt": "2025-11-02T08:30:00Z"
  }
}
```

**Error Responses**:
- `404 Not Found`: 紀錄不存在

---

### 2.3 新增健身紀錄

**端點**: `POST /workouts`

**說明**: 新增一筆健身紀錄

**Request**:
```http
POST /api/v1/workouts
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "date": "2025-11-02",
  "exerciseTypeId": "660e8400-e29b-41d4-a716-446655440000",
  "equipmentId": "770e8400-e29b-41d4-a716-446655440000",
  "durationMinutes": 30,
  "caloriesBurned": 250.50,
  "weight": 70.5,
  "notes": "今天狀態很好"
}
```

**Request Body**:
| 欄位 | 型別 | 必填 | 限制 | 說明 |
|------|------|------|------|------|
| date | date | Yes | ≤ 今天 | 日期（yyyy-MM-dd） |
| exerciseTypeId | uuid | Yes | - | 運動項目 ID |
| equipmentId | uuid | No | - | 運動器材 ID |
| durationMinutes | int | Yes | 1-480 | 運動時長（分鐘） |
| caloriesBurned | decimal | No | 1-5000 | 消耗熱量（若未提供則自動計算） |
| weight | decimal | No | > 0 | 體重（公斤，用於卡路里計算） |
| notes | string | No | ≤ 500 字元 | 備註 |

**Response**: `201 Created`
```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "date": "2025-11-02",
    "exerciseType": {
      "id": "660e8400-e29b-41d4-a716-446655440000",
      "name": "跑步",
      "category": "有氧"
    },
    "equipment": {
      "id": "770e8400-e29b-41d4-a716-446655440000",
      "name": "跑步機"
    },
    "durationMinutes": 30,
    "caloriesBurned": 250.50,
    "weight": 70.5,
    "notes": "今天狀態很好",
    "createdAt": "2025-11-02T08:30:00Z",
    "updatedAt": "2025-11-02T08:30:00Z"
  },
  "message": "紀錄新增成功"
}
```

**Error Responses**:
- `400 Bad Request`: 驗證失敗（詳見錯誤訊息）
- `409 Conflict`: 該日期已有相同運動項目的紀錄

---

### 2.4 更新健身紀錄

**端點**: `PUT /workouts/{id}`

**說明**: 更新已存在的健身紀錄

**Request**:
```http
PUT /api/v1/workouts/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "date": "2025-11-02",
  "exerciseTypeId": "660e8400-e29b-41d4-a716-446655440000",
  "equipmentId": "770e8400-e29b-41d4-a716-446655440000",
  "durationMinutes": 45,
  "caloriesBurned": 350.00,
  "weight": 70.5,
  "notes": "延長了 15 分鐘"
}
```

**Request Body**: 同新增紀錄

**Response**: `200 OK`
```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "date": "2025-11-02",
    "exerciseType": {
      "id": "660e8400-e29b-41d4-a716-446655440000",
      "name": "跑步",
      "category": "有氧"
    },
    "equipment": {
      "id": "770e8400-e29b-41d4-a716-446655440000",
      "name": "跑步機"
    },
    "durationMinutes": 45,
    "caloriesBurned": 350.00,
    "weight": 70.5,
    "notes": "延長了 15 分鐘",
    "createdAt": "2025-11-02T08:30:00Z",
    "updatedAt": "2025-11-02T09:15:00Z"
  },
  "message": "紀錄更新成功"
}
```

**Error Responses**:
- `400 Bad Request`: 驗證失敗
- `404 Not Found`: 紀錄不存在
- `409 Conflict`: 更新後的資料與其他紀錄衝突

---

### 2.5 刪除健身紀錄

**端點**: `DELETE /workouts/{id}`

**說明**: 刪除健身紀錄（軟刪除）

**Request**:
```http
DELETE /api/v1/workouts/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer <JWT_TOKEN>
```

**Response**: `204 No Content`

**Error Responses**:
- `404 Not Found`: 紀錄不存在

---

### 2.6 取得特定日期紀錄

**端點**: `GET /workouts/by-date/{date}`

**說明**: 取得特定日期的所有健身紀錄

**Request**:
```http
GET /api/v1/workouts/by-date/2025-11-02
Authorization: Bearer <JWT_TOKEN>
```

**Response**: `200 OK`
```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "date": "2025-11-02",
      "exerciseType": {
        "id": "660e8400-e29b-41d4-a716-446655440000",
        "name": "跑步",
        "category": "有氧"
      },
      "equipment": {
        "id": "770e8400-e29b-41d4-a716-446655440000",
        "name": "跑步機"
      },
      "durationMinutes": 30,
      "caloriesBurned": 250.50,
      "weight": 70.5,
      "notes": "今天狀態很好",
      "createdAt": "2025-11-02T08:30:00Z",
      "updatedAt": "2025-11-02T08:30:00Z"
    },
    {
      "id": "551e8400-e29b-41d4-a716-446655440001",
      "date": "2025-11-02",
      "exerciseType": {
        "id": "661e8400-e29b-41d4-a716-446655440001",
        "name": "重量訓練",
        "category": "重訓"
      },
      "equipment": {
        "id": "771e8400-e29b-41d4-a716-446655440001",
        "name": "啞鈴"
      },
      "durationMinutes": 60,
      "caloriesBurned": 300.00,
      "weight": 70.5,
      "notes": "練上半身",
      "createdAt": "2025-11-02T14:00:00Z",
      "updatedAt": "2025-11-02T14:00:00Z"
    }
  ]
}
```

---

## 3. 運動項目 API

### 3.1 取得運動項目清單

**端點**: `GET /exercise-types`

**說明**: 取得系統預設與使用者自訂的運動項目

**Request**:
```http
GET /api/v1/exercise-types?category=有氧&search=跑
Authorization: Bearer <JWT_TOKEN>
```

**Query Parameters**:
| 參數 | 型別 | 必填 | 說明 |
|------|------|------|------|
| category | string | No | 類別過濾（有氧、重訓、伸展、其他） |
| search | string | No | 搜尋關鍵字 |
| includeInactive | bool | No | 是否包含停用項目（預設 false） |

**Response**: `200 OK`
```json
{
  "data": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440000",
      "name": "跑步",
      "category": "有氧",
      "isCustom": false,
      "isActive": true,
      "createdAt": "2025-01-01T00:00:00Z"
    },
    {
      "id": "661e8400-e29b-41d4-a716-446655440001",
      "name": "跑步（間歇訓練）",
      "category": "有氧",
      "isCustom": true,
      "isActive": true,
      "createdAt": "2025-10-15T10:20:00Z"
    }
  ]
}
```

---

### 3.2 新增自訂運動項目

**端點**: `POST /exercise-types`

**說明**: 建立使用者自訂的運動項目

**Request**:
```http
POST /api/v1/exercise-types
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "name": "跑步（間歇訓練）",
  "category": "有氧"
}
```

**Request Body**:
| 欄位 | 型別 | 必填 | 限制 | 說明 |
|------|------|------|------|------|
| name | string | Yes | ≤ 100 字元 | 運動名稱 |
| category | string | Yes | 列舉值 | 類別（有氧、重訓、伸展、其他） |

**Response**: `201 Created`
```json
{
  "data": {
    "id": "661e8400-e29b-41d4-a716-446655440001",
    "name": "跑步（間歇訓練）",
    "category": "有氧",
    "isCustom": true,
    "isActive": true,
    "createdAt": "2025-11-02T10:30:00Z"
  },
  "message": "運動項目新增成功"
}
```

**Error Responses**:
- `400 Bad Request`: 驗證失敗
- `409 Conflict`: 該名稱已存在

---

### 3.3 更新運動項目狀態

**端點**: `PATCH /exercise-types/{id}`

**說明**: 啟用或停用運動項目（僅限自訂項目）

**Request**:
```http
PATCH /api/v1/exercise-types/661e8400-e29b-41d4-a716-446655440001
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "isActive": false
}
```

**Response**: `200 OK`
```json
{
  "data": {
    "id": "661e8400-e29b-41d4-a716-446655440001",
    "name": "跑步（間歇訓練）",
    "category": "有氧",
    "isCustom": true,
    "isActive": false,
    "createdAt": "2025-11-02T10:30:00Z"
  },
  "message": "運動項目已停用"
}
```

**Error Responses**:
- `403 Forbidden`: 無法修改系統預設項目
- `404 Not Found`: 項目不存在

---

### 3.4 取得運動器材清單

**端點**: `GET /equipments`

**說明**: 取得可選的運動器材清單

**Request**:
```http
GET /api/v1/equipments
Authorization: Bearer <JWT_TOKEN>
```

**Response**: `200 OK`
```json
{
  "data": [
    {
      "id": "770e8400-e29b-41d4-a716-446655440000",
      "name": "跑步機",
      "isActive": true
    },
    {
      "id": "771e8400-e29b-41d4-a716-446655440001",
      "name": "啞鈴",
      "isActive": true
    },
    {
      "id": "772e8400-e29b-41d4-a716-446655440002",
      "name": "槓鈴",
      "isActive": true
    }
  ]
}
```

---

## 4. 運動目標 API

### 4.1 取得運動目標

**端點**: `GET /goals`

**說明**: 取得使用者的運動目標

**Request**:
```http
GET /api/v1/goals?activeOnly=true
Authorization: Bearer <JWT_TOKEN>
```

**Query Parameters**:
| 參數 | 型別 | 必填 | 說明 |
|------|------|------|------|
| activeOnly | bool | No | 僅顯示啟用的目標（預設 true） |

**Response**: `200 OK`
```json
{
  "data": [
    {
      "id": "880e8400-e29b-41d4-a716-446655440000",
      "weeklyMinutes": 150,
      "weeklyCalories": 1000,
      "startDate": "2025-10-28",
      "endDate": null,
      "isActive": true,
      "createdAt": "2025-10-28T08:00:00Z",
      "updatedAt": "2025-10-28T08:00:00Z"
    }
  ]
}
```

---

### 4.2 設定運動目標

**端點**: `POST /goals`

**說明**: 建立新的運動目標

**Request**:
```http
POST /api/v1/goals
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "weeklyMinutes": 150,
  "weeklyCalories": 1000,
  "startDate": "2025-10-28",
  "endDate": null
}
```

**Request Body**:
| 欄位 | 型別 | 必填 | 限制 | 說明 |
|------|------|------|------|------|
| weeklyMinutes | int | No* | > 0 | 每週目標時長（分鐘） |
| weeklyCalories | decimal | No* | > 0 | 每週目標熱量（大卡） |
| startDate | date | Yes | - | 目標開始日期 |
| endDate | date | No | > startDate | 目標結束日期（null 表示持續有效） |

*註：`weeklyMinutes` 與 `weeklyCalories` 至少需設定一項

**Response**: `201 Created`
```json
{
  "data": {
    "id": "880e8400-e29b-41d4-a716-446655440000",
    "weeklyMinutes": 150,
    "weeklyCalories": 1000,
    "startDate": "2025-10-28",
    "endDate": null,
    "isActive": true,
    "createdAt": "2025-11-02T10:00:00Z",
    "updatedAt": "2025-11-02T10:00:00Z"
  },
  "message": "目標設定成功"
}
```

**Error Responses**:
- `400 Bad Request`: 驗證失敗

---

### 4.3 更新運動目標

**端點**: `PUT /goals/{id}`

**說明**: 更新已存在的運動目標

**Request**:
```http
PUT /api/v1/goals/880e8400-e29b-41d4-a716-446655440000
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "weeklyMinutes": 180,
  "weeklyCalories": 1200,
  "startDate": "2025-10-28",
  "endDate": null
}
```

**Request Body**: 同設定目標

**Response**: `200 OK`
```json
{
  "data": {
    "id": "880e8400-e29b-41d4-a716-446655440000",
    "weeklyMinutes": 180,
    "weeklyCalories": 1200,
    "startDate": "2025-10-28",
    "endDate": null,
    "isActive": true,
    "createdAt": "2025-11-02T10:00:00Z",
    "updatedAt": "2025-11-02T11:30:00Z"
  },
  "message": "目標更新成功"
}
```

---

### 4.4 停用運動目標

**端點**: `PATCH /goals/{id}/deactivate`

**說明**: 停用運動目標

**Request**:
```http
PATCH /api/v1/goals/880e8400-e29b-41d4-a716-446655440000/deactivate
Authorization: Bearer <JWT_TOKEN>
```

**Response**: `200 OK`
```json
{
  "data": {
    "id": "880e8400-e29b-41d4-a716-446655440000",
    "weeklyMinutes": 180,
    "weeklyCalories": 1200,
    "startDate": "2025-10-28",
    "endDate": "2025-11-02",
    "isActive": false,
    "createdAt": "2025-11-02T10:00:00Z",
    "updatedAt": "2025-11-02T12:00:00Z"
  },
  "message": "目標已停用"
}
```

---

## 5. 統計與趨勢 API

### 5.1 取得週統計

**端點**: `GET /statistics/weekly`

**說明**: 取得特定週的統計資料

**Request**:
```http
GET /api/v1/statistics/weekly?startDate=2025-10-28
Authorization: Bearer <JWT_TOKEN>
```

**Query Parameters**:
| 參數 | 型別 | 必填 | 說明 |
|------|------|------|------|
| startDate | date | Yes | 週一日期（yyyy-MM-dd） |

**Response**: `200 OK`
```json
{
  "data": {
    "weekStartDate": "2025-10-28",
    "weekEndDate": "2025-11-03",
    "totalMinutes": 210,
    "totalCalories": 1450.50,
    "workoutDays": 5,
    "dailyBreakdown": [
      {
        "date": "2025-10-28",
        "dayOfWeek": "星期一",
        "totalMinutes": 45,
        "totalCalories": 320.00,
        "workoutCount": 1,
        "hasWorkout": true
      },
      {
        "date": "2025-10-29",
        "dayOfWeek": "星期二",
        "totalMinutes": 60,
        "totalCalories": 450.00,
        "workoutCount": 2,
        "hasWorkout": true
      },
      {
        "date": "2025-10-30",
        "dayOfWeek": "星期三",
        "totalMinutes": 0,
        "totalCalories": 0,
        "workoutCount": 0,
        "hasWorkout": false
      },
      {
        "date": "2025-10-31",
        "dayOfWeek": "星期四",
        "totalMinutes": 30,
        "totalCalories": 210.50,
        "workoutCount": 1,
        "hasWorkout": true
      },
      {
        "date": "2025-11-01",
        "dayOfWeek": "星期五",
        "totalMinutes": 45,
        "totalCalories": 300.00,
        "workoutCount": 1,
        "hasWorkout": true
      },
      {
        "date": "2025-11-02",
        "dayOfWeek": "星期六",
        "totalMinutes": 30,
        "totalCalories": 170.00,
        "workoutCount": 1,
        "hasWorkout": true
      },
      {
        "date": "2025-11-03",
        "dayOfWeek": "星期日",
        "totalMinutes": 0,
        "totalCalories": 0,
        "workoutCount": 0,
        "hasWorkout": false
      }
    ],
    "goal": {
      "weeklyMinutes": 150,
      "weeklyCalories": 1000,
      "minutesProgress": 140.0,
      "caloriesProgress": 145.05
    }
  }
}
```

---

### 5.2 取得月統計

**端點**: `GET /statistics/monthly`

**說明**: 取得特定月份的統計資料

**Request**:
```http
GET /api/v1/statistics/monthly?year=2025&month=11
Authorization: Bearer <JWT_TOKEN>
```

**Query Parameters**:
| 參數 | 型別 | 必填 | 說明 |
|------|------|------|------|
| year | int | Yes | 年份 |
| month | int | Yes | 月份（1-12） |

**Response**: `200 OK`
```json
{
  "data": {
    "year": 2025,
    "month": 11,
    "totalMinutes": 450,
    "totalCalories": 3200.00,
    "totalWorkoutDays": 15,
    "averageDailyMinutes": 30,
    "averageDailyCalories": 213.33,
    "weeklyBreakdown": [
      {
        "weekNumber": 1,
        "weekStartDate": "2025-10-28",
        "totalMinutes": 210,
        "totalCalories": 1450.50,
        "workoutDays": 5
      },
      {
        "weekNumber": 2,
        "weekStartDate": "2025-11-04",
        "totalMinutes": 180,
        "totalCalories": 1300.00,
        "workoutDays": 6
      },
      {
        "weekNumber": 3,
        "weekStartDate": "2025-11-11",
        "totalMinutes": 60,
        "totalCalories": 449.50,
        "workoutDays": 4
      }
    ],
    "categoryBreakdown": [
      {
        "category": "有氧",
        "totalMinutes": 300,
        "totalCalories": 2100.00,
        "percentage": 66.67
      },
      {
        "category": "重訓",
        "totalMinutes": 120,
        "totalCalories": 900.00,
        "percentage": 26.67
      },
      {
        "category": "伸展",
        "totalMinutes": 30,
        "totalCalories": 200.00,
        "percentage": 6.67
      }
    ]
  }
}
```

---

### 5.3 取得趨勢資料

**端點**: `GET /statistics/trends`

**說明**: 取得歷史趨勢資料（用於圖表）

**Request**:
```http
GET /api/v1/statistics/trends?startDate=2025-10-01&endDate=2025-11-02&groupBy=week
Authorization: Bearer <JWT_TOKEN>
```

**Query Parameters**:
| 參數 | 型別 | 必填 | 說明 |
|------|------|------|------|
| startDate | date | Yes | 開始日期 |
| endDate | date | Yes | 結束日期 |
| groupBy | string | No | 聚合方式（day/week/month，預設 week） |

**Response**: `200 OK`
```json
{
  "data": {
    "startDate": "2025-10-01",
    "endDate": "2025-11-02",
    "groupBy": "week",
    "dataPoints": [
      {
        "period": "2025-09-30",
        "periodLabel": "2025年第40週",
        "totalMinutes": 150,
        "totalCalories": 1050.00,
        "workoutDays": 4,
        "averageMinutesPerDay": 37.5
      },
      {
        "period": "2025-10-07",
        "periodLabel": "2025年第41週",
        "totalMinutes": 180,
        "totalCalories": 1300.00,
        "workoutDays": 5,
        "averageMinutesPerDay": 36.0
      },
      {
        "period": "2025-10-14",
        "periodLabel": "2025年第42週",
        "totalMinutes": 210,
        "totalCalories": 1500.00,
        "workoutDays": 6,
        "averageMinutesPerDay": 35.0
      },
      {
        "period": "2025-10-21",
        "periodLabel": "2025年第43週",
        "totalMinutes": 195,
        "totalCalories": 1400.00,
        "workoutDays": 5,
        "averageMinutesPerDay": 39.0
      },
      {
        "period": "2025-10-28",
        "periodLabel": "2025年第44週",
        "totalMinutes": 210,
        "totalCalories": 1450.50,
        "workoutDays": 5,
        "averageMinutesPerDay": 42.0
      }
    ]
  }
}
```

---

### 5.4 取得運動項目分布

**端點**: `GET /statistics/exercise-distribution`

**說明**: 取得特定期間的運動項目分布統計

**Request**:
```http
GET /api/v1/statistics/exercise-distribution?startDate=2025-10-01&endDate=2025-11-02
Authorization: Bearer <JWT_TOKEN>
```

**Query Parameters**:
| 參數 | 型別 | 必填 | 說明 |
|------|------|------|------|
| startDate | date | Yes | 開始日期 |
| endDate | date | Yes | 結束日期 |

**Response**: `200 OK`
```json
{
  "data": {
    "startDate": "2025-10-01",
    "endDate": "2025-11-02",
    "totalWorkouts": 28,
    "distribution": [
      {
        "exerciseType": {
          "id": "660e8400-e29b-41d4-a716-446655440000",
          "name": "跑步",
          "category": "有氧"
        },
        "count": 12,
        "totalMinutes": 360,
        "totalCalories": 2520.00,
        "percentage": 42.86
      },
      {
        "exerciseType": {
          "id": "661e8400-e29b-41d4-a716-446655440001",
          "name": "重量訓練",
          "category": "重訓"
        },
        "count": 10,
        "totalMinutes": 600,
        "totalCalories": 3000.00,
        "percentage": 35.71
      },
      {
        "exerciseType": {
          "id": "662e8400-e29b-41d4-a716-446655440002",
          "name": "瑜伽",
          "category": "伸展"
        },
        "count": 6,
        "totalMinutes": 180,
        "totalCalories": 540.00,
        "percentage": 21.43
      }
    ]
  }
}
```

---

## 6. 共用資料型別

### WorkoutRecord DTO

```typescript
interface WorkoutRecordDto {
  date: string;                    // ISO 8601 date (yyyy-MM-dd)
  exerciseTypeId: string;          // UUID
  equipmentId?: string;            // UUID (optional)
  durationMinutes: number;         // 1-480
  caloriesBurned?: number;         // 1-5000 (optional, auto-calculated if not provided)
  weight?: number;                 // > 0 (optional)
  notes?: string;                  // max 500 characters
}
```

### WorkoutRecord Response

```typescript
interface WorkoutRecord {
  id: string;                      // UUID
  date: string;                    // ISO 8601 date
  exerciseType: ExerciseType;
  equipment?: Equipment;
  durationMinutes: number;
  caloriesBurned: number;
  weight?: number;
  notes?: string;
  createdAt: string;               // ISO 8601 datetime
  updatedAt: string;               // ISO 8601 datetime
}
```

### ExerciseType

```typescript
interface ExerciseType {
  id: string;                      // UUID
  name: string;
  category: '有氧' | '重訓' | '伸展' | '其他';
  isCustom: boolean;
  isActive: boolean;
  createdAt: string;               // ISO 8601 datetime
}
```

### Equipment

```typescript
interface Equipment {
  id: string;                      // UUID
  name: string;
  isActive: boolean;
}
```

### WorkoutGoal

```typescript
interface WorkoutGoal {
  id: string;                      // UUID
  weeklyMinutes?: number;          // > 0
  weeklyCalories?: number;         // > 0
  startDate: string;               // ISO 8601 date
  endDate?: string;                // ISO 8601 date (nullable)
  isActive: boolean;
  createdAt: string;               // ISO 8601 datetime
  updatedAt: string;               // ISO 8601 datetime
}
```

---

## 7. 錯誤處理

### 錯誤代碼

| HTTP Status | Error Code | 說明 |
|------------|-----------|------|
| 400 | VALIDATION_ERROR | 資料驗證失敗 |
| 401 | UNAUTHORIZED | 未授權（Token 無效或過期） |
| 403 | FORBIDDEN | 禁止存取（權限不足） |
| 404 | NOT_FOUND | 資源不存在 |
| 409 | CONFLICT | 資料衝突（如重複紀錄） |
| 429 | RATE_LIMIT_EXCEEDED | 超過速率限制 |
| 500 | INTERNAL_SERVER_ERROR | 伺服器內部錯誤 |
| 503 | SERVICE_UNAVAILABLE | 服務暫時不可用 |

### 錯誤回應範例

**驗證錯誤** (`400 Bad Request`):
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "請求資料驗證失敗",
    "details": [
      {
        "field": "durationMinutes",
        "message": "運動時長必須介於 1 至 480 分鐘",
        "value": 500
      },
      {
        "field": "date",
        "message": "日期不可為未來",
        "value": "2025-12-01"
      }
    ]
  },
  "timestamp": "2025-11-02T10:30:00Z"
}
```

**資源不存在** (`404 Not Found`):
```json
{
  "error": {
    "code": "NOT_FOUND",
    "message": "找不到指定的健身紀錄",
    "details": {
      "resourceType": "WorkoutRecord",
      "resourceId": "550e8400-e29b-41d4-a716-446655440000"
    }
  },
  "timestamp": "2025-11-02T10:30:00Z"
}
```

**資料衝突** (`409 Conflict`):
```json
{
  "error": {
    "code": "CONFLICT",
    "message": "該日期已有相同運動項目的紀錄",
    "details": {
      "existingRecordId": "551e8400-e29b-41d4-a716-446655440001",
      "date": "2025-11-02",
      "exerciseTypeId": "660e8400-e29b-41d4-a716-446655440000"
    }
  },
  "timestamp": "2025-11-02T10:30:00Z"
}
```

**未授權** (`401 Unauthorized`):
```json
{
  "error": {
    "code": "UNAUTHORIZED",
    "message": "身份驗證失敗，請重新登入",
    "details": {
      "reason": "Token 已過期"
    }
  },
  "timestamp": "2025-11-02T10:30:00Z"
}
```

---

## 附錄

### API 速率限制

- **一般端點**: 每分鐘 60 次請求
- **身份驗證端點**: 每分鐘 10 次請求
- **統計與趨勢端點**: 每分鐘 20 次請求

超過限制將回傳 `429 Too Many Requests`：
```json
{
  "error": {
    "code": "RATE_LIMIT_EXCEEDED",
    "message": "超過速率限制，請稍後再試",
    "details": {
      "limit": 60,
      "resetAt": "2025-11-02T10:31:00Z"
    }
  },
  "timestamp": "2025-11-02T10:30:45Z"
}
```

### 日期與時區

- 所有日期使用 ISO 8601 格式（`yyyy-MM-dd`）
- 所有日期時間使用 ISO 8601 格式並包含 UTC 時區（`yyyy-MM-ddTHH:mm:ssZ`）
- 前端需自行轉換為使用者本地時區顯示

### 版本管理

API 版本透過 URL path 指定（如 `/api/v1/`）。重大更新將發布新版本（如 `v2`），舊版本至少維護 6 個月。

---

**下一步**: 產生 `quickstart.md` 開發環境設定指南
