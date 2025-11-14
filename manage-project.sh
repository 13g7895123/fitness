#!/bin/bash

# 顏色定義
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "======================================"
echo "  Fitness Tracker 專案管理腳本"
echo "======================================"

# 檢查 Docker Compose 是否安裝
if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
    echo -e "${RED}錯誤: Docker Compose 未安裝${NC}"
    exit 1
fi

# 使用 docker compose 或 docker-compose
DOCKER_COMPOSE_CMD="docker compose"
if ! docker compose version &> /dev/null; then
    DOCKER_COMPOSE_CMD="docker-compose"
fi

# 檢查容器是否正在運行
check_containers() {
    echo -e "\n${YELLOW}檢查容器狀態...${NC}"

    POSTGRES_RUNNING=$(docker ps --filter "name=fitness-postgres" --filter "status=running" -q)
    BACKEND_RUNNING=$(docker ps --filter "name=fitness-backend-dev" --filter "status=running" -q)
    FRONTEND_RUNNING=$(docker ps --filter "name=fitness-frontend-dev" --filter "status=running" -q)

    if [ -n "$POSTGRES_RUNNING" ] || [ -n "$BACKEND_RUNNING" ] || [ -n "$FRONTEND_RUNNING" ]; then
        echo -e "${YELLOW}發現運行中的容器:${NC}"

        if [ -n "$POSTGRES_RUNNING" ]; then
            echo -e "  ✓ PostgreSQL (fitness-postgres)"
        fi

        if [ -n "$BACKEND_RUNNING" ]; then
            echo -e "  ✓ Backend (fitness-backend-dev)"
        fi

        if [ -n "$FRONTEND_RUNNING" ]; then
            echo -e "  ✓ Frontend (fitness-frontend-dev)"
        fi

        return 0
    else
        echo -e "${RED}沒有發現運行中的容器${NC}"
        return 1
    fi
}

# 停止所有容器
stop_containers() {
    echo -e "\n${YELLOW}停止所有容器...${NC}"
    $DOCKER_COMPOSE_CMD -f docker-compose.dev.yml down
    echo -e "${GREEN}容器已停止${NC}"
}

# 啟動所有容器
start_containers() {
    echo -e "\n${YELLOW}啟動所有容器...${NC}"
    $DOCKER_COMPOSE_CMD -f docker-compose.dev.yml up -d

    # 等待容器啟動
    echo -e "${YELLOW}等待容器啟動...${NC}"
    sleep 5

    # 檢查容器狀態
    echo -e "\n${YELLOW}容器狀態:${NC}"
    $DOCKER_COMPOSE_CMD -f docker-compose.dev.yml ps

    # 顯示服務地址
    API_PORT=$(grep "API_PORT" .env 2>/dev/null | cut -d '=' -f2 || echo "9103")
    FRONTEND_PORT=$(grep "FRONTEND_PORT" .env 2>/dev/null | cut -d '=' -f2 || echo "9202")
    POSTGRES_PORT=$(grep "POSTGRES_PORT" .env 2>/dev/null | cut -d '=' -f2 || echo "9302")

    echo -e "\n${GREEN}專案已啟動!${NC}"
    echo -e "${GREEN}服務地址:${NC}"
    echo -e "  🌐 前端: http://localhost:${FRONTEND_PORT}"
    echo -e "  🔧 後端 API: http://localhost:${API_PORT}"
    echo -e "  🗄️  PostgreSQL: localhost:${POSTGRES_PORT}"

    # 檢查前端是否需要啟動 dev server
    echo -e "\n${YELLOW}注意: 前端容器需要手動啟動開發伺服器${NC}"
    echo -e "執行以下命令啟動前端:"
    echo -e "  ${GREEN}docker exec -it fitness-frontend-dev npm run dev${NC}"
}

# 檢查健康狀態
check_health() {
    echo -e "\n${YELLOW}檢查服務健康狀態...${NC}"

    # 檢查 PostgreSQL
    if docker exec fitness-postgres pg_isready -U postgres &> /dev/null; then
        echo -e "  ${GREEN}✓ PostgreSQL: 健康${NC}"
    else
        echo -e "  ${RED}✗ PostgreSQL: 不健康${NC}"
    fi

    # 檢查後端
    API_PORT=$(grep "API_PORT" .env 2>/dev/null | cut -d '=' -f2 || echo "9103")
    if curl -s http://localhost:${API_PORT}/health &> /dev/null || curl -s http://localhost:${API_PORT} &> /dev/null; then
        echo -e "  ${GREEN}✓ Backend API: 健康${NC}"
    else
        echo -e "  ${YELLOW}⚠ Backend API: 無法連接 (可能還在啟動中)${NC}"
    fi
}

# 主邏輯
if check_containers; then
    echo -e "\n${YELLOW}專案已在運行中,將先停止再重新啟動...${NC}"
    stop_containers
    start_containers
    check_health
else
    echo -e "\n${YELLOW}專案未運行,開始啟動...${NC}"
    start_containers
    check_health
fi

echo -e "\n======================================"
echo -e "${GREEN}完成!${NC}"
echo -e "======================================"
