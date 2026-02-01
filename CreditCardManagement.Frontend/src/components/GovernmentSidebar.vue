<template>
  <aside
    class="gov-sidebar"
    :class="{ 
      'sidebar-collapsed': isCollapsed && windowWidth >= 1024, 
      'sidebar-mobile-hidden': !isMobileOpen && windowWidth < 1024
    }"
    aria-label="Navigation latérale"
  >
    <!-- Toggle Button -->
    <button
      class="sidebar-toggle"
      @click="toggleSidebar"
      :aria-expanded="!isCollapsed"
      aria-label="Réduire/Agrandir le menu"
    >
      <svg
        class="w-5 h-5 transition-transform"
        :class="{ 'rotate-180': isCollapsed }"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
      >
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 19l-7-7 7-7m8 14l-7-7 7-7" />
      </svg>
    </button>

    <!-- Navigation Items -->
    <nav class="sidebar-nav" role="navigation">
      <ul class="sidebar-menu">
        <li v-for="item in menuItems" :key="item.path">
          <a
            :href="item.path === '/' ? '#' : undefined"
            @click.prevent="handleItemClick(item)"
            class="sidebar-item"
            :class="{ 'sidebar-item-active': isActive(item.path) }"
            :aria-current="isActive(item.path) ? 'page' : undefined"
          >
            <span class="sidebar-icon" :aria-hidden="true">{{ item.icon }}</span>
            <span class="sidebar-label" :class="{ 'sidebar-label-hidden': isCollapsed }">
              {{ item.label }}
            </span>
            <span
              v-if="item.badge"
              class="sidebar-badge"
              :class="{ 'sidebar-badge-hidden': isCollapsed }"
            >
              {{ item.badge }}
            </span>
          </a>
        </li>
      </ul>
    </nav>

    <!-- Bottom Section - Actions rapides -->
    <div class="sidebar-footer">
      <div class="sidebar-footer-title" :class="{ 'sidebar-label-hidden': isCollapsed }">
        Actions rapides
      </div>
      <a
        href="#"
        @click.prevent="handleQuickAction('transfers')"
        class="sidebar-item sidebar-item-action"
      >
        <span class="sidebar-icon">💸</span>
        <span class="sidebar-label" :class="{ 'sidebar-label-hidden': isCollapsed }">
          Virement
        </span>
      </a>
      <a
        href="#"
        @click.prevent="handleQuickAction('payments')"
        class="sidebar-item sidebar-item-action"
      >
        <span class="sidebar-icon">💳</span>
        <span class="sidebar-label" :class="{ 'sidebar-label-hidden': isCollapsed }">
          Paiement
        </span>
      </a>
      <a
        href="#"
        @click.prevent="handleSupportClick"
        class="sidebar-item sidebar-item-support"
      >
        <span class="sidebar-icon">❓</span>
        <span class="sidebar-label" :class="{ 'sidebar-label-hidden': isCollapsed }">
          Support client
        </span>
      </a>
    </div>
  </aside>

  <!-- Mobile Overlay -->
  <div
    v-if="isMobileOpen"
    class="sidebar-overlay"
    @click="closeMobileSidebar"
    aria-hidden="true"
  ></div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCreditCardStore } from '../stores/creditCard'

const props = defineProps({
  onFeatureClick: {
    type: Function,
    default: null
  }
})

const emit = defineEmits(['feature-clicked', 'sidebar-state-changed'])

const route = useRoute()
const router = useRouter()
const creditCardStore = useCreditCardStore()

const isCollapsed = ref(false)
const isMobileOpen = ref(false)
const windowWidth = ref(typeof window !== 'undefined' ? window.innerWidth : 1024)

// Mettre à jour la largeur de la fenêtre
const updateWindowWidth = () => {
  if (typeof window !== 'undefined') {
    windowWidth.value = window.innerWidth
  }
}

const menuItems = computed(() => {
  const items = [
    { path: '/', label: 'Tableau de bord', icon: '🏠', id: 'dashboard' },
    { path: '/cards', label: 'Mes cartes', icon: '💳', badge: creditCardStore.cards?.length || null, id: 'cards' },
    { path: '/transactions', label: 'Transactions', icon: '📊', id: 'transactions' },
    { path: '/statements', label: 'Relevés', icon: '📄', id: 'statements' },
    { path: '/loans', label: 'Prêts', icon: '💰', id: 'loans' },
  ]
  return items
})

const isActive = (path) => {
  if (path === '/') {
    return route.path === '/'
  }
  return route.path.startsWith(path)
}

const toggleSidebar = () => {
  console.log('Toggle sidebar called, window width:', window.innerWidth)
  if (window.innerWidth < 1024) {
    isMobileOpen.value = !isMobileOpen.value
    console.log('Mobile toggle, isMobileOpen:', isMobileOpen.value)
    emit('sidebar-state-changed', { isOpen: isMobileOpen.value, isCollapsed: false })
  } else {
    isCollapsed.value = !isCollapsed.value
    localStorage.setItem('sidebarCollapsed', isCollapsed.value.toString())
    console.log('Desktop toggle, isCollapsed:', isCollapsed.value)
    emit('sidebar-state-changed', { isOpen: true, isCollapsed: isCollapsed.value })
  }
}

// Écouter l'événement du header pour toggle
const handleToggleEvent = () => {
  toggleSidebar()
}

const closeMobileSidebar = () => {
  isMobileOpen.value = false
}

const handleMobileClose = () => {
  if (window.innerWidth < 1024) {
    closeMobileSidebar()
  }
}

const handleItemClick = (item) => {
  handleMobileClose()
  
  // Navigation avec router
  if (item.path) {
    router.push(item.path)
  }
  
  // Émettre aussi l'événement pour compatibilité
  if (item.id) {
    emit('feature-clicked', { id: item.id, label: item.label, path: item.path })
  }
}

