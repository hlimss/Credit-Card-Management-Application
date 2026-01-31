<template>
  <div class="statement-view">
    <div class="statement-header">
      <h2>📄 Relevés Bancaires</h2>
      <button @click="showGenerateModal = true" class="btn-generate">
        ➕ Générer un relevé
      </button>
    </div>

    <!-- Generate Statement Modal -->
    <div v-if="showGenerateModal" class="modal-overlay" @click.self="showGenerateModal = false">
      <div class="modal-container">
        <div class="modal-header">
          <h3>Générer un relevé</h3>
          <button @click="showGenerateModal = false" class="close-btn">✕</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="generateStatement">
            <div class="form-group">
              <label>Carte *</label>
              <select v-model="generateForm.creditCardId" required class="form-input">
                <option value="">-- Sélectionner une carte --</option>
                <option v-for="card in cards" :key="card.id" :value="card.id">
                  {{ card.cardholderName }} - {{ card.cardType }}
                </option>
              </select>
            </div>
            <div class="form-group">
              <label>Type de relevé</label>
              <select v-model="generateForm.statementType" class="form-input">
                <option value="Monthly">Mensuel</option>
                <option value="Quarterly">Trimestriel</option>
                <option value="Annual">Annuel</option>
              </select>
            </div>
            <div class="form-group">
              <label>Date de début</label>
              <input v-model="generateForm.startDate" type="date" class="form-input" />
            </div>
            <div class="form-group">
              <label>Date de fin</label>
              <input v-model="generateForm.endDate" type="date" class="form-input" />
            </div>
            <div class="form-actions">
              <button type="button" @click="showGenerateModal = false" class="btn-cancel">Annuler</button>
              <button type="submit" :disabled="generating" class="btn-submit">
                {{ generating ? 'Génération...' : 'Générer' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Statements List -->
    <div v-if="loading" class="loading">Chargement...</div>
    <div v-else-if="statements.length === 0" class="empty-state">
      <p>Aucun relevé généré</p>
    </div>
    <div v-else class="statements-grid">
      <div v-for="statement in statements" :key="statement.id" class="statement-card" @click="viewStatement(statement)">
        <div class="statement-card-header">
          <h3>{{ statement.creditCardName }}</h3>
          <span class="statement-type">{{ statement.statementType }}</span>
        </div>
        <div class="statement-card-body">
          <div class="statement-period">
            <span>{{ formatDate(statement.startDate) }} - {{ formatDate(statement.endDate) }}</span>
          </div>
          <div class="statement-balances">
            <div class="balance-item">
              <span class="label">Solde d'ouverture:</span>
              <span class="value">{{ formatCurrency(statement.openingBalance) }}</span>
            </div>
            <div class="balance-item">
              <span class="label">Solde de clôture:</span>
              <span class="value">{{ formatCurrency(statement.closingBalance) }}</span>
            </div>
            <div class="balance-item">
              <span class="label">Crédits:</span>
              <span class="value positive">{{ formatCurrency(statement.totalCredits) }}</span>
            </div>
            <div class="balance-item">
              <span class="label">Débits:</span>
              <span class="value negative">{{ formatCurrency(statement.totalDebits) }}</span>
            </div>
            <div class="balance-item">
              <span class="label">Transactions:</span>
              <span class="value">{{ statement.transactionCount }}</span>
            </div>
          </div>
        </div>
        <div class="statement-card-footer">
          <span class="generated-date">Généré le {{ formatDate(statement.generatedAt) }}</span>
        </div>
      </div>
    </div>

    <!-- Statement Detail Modal -->
    <div v-if="selectedStatement" class="modal-overlay" @click.self="selectedStatement = null">
      <div class="modal-container large">
        <div class="modal-header">
          <h3>Détails du relevé</h3>
          <button @click="selectedStatement = null" class="close-btn">✕</button>
        </div>
        <div class="modal-body">
          <div class="statement-detail">
            <h4>{{ selectedStatement.creditCardName }}</h4>
            <p class="period">{{ formatDate(selectedStatement.startDate) }} - {{ formatDate(selectedStatement.endDate) }}</p>
            <div class="detail-grid">
              <div class="detail-item">
                <span class="label">Solde d'ouverture:</span>
                <span class="value">{{ formatCurrency(selectedStatement.openingBalance) }}</span>
              </div>
              <div class="detail-item">
                <span class="label">Solde de clôture:</span>
                <span class="value">{{ formatCurrency(selectedStatement.closingBalance) }}</span>
              </div>
              <div class="detail-item">
                <span class="label">Total crédits:</span>
                <span class="value positive">{{ formatCurrency(selectedStatement.totalCredits) }}</span>
              </div>
              <div class="detail-item">
                <span class="label">Total débits:</span>
                <span class="value negative">{{ formatCurrency(selectedStatement.totalDebits) }}</span>
              </div>
              <div class="detail-item">
                <span class="label">Nombre de transactions:</span>
                <span class="value">{{ selectedStatement.transactionCount }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { statementService } from '../services/statementService'
import { useCreditCardStore } from '../stores/creditCard'

const cards = computed(() => useCreditCardStore().cards.filter(c => c.isActive))
const statements = ref([])
const loading = ref(false)
const generating = ref(false)
const showGenerateModal = ref(false)
const selectedStatement = ref(null)

const generateForm = ref({
  creditCardId: '',
  statementType: 'Monthly',
  startDate: '',
  endDate: ''
})

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('fr-MA', { style: 'currency', currency: 'MAD' }).format(amount)
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('fr-FR')
}

const loadStatements = async () => {
  loading.value = true
  try {
    statements.value = await statementService.getAll()
  } catch (error) {
    console.error('Error loading statements:', error)
  } finally {
    loading.value = false
  }
}

const generateStatement = async () => {
  generating.value = true
  try {
    await statementService.generate(generateForm.value)
    showGenerateModal.value = false
    await loadStatements()
    alert('✅ Relevé généré avec succès!')
  } catch (error) {
    alert('Erreur lors de la génération du relevé: ' + (error.response?.data?.message || error.message))
  } finally {
    generating.value = false
  }
}

const viewStatement = (statement) => {
  selectedStatement.value = statement
}

onMounted(() => {
  loadStatements()
  // Set default dates
  const endDate = new Date()
  const startDate = new Date()
  startDate.setMonth(startDate.getMonth() - 1)
  generateForm.value.endDate = endDate.toISOString().split('T')[0]
  generateForm.value.startDate = startDate.toISOString().split('T')[0]
})
</script>

<script>
import { computed } from 'vue'
</script>

<style scoped>
.statement-view {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.statement-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.statement-header h2 {
  margin: 0;
  color: #333;
}

.btn-generate {
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s;
}

.btn-generate:hover {
  transform: translateY(-2px);
}

.statements-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
}

.statement-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.statement-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.statement-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #e9ecef;
}

.statement-card-header h3 {
  margin: 0;
  font-size: 1.1rem;
  color: #333;
}

.statement-type {
  background: #667eea;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.85rem;
}

.statement-period {
  color: #666;
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

.statement-balances {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.balance-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.balance-item .label {
  color: #666;
  font-size: 0.9rem;
}

.balance-item .value {
  font-weight: 600;
  color: #333;
}

.balance-item .value.positive {
  color: #10b981;
}

.balance-item .value.negative {
  color: #ef4444;
}

.statement-card-footer {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid #e9ecef;
  font-size: 0.85rem;
  color: #999;
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

.modal-container.large {
  max-width: 700px;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e9ecef;
}

.modal-header h3 {
  margin: 0;
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

.statement-detail h4 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.period {
  color: #666;
  margin-bottom: 1.5rem;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.detail-item .label {
  color: #666;
  font-size: 0.9rem;
}

.detail-item .value {
  font-weight: 600;
  font-size: 1.1rem;
  color: #333;
}

.loading,
.empty-state {
  text-align: center;
  padding: 3rem;
  color: #666;
}
</style>
