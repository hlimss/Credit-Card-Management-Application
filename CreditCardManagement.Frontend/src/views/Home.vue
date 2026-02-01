<template>
  <div class="modern-dashboard min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-purple-50">
    <div class="dashboard-container">

      <!-- Page Header with Add Button -->
      <div v-if="!creditCardStore.loading" class="page-header-section mb-6">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="page-title text-3xl font-bold text-gray-900 mb-2">💳 Mes Cartes Bancaires</h1>
            <p class="page-subtitle text-gray-600">Gérez toutes vos cartes bancaires en un seul endroit</p>
          </div>
          <button
            @click="showModal = true; editingCard = null"
            class="add-card-header-btn"
          >
            <span class="add-card-icon">➕</span>
            <span>Ajouter une carte</span>
          </button>
        </div>
      </div>

      <!-- Dashboard Stats -->
      <DashboardStats v-if="!creditCardStore.loading && creditCardStore.cards.length > 0" />

      <!-- Filters and Search -->
      <CardFilters 
        v-if="!creditCardStore.loading && creditCardStore.cards.length > 0"
        @search="handleSearch"
        @filter="handleFilter"
        @sort="handleSort"
      />

      <!-- Loading State -->
      <div v-if="creditCardStore.loading" class="loading-container">
        <div class="spinner-3d"></div>
        <p class="loading-text">Loading your cards...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="creditCardStore.error" class="error-container">
        <div class="error-icon">⚠️</div>
        <h3 class="error-title">Error</h3>
        <p class="error-message">{{ creditCardStore.error }}</p>
      </div>

      <!-- Cards Grid with 3D Cards -->
      <div v-else-if="filteredCards.length > 0" class="cards-grid">
        <div
          v-for="(card, index) in filteredCards"
          :key="card.id"
          class="card-wrapper"
          :style="{ 'animation-delay': `${index * 0.1}s` }"
        >
          <div class="card-container-3d">
            <!-- Card Status Badge -->
            <div class="card-status-badge" :class="card.isActive ? 'status-active' : 'status-inactive'">
              <span class="status-icon">{{ card.isActive ? '✓' : '✗' }}</span>
              <span class="status-text">{{ card.isActive ? 'Active' : 'Inactive' }}</span>
            </div>
            
            <CreditCard3D
              :card-number="card.cardNumber"
              :cardholder-name="card.cardholderName"
              :expiration-date="card.expirationDate"
              :card-type="card.cardType"
            />
            <div class="card-actions">
              <button
                @click="editCard(card)"
                class="action-btn edit-btn"
                title="Edit"
              >
                <span>✏️</span>
                <span class="tooltip">Edit</span>
              </button>
              <button
                @click="viewCardDetails(card)"
                class="action-btn view-btn"
                title="View Details"
              >
                <span>👁️</span>
                <span class="tooltip">Details</span>
              </button>
              <button
                @click="viewCardExpenses(card.id)"
                class="action-btn expenses-btn"
                title="View Expenses"
              >
                <span>💰</span>
                <span class="tooltip">Expenses</span>
              </button>
              <button
                @click="openAddExpenseModal(card.id)"
                class="action-btn add-expense-btn"
                :class="{ 'disabled': !card.isActive }"
                :disabled="!card.isActive"
                :title="card.isActive ? 'Add Expense' : 'Carte inactive - Impossible d\'ajouter une transaction'"
              >
                <span>💸</span>
                <span class="tooltip">{{ card.isActive ? 'Add Expense' : 'Carte inactive' }}</span>
              </button>
              <button
                @click="deleteCard(card.id)"
                class="action-btn delete-btn"
                title="Delete"
              >
                <span>🗑️</span>
                <span class="tooltip">Delete</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="empty-state">
        <div class="empty-icon">💳</div>
        <h3 class="empty-title">No credit cards found</h3>
        <p class="empty-message">
          {{ searchQuery ? 'Try adjusting your search or filters' : 'Add your first credit card to get started' }}
        </p>
        <button
          v-if="!searchQuery"
          @click="showModal = true; editingCard = null"
          class="empty-action-btn"
        >
          + Add Your First Card
        </button>
      </div>
    </div>

    <!-- Modern Modal -->
    <div
      v-if="showModal"
      class="modal-overlay"
      @click.self="closeModal"
    >
      <div class="modal-container">
        <div class="modal-header">
          <h2 class="modal-title">
            {{ editingCard ? '✏️ Edit Credit Card' : '➕ Add New Credit Card' }}
          </h2>
          <button @click="closeModal" class="modal-close-btn">×</button>
        </div>

        <form @submit.prevent="handleSubmit" class="modal-form">
      <!-- Error Display -->
      <div v-if="submitError" class="form-error">
        <div class="flex items-start gap-3">
          <span class="error-icon">⚠️</span>
          <div class="flex-1">
            <div class="font-semibold text-red-800 mb-1">Erreur de validation</div>
            <div class="text-red-700 text-sm">{{ submitError }}</div>
            <div v-if="submitError.includes('card number') || submitError.includes('numéro de carte')" class="mt-2 text-xs text-red-600">
              💡 Astuce : Utilisez un numéro de carte de test valide comme <code class="bg-red-100 px-1 rounded">4111 1111 1111 1111</code> (Visa) ou <code class="bg-red-100 px-1 rounded">5555 5555 5555 4444</code> (MasterCard)
            </div>
            <div v-if="submitError.includes('expiration') || submitError.includes('expiration')" class="mt-2 text-xs text-red-600">
              💡 Le format doit être MM/YY (ex: 12/25 pour décembre 2025)
            </div>
          </div>
        </div>
      </div>

          <!-- Card Preview -->
          <div v-if="form.cardNumber" class="card-preview">
            <CreditCard3D
              :card-number="form.cardNumber"
              :cardholder-name="form.cardholderName || 'CARDHOLDER NAME'"
              :expiration-date="form.expirationDate || 'MM/YY'"
              :card-type="detectedCardType"
            />
          </div>

          <div class="form-grid">
            <div class="form-group full-width">
              <label class="form-label">
                <span class="label-icon">🔢</span>
                Card Number
              </label>
              <input
                v-model="form.cardNumber"
                type="text"
                placeholder="1234 5678 9012 3456"
                maxlength="19"
                @input="formatCardNumberInput"
                class="form-input"
                :class="{ 'input-error': cardNumberError }"
              />
              <p v-if="cardNumberError" class="error-text">{{ cardNumberError }}</p>
            </div>

            <div class="form-group full-width">
              <label class="form-label">
                <span class="label-icon">👤</span>
                Cardholder Name
              </label>
              <input
                v-model="form.cardholderName"
                type="text"
                placeholder="John Doe"
                required
                class="form-input"
              />
            </div>

            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">📅</span>
                Expiration Date
              </label>
              <input
                v-model="form.expirationDate"
                type="text"
                placeholder="MM/YY"
                maxlength="5"
                @input="formatExpirationDate"
                class="form-input"
                :class="{ 'input-error': expirationError }"
              />
              <p v-if="expirationError" class="error-text">{{ expirationError }}</p>
            </div>

            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">🔒</span>
                CVV
              </label>
              <input
                v-model="form.cvv"
                type="text"
                placeholder="123"
                maxlength="4"
                @input="form.cvv = form.cvv.replace(/\D/g, '')"
                class="form-input"
                :class="{ 'input-error': cvvError }"
              />
              <p v-if="cvvError" class="error-text">{{ cvvError }}</p>
            </div>

            <div v-if="form.cardNumber" class="form-group full-width">
              <div class="card-type-badge">
                <span>Card Type:</span>
                <span class="card-type-value">{{ detectedCardType }}</span>
              </div>
            </div>

            <div class="form-group full-width">
              <label class="form-label">
                <span class="label-icon">📂</span>
                Category
              </label>
              <select v-model="form.category" class="form-input">
                <option value="Personnel">Personnel</option>
                <option value="Travail">Travail</option>
                <option value="Abonnements">Abonnements</option>
                <option value="Voyage">Voyage</option>
              </select>
            </div>

            <div class="form-group full-width">
              <label class="form-label">
                <span class="label-icon">🏷️</span>
                Tags (séparés par des virgules)
              </label>
              <input
                v-model="form.tags"
                type="text"
                placeholder="ex: Netflix, Spotify, Amazon"
                class="form-input"
              />
            </div>

            <div class="form-group full-width">
              <label class="form-label">
                <span class="label-icon">💰</span>
                Montant / Balance (optionnel)
              </label>
              <input
                v-model="form.balance"
                type="number"
                step="0.01"
                min="0"
                placeholder="0.00"
                class="form-input"
              />
              <p class="form-hint">Le montant sera envoyé par WhatsApp après l'ajout de la carte</p>
            </div>

            <div class="form-group full-width">
              <label class="form-label">
                <span class="label-icon">🔐</span>
                Code de confirmation *
              </label>
              <input
                v-model="form.confirmationCode"
                type="text"
                placeholder="Code de sécurité pour les transactions (ex: 1234)"
                maxlength="10"
                class="form-input"
                required
              />
              <p class="form-hint">Ce code sera demandé lors de chaque transaction</p>
            </div>

            <div v-if="editingCard" class="form-group full-width">
              <label class="form-label">
                <span class="label-icon">🔒</span>
                État de la carte
              </label>
              <div class="toggle-container">
                <label class="toggle-switch">
                  <input
                    type="checkbox"
                    v-model="form.isActive"
                  />
                  <span class="toggle-slider"></span>
                </label>
                <span class="toggle-label">
                  {{ form.isActive ? 'Carte active' : 'Carte bloquée' }}
                </span>
              </div>
            </div>
          </div>

          <div class="modal-actions">
            <button
              type="button"
              @click="closeModal"
              class="btn-secondary"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="!isFormValid || submitting"
              class="btn-primary"
            >
              <span v-if="submitting" class="btn-spinner"></span>
              <span v-else>{{ editingCard ? 'Update Card' : 'Add Card' }}</span>
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Card Expenses View -->
    <CardExpensesView
      v-if="selectedCardForExpenses"
      :card-id="selectedCardForExpenses"
      @close="selectedCardForExpenses = null"
    />

    <!-- Add Expense Modal -->
    <AddExpenseModal
      v-if="selectedCardForExpense"
      :show="showAddExpenseModal"
      :card-id="selectedCardForExpense"
      @close="closeAddExpenseModal"
      @expense-added="handleExpenseAdded"
    />

    <!-- Card Details Modal -->
    <div
      v-if="selectedCard"
      class="modal-overlay"
      @click.self="selectedCard = null"
    >
      <div class="modal-container details-modal">
        <div class="modal-header">
          <h2 class="modal-title">Card Details</h2>
          <button @click="selectedCard = null" class="modal-close-btn">×</button>
        </div>
        <div class="details-content">
          <CreditCard3D
            :card-number="selectedCard.cardNumber"
            :cardholder-name="selectedCard.cardholderName"
            :expiration-date="selectedCard.expirationDate"
            :card-type="selectedCard.cardType"
          />
          <div class="details-info">
            <div class="detail-item">
              <span class="detail-label">Card Type</span>
              <span class="detail-value">{{ selectedCard.cardType }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">Created</span>
              <span class="detail-value">{{ formatDate(selectedCard.createdAt) }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">Last Updated</span>
              <span class="detail-value">{{ formatDate(selectedCard.updatedAt) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useCreditCardStore } from '../stores/creditCard'
import { useNotificationStore } from '../stores/notification'
import { validateCardNumber, validateExpirationDate, validateCVV, detectCardType, formatCardNumber } from '../utils/validation'
import CreditCard3D from '../components/CreditCard3D.vue'
import DashboardStats from '../components/DashboardStats.vue'
import CardFilters from '../components/CardFilters.vue'
import CardExpensesView from '../components/CardExpensesView.vue'
import AddExpenseModal from '../components/AddExpenseModal.vue'
import AdvancedDashboard from '../components/AdvancedDashboard.vue'
import ExpirationAlerts from '../components/ExpirationAlerts.vue'
import { format } from 'date-fns'

const creditCardStore = useCreditCardStore()
const notificationStore = useNotificationStore()

const showModal = ref(false)
const editingCard = ref(null)
const selectedCard = ref(null)
const selectedCardForExpenses = ref(null)
const showAddExpenseModal = ref(false)
const selectedCardForExpense = ref(null)
const submitting = ref(false)
const searchQuery = ref('')
const currentFilter = ref('all')
const sortBy = ref('newest')

const form = ref({
  cardNumber: '',
  cardholderName: '',
  expirationDate: '',
  cvv: '',
  category: 'Personnel',
  tags: '',
  balance: null,
  confirmationCode: '',
  isActive: true
})

const cardNumberError = ref('')
const expirationError = ref('')
const cvvError = ref('')
const submitError = ref('')

const detectedCardType = computed(() => {
  if (!form.value.cardNumber) return 'Unknown'
  return detectCardType(form.value.cardNumber)
})

const isFormValid = computed(() => {
  return (
    form.value.cardNumber &&
    form.value.cardholderName &&
    form.value.expirationDate &&
    form.value.cvv &&
    !cardNumberError.value &&
    !expirationError.value &&
    !cvvError.value
  )
})

const filteredCards = computed(() => {
  let cards = [...creditCardStore.cards]

  // Filter
  if (currentFilter.value !== 'all') {
    if (currentFilter.value === 'expired') {
      cards = cards.filter(c => {
        const [month, year] = c.expirationDate.split('/')
        const expDate = new Date(2000 + parseInt(year), parseInt(month) - 1)
        return expDate < new Date()
      })
    } else {
      cards = cards.filter(c => c.cardType === currentFilter.value)
    }
  }

  // Search
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    cards = cards.filter(c => 
      c.cardholderName.toLowerCase().includes(query) ||
      c.cardNumber.includes(query) ||
      c.cardType.toLowerCase().includes(query)
    )
  }

  // Sort
  switch (sortBy.value) {
    case 'oldest':
      cards.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt))
      break
    case 'name':
      cards.sort((a, b) => a.cardholderName.localeCompare(b.cardholderName))
      break
    case 'type':
      cards.sort((a, b) => a.cardType.localeCompare(b.cardType))
      break
    default: // newest
      cards.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
  }

  return cards
})

