<template>
  <div v-if="show" class="modal-overlay" @click.self="close">
    <div class="modal-container">
      <div class="modal-header">
        <h2 class="modal-title">💸 Effectuer un virement</h2>
        <button @click="close" class="close-btn">✕</button>
      </div>

      <div class="modal-body">
        <form @submit.prevent="handleSubmit" class="transfer-form">
          <div class="form-group">
            <label>Compte source *</label>
            <select v-model="form.fromAccount" required class="form-input">
              <option value="">-- Sélectionner une carte --</option>
              <option 
                v-for="card in cards" 
                :key="card.id" 
                :value="card.id"
              >
                {{ card.cardholderName }} - {{ card.cardType }} (Solde: {{ formatCurrency(card.balance || 0) }})
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Compte bénéficiaire *</label>
            <input 
              v-model="form.toAccount" 
              type="text" 
              required 
              placeholder="Numéro de compte ou IBAN"
              class="form-input"
            />
          </div>

          <div class="form-group">
            <label>Nom du bénéficiaire *</label>
            <input 
              v-model="form.beneficiaryName" 
              type="text" 
              required 
              placeholder="Nom complet du bénéficiaire"
              class="form-input"
            />
          </div>

          <div class="form-group">
            <label>Montant (MAD) *</label>
            <input 
              v-model.number="form.amount" 
              type="number" 
              step="0.01"
              min="0.01"
              required 
              placeholder="0.00"
              class="form-input"
            />
          </div>

          <div class="form-group">
            <label>Type de virement</label>
            <select v-model="form.transferType" class="form-input">
              <option value="Internal">Interne (même banque)</option>
              <option value="External">Externe (autre banque)</option>
              <option value="International">International</option>
            </select>
          </div>

          <div class="form-group">
            <label>Description (optionnel)</label>
            <textarea 
              v-model="form.description" 
              rows="3"
              placeholder="Description du virement"
              class="form-input"
            ></textarea>
          </div>

          <div v-if="error" class="error-message">
            {{ error }}
          </div>

          <div class="form-actions">
            <button type="button" @click="close" class="btn-cancel">Annuler</button>
            <button type="submit" :disabled="submitting" class="btn-submit">
              <span v-if="submitting">Traitement...</span>
              <span v-else>Effectuer le virement</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { bankTransferService } from '../services/bankTransferService'
import { useCreditCardStore } from '../stores/creditCard'

const props = defineProps({
  show: Boolean
})

const emit = defineEmits(['close', 'success'])

const creditCardStore = useCreditCardStore()

const form = ref({
  fromAccount: '',
  toAccount: '',
  beneficiaryName: '',
  amount: 0,
  currency: 'MAD',
  transferType: 'Internal',
  description: ''
})

const submitting = ref(false)
const error = ref('')

const cards = computed(() => {
  return creditCardStore.cards.filter(c => c.isActive && (c.balance == null || c.balance > 0))
})

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('fr-MA', { 
    style: 'currency', 
    currency: 'MAD' 
  }).format(amount)
}

watch(() => props.show, (newVal) => {
  if (newVal) {
    form.value = {
      fromAccount: '',
      toAccount: '',
      beneficiaryName: '',
      amount: 0,
      currency: 'MAD',
      transferType: 'Internal',
      description: ''
    }
    error.value = ''
  }
})

const close = () => {
  emit('close')
}

const handleSubmit = async () => {
  if (!form.value.fromAccount || !form.value.toAccount || !form.value.beneficiaryName || form.value.amount <= 0) {
    error.value = 'Veuillez remplir tous les champs obligatoires'
    return
  }

  submitting.value = true
  error.value = ''

  try {
    await bankTransferService.create(form.value)
    emit('success', {
      amount: form.amount,
      currency: form.currency,
      beneficiaryName: form.beneficiaryName,
      toAccount: form.toAccount,
      transferType: form.transferType
    })
    close()
  } catch (err) {
    error.value = err.response?.data?.message || 'Erreur lors du virement'
    console.error('Transfer error:', err)
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  animation: fadeIn 0.3s;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.modal-container {
  background: white;
  border-radius: 16px;
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  animation: slideUp 0.3s;
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
  padding: 1.5rem;
  border-bottom: 1px solid #e9ecef;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 16px 16px 0 0;
}

.modal-title {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
}

.close-btn {
  background: rgba(255, 255, 255, 0.2);
  border: none;
  color: white;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  cursor: pointer;
  font-size: 1.2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: rotate(90deg);
}

.modal-body {
  padding: 1.5rem;
}

.transfer-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 600;
  color: #333;
  font-size: 0.95rem;
}

.form-input {
  padding: 0.75rem;
  border: 2px solid #e9ecef;
  border-radius: 8px;
  font-size: 1rem;
  transition: all 0.3s;
}

.form-input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.error-message {
  background: #fee;
  color: #c33;
  padding: 0.75rem;
  border-radius: 8px;
  border: 1px solid #fcc;
}

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
}

.btn-cancel,
.btn-submit {
  flex: 1;
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-cancel {
  background: #e9ecef;
  color: #666;
}

.btn-cancel:hover {
  background: #dee2e6;
}

.btn-submit {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-submit:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.btn-submit:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
