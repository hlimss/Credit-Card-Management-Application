<template>
  <header class="gov-header bg-white border-b border-gray-200 sticky top-0 z-50 shadow-sm">
    <div class="header-container max-w-[1920px] mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-14">
        <!-- Logo et Titre -->
        <div class="flex items-center gap-2">
          <!-- Bouton Toggle Sidebar -->
          <button
            class="sidebar-toggle-header"
            @click="toggleSidebar"
            aria-label="Ouvrir/Fermer le menu"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>
          
          <router-link to="/" class="flex items-center gap-2 no-underline group">
            <div class="logo-container-transparent">
              <Token2PayLogo class="logo-institutional compact" />
            </div>
            <div class="hidden sm:block">
              <h1 class="text-xs font-semibold text-gray-900 m-0 leading-tight">
                Token2Pay
              </h1>
              <p class="text-[9px] text-gray-500 m-0 leading-tight">
                Gestion des Cartes Bancaires
              </p>
            </div>
          </router-link>
        </div>

        <!-- Currency Ticker -->
        <div class="hidden lg:flex items-center flex-1 mx-4 min-w-0">
          <CurrencyTicker />
        </div>

        <!-- Profil Utilisateur -->
        <div class="flex items-center gap-1.5">
          <!-- Notifications -->
          <div class="relative" ref="notificationMenuRef">
            <button
              class="icon-button-small relative"
              aria-label="Notifications"
              @click="showNotifications = !showNotifications"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
              </svg>
              <!-- Badge de notification -->
              <span
                v-if="notificationStore.unreadCount > 0"
                class="notification-badge"
              >
                {{ notificationStore.unreadCount > 99 ? '99+' : notificationStore.unreadCount }}
              </span>
            </button>

            <!-- Dropdown Notifications -->
            <Transition
              enter-active-class="transition ease-out duration-100"
              enter-from-class="transform opacity-0 scale-95"
              enter-to-class="transform opacity-100 scale-100"
              leave-active-class="transition ease-in duration-75"
              leave-from-class="transform opacity-100 scale-100"
              leave-to-class="transform opacity-0 scale-95"
            >
              <div
                v-if="showNotifications"
                class="notification-dropdown"
                role="menu"
                aria-orientation="vertical"
              >
                <div class="notification-header">
                  <h3 class="notification-title">Notifications</h3>
                  <div class="notification-actions">
                    <button
                      v-if="notificationStore.unreadCount > 0"
                      @click="markAllAsRead"
                      class="notification-action-btn"
                    >
                      Tout marquer comme lu
                    </button>
                    <button
                      v-if="notificationStore.notifications.length > 0"
                      @click="clearAllNotifications"
                      class="notification-action-btn text-red-600"
                    >
                      Tout effacer
                    </button>
                  </div>
                </div>
                
                <div class="notification-list">
                  <div
                    v-if="notificationStore.notifications.length === 0"
                    class="notification-empty"
                  >
                    <p class="text-gray-500 text-sm">Aucune notification</p>
                  </div>
                  
                  <div
                    v-for="notification in notificationStore.recentNotifications"
                    :key="notification.id"
                    class="notification-item"
                    :class="{ 'notification-unread': !notification.read }"
                    @click="markAsRead(notification.id)"
                  >
                    <div class="notification-icon">{{ notification.icon }}</div>
                    <div class="notification-content">
                      <div class="notification-item-title">{{ notification.title }}</div>
                      <div class="notification-item-message">{{ notification.message }}</div>
                      <div class="notification-item-time">{{ formatTime(notification.timestamp) }}</div>
                    </div>
                    <button
                      @click.stop="deleteNotification(notification.id)"
                      class="notification-delete-btn"
                      aria-label="Supprimer"
                    >
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                      </svg>
                    </button>
                  </div>
                </div>
              </div>
            </Transition>
          </div>

          <!-- Menu Profil -->
          <div class="relative" ref="profileMenuRef">
            <button
              class="profile-button-small"
              @click="showProfileMenu = !showProfileMenu"
              aria-expanded="showProfileMenu"
              aria-haspopup="true"
              aria-label="Menu profil"
            >
              <div class="profile-avatar-small">
                {{ userInitials }}
              </div>
              <div class="hidden md:block text-left">
                <div class="profile-greeting-small">
                  {{ userGreeting }}
                </div>
              </div>
              <svg 
                class="w-3 h-3 text-gray-600 transition-transform ml-1"
                :class="{ 'rotate-180': showProfileMenu }"
                fill="none" 
                stroke="currentColor" 
                viewBox="0 0 24 24"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
              </svg>
            </button>

            <!-- Dropdown Menu -->
            <Transition
              enter-active-class="transition ease-out duration-100"
              enter-from-class="transform opacity-0 scale-95"
              enter-to-class="transform opacity-100 scale-100"
              leave-active-class="transition ease-in duration-75"
              leave-from-class="transform opacity-100 scale-100"
              leave-to-class="transform opacity-0 scale-95"
            >
              <div
                v-if="showProfileMenu"
                class="profile-dropdown"
                role="menu"
                aria-orientation="vertical"
              >
                <div class="profile-dropdown-header">
                  <div class="font-semibold text-gray-900">{{ userFullName }}</div>
                  <div class="text-sm text-gray-600">{{ userEmail }}</div>
                </div>
                <div class="profile-dropdown-divider"></div>
                <router-link
                  to="/settings"
                  class="profile-dropdown-item"
                  @click="showProfileMenu = false"
                  role="menuitem"
                >
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                  <span>Paramètres</span>
                </router-link>
                <button
                  class="profile-dropdown-item text-red-600"
                  @click="handleLogout"
                  role="menuitem"
                >
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                  </svg>
                  <span>Déconnexion</span>
                </button>
              </div>
            </Transition>
          </div>
        </div>
      </div>
    </div>

  </header>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useNotificationStore } from '../stores/notification'