onMounted(async () => {
  await creditCardStore.fetchCards()
  
  // Écouter l'événement pour ouvrir le modal d'ajout de carte depuis la sidebar
  window.addEventListener('open-add-card-modal', () => {
    showModal.value = true
    editingCard.value = null
  })
  
  // Écouter l'événement pour scroller vers les transactions
  window.addEventListener('scroll-to-transactions', () => {
    const transactionsSection = document.querySelector('.dashboard-stats')
    if (transactionsSection) {
      transactionsSection.scrollIntoView({ behavior: 'smooth' })
    }
  })
  
  // Écouter l'événement pour scroller vers les analytics
  window.addEventListener('scroll-to-analytics', () => {
    const analyticsSection = document.querySelector('.advanced-dashboard')
    if (analyticsSection) {
      analyticsSection.scrollIntoView({ behavior: 'smooth' })
    }
  })
})

const handleSearch = (query) => {
  searchQuery.value = query
}

const handleFilter = (filter) => {
  currentFilter.value = filter
}

const handleSort = (sort) => {
  sortBy.value = sort
}

const formatCardNumberInput = (event) => {
  let value = event.target.value.replace(/\D/g, '')
  form.value.cardNumber = formatCardNumber(value)
  
  const validation = validateCardNumber(value)
  cardNumberError.value = validation.valid ? '' : validation.message
}

