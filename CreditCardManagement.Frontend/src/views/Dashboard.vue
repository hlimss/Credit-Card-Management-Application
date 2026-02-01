<template>
  <div class="boa-dashboard min-h-screen bg-gray-50">
    <!-- Hero Section -->
    <section class="hero-section bg-gradient-to-br from-blue-900 via-blue-800 to-blue-900 text-white py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="hero-content text-center">
          <h1 class="hero-title text-4xl md:text-5xl font-bold mb-4">
            Token2Pay - Gestion des Cartes Bancaires
          </h1>
          <p class="hero-subtitle text-xl md:text-2xl mb-8 text-blue-100">
            Simplifiez votre vie avec une banque toujours à vos côtés.
          </p>
          <div class="hero-actions flex flex-wrap justify-center gap-4">
            <router-link to="/" class="hero-btn hero-btn-primary">
              <span class="hero-btn-icon">💳</span>
              <span>Mes cartes</span>
            </router-link>
            <router-link to="/transactions" class="hero-btn hero-btn-secondary">
              <span class="hero-btn-icon">📊</span>
              <span>Mes transactions</span>
            </router-link>
            <button
              @click="showAddCardModal = true"
              class="hero-btn hero-btn-outline"
            >
              <span class="hero-btn-icon">➕</span>
              <span>Ajouter une carte</span>
            </button>
          </div>
        </div>
      </div>
    </section>

    <!-- Quick Actions Section -->
    <section class="quick-actions-section py-12 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <h2 class="section-title text-2xl font-bold text-gray-900 mb-8 text-center">
          Que pouvons-nous faire pour vous ?
        </h2>
        <div class="quick-actions-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div
            v-for="action in quickActions"
            :key="action.id"
            class="action-card bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow p-6 cursor-pointer border border-gray-200"
            @click="handleActionClick(action)"
          >
            <div class="action-icon text-5xl mb-4 text-center">{{ action.icon }}</div>
            <h3 class="action-title text-lg font-semibold text-gray-900 mb-2 text-center">
              {{ action.title }}
            </h3>
            <p class="action-description text-sm text-gray-600 text-center">
              {{ action.description }}
            </p>
          </div>
        </div>
      </div>
    </section>

    <!-- Stats Section -->
    <section class="stats-section py-12 bg-gray-50">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <h2 class="section-title text-2xl font-bold text-gray-900 mb-8 text-center">
          Aperçu de vos comptes
        </h2>
        <DashboardStats v-if="!creditCardStore.loading && creditCardStore.cards.length > 0" />
      </div>
    </section>

    <!-- Services Section -->
    <section class="services-section py-12 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <h2 class="section-title text-2xl font-bold text-gray-900 mb-8 text-center">
          Nos solutions dédiées
        </h2>
        <div class="services-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div
            v-for="service in services"
            :key="service.id"
            class="service-card bg-gradient-to-br from-blue-50 to-blue-100 rounded-lg p-6 hover:shadow-lg transition-shadow cursor-pointer"
            @click="handleServiceClick(service)"
          >
            <div class="service-icon text-4xl mb-3">{{ service.icon }}</div>
            <h3 class="service-title text-lg font-semibold text-gray-900 mb-2">
              {{ service.title }}
            </h3>
            <p class="service-description text-sm text-gray-600">
              {{ service.description }}
            </p>
          </div>
        </div>
      </div>
    </section>

    <!-- Add Card Modal -->
    <div
      v-if="showAddCardModal"
      class="modal-overlay fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      @click.self="showAddCardModal = false"
    >
      <div class="modal-container bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="modal-header flex justify-between items-center p-6 border-b">
          <h3 class="modal-title text-xl font-semibold text-gray-900">
            Ajouter une nouvelle carte
          </h3>
          <button
            @click="showAddCardModal = false"
            class="modal-close text-gray-400 hover:text-gray-600 text-2xl"
          >
            ×
          </button>
        </div>
        <div class="modal-body p-6">
          <p class="text-gray-600 mb-4">
            Pour ajouter une nouvelle carte, veuillez retourner à la page principale.
          </p>
          <div class="flex gap-4">
            <router-link
              to="/cards"
              class="btn-primary px-6 py-2 rounded-md"
              @click="showAddCardModal = false"
            >
              Aller aux cartes
            </router-link>
            <button
              @click="showAddCardModal = false"
              class="btn-secondary px-6 py-2 rounded-md"
            >
              Annuler
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useCreditCardStore } from '../stores/creditCard'
import DashboardStats from '../components/GovernmentDashboardStats.vue'

const router = useRouter()
const creditCardStore = useCreditCardStore()
const showAddCardModal = ref(false)

