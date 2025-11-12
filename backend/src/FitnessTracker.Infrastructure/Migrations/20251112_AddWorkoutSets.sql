-- 建立 WorkoutSets 資料表
CREATE TABLE IF NOT EXISTS "WorkoutSets" (
    "Id" SERIAL PRIMARY KEY,
    "WorkoutRecordId" INTEGER NOT NULL,
    "SetNumber" INTEGER NOT NULL,
    "Repetitions" INTEGER NOT NULL,
    "Weight" DECIMAL(10,2),
    "Notes" VARCHAR(500),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT "FK_WorkoutSets_WorkoutRecords"
        FOREIGN KEY ("WorkoutRecordId")
        REFERENCES "WorkoutRecords"("Id")
        ON DELETE CASCADE
);

-- 建立索引
CREATE INDEX IF NOT EXISTS "IX_WorkoutSets_WorkoutRecordId"
    ON "WorkoutSets"("WorkoutRecordId");

CREATE INDEX IF NOT EXISTS "IX_WorkoutSets_WorkoutRecordId_SetNumber"
    ON "WorkoutSets"("WorkoutRecordId", "SetNumber");

-- 註解
COMMENT ON TABLE "WorkoutSets" IS '訓練組數記錄';
COMMENT ON COLUMN "WorkoutSets"."Id" IS '主鍵';
COMMENT ON COLUMN "WorkoutSets"."WorkoutRecordId" IS '所屬訓練記錄 ID';
COMMENT ON COLUMN "WorkoutSets"."SetNumber" IS '組數順序';
COMMENT ON COLUMN "WorkoutSets"."Repetitions" IS '次數';
COMMENT ON COLUMN "WorkoutSets"."Weight" IS '重量(公斤)';
COMMENT ON COLUMN "WorkoutSets"."Notes" IS '備註';
COMMENT ON COLUMN "WorkoutSets"."CreatedAt" IS '建立時間';
COMMENT ON COLUMN "WorkoutSets"."UpdatedAt" IS '更新時間';
COMMENT ON COLUMN "WorkoutSets"."IsDeleted" IS '是否已刪除';