const formatExpirationDate = (event) => {
  let value = event.target.value.replace(/\D/g, '')
  if (value.length >= 2) {
    value = value.substring(0, 2) + '/' + value.substring(2, 4)
  }
  form.value.expirationDate = value
  
  const validation = validateExpirationDate(value)
  expirationError.value = validation.valid ? '' : validation.message
}

watch(() => form.value.cvv, (newValue) => {
  const validation = validateCVV(newValue)
  cvvError.value = validation.valid ? '' : validation.message
})

const editCard = (card) => {
  editingCard.value = card
  form.value = {
    cardNumber: card.cardNumber.replace(/\s/g, ''),
    cardholderName: card.cardholderName,
    expirationDate: card.expirationDate,
    cvv: '',
    category: card.category || 'Personnel',
    tags: card.tags || '',
    balance: card.balance || null,
    confirmationCode: card.confirmationCode || '',
    isActive: card.isActive !== undefined ? card.isActive : true
  }
  showModal.value = true
}

const viewCardDetails = (card) => {
  selectedCard.value = card
}

const viewCardExpenses = (cardId) => {
  selectedCardForExpenses.value = cardId
}

const openAddExpenseModal = (cardId) => {
  const card = creditCardStore.cards.find(c => c.id === cardId)
  if (!card) return
  
  if (!card.isActive) {
    alert('⚠️ Cette carte est inactive. Veuillez l\'activer dans les paramètres pour effectuer des transactions.')
    return
  }
  
  selectedCardForExpense.value = cardId
  showAddExpenseModal.value = true
}

