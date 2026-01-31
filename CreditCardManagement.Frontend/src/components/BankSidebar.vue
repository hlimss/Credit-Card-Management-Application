<template>
  <div class="sidebar-container">
    <!-- Toggle Button -->
    <button 
      @click="toggleSidebar" 
      class="sidebar-toggle"
      :class="{ 'open': isOpen }"
    >
      <span v-if="!isOpen">☰</span>
      <span v-else>✕</span>
    </button>

    <!-- Sidebar -->
    <aside 
      class="bank-sidebar"
      :class="{ 'open': isOpen }"
    >
      <div class="sidebar-header">
        <h2 class="sidebar-title">🏦 Services Bancaires</h2>
      </div>

      <div class="sidebar-content">
        <!-- Bank Selection -->
        <div class="sidebar-section">
          <h3 class="section-title">Sélectionner une banque</h3>
          <select 
            v-model="selectedBank" 
            @change="onBankChange"
            class="bank-select"
          >
            <option value="">-- Choisir une banque --</option>
            <option v-for="bank in banks" :key="bank.id" :value="bank.id">
              {{ bank.name }}
            </option>
          </select>
        </div>

        <!-- Card Packs -->
        <div v-if="selectedBank" class="sidebar-section">
          <h3 class="section-title">📦 Packs de cartes disponibles</h3>
          <div class="card-packs">
            <div 
              v-for="pack in availablePacks" 
              :key="pack.id"
              class="card-pack"
              :class="{ 'selected': selectedPack?.id === pack.id }"
              @click="selectPack(pack)"
            >
              <div class="pack-header">
                <span class="pack-icon">{{ pack.icon }}</span>
                <h4 class="pack-name">{{ pack.name }}</h4>
              </div>
              <p class="pack-description">{{ pack.description }}</p>
              <div class="pack-features">
                <span 
                  v-for="feature in pack.features" 
                  :key="feature"
                  class="feature-badge"
                >
                  {{ feature }}
                </span>
              </div>
              <div class="pack-price">
                <span class="price-label">Prix:</span>
                <span class="price-value">{{ pack.price }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Bank Features -->
        <div class="sidebar-section">
          <h3 class="section-title">⚙️ Fonctionnalités</h3>
          <ul class="features-list">
            <li 
              v-for="feature in bankFeatures" 
              :key="feature.id"
              class="feature-item"
              @click="handleFeatureClick(feature)"
            >
              <span class="feature-icon">{{ feature.icon }}</span>
              <span class="feature-text">{{ feature.name }}</span>
            </li>
          </ul>
        </div>

        <!-- Quick Actions -->
        <div class="sidebar-section">
          <h3 class="section-title">⚡ Actions rapides</h3>
          <div class="quick-actions">
            <button 
              v-for="action in quickActions" 
              :key="action.id"
              @click="handleQuickAction(action)"
              class="quick-action-btn"
            >
              <span class="action-icon">{{ action.icon }}</span>
              <span class="action-text">{{ action.name }}</span>
            </button>
          </div>
        </div>
      </div>
    </aside>

    <!-- Overlay when sidebar is open -->
    <div 
      v-if="isOpen" 
      class="sidebar-overlay"
      @click="closeSidebar"
    ></div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const isOpen = ref(false)
const selectedBank = ref('')
const selectedPack = ref(null)

const banks = ref([
  { id: 'attijari', name: 'Attijariwafa Bank', icon: '🏦' },
  { id: 'bmce', name: 'BMCE Bank', icon: '🏦' },
  { id: 'cih', name: 'CIH Bank', icon: '🏦' },
  { id: 'bmci', name: 'BMCI', icon: '🏦' },
  { id: 'credit', name: 'Crédit du Maroc', icon: '🏦' },
  { id: 'sgmb', name: 'SGMB', icon: '🏦' },
  { id: 'banque', name: 'Banque Populaire', icon: '🏦' }
])

// Ajouter des packs pour les autres banques aussi
const cardPacks = ref({
  attijari: [
    {
      id: 'attijari-premium',
      name: 'Pack Premium',
      icon: '💎',
      description: 'Carte premium avec avantages exclusifs',
      features: ['Cashback 2%', 'Assurance voyage', 'Conciergerie'],
      price: 'Gratuit'
    },
    {
      id: 'attijari-classic',
      name: 'Pack Classic',
      icon: '💳',
      description: 'Carte classique pour usage quotidien',
      features: ['Cashback 1%', 'Assurance achat'],
      price: 'Gratuit'
    },
    {
      id: 'attijari-business',
      name: 'Pack Business',
      icon: '💼',
      description: 'Carte professionnelle avec avantages entreprise',
      features: ['Frais réduits', 'Support prioritaire', 'Rapports détaillés'],
      price: '500 MAD/an'
    }
  ],
  bmce: [
    {
      id: 'bmce-gold',
      name: 'Pack Gold',
      icon: '🥇',
      description: 'Carte Gold avec privilèges',
      features: ['Cashback 1.5%', 'Lounge aéroport', 'Assurance'],
      price: 'Gratuit'
    },
    {
      id: 'bmce-platinum',
      name: 'Pack Platinum',
      icon: '💎',
      description: 'Carte Platinum premium',
      features: ['Cashback 3%', 'Voyage', 'Conciergerie 24/7'],
      price: '1200 MAD/an'
    }
  ],
  cih: [
    {
      id: 'cih-standard',
      name: 'Pack Standard',
      icon: '💳',
      description: 'Carte standard CIH',
      features: ['Cashback 0.5%', 'Assurance'],
      price: 'Gratuit'
    },
    {
      id: 'cih-premium',
      name: 'Pack Premium',
      icon: '⭐',
      description: 'Carte premium CIH',
      features: ['Cashback 2%', 'Assurance étendue', 'Support VIP'],
      price: '800 MAD/an'
    }
  ],
  bmci: [
    {
      id: 'bmci-classic',
      name: 'Pack Classic',
      icon: '💳',
      description: 'Carte classique BMCI',
      features: ['Cashback 1%', 'Assurance'],
      price: 'Gratuit'
    }
  ],
  credit: [
    {
      id: 'credit-standard',
      name: 'Pack Standard',
      icon: '💳',
      description: 'Carte standard Crédit du Maroc',
      features: ['Cashback 0.5%', 'Assurance'],
      price: 'Gratuit'
    }
  ],
  sgmb: [
    {
      id: 'sgmb-classic',
      name: 'Pack Classic',
      icon: '💳',
      description: 'Carte classique SGMB',
      features: ['Cashback 1%', 'Assurance'],
      price: 'Gratuit'
    }
  ],
  banque: [
    {
      id: 'banque-populaire',
      name: 'Pack Populaire',
      icon: '💳',
      description: 'Carte Banque Populaire',
      features: ['Cashback 1%', 'Assurance', 'Avantages membres'],
      price: 'Gratuit'
    }
  ]
})


const bankFeatures = ref([
  { id: 'transfers', name: 'Virements', icon: '💸' },
  { id: 'payments', name: 'Paiements', icon: '💳' },
  { id: 'loans', name: 'Prêts', icon: '💰' },
  { id: 'investments', name: 'Investissements', icon: '📈' },
  { id: 'insurance', name: 'Assurances', icon: '🛡️' },
  { id: 'statements', name: 'Relevés', icon: '📄' },
  { id: 'support', name: 'Support client', icon: '📞' }
])

const quickActions = ref([
  { id: 'add-card', name: 'Ajouter une carte', icon: '➕' },
  { id: 'view-cards', name: 'Mes cartes', icon: '💳' },
  { id: 'transactions', name: 'Transactions', icon: '📊' },
  { id: 'analytics', name: 'Analyses', icon: '📈' }
])

const availablePacks = computed(() => {
  if (!selectedBank.value) return []
  return cardPacks.value[selectedBank.value] || []
})

const toggleSidebar = () => {
  isOpen.value = !isOpen.value
}

const closeSidebar = () => {
  isOpen.value = false
}

const onBankChange = () => {
  selectedPack.value = null
}

const selectPack = (pack) => {
  selectedPack.value = pack
  // Émettre un événement pour que le parent puisse gérer la sélection
  emit('pack-selected', { bank: selectedBank.value, pack })
}

const handleFeatureClick = (feature) => {
  emit('feature-clicked', feature)
}

const handleQuickAction = (action) => {
  emit('quick-action', action)
}

const emit = defineEmits(['pack-selected', 'feature-clicked', 'quick-action'])

onMounted(() => {
  // Optionnel: charger les données depuis l'API
})
</script>

<style scoped>
.sidebar-container {
  position: relative;
}

.sidebar-toggle {
  position: fixed;
  left: 20px;
  top: 100px;
  z-index: 1000;
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  color: white;
  font-size: 1.5rem;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.sidebar-toggle:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.2);
}