import Token2PayLogo from './Token2PayLogo.vue'
import CurrencyTicker from './CurrencyTicker.vue'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const notificationStore = useNotificationStore()

const showProfileMenu = ref(false)
const showNotifications = ref(false)
const profileMenuRef = ref(null)
const notificationMenuRef = ref(null)

// Navigation supprimée - tout est dans la sidebar

const toggleSidebar = () => {
  // Émettre un événement personnalisé pour que le sidebar l'écoute
  console.log('Header toggle button clicked')
  window.dispatchEvent(new CustomEvent('toggle-sidebar'))
}

const userGreeting = computed(() => {
  return authStore.userFirstName 
    ? `Mr. ${authStore.userFirstName}` 
    : authStore.user?.email?.split('@')[0] || 'Utilisateur'
})

const userEmail = computed(() => {
  return authStore.user?.email || authStore.userEmail || ''
})

const userFullName = computed(() => {
  if (authStore.userFirstName && authStore.userLastName) {
    return `${authStore.userFirstName} ${authStore.userLastName}`
  }
  return authStore.userFirstName || userEmail.value.split('@')[0] || 'Utilisateur'
})

const userInitials = computed(() => {
  const name = userFullName.value
  const parts = name.split(' ')
  if (parts.length >= 2) {
    return (parts[0][0] + parts[1][0]).toUpperCase()
  }
  return name.substring(0, 2).toUpperCase()
})

const breadcrumbs = computed(() => {
  const crumbs = []
  const path = route.path
  
  if (path === '/') return []
  
  const pathParts = path.split('/').filter(Boolean)
  let currentPath = ''
  
  pathParts.forEach((part, index) => {
    currentPath += `/${part}`
    const label = part.charAt(0).toUpperCase() + part.slice(1).replace(/-/g, ' ')
    crumbs.push({ path: currentPath, label })
  })
  
  return crumbs
})


const handleLogout = async () => {
  showProfileMenu.value = false
  await authStore.logout()
  router.push('/login')
}

const handleClickOutside = (event) => {
  if (profileMenuRef.value && !profileMenuRef.value.contains(event.target)) {
    showProfileMenu.value = false
  }
  if (notificationMenuRef.value && !notificationMenuRef.value.contains(event.target)) {
    showNotifications.value = false
  }
}

const markAsRead = (notificationId) => {
  notificationStore.markAsRead(notificationId)
}

const markAllAsRead = () => {
  notificationStore.markAllAsRead()
}

const deleteNotification = (notificationId) => {
  notificationStore.deleteNotification(notificationId)
}

const clearAllNotifications = () => {
  if (confirm('Êtes-vous sûr de vouloir effacer toutes les notifications ?')) {
    notificationStore.clearAll()
  }
}