const closeAddExpenseModal = () => {
  showAddExpenseModal.value = false
  selectedCardForExpense.value = null
}

const handleExpenseAdded = (expenseData) => {
  // Refresh cards to update balance if needed
  creditCardStore.fetchCards()
  
  // Ajouter une notification
  const card = creditCardStore.cards.find(c => c.id === selectedCardForExpense.value)
  if (card && expenseData) {
    const formatAmount = (amount, currency) => {
      return new Intl.NumberFormat('fr-FR', {
        style: 'currency',
        currency: currency || 'MAD'
      }).format(amount)
    }
    
    notificationStore.addNotification({
      type: 'success',
      icon: '💸',
      title: 'Dépense ajoutée',
      message: `Dépense de ${formatAmount(expenseData.amount, expenseData.currency)} ajoutée à la carte ${card.cardholderName}`
    })
  }
  
  // Optionally refresh expenses view if open
  if (selectedCardForExpenses.value) {
    // Trigger a refresh by closing and reopening
    const cardId = selectedCardForExpenses.value
    selectedCardForExpenses.value = null
    setTimeout(() => {
      selectedCardForExpenses.value = cardId
    }, 100)
  }
}

const closeModal = () => {
  showModal.value = false
  editingCard.value = null
  form.value = {
    cardNumber: '',
    cardholderName: '',
    expirationDate: '',
    cvv: '',
    category: 'Personnel',
    tags: '',
    balance: null,
    confirmationCode: '',
    isActive: true
  }
  cardNumberError.value = ''
  expirationError.value = ''
  cvvError.value = ''
  submitError.value = ''
}