.sidebar-toggle.open {
  left: 320px;
}

.bank-sidebar {
  position: fixed;
  left: -350px;
  top: 0;
  width: 350px;
  height: 100vh;
  background: linear-gradient(180deg, #ffffff 0%, #f8f9fa 100%);
  box-shadow: 2px 0 20px rgba(0, 0, 0, 0.1);
  transition: left 0.3s ease;
  z-index: 999;
  overflow-y: auto;
  overflow-x: hidden;
}

.bank-sidebar.open {
  left: 0;
}

.sidebar-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem 1.5rem;
  color: white;
}

.sidebar-title {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
}

.sidebar-content {
  padding: 1.5rem;
}

.sidebar-section {
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #e9ecef;
}

.sidebar-section:last-child {
  border-bottom: none;
}

.section-title {
  font-size: 1.1rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 1rem;
}

.bank-select {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e9ecef;
  border-radius: 8px;
  font-size: 1rem;
  background: white;
  cursor: pointer;
  transition: all 0.3s;
}

.bank-select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.card-packs {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.card-pack {
  background: white;
  border: 2px solid #e9ecef;
  border-radius: 12px;
  padding: 1.25rem;
  cursor: pointer;
  transition: all 0.3s;
}

.card-pack:hover {
  border-color: #667eea;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.15);
}

.card-pack.selected {
  border-color: #667eea;
  background: linear-gradient(135deg, rgba(102, 126, 234, 0.1) 0%, rgba(118, 75, 162, 0.1) 100%);
}

.pack-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.5rem;
}