const quickActions = [
  {
    id: 'cards',
    icon: '💳',
    title: 'Mes cartes',
    description: 'Gérez toutes vos cartes bancaires',
    route: '/'
  },
  {
    id: 'transactions',
    icon: '📊',
    title: 'Mes transactions',
    description: 'Consultez l\'historique complet',
    route: '/transactions'
  },
  {
    id: 'statements',
    icon: '📄',
    title: 'Relevés bancaires',
    description: 'Générez et consultez vos relevés',
    route: '/statements'
  },
  {
    id: 'loans',
    icon: '🏦',
    title: 'Prêts',
    description: 'Gérez vos prêts et crédits',
    route: '/loans'
  },
  {
    id: 'analytics',
    icon: '📈',
    title: 'Analyses',
    description: 'Analysez vos dépenses',
    route: '/analytics'
  },
  {
    id: 'add-card',
    icon: '➕',
    title: 'Ajouter une carte',
    description: 'Enregistrez une nouvelle carte',
    action: 'add-card'
  }
]

const services = [
  {
    id: 'transfers',
    icon: '💸',
    title: 'Virements',
    description: 'Effectuez des virements bancaires',
    action: 'transfer'
  },
  {
    id: 'payments',
    icon: '💳',
    title: 'Paiements',
    description: 'Payez vos factures et abonnements',
    action: 'payment'
  },
  {
    id: 'statements',
    icon: '📄',
    title: 'Relevés',
    description: 'Consultez vos relevés bancaires',
    route: '/statements'
  },
  {
    id: 'loans',
    icon: '🏦',
    title: 'Prêts',
    description: 'Demandez un prêt',
    route: '/loans'
  }
]

function handleActionClick(action) {
  if (action.route) {
    router.push(action.route)
  } else if (action.action === 'add-card') {
    showAddCardModal.value = true
  }
}

function handleServiceClick(service) {
  if (service.route) {
    router.push(service.route)
  } else if (service.action) {
    // Emit event for transfer/payment modals
    window.dispatchEvent(new CustomEvent('open-modal', { detail: service.action }))
  }
}
</script>

<style scoped>
.boa-dashboard {
  padding-top: 56px; /* Header height */
}

.hero-section {
  background: linear-gradient(135deg, #1E3A8A 0%, #3B82F6 50%, #1E3A8A 100%);
  position: relative;
  overflow: hidden;
}

.hero-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.05'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E");
  opacity: 0.1;
}

.hero-content {
  position: relative;
  z-index: 1;
}

.hero-title {
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.hero-subtitle {
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
}

.hero-actions {
  @apply flex flex-wrap justify-center gap-4;
}

.hero-btn {
  @apply inline-flex items-center gap-2 px-6 py-3 rounded-lg font-semibold transition-all duration-200 shadow-lg;
}

.hero-btn-primary {
  @apply bg-white text-blue-900 hover:bg-blue-50 hover:shadow-xl transform hover:-translate-y-0.5;
}

.hero-btn-secondary {
  @apply bg-blue-700 text-white hover:bg-blue-600 hover:shadow-xl transform hover:-translate-y-0.5;
}

.hero-btn-outline {
  @apply bg-transparent border-2 border-white text-white hover:bg-white hover:text-blue-900 hover:shadow-xl transform hover:-translate-y-0.5;
}

.hero-btn-icon {
  @apply text-xl;
}

.section-title {
  @apply text-2xl font-bold text-gray-900 mb-8 text-center;
}

.action-card {
  transition: all 0.3s ease;
}

.action-card:hover {
  transform: translateY(-4px);
}

.action-icon {
  @apply text-5xl mb-4 text-center;
}

.action-title {
  @apply text-lg font-semibold text-gray-900 mb-2 text-center;
}

.action-description {
  @apply text-sm text-gray-600 text-center;
}

.service-card {
  transition: all 0.3s ease;
}

.service-card:hover {
  transform: translateY(-4px);
}

.service-icon {
  @apply text-4xl mb-3;
}

.service-title {
  @apply text-lg font-semibold text-gray-900 mb-2;
}

.service-description {
  @apply text-sm text-gray-600;
}

.modal-overlay {
  @apply fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50;
}

.modal-container {
  @apply bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto;
}

.modal-header {
  @apply flex justify-between items-center p-6 border-b;
}

.modal-title {
  @apply text-xl font-semibold text-gray-900;
}

.modal-close {
  @apply text-gray-400 hover:text-gray-600 text-2xl cursor-pointer;
}

.modal-body {
  @apply p-6;
}

.btn-primary {
  @apply px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors font-medium;
}

.btn-secondary {
  @apply px-6 py-2 bg-gray-100 text-gray-700 rounded-md hover:bg-gray-200 transition-colors;
}
</style>