const handleSubmit = async () => {
  if (!isFormValid.value) return

  submitting.value = true
  submitError.value = ''

  // Validation supplémentaire côté client
  if (!form.value.confirmationCode || form.value.confirmationCode.trim().length < 4) {
    submitError.value = 'Le code de confirmation est requis (minimum 4 caractères)'
    submitting.value = false
    return
  }

  const cardData = {
    cardNumber: form.value.cardNumber.replace(/\s/g, ''),
    cardholderName: form.value.cardholderName,
    expirationDate: form.value.expirationDate,
    cvv: form.value.cvv,
    category: form.value.category,
    tags: form.value.tags || undefined,
    balance: form.value.balance ? parseFloat(form.value.balance) : undefined,
    confirmationCode: form.value.confirmationCode.trim()
  }
  
  if (editingCard.value) {
    cardData.isActive = form.value.isActive
  }

  try {
    let result
    if (editingCard.value) {
      result = await creditCardStore.updateCard(editingCard.value.id, cardData)
    } else {
      result = await creditCardStore.createCard(cardData)
    }

    submitting.value = false

    if (result.success) {
    // Ajouter une notification
    if (editingCard.value) {
      notificationStore.addNotification({
        type: 'success',
        icon: '✏️',
        title: 'Carte modifiée',
        message: `La carte ${form.value.cardholderName} a été modifiée avec succès`
      })
    } else {
      notificationStore.addNotification({
        type: 'success',
        icon: '💳',
        title: 'Carte ajoutée',
        message: `La carte ${form.value.cardholderName} (${detectedCardType.value}) a été ajoutée avec succès`
      })
    }
    
    closeModal()
    await creditCardStore.fetchCards()
  } else {
    submitError.value = result.error || 'An error occurred'
    console.error('Credit card error:', result.error)
    
    // Notification d'erreur
    notificationStore.addNotification({
      type: 'error',
      icon: '⚠️',
      title: 'Erreur',
      message: result.error || 'Une erreur est survenue lors de l\'opération'
    })
    }
  } catch (error) {
    submitting.value = false
    console.error('Error submitting card:', error)
    console.error('Error response:', error.response?.data)
    
    // Extraire le message d'erreur détaillé
    let errorMessage = 'Une erreur est survenue lors de l\'opération'
    
    // Vérifier d'abord les erreurs de validation ModelState
    if (error.response?.data) {
      const data = error.response.data
      
      // Si c'est un objet ModelState avec des erreurs de validation
      if (typeof data === 'object' && !data.message && !data.error) {
        const errors = []
        for (const key in data) {
          if (Array.isArray(data[key])) {
            errors.push(...data[key])
          } else if (typeof data[key] === 'string') {
            errors.push(data[key])
          }
        }
        if (errors.length > 0) {
          errorMessage = errors.join(', ')
        }
      } else if (data.message) {
        errorMessage = data.message
      } else if (data.error) {
        errorMessage = data.error
      } else if (typeof data === 'string') {
        errorMessage = data
      }
    } else if (error.message) {
      errorMessage = error.message
    }
    
    submitError.value = errorMessage
    
    // Notification d'erreur
    notificationStore.addNotification({
      type: 'error',
      icon: '⚠️',
      title: 'Erreur lors de l\'ajout de la carte',
      message: errorMessage
    })
  }
}

