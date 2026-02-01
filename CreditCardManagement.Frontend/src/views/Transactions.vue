<template>
  <div class="transactions-page min-h-screen bg-gray-50">
    <div class="transactions-container max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="page-header mb-8">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="page-title">📊 Mes Transactions</h1>
            <p class="page-subtitle">Consultez l'historique complet de vos transactions</p>
          </div>
          <button
            @click="showTransactionForm = !showTransactionForm"
            class="btn-primary flex items-center gap-2"
          >
            <span>{{ showTransactionForm ? '−' : '+' }}</span>
            <span>{{ showTransactionForm ? 'Masquer' : 'Nouvelle transaction' }}</span>
          </button>
        </div>
      </div>

      <!-- Transaction Form Section -->
      <div v-if="showTransactionForm" class="transaction-form-section mb-8 bg-white rounded-lg shadow-sm p-6">
        <h2 class="form-section-title text-xl font-semibold text-gray-900 mb-6">
          💸 Créer une nouvelle transaction
        </h2>
        
        <form @submit.prevent="handleCreateTransaction" class="transaction-form">
          <!-- Error Display -->
          <div v-if="formError" class="form-error bg-red-50 border border-red-200 rounded-md p-4 mb-4">
            <div class="flex items-center gap-2">
              <span class="text-red-600">⚠️</span>
              <span class="text-red-800">{{ formError }}</span>
            </div>
          </div>

          <!-- Success Message -->
          <div v-if="formSuccess" class="form-success bg-green-50 border border-green-200 rounded-md p-4 mb-4">
            <div class="flex items-center gap-2">
              <span class="text-green-600">✅</span>
              <span class="text-green-800">{{ formSuccess }}</span>
            </div>
          </div>

          <div class="form-grid grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Card Selection -->
            <div class="form-group md:col-span-2">
              <label class="form-label">
                <span class="label-icon">💳</span>
                Carte bancaire *
              </label>
              <select
                v-model="transactionForm.creditCardId"
                class="form-input"
                required
                @change="onCardChange"
              >
                <option value="">-- Sélectionner une carte --</option>
                <option
                  v-for="card in cards"
                  :key="card.id"
                  :value="card.id"
                  :disabled="!card.isActive"
                >
                  {{ card.cardholderName }} - {{ card.cardType }} 
                  <span v-if="card.balance !== null && card.balance !== undefined">
                    (Solde: {{ formatCurrency(card.balance) }})
                  </span>
                  <span v-if="!card.isActive" class="text-red-600"> - INACTIVE</span>
                </option>
              </select>
            </div>

            <!-- Transaction Type -->
            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">📊</span>
                Type de transaction *
              </label>
              <select
                v-model="transactionForm.transactionType"
                class="form-input"
                required
              >
                <option value="Expense">💸 Dépense</option>
                <option value="Credit">💰 Crédit</option>
                <option value="Refund">↩️ Remboursement</option>
              </select>
            </div>

            <!-- Amount -->
            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">💵</span>
                Montant *
              </label>
              <input
                v-model.number="transactionForm.amount"
                type="number"
                step="0.01"
                min="0.01"
                max="999999.99"
                class="form-input"
                placeholder="0.00"
                required
              />
            </div>

            <!-- Merchant Name -->
            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">🏪</span>
                Marchand / Commerçant *
              </label>
              <input
                v-model="transactionForm.merchantName"
                type="text"
                class="form-input"
                placeholder="e.g., Amazon, Starbucks, Supermarché"
                required
                maxlength="200"
              />
            </div>

            <!-- Category -->
            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">📂</span>
                Catégorie *
              </label>
              <select
                v-model="transactionForm.category"
                class="form-input"
                required
              >
                <option value="Food">🍔 Alimentation</option>
                <option value="Transport">🚗 Transport</option>
                <option value="Shopping">🛍️ Shopping</option>
                <option value="Entertainment">🎬 Divertissement</option>
                <option value="Bills">💳 Factures</option>
                <option value="Healthcare">🏥 Santé</option>
                <option value="Travel">✈️ Voyage</option>
                <option value="Education">📚 Éducation</option>
                <option value="Other">📦 Autre</option>
              </select>
            </div>

            <!-- Currency -->
            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">🌍</span>
                Devise *
              </label>
              <select
                v-model="transactionForm.currency"
                class="form-input"
                required
              >
                <option value="MAD">MAD (د.م)</option>
                <option value="USD">USD ($)</option>
                <option value="EUR">EUR (€)</option>
                <option value="GBP">GBP (£)</option>
                <option value="JPY">JPY (¥)</option>
              </select>
            </div>

            <!-- Transaction Date -->
            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">📅</span>
                Date de transaction *
              </label>
              <input
                v-model="transactionForm.transactionDate"
                type="datetime-local"
                class="form-input"
                required
              />
            </div>

            <!-- Location -->
            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">📍</span>
                Localisation (Optionnel)
              </label>
              <input
                v-model="transactionForm.location"
                type="text"
                class="form-input"
                placeholder="e.g., Casablanca, Maroc"
                maxlength="100"
              />
            </div>

            <!-- Description -->
            <div class="form-group md:col-span-2">
              <label class="form-label">
                <span class="label-icon">📝</span>
                Description (Optionnel)
              </label>
              <textarea
                v-model="transactionForm.description"
                class="form-input"
                rows="3"
                placeholder="Ajoutez des détails supplémentaires..."
                maxlength="500"
              ></textarea>
            </div>

            <!-- Confirmation Code -->
            <div class="form-group md:col-span-2">
              <label class="form-label">
                <span class="label-icon">🔐</span>
                Code de confirmation de la carte *
              </label>
              <input
                v-model="transactionForm.confirmationCode"
                type="text"
                class="form-input"
                placeholder="Entrez le code de confirmation"
                required
                maxlength="10"
                autocomplete="off"
              />
              <p class="form-hint text-sm text-gray-500 mt-1">
                Code de sécurité configuré pour la carte sélectionnée
              </p>
            </div>
          </div>

          <!-- Form Actions -->
          <div class="form-actions flex justify-end gap-4 mt-6">
            <button
              type="button"
              @click="resetTransactionForm"
              class="btn-secondary"
            >
              Annuler
            </button>
            <button
              type="submit"
              :disabled="creatingTransaction"
              class="btn-primary"
            >
              <span v-if="creatingTransaction" class="flex items-center gap-2">
                <span class="spinner-small"></span>
                <span>Création...</span>
              </span>
              <span v-else class="flex items-center gap-2">
                <span>💸</span>
                <span>Créer la transaction</span>
              </span>
            </button>
          </div>
        </form>
      </div>

      <!-- Filters -->
      <div class="filters-section mb-6 bg-white rounded-lg shadow-sm p-4">
        <div class="flex flex-wrap gap-4 items-end">
          <div class="filter-group flex-1 min-w-[200px]">
            <label class="filter-label">Date de début</label>
            <input
              v-model="filters.startDate"
              type="date"
              class="filter-input"
              @change="loadTransactions"
            />
          </div>
          <div class="filter-group flex-1 min-w-[200px]">
            <label class="filter-label">Date de fin</label>
            <input
              v-model="filters.endDate"
              type="date"
              class="filter-input"
              @change="loadTransactions"
            />
          </div>
          <div class="filter-group">
            <button
              @click="resetFilters"
              class="btn-secondary"
            >
              Réinitialiser
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="loading-state text-center py-12">
        <div class="spinner mx-auto mb-4"></div>
        <p class="text-gray-600">Chargement des transactions...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="error-state bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
        <p class="text-red-800">{{ error }}</p>
      </div>

      <!-- Transactions Table -->
      <div v-else-if="transactions.length > 0" class="transactions-table-container bg-white rounded-lg shadow-sm overflow-hidden">
        <div class="overflow-x-auto">
          <table class="transactions-table w-full">
            <thead>
              <tr class="table-header-row">
                <th class="table-header">Date</th>
                <th class="table-header">Marchand</th>
                <th class="table-header">Catégorie</th>
                <th class="table-header">Type</th>
                <th class="table-header text-right">Montant</th>
                <th class="table-header">Devise</th>
                <th class="table-header">Localisation</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="transaction in transactions"
                :key="transaction.id"
                class="table-row"
              >
                <td class="table-cell">
                  <div class="date-cell">
                    <span class="date-day">{{ formatDay(transaction.transactionDate) }}</span>
                    <span class="date-time">{{ formatTime(transaction.transactionDate) }}</span>
                  </div>
                </td>
                <td class="table-cell">
                  <div class="merchant-cell">
                    <span class="merchant-name">{{ transaction.merchantName }}</span>
                    <span v-if="transaction.description" class="merchant-desc">{{ transaction.description }}</span>
                  </div>
                </td>
                <td class="table-cell">
                  <span class="category-badge" :class="getCategoryClass(transaction.category)">
                    {{ transaction.category }}
                  </span>
                </td>
                <td class="table-cell">
                  <span class="type-badge" :class="getTypeClass(transaction.transactionType)">
                    {{ getTypeLabel(transaction.transactionType) }}
                  </span>
                </td>
                <td class="table-cell text-right">
                  <span
                    class="amount-value"
                    :class="transaction.transactionType === 'Expense' ? 'text-red-600' : 'text-green-600'"
                  >
                    {{ transaction.transactionType === 'Expense' ? '-' : '+' }}{{ formatCurrency(transaction.amount) }}
                  </span>
                </td>
                <td class="table-cell">
                  <span class="currency-badge">{{ transaction.currency }}</span>
                </td>
                <td class="table-cell">
                  <span class="location-text">{{ transaction.location || 'N/A' }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="empty-state bg-white rounded-lg shadow-sm p-12 text-center">
        <div class="empty-icon text-6xl mb-4">📭</div>
        <h3 class="empty-title text-xl font-semibold text-gray-900 mb-2">Aucune transaction</h3>
        <p class="empty-text text-gray-600 mb-6">Vous n'avez pas encore effectué de transactions.</p>
        <router-link to="/" class="btn-primary inline-block">
          Retour au tableau de bord
        </router-link>
      </div>

      <!-- Summary Stats -->
      <div v-if="transactions.length > 0" class="summary-stats mt-8 grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="stat-card bg-white rounded-lg shadow-sm p-6">
          <div class="stat-icon text-3xl mb-2">💰</div>
          <div class="stat-label text-sm text-gray-600 mb-1">Total dépenses</div>
          <div class="stat-value text-2xl font-bold text-red-600">
            {{ formatCurrency(totalExpenses) }}
          </div>
        </div>
        <div class="stat-card bg-white rounded-lg shadow-sm p-6">
          <div class="stat-icon text-3xl mb-2">📈</div>
          <div class="stat-label text-sm text-gray-600 mb-1">Total crédits</div>
          <div class="stat-value text-2xl font-bold text-green-600">
            {{ formatCurrency(totalCredits) }}
          </div>
        </div>
        <div class="stat-card bg-white rounded-lg shadow-sm p-6">
          <div class="stat-icon text-3xl mb-2">📊</div>
          <div class="stat-label text-sm text-gray-600 mb-1">Nombre de transactions</div>
          <div class="stat-value text-2xl font-bold text-gray-900">
            {{ transactions.length }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import transactionService from '../services/transactionService'
import { useCreditCardStore } from '../stores/creditCard'
import { useNotificationStore } from '../stores/notification'

const creditCardStore = useCreditCardStore()
const notificationStore = useNotificationStore()
const transactions = ref([])
const loading = ref(true)
const error = ref('')
const showTransactionForm = ref(false)
const creatingTransaction = ref(false)
const formError = ref('')
const formSuccess = ref('')

const filters = ref({
  startDate: '',
  endDate: ''
})

const cards = computed(() => creditCardStore.cards || [])

const transactionForm = ref({
  creditCardId: '',
  merchantName: '',
  amount: '',
  category: 'Other',
  description: '',
  transactionDate: new Date().toISOString().slice(0, 16),
  location: '',
  currency: 'MAD',
  transactionType: 'Expense',
  confirmationCode: ''
})

const totalExpenses = computed(() => {
  return transactions.value
    .filter(t => t.transactionType === 'Expense')
    .reduce((sum, t) => sum + parseFloat(t.amount), 0)
})

const totalCredits = computed(() => {
  return transactions.value
    .filter(t => t.transactionType === 'Credit' || t.transactionType === 'Refund')
    .reduce((sum, t) => sum + parseFloat(t.amount), 0)
})

async function loadTransactions() {
  loading.value = true
  error.value = ''
  try {
    const startDate = filters.value.startDate ? new Date(filters.value.startDate).toISOString() : null
    const endDate = filters.value.endDate ? new Date(filters.value.endDate).toISOString() : null
    transactions.value = await transactionService.getAllTransactions(startDate, endDate)
  } catch (err) {
    console.error('Error loading transactions:', err)
    error.value = err.response?.data?.message || 'Erreur lors du chargement des transactions'
  } finally {
    loading.value = false
  }
}

function resetFilters() {
  filters.value = {
    startDate: '',
    endDate: ''
  }
  loadTransactions()
}

function formatCurrency(amount) {
  return new Intl.NumberFormat('fr-FR', {
    style: 'currency',
    currency: 'MAD',
    minimumFractionDigits: 2
  }).format(amount)
}

function formatDay(dateString) {
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('fr-FR', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(date)
}

function formatTime(dateString) {
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('fr-FR', {
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

function getCategoryClass(category) {
  const classes = {
    'Food': 'bg-orange-100 text-orange-800',
    'Transport': 'bg-blue-100 text-blue-800',
    'Shopping': 'bg-purple-100 text-purple-800',
    'Entertainment': 'bg-pink-100 text-pink-800',
    'Other': 'bg-gray-100 text-gray-800'
  }
  return classes[category] || classes.Other
}

function getTypeClass(type) {
  const classes = {
    'Expense': 'bg-red-100 text-red-800',
    'Credit': 'bg-green-100 text-green-800',
    'Refund': 'bg-blue-100 text-blue-800'
  }
  return classes[type] || classes.Expense
}

function getTypeLabel(type) {
  const labels = {
    'Expense': 'Dépense',
    'Credit': 'Crédit',
    'Refund': 'Remboursement'
  }
  return labels[type] || type
}

function onCardChange() {
  // Reset confirmation code when card changes
  transactionForm.value.confirmationCode = ''
}

function resetTransactionForm() {
  transactionForm.value = {
    creditCardId: '',
    merchantName: '',
    amount: '',
    category: 'Other',
    description: '',
    transactionDate: new Date().toISOString().slice(0, 16),
    location: '',
    currency: 'MAD',
    transactionType: 'Expense',
    confirmationCode: ''
  }
  formError.value = ''
  formSuccess.value = ''
}

async function handleCreateTransaction() {
  formError.value = ''
  formSuccess.value = ''
  creatingTransaction.value = true

  try {
    if (!transactionForm.value.creditCardId) {
      formError.value = 'Veuillez sélectionner une carte'
      creatingTransaction.value = false
      return
    }

    // Vérifier si la carte est active
    const selectedCard = cards.value.find(c => c.id === transactionForm.value.creditCardId)
    if (selectedCard && !selectedCard.isActive) {
      formError.value = '⚠️ Cette carte est inactive. Veuillez l\'activer dans les paramètres pour effectuer des transactions.'
      creatingTransaction.value = false
      return
    }

    if (!transactionForm.value.confirmationCode || transactionForm.value.confirmationCode.trim().length < 4) {
      formError.value = 'Le code de confirmation est requis (minimum 4 caractères)'
      creatingTransaction.value = false
      return
    }

    const transactionData = {
      creditCardId: transactionForm.value.creditCardId,
      merchantName: transactionForm.value.merchantName,
      amount: parseFloat(transactionForm.value.amount),
      category: transactionForm.value.category,
      description: transactionForm.value.description || null,
      transactionDate: new Date(transactionForm.value.transactionDate).toISOString(),
      location: transactionForm.value.location || null,
      currency: transactionForm.value.currency,
      transactionType: transactionForm.value.transactionType,
      confirmationCode: transactionForm.value.confirmationCode.trim()
    }

    await transactionService.createTransaction(transactionData)
    
    formSuccess.value = 'Transaction créée avec succès !'
    
    // Ajouter une notification (réutiliser selectedCard déclaré plus haut)
    notificationStore.addNotification({
      type: 'success',
      icon: '💸',
      title: 'Transaction créée',
      message: `Transaction de ${formatCurrency(transactionData.amount)} ${transactionData.currency} effectuée avec la carte ${selectedCard?.cardholderName || 'sélectionnée'}`
    })
    
    resetTransactionForm()
    
    // Recharger les transactions et les cartes
    await loadTransactions()
    await creditCardStore.fetchCards()
    
    // Masquer le formulaire après 2 secondes
    setTimeout(() => {
      showTransactionForm.value = false
      formSuccess.value = ''
    }, 2000)
  } catch (err) {
    console.error('Error creating transaction:', err)
    const errorMessage = err.response?.data?.message || err.response?.data?.error || 'Erreur lors de la création de la transaction. Veuillez réessayer.'
    formError.value = errorMessage
  } finally {
    creatingTransaction.value = false
  }
}

onMounted(async () => {
  await loadTransactions()
  if (cards.value.length === 0) {
    await creditCardStore.fetchCards()
  }
})
</script>

<style scoped>
.transactions-page {
  padding-top: 56px; /* Header height */
}

.page-header {
  @apply mb-8;
}

.page-title {
  @apply text-3xl font-bold text-gray-900 mb-2;
}

.page-subtitle {
  @apply text-gray-600;
}

.filters-section {
  @apply bg-white rounded-lg shadow-sm p-4 mb-6;
}

.filter-group {
  @apply flex flex-col gap-1;
}

.filter-label {
  @apply text-sm font-medium text-gray-700 mb-1;
}

.filter-input {
  @apply px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent;
}

.btn-secondary {
  @apply px-4 py-2 bg-gray-100 text-gray-700 rounded-md hover:bg-gray-200 transition-colors;
}

.btn-primary {
  @apply px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors font-medium;
}

.loading-state {
  @apply text-center py-12;
}

.spinner {
  @apply w-12 h-12 border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin mx-auto mb-4;
}

.transactions-table-container {
  @apply bg-white rounded-lg shadow-sm overflow-hidden;
}

.transactions-table {
  @apply w-full;
}

.table-header-row {
  @apply bg-gray-50 border-b border-gray-200;
}

.table-header {
  @apply px-6 py-4 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider;
}

.table-row {
  @apply border-b border-gray-100 hover:bg-gray-50 transition-colors;
}

.table-row:last-child {
  @apply border-b-0;
}

.table-cell {
  @apply px-6 py-4 text-sm;
}

.date-cell {
  @apply flex flex-col;
}

.date-day {
  @apply font-medium text-gray-900;
}

.date-time {
  @apply text-xs text-gray-500;
}

.merchant-cell {
  @apply flex flex-col;
}

.merchant-name {
  @apply font-medium text-gray-900;
}

.merchant-desc {
  @apply text-xs text-gray-500 mt-0.5;
}

.category-badge,
.type-badge,
.currency-badge {
  @apply inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium;
}

.location-text {
  @apply text-gray-600;
}

.amount-value {
  @apply font-semibold;
}

.empty-state {
  @apply bg-white rounded-lg shadow-sm p-12 text-center;
}

.empty-icon {
  @apply text-6xl mb-4;
}

.empty-title {
  @apply text-xl font-semibold text-gray-900 mb-2;
}

.empty-text {
  @apply text-gray-600 mb-6;
}

.summary-stats {
  @apply grid grid-cols-1 md:grid-cols-3 gap-6 mt-8;
}

.stat-card {
  @apply bg-white rounded-lg shadow-sm p-6;
}

.stat-icon {
  @apply text-3xl mb-2;
}

.stat-label {
  @apply text-sm text-gray-600 mb-1;
}

.stat-value {
  @apply text-2xl font-bold;
}

.transaction-form-section {
  @apply bg-white rounded-lg shadow-sm p-6 mb-8;
}

.form-section-title {
  @apply text-xl font-semibold text-gray-900 mb-6;
}

.transaction-form {
  @apply space-y-6;
}

.form-grid {
  @apply grid grid-cols-1 md:grid-cols-2 gap-6;
}

.form-group {
  @apply flex flex-col gap-1;
}

.form-label {
  @apply flex items-center gap-2 text-sm font-medium text-gray-700 mb-1;
}

.label-icon {
  @apply text-base;
}

.form-input {
  @apply px-3 py-2 border border-gray-300 rounded-md 
         focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent
         transition-all duration-200;
}

.form-input:disabled {
  @apply bg-gray-100 cursor-not-allowed;
}

.form-hint {
  @apply text-sm text-gray-500 mt-1;
}

.form-error {
  @apply bg-red-50 border border-red-200 rounded-md p-4;
}

.form-success {
  @apply bg-green-50 border border-green-200 rounded-md p-4;
}

.form-actions {
  @apply flex justify-end gap-4 mt-6;
}

.spinner-small {
  @apply w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin;
}
</style>
