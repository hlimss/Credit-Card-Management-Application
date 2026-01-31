<template>
  <div v-if="show" class="modal-overlay" @click.self="close">
    <div class="modal-container-3d">
      <div class="modal-header">
        <h2 class="modal-title">
          <span class="title-icon">💸</span>
          Add Expense
        </h2>
        <button @click="close" class="close-btn">✕</button>
      </div>

      <form @submit.prevent="handleSubmit" class="expense-form">
        <!-- Error Display -->
        <div v-if="error" class="form-error-3d">
          <span class="error-icon">⚠️</span>
          <span>{{ error }}</span>
        </div>

        <!-- Card Info Display -->
        <div class="card-info-display">
          <div class="card-info-item">
            <span class="info-label">Card:</span>
            <span class="info-value">{{ selectedCard?.cardholderName }}</span>
          </div>
          <div class="card-info-item">
            <span class="info-label">Type:</span>
            <span class="info-value">{{ selectedCard?.cardType }}</span>
          </div>
          <div class="card-info-item" v-if="selectedCard?.balance !== null && selectedCard?.balance !== undefined">
            <span class="info-label">Balance:</span>
            <span class="info-value">{{ formatCurrency(selectedCard.balance) }}</span>
          </div>
        </div>

        <!-- Merchant Name -->
        <div class="form-group">
          <label class="form-label">
            <span class="label-icon">🏪</span>
            Merchant Name *
          </label>
          <input
            v-model="formData.merchantName"
            type="text"
            class="form-input-3d"
            placeholder="e.g., Amazon, Starbucks, Gas Station"
            required
            maxlength="200"
          />
        </div>

        <!-- Amount -->
        <div class="form-group">
          <label class="form-label">
            <span class="label-icon">💰</span>
            Amount *
          </label>
          <input
            v-model.number="formData.amount"
            type="number"
            step="0.01"
            min="0.01"
            max="999999.99"
            class="form-input-3d"
            placeholder="0.00"
            required
          />
        </div>

        <!-- Category -->
        <div class="form-group">
          <label class="form-label">
            <span class="label-icon">📂</span>
            Category *
          </label>
          <select v-model="formData.category" class="form-input-3d" required>
            <option value="Food">🍔 Food</option>
            <option value="Transport">🚗 Transport</option>
            <option value="Shopping">🛍️ Shopping</option>
            <option value="Entertainment">🎬 Entertainment</option>
            <option value="Bills">💳 Bills</option>
            <option value="Healthcare">🏥 Healthcare</option>
            <option value="Travel">✈️ Travel</option>
            <option value="Education">📚 Education</option>
            <option value="Other">📦 Other</option>
          </select>
        </div>

        <!-- Description -->
        <div class="form-group">
          <label class="form-label">
            <span class="label-icon">📝</span>
            Description (Optional)
          </label>
          <textarea
            v-model="formData.description"
            class="form-input-3d"
            rows="3"
            placeholder="Add any additional details..."
            maxlength="500"
          ></textarea>
        </div>

        <!-- Transaction Date -->
        <div class="form-group">
          <label class="form-label">
            <span class="label-icon">📅</span>
            Transaction Date *
          </label>
          <input
            v-model="formData.transactionDate"
            type="datetime-local"
            class="form-input-3d"
            required
          />
        </div>

        <!-- Location (Optional) -->
        <div class="form-group">
          <label class="form-label">
            <span class="label-icon">📍</span>
            Location (Optional)
          </label>
          <input
            v-model="formData.location"
            type="text"
            class="form-input-3d"
            placeholder="e.g., New York, NY"
            maxlength="100"
          />
        </div>

        <!-- Currency -->
        <div class="form-group">
          <label class="form-label">
            <span class="label-icon">💵</span>
            Currency *
          </label>
          <select v-model="formData.currency" class="form-input-3d" required>
            <option value="USD">USD ($)</option>
            <option value="EUR">EUR (€)</option>
            <option value="MAD">MAD (د.م)</option>
            <option value="GBP">GBP (£)</option>
            <option value="JPY">JPY (¥)</option>
          </select>
        </div>

        <!-- Action Buttons -->
        <div class="form-actions">
          <button type="button" @click="close" class="btn-cancel">
            Cancel
          </button>
          <button type="submit" :disabled="loading" class="btn-submit-3d">
            <span v-if="loading" class="btn-spinner"></span>
            <span v-else>💸 Add Expense</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import transactionService from '../services/transactionService'
import { useCreditCardStore } from '../stores/creditCard'

const props = defineProps({
  show: {
    type: Boolean,
    default: false
  },
  cardId: {
    type: String,
    required: true
  }
})