const deleteCard = async (id) => {
  const card = creditCardStore.cards.find(c => c.id === id)
  if (!card) return
  
  if (!confirm('Êtes-vous sûr de vouloir supprimer cette carte bancaire ?')) {
    return
  }

  const result = await creditCardStore.deleteCard(id)
  if (result.success) {
    // Ajouter une notification
    notificationStore.addNotification({
      type: 'info',
      icon: '🗑️',
      title: 'Carte supprimée',
      message: `La carte ${card.cardholderName} a été supprimée`
    })
    
    await creditCardStore.fetchCards()
  } else {
    alert(result.error || 'Failed to delete credit card')
    
    // Notification d'erreur
    notificationStore.addNotification({
      type: 'error',
      icon: '⚠️',
      title: 'Erreur',
      message: result.error || 'Impossible de supprimer la carte'
    })
  }
}

const formatDate = (dateString) => {
  try {
    return format(new Date(dateString), 'MMM dd, yyyy')
  } catch {
    return dateString
  }
}
</script>

<style scoped>
/* Modern Dashboard Styles */
.modern-dashboard {
  padding: 0;
  min-height: 100vh;
}

.dashboard-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.dashboard-title {
  font-size: 2.5rem;
  font-weight: 800;
  color: white;
  margin: 0;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.dashboard-title {
  font-size: 2.5rem;
  font-weight: 800;
  color: white;
  margin: 0;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.dashboard-subtitle {
  color: rgba(255, 255, 255, 0.9);
  margin-top: 0.5rem;
  font-size: 1.1rem;
}

.page-header-section {
  @apply mb-6 px-4 sm:px-6 lg:px-8 pt-8;
}

.page-title {
  @apply text-3xl font-bold text-gray-900 mb-2;
}

.page-subtitle {
  @apply text-gray-600;
}

.add-card-header-btn {
  @apply flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-lg
         hover:bg-blue-700 transition-colors font-semibold shadow-md
         focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2;
}

.add-card-icon {
  @apply text-xl;
}

.add-card-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem 2rem;
  background: white;
  color: #667eea;
  border: none;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.add-card-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.2);
}

.btn-icon {
  font-size: 1.5rem;
  font-weight: 300;
}

.dashboard-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem;
}

.page-header-section {
  @apply mb-6;
}

.page-title {
  @apply text-3xl font-bold text-gray-900 mb-2;
}

.page-subtitle {
  @apply text-gray-600;
}

.add-card-header-btn {
  @apply flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-lg
         hover:bg-blue-700 transition-colors font-semibold shadow-md
         focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2;
}

.add-card-icon {
  @apply text-xl;
}

.cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 2rem;
  margin-top: 2rem;
}

