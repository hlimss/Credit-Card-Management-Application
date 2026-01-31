<template>
  <div class="loan-view">
    <div class="loan-header">
      <h2>💰 Gestion des Prêts</h2>
      <button @click="showCreateModal = true" class="btn-create">
        ➕ Nouveau prêt
      </button>
    </div>

    <!-- Create Loan Modal -->
    <div v-if="showCreateModal" class="modal-overlay" @click.self="showCreateModal = false">
      <div class="modal-container">
        <div class="modal-header">
          <h3>Créer un prêt</h3>
          <button @click="showCreateModal = false" class="close-btn">✕</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="createLoan">
            <div class="form-group">
              <label>Type de prêt *</label>
              <select v-model="loanForm.loanType" required class="form-input">
                <option value="">-- Sélectionner --</option>
                <option value="Personal">Personnel</option>
                <option value="Mortgage">Hypothécaire</option>
                <option value="Auto">Automobile</option>
                <option value="Business">Commercial</option>
                <option value="Education">Éducation</option>
              </select>
            </div>
            <div class="form-group">
              <label>Nom du prêt *</label>
              <input v-model="loanForm.loanName" type="text" required class="form-input" placeholder="Ex: Prêt immobilier" />
            </div>
            <div class="form-group">
              <label>Montant principal (MAD) *</label>
              <input v-model.number="loanForm.principalAmount" type="number" step="0.01" min="0.01" required class="form-input" />
            </div>
            <div class="form-group">
              <label>Taux d'intérêt annuel (%) *</label>
              <input v-model.number="loanForm.interestRate" type="number" step="0.01" min="0" max="100" required class="form-input" />
            </div>
            <div class="form-group">
              <label>Durée (mois) *</label>
              <input v-model.number="loanForm.termMonths" type="number" min="1" required class="form-input" />
            </div>
            <div class="form-group">
              <label>Date de début</label>
              <input v-model="loanForm.startDate" type="date" class="form-input" />
            </div>
            <div class="form-group">
              <label>Description</label>
              <textarea v-model="loanForm.description" rows="3" class="form-input"></textarea>
            </div>
            <div class="form-actions">
              <button type="button" @click="showCreateModal = false" class="btn-cancel">Annuler</button>
              <button type="submit" :disabled="creating" class="btn-submit">
                {{ creating ? 'Création...' : 'Créer' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Loans List -->
    <div v-if="loading" class="loading">Chargement...</div>
    <div v-else-if="loans.length === 0" class="empty-state">
      <p>Aucun prêt enregistré</p>
    </div>
    <div v-else class="loans-grid">
      <div v-for="loan in loans" :key="loan.id" class="loan-card">
        <div class="loan-card-header">
          <h3>{{ loan.loanName }}</h3>
          <span :class="['status-badge', loan.status.toLowerCase()]">{{ loan.status }}</span>
        </div>
        <div class="loan-card-body">
          <div class="loan-info">
            <div class="info-item">
              <span class="label">Type:</span>
              <span class="value">{{ loan.loanType }}</span>
            </div>
            <div class="info-item">
              <span class="label">Montant principal:</span>
              <span class="value">{{ formatCurrency(loan.principalAmount) }}</span>
            </div>
            <div class="info-item">
              <span class="label">Montant restant:</span>
              <span class="value highlight">{{ formatCurrency(loan.remainingAmount) }}</span>
            </div>
            <div class="info-item">
              <span class="label">Taux d'intérêt:</span>
              <span class="value">{{ loan.interestRate }}%</span>
            </div>
            <div class="info-item">
              <span class="label">Paiement mensuel:</span>
              <span class="value">{{ formatCurrency(loan.monthlyPayment) }}</span>
            </div>
            <div class="info-item">
              <span class="label">Durée:</span>
              <span class="value">{{ loan.remainingMonths }} / {{ loan.termMonths }} mois</span>
            </div>
            <div class="info-item">
              <span class="label">Prochain paiement:</span>
              <span class="value">{{ formatDate(loan.nextPaymentDate) }}</span>
            </div>
          </div>
          <div class="progress-bar">
            <div class="progress-fill" :style="{ width: `${((loan.termMonths - loan.remainingMonths) / loan.termMonths) * 100}%` }"></div>
          </div>
        </div>
        <div class="loan-card-footer">
          <button @click.stop="showPaymentModal(loan)" class="btn-payment" :disabled="loan.status !== 'Active'">
            💳 Effectuer un paiement
          </button>
          <button @click.stop="deleteLoan(loan.id)" class="btn-delete">🗑️ Supprimer</button>
        </div>
      </div>
    </div>

    <!-- Payment Modal -->
    <div v-if="selectedLoan" class="modal-overlay" @click.self="selectedLoan = null">
      <div class="modal-container">
        <div class="modal-header">
          <h3>Effectuer un paiement</h3>
          <button @click="selectedLoan = null" class="close-btn">✕</button>
        </div>
        <div class="modal-body">
          <div class="payment-info">
            <p><strong>Prêt:</strong> {{ selectedLoan.loanName }}</p>
            <p><strong>Paiement mensuel suggéré:</strong> {{ formatCurrency(selectedLoan.monthlyPayment) }}</p>
            <p><strong>Montant restant:</strong> {{ formatCurrency(selectedLoan.remainingAmount) }}</p>
          </div>
          <form @submit.prevent="makePayment">
            <div class="form-group">
              <label>Montant (MAD) *</label>
              <input v-model.number="paymentAmount" type="number" step="0.01" min="0.01" :max="selectedLoan.remainingAmount" required class="form-input" />
            </div>
            <div class="form-actions">
              <button type="button" @click="selectedLoan = null" class="btn-cancel">Annuler</button>
              <button type="submit" :disabled="processing" class="btn-submit">
                {{ processing ? 'Traitement...' : 'Payer' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { loanService } from '../services/loanService'

const loans = ref([])
const loading = ref(false)
const creating = ref(false)
const processing = ref(false)
const showCreateModal = ref(false)
const selectedLoan = ref(null)
const paymentAmount = ref(0)

const loanForm = ref({
  loanType: '',
  loanName: '',
  principalAmount: 0,
  interestRate: 0,
  termMonths: 0,
  startDate: '',
  description: ''
})

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('fr-MA', { style: 'currency', currency: 'MAD' }).format(amount)
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('fr-FR')
}

const loadLoans = async () => {
  loading.value = true
  try {
    loans.value = await loanService.getAll()
  } catch (error) {
    console.error('Error loading loans:', error)
  } finally {
    loading.value = false
  }
}

const createLoan = async () => {
  creating.value = true
  try {
    await loanService.create(loanForm.value)
    showCreateModal.value = false
    loanForm.value = {
      loanType: '',
      loanName: '',
      principalAmount: 0,
      interestRate: 0,
      termMonths: 0,
      startDate: '',
      description: ''
    }
    await loadLoans()
    alert('✅ Prêt créé avec succès!')
  } catch (error) {
    alert('Erreur lors de la création du prêt: ' + (error.response?.data?.message || error.message))
  } finally {
    creating.value = false
  }
}

const showPaymentModal = (loan) => {
  selectedLoan.value = loan
  paymentAmount.value = loan.monthlyPayment
}

const makePayment = async () => {
  processing.value = true
  try {
    await loanService.makePayment(selectedLoan.value.id, paymentAmount.value)
    selectedLoan.value = null
    await loadLoans()
    alert('✅ Paiement effectué avec succès!')
  } catch (error) {
    alert('Erreur lors du paiement: ' + (error.response?.data?.message || error.message))
  } finally {
    processing.value = false
  }
}

const deleteLoan = async (id) => {
  if (!confirm('Êtes-vous sûr de vouloir supprimer ce prêt?')) {
    return
  }
  try {
    await loanService.delete(id)
    await loadLoans()
    alert('✅ Prêt supprimé avec succès!')
  } catch (error) {
    alert('Erreur lors de la suppression: ' + (error.response?.data?.message || error.message))
  }
}

onMounted(() => {
  loadLoans()
  const today = new Date()
  loanForm.value.startDate = today.toISOString().split('T')[0]
})
</script>

<style scoped>
.loan-view {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.loan-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.loan-header h2 {
  margin: 0;
  color: #333;
}

.btn-create {
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
}

.loans-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 1.5rem;
}

.loan-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.loan-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #e9ecef;
}

.loan-card-header h3 {
  margin: 0;
  color: #333;
}

.status-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: 600;
}

.status-badge.active {
  background: #10b981;
  color: white;
}

.status-badge.paid {
  background: #3b82f6;
  color: white;
}

.status-badge.defaulted {
  background: #ef4444;
  color: white;
}

.loan-info {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.info-item .label {
  color: #666;
  font-size: 0.9rem;
}

.info-item .value {
  font-weight: 600;
  color: #333;
}

.info-item .value.highlight {
  color: #667eea;
  font-size: 1.1rem;
}

.progress-bar {
  width: 100%;
  height: 8px;
  background: #e9ecef;
  border-radius: 4px;
  overflow: hidden;
  margin: 1rem 0;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  transition: width 0.3s;
}

.loan-card-footer {
  display: flex;
  gap: 0.75rem;
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid #e9ecef;
}

.btn-payment,
.btn-delete {
  flex: 1;
  padding: 0.75rem;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: opacity 0.2s;
}

.btn-payment {
  background: #10b981;
  color: white;
}

.btn-payment:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-delete {
  background: #ef4444;
  color: white;
}

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
}

.modal-container {
  background: white;
  border-radius: 16px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e9ecef;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
}

.modal-body {
  padding: 1.5rem;
}

.payment-info {
  background: #f3f4f6;
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
}

.payment-info p {
  margin: 0.5rem 0;
  color: #333;
}

.form-group {
  margin-bottom: 1.25rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #333;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e9ecef;
  border-radius: 8px;
  font-size: 1rem;
}

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
}

.btn-cancel,
.btn-submit {
  flex: 1;
  padding: 0.75rem;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
}

.btn-cancel {
  background: #e9ecef;
  color: #666;
}

.btn-submit {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.loading,
.empty-state {
  text-align: center;
  padding: 3rem;
  color: #666;
}
</style>
