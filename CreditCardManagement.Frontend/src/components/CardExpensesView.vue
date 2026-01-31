<template>
  <div class="card-expenses-view">
    <div class="expenses-header">
      <button @click="$emit('close')" class="close-btn">✕</button>
      <h2 class="expenses-title">💳 Dépenses - {{ cardExpenses?.cardholderName }}</h2>
      <p class="card-type">{{ cardExpenses?.cardType }} • {{ cardExpenses?.last4Digits }}</p>
    </div>

    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Chargement des dépenses...</p>
    </div>

    <div v-else-if="cardExpenses" class="expenses-content">
      <!-- Statistiques principales -->
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-icon">💰</div>
          <div class="stat-info">
            <p class="stat-label">Total dépensé</p>
            <p class="stat-value">{{ formatCurrency(cardExpenses.totalAmount) }}</p>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon">📊</div>
          <div class="stat-info">
            <p class="stat-label">Transactions</p>
            <p class="stat-value">{{ cardExpenses.totalTransactions }}</p>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon">📈</div>
          <div class="stat-info">
            <p class="stat-label">Moyenne</p>
            <p class="stat-value">{{ formatCurrency(cardExpenses.averageAmount) }}</p>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon">📅</div>
          <div class="stat-info">
            <p class="stat-label">Ce mois</p>
            <p class="stat-value">{{ formatCurrency(cardExpenses.thisMonthAmount || 0) }}</p>
          </div>
        </div>
      </div>

      <!-- Breakdown par catégorie -->
      <div class="category-breakdown">
        <h3 class="section-title">📂 Répartition par catégorie</h3>
        <div class="category-list">
          <div
            v-for="(amount, category) in cardExpenses.categoryBreakdown"
            :key="category"
            class="category-item"
          >
            <div class="category-info">
              <span class="category-name">{{ category }}</span>
              <span class="category-amount">{{ formatCurrency(amount) }}</span>
            </div>
            <div class="category-bar">
              <div
                class="category-fill"
                :style="{ width: `${(amount / cardExpenses.totalAmount) * 100}%` }"
              ></div>
            </div>
          </div>
        </div>
      </div>

      <!-- Transactions récentes -->
      <div class="recent-transactions">
        <h3 class="section-title">🕐 Transactions récentes</h3>
        <div class="transactions-list">
          <div
            v-for="transaction in cardExpenses.recentTransactions"
            :key="transaction.id"
            class="transaction-item"
          >
            <div class="transaction-icon">💸</div>
            <div class="transaction-details">
              <p class="transaction-merchant">{{ transaction.merchantName }}</p>
              <p class="transaction-meta">
                {{ formatDate(transaction.transactionDate) }} • {{ transaction.category }}
              </p>
            </div>
            <div class="transaction-amount">
              <span class="amount-value">{{ formatCurrency(transaction.amount) }}</span>
            </div>
          </div>
          <div v-if="cardExpenses.recentTransactions.length === 0" class="empty-transactions">
            <p>Aucune transaction enregistrée</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import transactionService from '../services/transactionService'

const props = defineProps({
  cardId: {
    type: String,
    required: true
  }
})

const emit = defineEmits(['close'])

const cardExpenses = ref(null)
const loading = ref(true)

onMounted(async () => {
  try {
    loading.value = true
    cardExpenses.value = await transactionService.getCardExpenses(props.cardId)
  } catch (error) {
    console.error('Error loading card expenses:', error)
  } finally {
    loading.value = false
  }
})

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('fr-FR', {
    style: 'currency',
    currency: 'USD'
  }).format(amount)
}

const formatDate = (dateString) => {
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('fr-FR', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(date)
}
</script>

<style scoped>
.card-expenses-view {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.8);
  backdrop-filter: blur(10px);
  z-index: 1000;
  display: flex;
  flex-direction: column;
  overflow-y: auto;
}

.expenses-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem;
  color: white;
  position: sticky;
  top: 0;
  z-index: 10;
}

.close-btn {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  color: white;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  cursor: pointer;
  font-size: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: rotate(90deg);
}

.expenses-title {
  font-size: 2rem;
  font-weight: 800;
  margin-bottom: 0.5rem;
}

.card-type {
  opacity: 0.9;
  font-size: 1rem;
}

.expenses-content {
  flex: 1;
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s;
}

.stat-card:hover {
  transform: translateY(-5px);
}

.stat-icon {
  font-size: 2.5rem;
}

.stat-label {
  color: #6b7280;
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1f2937;
}

.category-breakdown,
.recent-transactions {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
}

.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
  color: #1f2937;
}

.category-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.category-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.category-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.category-name {
  font-weight: 600;
  color: #374151;
}

.category-amount {
  font-weight: 700;
  color: #667eea;
}

.category-bar {
  height: 8px;
  background: #e5e7eb;
  border-radius: 4px;
  overflow: hidden;
}

.category-fill {
  height: 100%;
  background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
  transition: width 0.5s ease;
}

.transactions-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.transaction-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 12px;
  transition: all 0.3s;
}

.transaction-item:hover {
  background: #f3f4f6;
  transform: translateX(5px);
}

.transaction-icon {
  font-size: 2rem;
}

.transaction-details {
  flex: 1;
}

.transaction-merchant {
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.25rem;
}

.transaction-meta {
  font-size: 0.875rem;
  color: #6b7280;
}

.transaction-amount {
  font-size: 1.25rem;
  font-weight: 700;
  color: #ef4444;
}

.empty-transactions {
  text-align: center;
  padding: 2rem;
  color: #6b7280;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  color: white;
}

.spinner {
  width: 50px;
  height: 50px;
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

@media (max-width: 768px) {
  .expenses-content {
    padding: 1rem;
  }

  .stats-grid {
    grid-template-columns: 1fr;
  }
}
</style>
