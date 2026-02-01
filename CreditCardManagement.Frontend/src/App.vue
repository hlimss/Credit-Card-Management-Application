<template>
  <div id="app" class="min-h-screen bg-background flex flex-col">
    <!-- Skip Link pour accessibilité -->
    <a href="#main-content" class="skip-link">Aller au contenu principal</a>

    <!-- Header Gouvernemental -->
    <GovernmentHeader v-if="authStore.isAuthenticated" />

    <!-- Sidebar Gouvernementale -->
    <GovernmentSidebar 
      v-if="authStore.isAuthenticated"
      @feature-clicked="handleFeatureClick"
      @sidebar-state-changed="handleSidebarStateChange"
    />

    <!-- Transfer Modal -->
    <TransferModal 
      v-if="authStore.isAuthenticated"
      :show="showTransferModal"
      @close="showTransferModal = false"
      @success="handleTransferSuccess"
    />

    <!-- Payment Modal -->
    <PaymentModal 
      v-if="authStore.isAuthenticated"
      :show="showPaymentModal"
      @close="showPaymentModal = false"
      @success="handlePaymentSuccess"
    />

    <!-- Main Content -->
    <main 
      id="main-content"
      class="main-content flex-1 transition-all duration-300"
      :class="{ 
        'main-with-sidebar': authStore.isAuthenticated,
        'main-sidebar-collapsed': sidebarState.isCollapsed,
        'main-sidebar-mobile-open': sidebarState.isMobileOpen
      }"
      :style="mainContentStyle"
    >
      <router-view />
    </main>

    <!-- Footer -->
    <GovernmentFooter 
      v-if="authStore.isAuthenticated" 
      :is-collapsed="sidebarState.isCollapsed"
      :is-mobile-open="sidebarState.isMobileOpen"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useAuthStore } from './stores/auth'
import { useRouter } from 'vue-router'
import GovernmentHeader from './components/GovernmentHeader.vue'
import GovernmentSidebar from './components/GovernmentSidebar.vue'
import GovernmentFooter from './components/GovernmentFooter.vue'
import TransferModal from './components/TransferModal.vue'
import PaymentModal from './components/PaymentModal.vue'
import { useCreditCardStore } from './stores/creditCard'
import { useNotificationStore } from './stores/notification'

const authStore = useAuthStore()
const router = useRouter()
const creditCardStore = useCreditCardStore()
const notificationStore = useNotificationStore()

const showTransferModal = ref(false)
const showPaymentModal = ref(false)

const sidebarState = ref({
  isOpen: false,
  isCollapsed: false,
  isMobileOpen: false
})

const windowWidth = ref(typeof window !== 'undefined' ? window.innerWidth : 1024)

const updateWindowWidth = () => {
  if (typeof window !== 'undefined') {
    windowWidth.value = window.innerWidth
  }
}

const mainContentStyle = computed(() => {
  if (!authStore.isAuthenticated) return {}
  
  // Sur mobile, le sidebar est en overlay, donc pas de décalage
  if (windowWidth.value < 1024) {
    return {}
  }
  
  // Sur desktop, décaler selon l'état collapsed/expanded
  if (sidebarState.value.isCollapsed) {
    return { marginLeft: '72px' } // Largeur sidebar collapsed
  } else {
    return { marginLeft: '256px' } // Largeur sidebar expanded
  }
})


function handleSidebarStateChange(state) {
  sidebarState.value = {
    isOpen: state.isOpen,
    isCollapsed: state.isCollapsed,
    isMobileOpen: state.isOpen && !state.isCollapsed && window.innerWidth < 1024
  }
}

onMounted(async () => {
  // Load user details if authenticated
  if (authStore.isAuthenticated && !authStore.userFirstName) {
    await authStore.fetchUserDetails()
  }
  
  // Écouter les changements de taille de fenêtre
  if (typeof window !== 'undefined') {
    window.addEventListener('resize', updateWindowWidth)
    updateWindowWidth()
  }
})

onUnmounted(() => {
  if (typeof window !== 'undefined') {
    window.removeEventListener('resize', updateWindowWidth)
  }
})

const handleLogout = async () => {
  await authStore.logout()
  router.push('/login')
}


const handleFeatureClick = (feature) => {
  if (!feature || !feature.id) return
  
  // Navigation ou ouverture de modals selon la fonctionnalité
  switch (feature.id) {
    case 'dashboard':
      router.push('/')
      break
    case 'cards':
      router.push('/cards')
      break
    case 'transactions':
      router.push('/transactions')
      break
    case 'analytics':
      router.push('/analytics')
      break
    case 'statements':
      router.push('/statements')
      break
    case 'loans':
      router.push('/loans')
      break
    case 'transfers':
      // Charger les cartes si nécessaire
      if (creditCardStore.cards.length === 0) {
        creditCardStore.fetchCards()
      }
      showTransferModal.value = true
      break
    case 'payments':
      // Charger les cartes si nécessaire
      if (creditCardStore.cards.length === 0) {
        creditCardStore.fetchCards()
      }
      showPaymentModal.value = true
      break
    default:
      // Si c'est une route, naviguer
      if (feature.path) {
        router.push(feature.path)
      }
  }
}

const handleTransferSuccess = (transferData) => {
  // Recharger les cartes pour mettre à jour les balances
  creditCardStore.fetchCards()
  
  // Ajouter une notification
  if (transferData) {
    const formatAmount = (amount, currency) => {
      return new Intl.NumberFormat('fr-FR', {
        style: 'currency',
        currency: currency || 'MAD'
      }).format(amount)
    }
    
    notificationStore.addNotification({
      type: 'success',
      icon: '💸',
      title: 'Virement effectué',
      message: `Virement de ${formatAmount(transferData.amount, transferData.currency)} vers ${transferData.beneficiaryName || transferData.toAccount} effectué avec succès`
    })
  }
}

const handlePaymentSuccess = (paymentData) => {
  // Recharger les cartes pour mettre à jour les balances
  creditCardStore.fetchCards()
  
  // Ajouter une notification
  if (paymentData) {
    const formatAmount = (amount, currency) => {
      return new Intl.NumberFormat('fr-FR', {
        style: 'currency',
        currency: currency || 'MAD'
      }).format(amount)
    }
    
    notificationStore.addNotification({
      type: 'success',
      icon: '💳',
      title: 'Paiement effectué',
      message: `Paiement de ${formatAmount(paymentData.amount, paymentData.currency)} pour ${paymentData.merchantName || paymentData.paymentType} effectué avec succès`
    })
  }
}

</script>

<style scoped>
.main-content {
  @apply min-h-screen;
  padding-top: 56px; /* Header seulement */
  transition: margin-left 0.3s ease-in-out;
}

.main-with-sidebar {
  /* Le margin-left est géré dynamiquement via computed */
}

@media (max-width: 1023px) {
  .main-content {
    margin-left: 0 !important; /* Pas de décalage sur mobile */
  }
}
</style>

