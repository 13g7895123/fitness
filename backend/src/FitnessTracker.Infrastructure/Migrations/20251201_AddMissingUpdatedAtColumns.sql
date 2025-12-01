-- Add missing UpdatedAt columns to Equipments and ExerciseTypes tables
-- These columns exist in Entity classes but were missing from the database schema

ALTER TABLE "Equipments" ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp with time zone NOT NULL DEFAULT NOW();
ALTER TABLE "ExerciseTypes" ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp with time zone NOT NULL DEFAULT NOW();
