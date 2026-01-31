<template>
  <div v-if="show" class="modal-overlay" @click.self="close">
    <div class="modal-container">
      <div class="modal-header">
        <h2 class="modal-title">💳 Effectuer un paiement</h2>
        <button @click="close" class="close-btn">✕</button>
      </div>

      <div class="modal-body">
        <form @submit.prevent="handleSubmit" class="payment-form">
          <div class="form-group">
            <label>Type de paiement *</label>
            <select v-model="form.paymentType" required @change="onPaymentTypeChange" class="form-input">
              <option value="">-- Sélectionner un type --</option>
              <option v-for="type in paymentTypes" :key="type" :value="type">
                {{ type }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label>Payer avec *</label>
            <select v-model="form.creditCardId" required class="form-input">
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
            <label>Fournisseur / Marchand *</label>
            <input 
              v-model="form.merchantName" 
              type="text" 
              required 
              :placeholder="merchantPlaceholder"
              class="form-input"
            />
          </div>

          <div class="form-group">
            <label>Numéro de référence *</label>
            <input 
              v-model="form.referenceNumber" 
              type="text" 
              required 
              placeholder="Numéro de facture, abonnement, etc."
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
            <label>Description (optionnel)</label>
            <textarea 
              v-model="form.description" 
              rows="3"
              placeholder="Description du paiement"
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
              <span v-else>Effectuer le paiement</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { paymentService } from '../services/paymentService'
import { useCreditCardStore } from '../stores/creditCard'

const props = defineProps({
  show: Boolean
})

const emit = defineEmits(['close', 'success'])

const creditCardStore = useCreditCardStore()

const form = ref({
  paymentType: '',
  creditCardId: '',
  merchantName: '',
  referenceNumber: '',
  amount: 0,
  currency: 'MAD',
  description: ''
})

const paymentTypes = ref([])
const submitting = ref(false)
const error = ref('')

const cards = computed(() => creditCardStore.cards.filter(c => c.isActive))

const merchantPlaceholder = computed(() => {
  const placeholders = {
    'Vignette': 'Ex: Autoroute, Parking',
    'Abonnement Téléphonique': 'Ex: IAM, Orange, Inwi',
    'Facture Électricité': 'Ex: ONE, AMENDIS',
    'Facture Eau': 'Ex: RADEEF, LYDEC',
    'Facture Internet': 'Ex: IAM, Orange, Inwi',
    'Facture TV': 'Ex: Maroc Telecom',
    'Abonnement Streaming': 'Ex: Netflix, Spotify, YouTube Premium',
    'Assurance': 'Ex: AXA, Wafa Assurance',
    'Impôts': 'Ex: Administration fiscale'
  }
  return placeholders[form.value.paymentType] || 'Nom du fournisseur'
})

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('fr-MA', { 
    style: 'currency', 
    currency: 'MAD' 
  }).format(amount)
}

onMounted(async () => {
  try {
    paymentTypes.value = await paymentService.getPaymentTypes()
  } catch (err) {
    console.error('Error loading payment types:', err)
    // Fallback
    paymentTypes.value = [
      'Vignette',
      'Abonnement Téléphonique',
      'Facture Électricité',
      'Facture Eau',
      'Facture Internet',
      'Facture TV',
      'Abonnement Streaming',
      'Assurance',
      'Impôts',
      'Autre'
    ]
  }
})

watch(() => props.show, (newVal) => {
  if (newVal) {
    form.value = {
      paymentType: '',
      creditCardId: '',
      merchantName: '',
      referenceNumber: '',
      amount: 0,
      currency: 'MAD',
      description: ''
    }
    error.value = ''
  }
})

const onPaymentTypeChange = () => {
  // Auto-remplir le marchand selon le type
  const autoMerchants = {
    'Vignette': 'Autoroute du Maroc',
    'Abonnement Téléphonique': 'IAM',
    'Facture Électricité': 'ONE',
    'Facture Eau': 'RADEEF',
    'Facture Internet': 'IAM',
    'Facture TV': 'Maroc Telecom',
    'Abonnement Streaming': 'Netflix',
    'Assurance': 'AXA Assurance'
  }
  
  if (autoMerchants[form.value.paymentType]) {
    form.value.merchantName = autoMerchants[form.value.paymentType]
  }
}

const close = () => {
  emit('close')
}

const handleSubmit = async () => {
  if (!form.value.paymentType || !form.value.creditCardId || !form.value.merchantName || 
      !form.value.referenceNumber || form.value.amount <= 0) {
    error.value = 'Veuillez remplir tous les champs obligatoires'
    return
  }

  submitting.value = true
  error.value = ''

  try {
    const paymentData = {
      ...form.value,
      creditCardId: form.value.creditCardId ? form.value.creditCardId : null
    }
    await paymentService.create(paymentData)
    emit('success')
    close()
  } catch (err) {
    error.value = err.response?.data?.message || 'Erreur lors du paiement'
    console.error('Payment error:', err)
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

.payment-form {
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