const formatTime = (timestamp) => {
  const date = new Date(timestamp)
  const now = new Date()
  const diff = now - date
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)

  if (minutes < 1) return 'À l\'instant'
  if (minutes < 60) return `Il y a ${minutes} min`
  if (hours < 24) return `Il y a ${hours} h`
  if (days < 7) return `Il y a ${days} j`
  
  return new Intl.DateTimeFormat('fr-FR', {
    day: 'numeric',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  if (authStore.isAuthenticated && !authStore.userFirstName) {
    authStore.fetchUserDetails()
  }
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.gov-header {
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.header-container {
  height: 56px; /* Réduit de 64px à 56px */
}

.logo-container-transparent {
  @apply flex items-center justify-center;
  opacity: 0.8;
  transition: opacity 0.2s;
}

.logo-container-transparent:hover {
  opacity: 1;
}

.logo-institutional {
  @apply w-auto;
  filter: drop-shadow(0 0.5px 1px rgba(0, 0, 0, 0.06));
}

.sidebar-toggle-header {
  @apply p-1.5 rounded-md text-gray-600 hover:bg-gray-50 
         transition-colors duration-200 focus:outline-none focus:ring-2 
         focus:ring-blue-500 focus:ring-offset-1 mr-1;
  min-width: 32px;
  min-height: 32px;
}

.icon-button-small {
  @apply p-1.5 rounded-md text-gray-600 hover:bg-gray-50 
         transition-colors duration-200 focus:outline-none focus:ring-2 
         focus:ring-blue-500 focus:ring-offset-1;
  min-width: 32px;
  min-height: 32px;
}

.profile-button-small {
  @apply flex items-center gap-2 px-2 py-1.5 rounded-md hover:bg-gray-50
         transition-colors duration-200 focus:outline-none focus:ring-2 
         focus:ring-blue-500 focus:ring-offset-1;
}

.profile-avatar-small {
  @apply w-7 h-7 rounded-full bg-blue-900 text-white flex items-center 
         justify-center font-semibold text-xs flex-shrink-0;
}

.profile-greeting-small {
  @apply text-xs font-medium text-gray-900;
}

.profile-dropdown {
  @apply absolute right-0 mt-2 w-64 bg-white rounded-lg shadow-lg border border-gray-200
         py-1 z-50;
}

.profile-dropdown-header {
  @apply px-4 py-3 border-b border-gray-200;
}

.profile-dropdown-divider {
  @apply border-t border-gray-200 my-1;
}

.profile-dropdown-item {
  @apply flex items-center gap-3 px-4 py-2 text-sm text-gray-900 
         hover:bg-gray-50 transition-colors duration-150 w-full text-left
         focus:outline-none focus:bg-gray-50;
}

.breadcrumb-container {
  min-height: 32px;
}

.breadcrumb-link {
  @apply text-gray-600 hover:text-blue-900 transition-colors duration-150;
}

.breadcrumb-current {
  @apply text-gray-900 font-medium;
}

.breadcrumb-separator {
  @apply text-gray-600;
}

.notification-badge {
  @apply absolute -top-1 -right-1 bg-red-500 text-white text-xs font-bold 
         rounded-full min-w-[18px] h-[18px] flex items-center justify-center px-1;
  font-size: 10px;
  line-height: 1;
}

.notification-dropdown {
  @apply absolute right-0 mt-2 w-96 bg-white rounded-lg shadow-lg border border-gray-200
         z-50 max-h-[500px] flex flex-col;
}

.notification-header {
  @apply px-4 py-3 border-b border-gray-200 flex items-center justify-between;
}

.notification-title {
  @apply text-lg font-semibold text-gray-900;
}

.notification-actions {
  @apply flex gap-2;
}

.notification-action-btn {
  @apply text-xs text-blue-600 hover:text-blue-800 transition-colors;
}

.notification-list {
  @apply overflow-y-auto flex-1;
  max-height: 400px;
}

.notification-empty {
  @apply px-4 py-8 text-center;
}

.notification-item {
  @apply px-4 py-3 border-b border-gray-100 hover:bg-gray-50 transition-colors
         flex items-start gap-3 cursor-pointer relative;
}

.notification-item.notification-unread {
  @apply bg-blue-50;
}

.notification-item.notification-unread::before {
  content: '';
  @apply absolute left-0 top-0 bottom-0 w-1 bg-blue-600;
}

.notification-icon {
  @apply text-2xl flex-shrink-0;
}

.notification-content {
  @apply flex-1 min-w-0;
}

.notification-item-title {
  @apply font-semibold text-gray-900 text-sm mb-1;
}

.notification-item-message {
  @apply text-sm text-gray-600 mb-1;
}

.notification-item-time {
  @apply text-xs text-gray-500;
}

.notification-delete-btn {
  @apply p-1 text-gray-400 hover:text-red-600 transition-colors flex-shrink-0;
}
</style>