.card-wrapper {
  animation: fadeInUp 0.6s ease-out forwards;
  opacity: 0;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.card-container-3d {
  position: relative;
  background: white;
  border-radius: 20px;
  padding: 1.5rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
  transition: all 0.3s;
}

.card-status-badge {
  position: absolute;
  top: 1rem;
  right: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
  z-index: 10;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.card-status-badge.status-active {
  background: #10b981;
  color: white;
}

.card-status-badge.status-inactive {
  background: #ef4444;
  color: white;
}

.status-icon {
  font-size: 1rem;
  font-weight: bold;
}

.status-text {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.card-container-3d:hover {
  transform: translateY(-8px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
}

.card-actions {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid #e5e7eb;
}

.action-btn {
  position: relative;
  padding: 0.75rem;
  background: #f3f4f6;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 1.2rem;
}

.action-btn.disabled {
  opacity: 0.5;
  cursor: not-allowed;
  pointer-events: none;
}

.action-btn:hover {
  transform: scale(1.1);
  background: #e5e7eb;
}

.action-btn.edit-btn:hover { background: #dbeafe; }
.action-btn.view-btn:hover { background: #f0fdf4; }
.action-btn.expenses-btn:hover { background: #fef3c7; }
.action-btn.add-expense-btn {
  background: #dbeafe;
  color: #1e40af;
}
.action-btn.add-expense-btn:hover { background: #bfdbfe; }
.action-btn.delete-btn:hover { background: #fee2e2; }

.tooltip {
  position: absolute;
  bottom: 100%;
  left: 50%;
  transform: translateX(-50%);
  background: #1f2937;
  color: white;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.75rem;
  white-space: nowrap;
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.3s;
  margin-bottom: 0.5rem;
}

.action-btn:hover .tooltip {
  opacity: 1;
}

/* Loading, Error, Empty States */
.loading-container,
.error-container,
.empty-state {
  text-align: center;
  padding: 4rem 2rem;
}

.spinner-3d {
  width: 60px;
  height: 60px;
  margin: 0 auto 1rem;
  border: 4px solid #e5e7eb;
  border-top-color: #667eea;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.loading-text {
  color: #6b7280;
  font-size: 1.1rem;
}

.error-icon,
.empty-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
}

.error-title,
.empty-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.error-message,
.empty-message {
  color: #6b7280;
  margin-bottom: 1.5rem;
}

.empty-action-btn {
  padding: 1rem 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.empty-action-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
  animation: fadeIn 0.3s;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.modal-container {
  background: white;
  border-radius: 24px;
  width: 100%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  animation: slideUp 0.3s;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 2rem;
  border-bottom: 1px solid #e5e7eb;
}

.modal-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1f2937;
}

.modal-close-btn {
  width: 40px;
  height: 40px;
  border: none;
  background: #f3f4f6;
  border-radius: 10px;
  font-size: 1.5rem;
  cursor: pointer;
  transition: all 0.3s;
  color: #6b7280;
}

.modal-close-btn:hover {
  background: #e5e7eb;
  color: #1f2937;
}

.modal-form {
  padding: 2rem;
}

.card-preview {
  margin-bottom: 2rem;
  display: flex;
  justify-content: center;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

.form-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
}

.label-icon {
  font-size: 1.2rem;
}

.form-input {
  padding: 0.875rem;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  font-size: 1rem;
  transition: all 0.3s;
}

.form-input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.form-input.input-error {
  border-color: #ef4444;
}

.error-text {
  color: #ef4444;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.form-error {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem;
  background: #fee2e2;
  border: 1px solid #fecaca;
  border-radius: 12px;
  color: #991b1b;
  margin-bottom: 1.5rem;
}

.error-icon {
  font-size: 1.2rem;
}

.card-type-badge {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 12px;
  font-weight: 600;
}

.card-type-value {
  font-size: 1.1rem;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 1px solid #e5e7eb;
}

.btn-secondary,
.btn-primary {
  padding: 0.875rem 2rem;
  border: none;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-secondary {
  background: #f3f4f6;
  color: #374151;
}

.btn-secondary:hover {
  background: #e5e7eb;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

.details-modal {
  max-width: 500px;
}

.details-content {
  padding: 2rem;
  text-align: center;
}

.details-info {
  margin-top: 2rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 12px;
}

.detail-label {
  font-weight: 600;
  color: #6b7280;
}

.detail-value {
  font-weight: 700;
  color: #1f2937;
}

/* Responsive */
@media (max-width: 768px) {
  .dashboard-title {
    font-size: 1.75rem;
  }

  .header-content {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  .cards-grid {
    grid-template-columns: 1fr;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
