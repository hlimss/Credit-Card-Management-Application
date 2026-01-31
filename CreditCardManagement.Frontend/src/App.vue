<template>
  <div id="app" class="min-h-screen">
    <nav v-if="authStore.isAuthenticated" class="modern-nav">
      <div class="nav-content">
        <router-link to="/" class="nav-logo">
          <Token2PayLogo class="compact" />
        </router-link>
        <div class="nav-actions">
          <div class="user-info">
            <span class="user-greeting">
              Bonjour {{ authStore.userFirstName ? `Mr. ${authStore.userFirstName}` : authStore.user?.email }}
            </span>
          </div>
          <button
            @click="handleLogout"
            class="logout-btn"
          >
            <span>🚪</span>
            <span>Logout</span>
          </button>
        </div>
      </div>
    </nav>

    <!-- Bank Sidebar -->
    <BankSidebar 
      v-if="authStore.isAuthenticated"
      @pack-selected="handlePackSelected"
      @feature-clicked="handleFeatureClick"
      @quick-action="handleQuickAction"
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

    <main :class="{ 'with-sidebar': authStore.isAuthenticated }">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from './stores/auth'
import { useRouter } from 'vue-router'
import Token2PayLogo from './components/Token2PayLogo.vue'
import BankSidebar from './components/BankSidebar.vue'
import TransferModal from './components/TransferModal.vue'
import PaymentModal from './components/PaymentModal.vue'
import { useCreditCardStore } from './stores/creditCard'

const authStore = useAuthStore()
const router = useRouter()
const creditCardStore = useCreditCardStore()

const showTransferModal = ref(false)
const showPaymentModal = ref(false)

onMounted(async () => {
  // Load user details if authenticated
  if (authStore.isAuthenticated && !authStore.userFirstName) {
    await authStore.fetchUserDetails()
  }
})

const handleLogout = async () => {
  await authStore.logout()
  router.push('/login')
}

const handlePackSelected = async (data) => {
  const { bank, pack } = data
  
  if (!pack) {
    alert('Veuillez sélectionner un pack de cartes')
    return
  }

  // Demander confirmation
  const confirmMessage = `Voulez-vous créer le pack "${pack.name}" de ${banks.find(b => b.id === bank)?.name}?\n\n${pack.description}\n\nFonctionnalités: ${pack.features.join(', ')}`
  
  if (!confirm(confirmMessage)) {
    return
  }

  try {
    // Importer le service de packs de cartes
    const { cardPackService } = await import('./services/cardPackService')
    
    // Obtenir le nom complet de l'utilisateur
    const userFullName = authStore.userFullName || 
                        `${authStore.userFirstName || ''} ${authStore.userLastName || ''}`.trim() ||
                        authStore.userEmail ||
                        authStore.user?.email ||
                        'Cardholder Name'

    // Appliquer le pack
    const result = await cardPackService.applyCardPack(bank, pack.id, userFullName)
    
    if (result.success) {
      alert(`✅ ${result.message}\n\nPack: ${result.pack}\nCartes créées: ${result.cards.length}`)
      // Recharger les cartes si on est sur la page d'accueil
      if (router.currentRoute.value.path === '/') {
        window.location.reload()
      } else {
        router.push('/')
      }
    }
  } catch (error) {
    console.error('Erreur lors de l\'application du pack:', error)
    alert(`❌ Erreur: ${error.message || 'Impossible de créer les cartes du pack'}`)
  }
}

const handleFeatureClick = (feature) => {
  // Navigation ou ouverture de modals selon la fonctionnalité
  switch (feature.id) {
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
    case 'loans':
      router.push('/loans')
      break
    case 'investments':
      router.push('/analytics')
      break
    case 'insurance':
      alert('🛡️ Fonctionnalité Assurances\n\nCette fonctionnalité sera bientôt disponible!')
      break
    case 'statements':
      router.push('/statements')
      break
    case 'support':
      alert('📞 Support Client\n\nContactez-nous à: support@tokenpay.com\nTél: +212 XXX XXX XXX')
      break
    default:
      console.log('Fonctionnalité:', feature)
  }
}

const handleTransferSuccess = () => {
  // Recharger les cartes pour mettre à jour les balances
  creditCardStore.fetchCards()
  alert('✅ Virement effectué avec succès!')
}

const handlePaymentSuccess = () => {
  // Recharger les cartes pour mettre à jour les balances
  creditCardStore.fetchCards()
  alert('✅ Paiement effectué avec succès!')
}

const handleQuickAction = (action) => {
  switch (action.id) {
    case 'add-card':
      // Naviguer vers la page d'accueil et déclencher l'ouverture du modal
      if (router.currentRoute.value.path !== '/') {
        router.push('/')
      }
      // Émettre un événement personnalisé pour ouvrir le modal
      setTimeout(() => {
        window.dispatchEvent(new CustomEvent('open-add-card-modal'))
      }, 100)
      break
    case 'view-cards':
      router.push('/')
      break
    case 'transactions':
      router.push('/')
      // Optionnel: scroll vers la section des transactions
      setTimeout(() => {
        window.dispatchEvent(new CustomEvent('scroll-to-transactions'))
      }, 100)
      break
    case 'analytics':
      router.push('/')
      // Optionnel: scroll vers la section analytics
      setTimeout(() => {
        window.dispatchEvent(new CustomEvent('scroll-to-analytics'))
      }, 100)
      break
    default:
      console.log('Action rapide:', action)
  }
}

// Liste des banques pour le handler
const banks = [
  { id: 'attijari', name: 'Attijariwafa Bank' },
  { id: 'bmce', name: 'BMCE Bank' },
  { id: 'cih', name: 'CIH Bank' },
  { id: 'bmci', name: 'BMCI' },
  { id: 'credit', name: 'Crédit du Maroc' },
  { id: 'sgmb', name: 'SGMB' },
  { id: 'banque', name: 'Banque Populaire' }
]
</script>

<style scoped>
.modern-nav {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 100;
}

.nav-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.nav-logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
  color: white;
  font-size: 1.5rem;
  font-weight: 800;
  transition: transform 0.3s;
}

.nav-logo:hover {
  transform: scale(1.05);
}

.logo-icon {
  font-size: 2rem;
}

.nav-actions {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.user-info {
  display: flex;
  align-items: center;
}

.user-greeting {
  color: rgba(255, 255, 255, 0.95);
  font-size: 0.95rem;
  font-weight: 600;
  padding: 0.5rem 1rem;
  background: rgba(255, 255, 255, 0.15);
  border-radius: 8px;
}

.logout-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 10px;
  color: white;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.logout-btn:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

main {
  min-height: calc(100vh - 80px);
  transition: margin-left 0.3s ease;
}

main.with-sidebar {
  margin-left: 0;
}

@media (min-width: 768px) {
  main.with-sidebar {
    margin-left: 0; /* La sidebar est en overlay, donc pas besoin de margin */
  }
}
</style>

