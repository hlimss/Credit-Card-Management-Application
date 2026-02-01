<template>
  <div class="analytics-view">
    <h2>📈 Analyses et Statistiques</h2>
    
    <div v-if="loading" class="loading">Chargement des données...</div>
    
    <div v-else class="analytics-content">
      <!-- Summary Cards -->
      <div class="summary-grid">
        <div class="summary-card">
          <div class="summary-icon">💳</div>
          <div class="summary-info">
            <h3>{{ cards.length }}</h3>
            <p>Cartes actives</p>
          </div>
        </div>
        <div class="summary-card">
          <div class="summary-icon">💰</div>
          <div class="summary-info">
            <h3>{{ formatCurrency(totalBalance) }}</h3>
            <p>Solde total</p>
          </div>
        </div>
        <div class="summary-card">
          <div class="summary-icon">📊</div>
          <div class="summary-info">
            <h3>{{ totalTransactions }}</h3>
            <p>Transactions</p>
          </div>
        </div>
        <div class="summary-card">
          <div class="summary-icon">📉</div>
          <div class="summary-info">
            <h3>{{ formatCurrency(totalExpenses) }}</h3>
            <p>Dépenses totales</p>
          </div>
        </div>
      </div>

      <!-- Expenses by Category -->
      <div class="analytics-section">
        <h3>Dépenses par catégorie</h3>
        <div class="category-chart">
          <div v-for="(amount, category) in expensesByCategory" :key="category" class="category-item">
            <div class="category-header">
              <span class="category-name">{{ category }}</span>
              <span class="category-amount">{{ formatCurrency(amount) }}</span>
            </div>
            <div class="category-bar">
              <div 
                class="category-fill" 
                :style="{ width: `${(amount / maxCategoryExpense) * 100}%` }"
              ></div>
            </div>
          </div>
        </div>
      </div>

      <!-- Monthly Trends -->
      <div class="analytics-section">
        <h3>Tendances mensuelles</h3>
        <div class="monthly-trends">
          <div v-for="month in monthlyData" :key="month.month" class="month-item">
            <div class="month-header">
              <span>{{ month.month }}</span>
            </div>
            <div class="month-stats">
              <div class="stat-item">
                <span class="stat-label">Revenus:</span>
                <span class="stat-value positive">{{ formatCurrency(month.credits) }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">Dépenses:</span>
                <span class="stat-value negative">{{ formatCurrency(month.debits) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Card Performance -->
      <div class="analytics-section">
        <h3>Performance des cartes</h3>
        <div class="card-performance">
          <div v-for="card in cards" :key="card.id" class="performance-card">
            <h4>{{ card.cardholderName }}</h4>
            <div class="performance-stats">
              <div class="stat">
                <span class="stat-label">Solde:</span>
                <span class="stat-value">{{ formatCurrency(card.balance || 0) }}</span>
              </div>
              <div class="stat">
                <span class="stat-label">Type:</span>
                <span class="stat-value">{{ card.cardType }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useCreditCardStore } from '../stores/creditCard'
import transactionService from '../services/transactionService'

const creditCardStore = useCreditCardStore()
const transactions = ref([])
const loading = ref(false)

const cards = computed(() => creditCardStore.cards.filter(c => c.isActive))

const totalBalance = computed(() => {
  return cards.value.reduce((sum, card) => sum + (card.balance || 0), 0)
})

const totalTransactions = computed(() => transactions.value.length)

const totalExpenses = computed(() => {
  return transactions.value
    .filter(t => t.transactionType === 'Expense')
    .reduce((sum, t) => sum + t.amount, 0)
})

const expensesByCategory = computed(() => {
  const categoryMap = {}
  transactions.value
    .filter(t => t.transactionType === 'Expense')
    .forEach(t => {
      categoryMap[t.category] = (categoryMap[t.category] || 0) + t.amount
    })
  return categoryMap
})

const maxCategoryExpense = computed(() => {
  const amounts = Object.values(expensesByCategory.value)
  return amounts.length > 0 ? Math.max(...amounts) : 1
})

const monthlyData = computed(() => {
  const monthMap = {}
  transactions.value.forEach(t => {
    const month = new Date(t.transactionDate).toLocaleDateString('fr-FR', { year: 'numeric', month: 'long' })
    if (!monthMap[month]) {
      monthMap[month] = { month, credits: 0, debits: 0 }
    }
    if (t.transactionType === 'Credit' || t.transactionType === 'Refund') {
      monthMap[month].credits += t.amount
    } else if (t.transactionType === 'Expense') {
      monthMap[month].debits += t.amount
    }
  })
  return Object.values(monthMap).slice(-6).reverse()
})

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('fr-MA', { style: 'currency', currency: 'MAD' }).format(amount)
}

const loadData = async () => {
  loading.value = true
  try {
    await creditCardStore.fetchCards()
    // Load all transactions
    try {
      transactions.value = await transactionService.getAllTransactions()
    } catch (error) {
      console.error('Error loading transactions:', error)
    }
  } catch (error) {
    console.error('Error loading analytics data:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.analytics-view {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.analytics-view h2 {
  margin-bottom: 2rem;
  color: #333;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.summary-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  gap: 1rem;
}

.summary-icon {
  font-size: 2.5rem;
}

.summary-info h3 {
  margin: 0;
  font-size: 1.8rem;
  color: #333;
}

.summary-info p {
  margin: 0.25rem 0 0 0;
  color: #666;
  font-size: 0.9rem;
}

.analytics-section {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.analytics-section h3 {
  margin: 0 0 1.5rem 0;
  color: #333;
}

.category-chart {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.category-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.category-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.category-name {
  font-weight: 600;
  color: #333;
}

.category-amount {
  font-weight: 600;
  color: #667eea;
}

.category-bar {
  width: 100%;
  height: 8px;
  background: #e9ecef;
  border-radius: 4px;
  overflow: hidden;
}

.category-fill {
  height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  transition: width 0.3s;
}

.monthly-trends {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
}

.month-item {
  background: #f9fafb;
  border-radius: 8px;
  padding: 1rem;
}

.month-header {
  font-weight: 600;
  color: #333;
  margin-bottom: 0.75rem;
}

.month-stats {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.stat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.stat-label {
  color: #666;
  font-size: 0.9rem;
}

.stat-value {
  font-weight: 600;
}

.stat-value.positive {
  color: #10b981;
}

.stat-value.negative {
  color: #ef4444;
}

.card-performance {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1rem;
}

.performance-card {
  background: #f9fafb;
  border-radius: 8px;
  padding: 1rem;
}

.performance-card h4 {
  margin: 0 0 0.75rem 0;
  color: #333;
}

.performance-stats {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.stat {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.loading {
  text-align: center;
  padding: 3rem;
  color: #666;
}
</style>