const emit = defineEmits(['close', 'expense-added'])

const creditCardStore = useCreditCardStore()
const loading = ref(false)
const error = ref('')

const selectedCard = computed(() => {
  return creditCardStore.cards.find(c => c.id === props.cardId)
})

const formData = ref({
  merchantName: '',
  amount: '',
  category: 'Other',
  description: '',
  transactionDate: new Date().toISOString().slice(0, 16),
  location: '',
  currency: 'USD'
})

watch(() => props.show, (newVal) => {
  if (newVal) {
    // Reset form when modal opens
    formData.value = {
      merchantName: '',
      amount: '',
      category: 'Other',
      description: '',
      transactionDate: new Date().toISOString().slice(0, 16),
      location: '',
      currency: 'USD'
    }
    error.value = ''
  }
})

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(amount)
}

const handleSubmit = async () => {
  error.value = ''
  
  if (!formData.value.merchantName || !formData.value.amount || formData.value.amount <= 0) {
    error.value = 'Please fill in all required fields'
    return
  }

  loading.value = true

  try {
    const transactionData = {
      creditCardId: props.cardId,
      merchantName: formData.value.merchantName,
      amount: parseFloat(formData.value.amount),
      category: formData.value.category,
      description: formData.value.description || null,
      transactionDate: new Date(formData.value.transactionDate).toISOString(),
      location: formData.value.location || null,
      currency: formData.value.currency,
      transactionType: 'Expense'
    }

    await transactionService.createTransaction(transactionData)
    
    emit('expense-added')
    close()
  } catch (err) {
    console.error('Error creating expense:', err)
    error.value = err.response?.data?.message || 'Failed to add expense. Please try again.'
  } finally {
    loading.value = false
  }
}

const close = () => {
  emit('close')
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(10px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
  padding: 1rem;
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.modal-container-3d {
  background: rgba(255, 255, 255, 0.98);
  backdrop-filter: blur(20px);
  border-radius: 24px;
  padding: 2rem;
  max-width: 600px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 
    0 25px 50px rgba(0, 0, 0, 0.25),
    0 0 0 1px rgba(255, 255, 255, 0.5) inset;
  transform-style: preserve-3d;
  animation: slideUp 0.3s ease-out;
}

@keyframes slideUp {
  from {
    transform: translateY(50px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #e5e7eb;
}

.modal-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 1.5rem;
  font-weight: 800;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.title-icon {
  font-size: 1.5rem;
  -webkit-text-fill-color: initial;
}

.close-btn {
  background: #f3f4f6;
  border: none;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  cursor: pointer;
  font-size: 1.25rem;
  color: #6b7280;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-btn:hover {
  background: #e5e7eb;
  transform: rotate(90deg);
}

.card-info-display {
  background: linear-gradient(135deg, #f0f9ff 0%, #e0e7ff 100%);
  border-radius: 12px;
  padding: 1rem;
  margin-bottom: 1.5rem;
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
}

.card-info-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.info-label {
  font-size: 0.75rem;
  color: #6b7280;
  font-weight: 600;
}

.info-value {
  font-size: 0.875rem;
  color: #1f2937;
  font-weight: 700;
}

.expense-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 600;
  color: #374151;
  font-size: 0.875rem;
}

.label-icon {
  font-size: 1rem;
}

.form-input-3d {
  padding: 0.75rem 1rem;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  font-size: 1rem;
  transition: all 0.3s;
  background: white;
}

.form-input-3d:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
  transform: translateY(-2px);
}

.form-input-3d textarea {
  resize: vertical;
  min-height: 80px;
}

.form-error-3d {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.1) 0%, rgba(220, 38, 38, 0.1) 100%);
  border: 2px solid rgba(239, 68, 68, 0.3);
  border-radius: 12px;
  color: #991b1b;
  font-weight: 500;
}

.error-icon {
  font-size: 1.25rem;
}

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
}

.btn-cancel {
  flex: 1;
  padding: 0.875rem 2rem;
  background: #f3f4f6;
  border: none;
  border-radius: 12px;
  font-weight: 600;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-cancel:hover {
  background: #e5e7eb;
  transform: translateY(-2px);
}

.btn-submit-3d {
  flex: 2;
  padding: 0.875rem 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  border-radius: 12px;
  font-weight: 700;
  color: white;
  cursor: pointer;
  transition: all 0.3s;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.btn-submit-3d:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.6);
}

.btn-submit-3d:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

@media (max-width: 640px) {
  .modal-container-3d {
    padding: 1.5rem;
  }

  .form-actions {
    flex-direction: column;
  }
}
</style>
