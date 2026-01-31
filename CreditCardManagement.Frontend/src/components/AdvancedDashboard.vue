<template>
  <div class="advanced-dashboard">
    <div class="dashboard-header-3d">
      <h2 class="dashboard-title-3d">📊 Dashboard Interactif</h2>
      <div class="date-range-selector">
        <select v-model="selectedPeriod" @change="loadAnalytics" class="period-select">
          <option value="7">7 derniers jours</option>
          <option value="30">30 derniers jours</option>
          <option value="90">3 derniers mois</option>
          <option value="365">12 derniers mois</option>
        </select>
      </div>
    </div>

    <div v-if="loading" class="loading-state-3d">
      <div class="spinner-3d-large"></div>
      <p>Chargement des données...</p>
    </div>

    <div v-else-if="analytics" class="dashboard-content-3d">
      <!-- Statistiques principales -->
      <div class="stats-grid-3d">
        <div class="stat-card-3d">
          <div class="stat-icon-3d">💰</div>
          <div class="stat-content">
            <p class="stat-label-3d">Total Dépensé</p>
            <p class="stat-value-3d">{{ formatCurrency(analytics.totalExpenses) }}</p>
          </div>
        </div>
        <div class="stat-card-3d">
          <div class="stat-icon-3d">📊</div>
          <div class="stat-content">
            <p class="stat-label-3d">Transactions</p>
            <p class="stat-value-3d">{{ analytics.totalTransactions }}</p>
          </div>
        </div>
        <div class="stat-card-3d">
          <div class="stat-icon-3d">📈</div>
          <div class="stat-content">
            <p class="stat-label-3d">Moyenne</p>
            <p class="stat-value-3d">{{ formatCurrency(analytics.averageTransaction) }}</p>
          </div>
        </div>
        <div class="stat-card-3d">
          <div class="stat-icon-3d">🎯</div>
          <div class="stat-content">
            <p class="stat-label-3d">vs Moyenne</p>
            <p class="stat-value-3d" :class="comparisonClass">
              {{ comparisonText }}
            </p>
          </div>
        </div>
      </div>

      <!-- Graphiques 3D et Visualisations -->
      <div class="charts-grid">
        <!-- Graphique par catégorie -->
        <div class="chart-container-3d">
          <h3 class="chart-title">📂 Répartition par Catégorie</h3>
          <div class="chart-wrapper">
            <canvas ref="categoryChart"></canvas>
          </div>
        </div>

        <!-- Heatmap des dépenses -->
        <div class="chart-container-3d">
          <h3 class="chart-title">🔥 Heatmap des Dépenses</h3>
          <div class="heatmap-container">
            <div class="heatmap-grid">
              <div
                v-for="(amount, date) in dailyExpenses"
                :key="date"
                class="heatmap-cell"
                :style="getHeatmapStyle(amount)"
                :title="`${formatDate(date)}: ${formatCurrency(amount)}`"
              >
                <span class="heatmap-amount">{{ formatShortCurrency(amount) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Graphique temporel -->
        <div class="chart-container-3d">
          <h3 class="chart-title">📅 Évolution Temporelle</h3>
          <div class="chart-wrapper">
            <canvas ref="timelineChart"></canvas>
          </div>
        </div>

        <!-- Projections financières -->
        <div class="chart-container-3d">
          <h3 class="chart-title">🔮 Projections (6-12 mois)</h3>
          <div class="projections-container">
            <div class="projection-item" v-for="(projection, index) in projections" :key="index">
              <div class="projection-month">{{ projection.month }}</div>
              <div class="projection-bar-container">
                <div
                  class="projection-bar"
                  :style="{ width: `${(projection.amount / maxProjection) * 100}%` }"
                >
                  <span class="projection-amount">{{ formatCurrency(projection.amount) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { Chart, registerables } from 'chart.js'
import transactionService from '../services/transactionService'

Chart.register(...registerables)

const loading = ref(true)
const analytics = ref(null)
const selectedPeriod = ref(30)
const categoryChart = ref(null)
const timelineChart = ref(null)
let categoryChartInstance = null
let timelineChartInstance = null

const dailyExpenses = computed(() => {
  if (!analytics.value?.dailyExpenses) return {}
  return analytics.value.dailyExpenses
})

const projections = computed(() => {
  if (!analytics.value) return []
  
  const monthly = analytics.value.monthlyExpenses || {}
  const months = Object.keys(monthly).sort()
  const values = months.map(m => monthly[m])
  
  if (values.length < 2) return []
  
  // Calcul de la tendance
  const avg = values.reduce((a, b) => a + b, 0) / values.length
  const trend = values.length > 1 ? (values[values.length - 1] - values[0]) / values.length : 0
  
  // Projections sur 12 mois
  const projections = []
  const lastValue = values[values.length - 1] || avg
  
  for (let i = 1; i <= 12; i++) {
    const date = new Date()
    date.setMonth(date.getMonth() + i)
    const monthName = date.toLocaleDateString('fr-FR', { month: 'long', year: 'numeric' })
    const projected = Math.max(0, lastValue + (trend * i))
    
    projections.push({
      month: monthName,
      amount: projected
    })
  }
  
  return projections
})

const maxProjection = computed(() => {
  if (projections.value.length === 0) return 1
  return Math.max(...projections.value.map(p => p.amount))
})

const comparisonText = computed(() => {
  if (!analytics.value) return 'N/A'
  const userAvg = analytics.value.averageTransaction
  const marketAvg = 75 // Moyenne du marché (exemple)
  const diff = ((userAvg - marketAvg) / marketAvg) * 100
  
  if (Math.abs(diff) < 5) return '≈ Moyenne'
  return diff > 0 ? `+${diff.toFixed(1)}%` : `${diff.toFixed(1)}%`
})

const comparisonClass = computed(() => {
  if (!analytics.value) return ''
  const userAvg = analytics.value.averageTransaction
  const marketAvg = 75
  const diff = userAvg - marketAvg
  
  if (Math.abs(diff) < 5) return 'neutral'
  return diff > 0 ? 'above' : 'below'
})

onMounted(async () => {
  await loadAnalytics()
})

onUnmounted(() => {
  if (categoryChartInstance) categoryChartInstance.destroy()
  if (timelineChartInstance) timelineChartInstance.destroy()
})

const loadAnalytics = async () => {
  try {
    loading.value = true
    const endDate = new Date()
    const startDate = new Date()
    startDate.setDate(startDate.getDate() - parseInt(selectedPeriod.value))
    
    analytics.value = await transactionService.getExpenseAnalytics(
      startDate.toISOString(),
      endDate.toISOString()
    )
    
    await nextTick()
    renderCharts()
  } catch (error) {
    console.error('Error loading analytics:', error)
  } finally {
    loading.value = false
  }
}

const renderCharts = () => {
  if (!analytics.value) return
  
  // Graphique par catégorie
  if (categoryChart.value) {
    const categoryData = analytics.value.categoryBreakdown || {}
    const labels = Object.keys(categoryData)
    const data = Object.values(categoryData)
    
    if (categoryChartInstance) categoryChartInstance.destroy()
    
    categoryChartInstance = new Chart(categoryChart.value, {
      type: 'doughnut',
      data: {
        labels,
        datasets: [{
          data,
          backgroundColor: [
            '#667eea', '#764ba2', '#f093fb', '#4facfe',
            '#43e97b', '#fa709a', '#fee140', '#30cfd0'
          ]
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'bottom'
          }
        }
      }
    })
  }
  
  // Graphique temporel
  if (timelineChart.value) {
    const dailyData = analytics.value.dailyExpenses || {}
    const dates = Object.keys(dailyData).sort()
    const amounts = dates.map(d => dailyData[d])
    
    if (timelineChartInstance) timelineChartInstance.destroy()
    
    timelineChartInstance = new Chart(timelineChart.value, {
      type: 'line',
      data: {
        labels: dates.map(d => formatDate(d)),
        datasets: [{
          label: 'Dépenses quotidiennes',
          data: amounts,
          borderColor: '#667eea',
          backgroundColor: 'rgba(102, 126, 234, 0.1)',
          tension: 0.4,
          fill: true
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: false
          }
        },
        scales: {
          y: {
            beginAtZero: true
          }
        }
      }
    })
  }
}

const getHeatmapStyle = (amount) => {
  if (!analytics.value) return {}
  const maxAmount = Math.max(...Object.values(dailyExpenses.value))
  const intensity = maxAmount > 0 ? amount / maxAmount : 0
  const opacity = 0.3 + (intensity * 0.7)
  
  return {
    backgroundColor: `rgba(102, 126, 234, ${opacity})`,
    transform: `scale(${0.8 + intensity * 0.2})`
  }
}

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('fr-FR', {
    style: 'currency',
    currency: 'USD'
  }).format(amount)
}

const formatShortCurrency = (amount) => {
  if (amount < 1000) return `$${Math.round(amount)}`
  return `$${(amount / 1000).toFixed(1)}k`
}

const formatDate = (dateString) => {
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('fr-FR', {
    day: '2-digit',
    month: 'short'
  }).format(date)
}

import { nextTick } from 'vue'
</script>

<style scoped>
.advanced-dashboard {
  padding: 2rem;
  background: linear-gradient(to bottom, #f8fafc, #e0e7ff);
  min-height: 100vh;
}

.dashboard-header-3d {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: white;
  border-radius: 20px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
}

.dashboard-title-3d {
  font-size: 2rem;
  font-weight: 800;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.period-select {
  padding: 0.75rem 1.5rem;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  font-size: 1rem;
  background: white;
  cursor: pointer;
  transition: all 0.3s;
}

.period-select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.stats-grid-3d {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.stat-card-3d {
  background: white;
  border-radius: 20px;
  padding: 2rem;
  display: flex;
  align-items: center;
  gap: 1.5rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
  transition: all 0.3s;
  transform-style: preserve-3d;
}

.stat-card-3d:hover {
  transform: translateY(-10px) rotateX(5deg);
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
}

.stat-icon-3d {
  font-size: 3rem;
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.1));
}

.stat-label-3d {
  color: #6b7280;
  font-size: 0.875rem;
  margin-bottom: 0.5rem;
}

.stat-value-3d {
  font-size: 2rem;
  font-weight: 800;
  color: #1f2937;
}

.stat-value-3d.above {
  color: #10b981;
}

.stat-value-3d.below {
  color: #ef4444;
}

.stat-value-3d.neutral {
  color: #6b7280;
}

.charts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));
  gap: 2rem;
}

.chart-container-3d {
  background: white;
  border-radius: 20px;
  padding: 2rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
  transition: all 0.3s;
}

.chart-container-3d:hover {
  transform: translateY(-5px);
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
}

.chart-title {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
  color: #1f2937;
}

.chart-wrapper {
  height: 300px;
  position: relative;
}

.heatmap-container {
  padding: 1rem;
}

.heatmap-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(60px, 1fr));
  gap: 0.5rem;
}

.heatmap-cell {
  aspect-ratio: 1;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
  cursor: pointer;
  position: relative;
}

.heatmap-cell:hover {
  transform: scale(1.1);
  z-index: 10;
}

.heatmap-amount {
  font-size: 0.7rem;
  color: white;
  font-weight: 600;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
}

.projections-container {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.projection-item {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.projection-month {
  min-width: 150px;
  font-weight: 600;
  color: #374151;
}

.projection-bar-container {
  flex: 1;
  height: 40px;
  background: #e5e7eb;
  border-radius: 20px;
  overflow: hidden;
  position: relative;
}

.projection-bar {
  height: 100%;
  background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding-right: 1rem;
  transition: width 0.5s ease;
}

.projection-amount {
  color: white;
  font-weight: 700;
  font-size: 0.875rem;
}

.loading-state-3d {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
}

.spinner-3d-large {
  width: 60px;
  height: 60px;
  border: 5px solid rgba(102, 126, 234, 0.2);
  border-top-color: #667eea;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

@media (max-width: 768px) {
  .charts-grid {
    grid-template-columns: 1fr;
  }
  
  .stats-grid-3d {
    grid-template-columns: 1fr;
  }
}
</style>