.pack-icon {
  font-size: 1.5rem;
}

.pack-name {
  margin: 0;
  font-size: 1.1rem;
  font-weight: 600;
  color: #333;
}

.pack-description {
  font-size: 0.9rem;
  color: #666;
  margin-bottom: 0.75rem;
}

.pack-features {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.feature-badge {
  background: #e9ecef;
  color: #495057;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
}

.pack-price {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 600;
  color: #667eea;
}

.price-label {
  font-size: 0.9rem;
  color: #666;
}

.price-value {
  font-size: 1.1rem;
}

.features-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.feature-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s;
  margin-bottom: 0.5rem;
}

.feature-item:hover {
  background: #f8f9fa;
  transform: translateX(5px);
}

.feature-icon {
  font-size: 1.5rem;
}

.feature-text {
  font-size: 1rem;
  color: #333;
  font-weight: 500;
}

.quick-actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.quick-action-btn {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s;
}

.quick-action-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.action-icon {
  font-size: 1.25rem;
}

.sidebar-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  z-index: 998;
  animation: fadeIn 0.3s;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

/* Scrollbar styling */
.bank-sidebar::-webkit-scrollbar {
  width: 6px;
}

.bank-sidebar::-webkit-scrollbar-track {
  background: #f1f1f1;
}

.bank-sidebar::-webkit-scrollbar-thumb {
  background: #667eea;
  border-radius: 3px;
}

.bank-sidebar::-webkit-scrollbar-thumb:hover {
  background: #764ba2;
}
</style>
