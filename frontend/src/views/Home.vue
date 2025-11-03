<template>
  <v-container class="home-page">
    <div class="page-header">
      <h1>{{ $t('app.title') }}</h1>
      <p>{{ $t('statistics.weekly') }}</p>
    </div>

    <v-loading
      :visible="isLoading"
      :text="$t('common.loading')"
    ></v-loading>

    <div v-if="!isLoading" class="content">
      <v-row>
        <v-col cols="12">
          <WeeklySummaryCard />
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="6">
          <WeeklyComparisonCard />
        </v-col>
        <v-col cols="12" md="6">
          <v-card>
            <v-card-title>快速操作</v-card-title>
            <v-card-text>
              <v-btn color="primary" block class="mb-2">
                {{ $t('workout.addRecord') }}
              </v-btn>
              <v-btn color="secondary" block>
                查看詳細
              </v-btn>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <DailyBarChart />
        </v-col>
      </v-row>
    </div>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useStatisticsStore } from '@/stores/statistics'
import { useStatisticsService } from '@/services/statisticsService'
import WeeklySummaryCard from '@/components/workout/WeeklySummaryCard.vue'
import WeeklyComparisonCard from '@/components/workout/WeeklyComparisonCard.vue'
import DailyBarChart from '@/components/charts/DailyBarChart.vue'
import Loading from '@/components/common/Loading.vue'

const statisticsStore = useStatisticsStore()
const { getWeeklySummary } = useStatisticsService()

const isLoading = () => statisticsStore.isLoading

onMounted(async () => {
  await getWeeklySummary()
})
</script>

<style scoped>
.home-page {
  padding: 24px;
}

.page-header {
  margin-bottom: 32px;
}

.page-header h1 {
  margin: 0 0 8px 0;
  font-size: 28px;
  font-weight: bold;
}

.page-header p {
  margin: 0;
  color: #999;
}

.content {
  width: 100%;
}

.mb-2 {
  margin-bottom: 8px !important;
}
</style>