const handleSupportClick = () => {
  handleMobileClose()
  alert('📞 Support Client\n\nContactez-nous à: support@tokenpay.com\nTél: +212 XXX XXX XXX')
}

const handleQuickAction = (action) => {
  handleMobileClose()
  emit('feature-clicked', { id: action, label: action === 'transfers' ? 'Virement' : 'Paiement' })
}

const handleResize = () => {
  updateWindowWidth()
  if (window.innerWidth >= 1024) {
    isMobileOpen.value = false
  }
}

onMounted(() => {
  // Initialiser la largeur de la fenêtre
  updateWindowWidth()
  
  // Restaurer l'état du sidebar depuis localStorage
  const savedState = localStorage.getItem('sidebarCollapsed')
  if (savedState !== null) {
    isCollapsed.value = savedState === 'true'
  }

  // Charger les cartes pour le badge
  if (creditCardStore.cards.length === 0) {
    creditCardStore.fetchCards()
  }

  window.addEventListener('resize', handleResize)
  window.addEventListener('toggle-sidebar', handleToggleEvent)
  
  // Émettre l'état initial du sidebar
  if (window.innerWidth < 1024) {
    emit('sidebar-state-changed', { isOpen: isMobileOpen.value, isCollapsed: false })
  } else {
    emit('sidebar-state-changed', { isOpen: true, isCollapsed: isCollapsed.value })
  }
})

onUnmounted(() => {
  window.removeEventListener('resize', handleResize)
  window.removeEventListener('toggle-sidebar', handleToggleEvent)
})

watch(() => route.path, () => {
  // Fermer le sidebar mobile lors de la navigation
  if (window.innerWidth < 1024) {
    closeMobileSidebar()
  }
})

watch([isCollapsed, isMobileOpen], () => {
  // Émettre l'état du sidebar quand il change
  if (window.innerWidth < 1024) {
    emit('sidebar-state-changed', { isOpen: isMobileOpen.value, isCollapsed: false })
  } else {
    emit('sidebar-state-changed', { isOpen: true, isCollapsed: isCollapsed.value })
  }
}, { immediate: true })
</script>

<style scoped>
.gov-sidebar {
  @apply fixed left-0 top-0 h-full bg-white border-r border-gray-200 z-40
         transition-all duration-300 ease-in-out
         flex flex-col;
  width: 256px;
  padding-top: 56px; /* Hauteur du header réduite */
}

/* Sur desktop, le sidebar est toujours visible par défaut */
@media (min-width: 1024px) {
  .gov-sidebar {
    transform: translateX(0) !important;
    display: flex !important;
  }
  
  .sidebar-mobile-hidden {
    transform: translateX(0) !important;
    display: flex !important;
  }
}

.sidebar-collapsed {
  width: 72px;
}

.sidebar-toggle {
  @apply absolute -right-3 top-16 w-6 h-6 bg-white border border-gray-200 
         rounded-full shadow-md flex items-center justify-center
         text-gray-600 hover:text-blue-900 hover:bg-gray-50
         transition-all duration-200 focus:outline-none focus:ring-2 
         focus:ring-blue-500 z-10;
}

.sidebar-nav {
  @apply flex-1 overflow-y-auto py-4;
}

.sidebar-menu {
  @apply list-none m-0 p-0;
}

.sidebar-item {
  @apply flex items-center gap-3 px-4 py-3 mx-2 rounded-md text-gray-600
         transition-all duration-200 hover:bg-gray-50 hover:text-gray-900
         focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2
         relative cursor-pointer;
}

.sidebar-item-active {
  @apply bg-blue-50 text-blue-900 font-medium;
}

.sidebar-item-active::before {
  content: '';
  @apply absolute left-0 top-0 bottom-0 w-1 bg-blue-900 rounded-r;
}

.sidebar-icon {
  @apply text-xl flex-shrink-0 w-6 text-center;
}

.sidebar-label {
  @apply flex-1 text-sm transition-opacity duration-200;
}

.sidebar-label-hidden {
  @apply opacity-0 w-0 overflow-hidden;
}

.sidebar-badge {
  @apply bg-blue-900 text-white text-xs font-semibold px-2 py-0.5 rounded-full
         min-w-[20px] text-center;
}

.sidebar-badge-hidden {
  @apply hidden;
}

.sidebar-footer {
  @apply border-t border-gray-200 pt-2 pb-4;
}

.sidebar-footer-title {
  @apply px-4 py-2 text-xs font-semibold text-gray-500 uppercase tracking-wider;
}

.sidebar-item-action {
  @apply text-gray-600;
}

.sidebar-item-support {
  @apply text-gray-600;
}

/* Mobile Styles */
@media (max-width: 1023px) {
  .gov-sidebar {
    transform: translateX(-100%);
    width: 256px;
    padding-top: 64px;
  }

  .sidebar-mobile-hidden {
    transform: translateX(-100%);
  }

  .gov-sidebar:not(.sidebar-mobile-hidden) {
    transform: translateX(0);
  }

  .sidebar-collapsed {
    width: 256px; /* Pas de collapse sur mobile */
  }
}

.sidebar-overlay {
  @apply fixed inset-0 bg-black bg-opacity-50 z-30 lg:hidden;
}

/* Scrollbar pour sidebar */
.sidebar-nav::-webkit-scrollbar {
  width: 4px;
}

.sidebar-nav::-webkit-scrollbar-track {
  @apply bg-transparent;
}

.sidebar-nav::-webkit-scrollbar-thumb {
  @apply bg-gray-300 rounded-full;
}

.sidebar-nav::-webkit-scrollbar-thumb:hover {
  @apply bg-gray-400;
}
</style>
