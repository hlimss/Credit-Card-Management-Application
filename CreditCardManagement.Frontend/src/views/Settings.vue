<template>
  <div class="settings-page min-h-screen bg-gray-50">
    <div class="settings-container max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="page-header mb-8">
        <h1 class="page-title">⚙️ Paramètres</h1>
        <p class="page-subtitle">Gérez vos informations personnelles et vos cartes bancaires</p>
      </div>

      <!-- Personal Information Section -->
      <div class="settings-section bg-white rounded-lg shadow-sm p-6 mb-8">
        <h2 class="section-title text-xl font-semibold text-gray-900 mb-6">
          👤 Informations personnelles
        </h2>

        <form @submit.prevent="handleUpdateProfile" class="profile-form">
          <!-- Error Display -->
          <div v-if="profileError" class="form-error bg-red-50 border border-red-200 rounded-md p-4 mb-4">
            <div class="flex items-center gap-2">
              <span class="text-red-600">⚠️</span>
              <span class="text-red-800">{{ profileError }}</span>
            </div>
          </div>

          <!-- Success Message -->
          <div v-if="profileSuccess" class="form-success bg-green-50 border border-green-200 rounded-md p-4 mb-4">
            <div class="flex items-center gap-2">
              <span class="text-green-600">✅</span>
              <span class="text-green-800">{{ profileSuccess }}</span>
            </div>
          </div>

          <div class="form-grid grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- First Name -->
            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">👤</span>
                Prénom *
              </label>
              <input
                v-model="profileForm.firstName"
                type="text"
                class="form-input"
                placeholder="Votre prénom"
                required
                maxlength="50"
              />
            </div>

            <!-- Last Name -->
            <div class="form-group">
              <label class="form-label">
                <span class="label-icon">👤</span>
                Nom *
              </label>
              <input
                v-model="profileForm.lastName"
                type="text"
                class="form-input"
                placeholder="Votre nom"
                required
                maxlength="50"
              />
            </div>

            <!-- Email -->
            <div class="form-group md:col-span-2">
              <label class="form-label">
                <span class="label-icon">📧</span>
                Adresse e-mail *
              </label>
              <input
                v-model="profileForm.email"
                type="email"
                class="form-input"
                placeholder="votre.email@example.com"
                required
                maxlength="255"
              />
            </div>

            <!-- Phone Number -->
            <div class="form-group md:col-span-2">
              <label class="form-label">
                <span class="label-icon">📱</span>
                Numéro de téléphone
              </label>
              <input
                v-model="profileForm.phoneNumber"
                type="tel"
                class="form-input"
                placeholder="+212 6XX XXX XXX"
                maxlength="20"
              />
              <p class="form-hint text-sm text-gray-500 mt-1">
                Optionnel - Utilisé pour les notifications WhatsApp
              </p>
            </div>
          </div>

          <!-- Form Actions -->
          <div class="form-actions flex justify-end gap-4 mt-6">
            <button
              type="button"
              @click="resetProfileForm"
              class="btn-secondary"
            >
              Annuler
            </button>
            <button
              type="submit"
              :disabled="updatingProfile"
              class="btn-primary"
            >
              <span v-if="updatingProfile" class="flex items-center gap-2">
                <span class="spinner-small"></span>
                <span>Enregistrement...</span>
              </span>
              <span v-else class="flex items-center gap-2">
                <span>💾</span>
                <span>Enregistrer les modifications</span>
              </span>
            </button>
          </div>
        </form>
      </div>

      <!-- Credit Cards Management Section -->
      <div class="settings-section bg-white rounded-lg shadow-sm p-6">
        <h2 class="section-title text-xl font-semibold text-gray-900 mb-6">
          💳 Gestion des cartes bancaires
        </h2>

        <!-- Loading State -->
        <div v-if="cardsLoading" class="loading-state text-center py-12">
          <div class="spinner mx-auto mb-4"></div>
          <p class="text-gray-600">Chargement des cartes...</p>
        </div>

        <!-- Error State -->
        <div v-else-if="cardsError" class="error-state bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
          <p class="text-red-800">{{ cardsError }}</p>
        </div>

        <!-- Cards List -->
        <div v-else-if="cards.length > 0" class="cards-list space-y-4">
          <div
            v-for="card in cards"
            :key="card.id"
            class="card-item bg-gray-50 rounded-lg p-4 border border-gray-200 hover:border-blue-300 transition-all"
          >
            <div class="flex items-center justify-between">
              <div class="flex-1">
                <div class="flex items-center gap-3 mb-2">
                  <div class="card-icon text-2xl">💳</div>
                  <div>
                    <h3 class="card-name font-semibold text-gray-900">
                      {{ card.cardholderName }}
                    </h3>
                    <p class="card-type text-sm text-gray-600">
                      {{ card.cardType }} • {{ formatCardNumber(card.cardNumber) }}
                    </p>
                    <p v-if="card.balance !== null && card.balance !== undefined" class="card-balance text-sm text-gray-700 mt-1">
                      Solde: <span class="font-semibold">{{ formatCurrency(card.balance) }}</span>
                    </p>
                  </div>
                </div>
              </div>

              <!-- Toggle Switch -->
              <div class="toggle-container flex items-center gap-3">
                <span class="toggle-label text-sm font-medium" :class="card.isActive ? 'text-green-600' : 'text-gray-500'">
                  {{ card.isActive ? 'Active' : 'Inactive' }}
                </span>
                <label class="toggle-switch relative inline-block w-14 h-8">
                  <input
                    type="checkbox"
                    :checked="card.isActive"
                    @change="toggleCardStatus(card.id, $event.target.checked)"
                    :disabled="togglingCardId === card.id"
                    class="sr-only"
                  />
                  <span
                    class="toggle-slider absolute inset-0 rounded-full transition-all duration-300"
                    :class="card.isActive ? 'bg-blue-600' : 'bg-gray-300'"
                  >
                    <span
                      class="toggle-thumb absolute top-1 left-1 w-6 h-6 bg-white rounded-full transition-all duration-300 shadow-md"
                      :class="card.isActive ? 'translate-x-6' : 'translate-x-0'"
                    ></span>
                  </span>
                </label>
                <span v-if="togglingCardId === card.id" class="spinner-small"></span>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else class="empty-state bg-gray-50 rounded-lg p-12 text-center">
          <div class="empty-icon text-6xl mb-4">💳</div>
          <h3 class="empty-title text-xl font-semibold text-gray-900 mb-2">Aucune carte</h3>
          <p class="empty-text text-gray-600 mb-6">Vous n'avez pas encore ajouté de carte bancaire.</p>
          <router-link to="/cards" class="btn-primary inline-block">
            Ajouter une carte
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '../stores/auth'
import { useCreditCardStore } from '../stores/creditCard'
import { useNotificationStore } from '../stores/notification'
import { creditCardService } from '../services/creditCardService'
import { authService } from '../services/authService'

const authStore = useAuthStore()
const creditCardStore = useCreditCardStore()
const notificationStore = useNotificationStore()

const profileForm = ref({
  firstName: '',
  lastName: '',
  email: '',
  phoneNumber: ''
})

const profileError = ref('')
const profileSuccess = ref('')
const updatingProfile = ref(false)
const cardsLoading = ref(false)
const cardsError = ref('')
const togglingCardId = ref(null)

const cards = computed(() => creditCardStore.cards || [])

async function loadUserProfile() {
  try {
    const user = await authService.getCurrentUser()
    profileForm.value = {
      firstName: user.firstName || '',
      lastName: user.lastName || '',
      email: user.email || '',
      phoneNumber: user.phoneNumber || ''
    }
  } catch (error) {
    console.error('Error loading user profile:', error)
    profileError.value = 'Erreur lors du chargement de vos informations'
  }
}

async function loadCards() {
  cardsLoading.value = true
  cardsError.value = ''
  try {
    await creditCardStore.fetchCards()
  } catch (error) {
    console.error('Error loading cards:', error)
    cardsError.value = 'Erreur lors du chargement des cartes'
  } finally {
    cardsLoading.value = false
  }
}

async function handleUpdateProfile() {
  profileError.value = ''
  profileSuccess.value = ''
  updatingProfile.value = true

  try {
    const updateData = {
      firstName: profileForm.value.firstName,
      lastName: profileForm.value.lastName,
      email: profileForm.value.email,
      phoneNumber: profileForm.value.phoneNumber || null
    }

    const response = await authService.updateProfile(updateData)
    
    // Update auth store
    authStore.user = {
      ...authStore.user,
      firstName: response.firstName,
      lastName: response.lastName,
      email: response.email,
      phoneNumber: response.phoneNumber,
      fullName: response.fullName
    }
    localStorage.setItem('user', JSON.stringify(authStore.user))
    
    // Ajouter une notification
    notificationStore.addNotification({
      type: 'success',
      icon: '👤',
      title: 'Profil mis à jour',
      message: 'Vos informations personnelles ont été mises à jour avec succès'
    })
    
    profileSuccess.value = 'Informations mises à jour avec succès !'
    
    setTimeout(() => {
      profileSuccess.value = ''
    }, 3000)
  } catch (error) {
    console.error('Error updating profile:', error)
    const errorMessage = error.response?.data?.message || 'Erreur lors de la mise à jour. Veuillez réessayer.'
    profileError.value = errorMessage
  } finally {
    updatingProfile.value = false
  }
}

function resetProfileForm() {
  loadUserProfile()
  profileError.value = ''
  profileSuccess.value = ''
}

async function toggleCardStatus(cardId, isActive) {
  togglingCardId.value = cardId
  
  try {
    // Send isActive with camelCase (ASP.NET Core uses camelCase by default for JSON)
    console.log('Toggling card status:', { cardId, isActive })
    await creditCardService.update(cardId, { isActive: isActive })
    
    const card = creditCardStore.cards.find(c => c.id === cardId)
    
    // Ajouter une notification
    notificationStore.addNotification({
      type: 'info',
      icon: isActive ? '✅' : '❌',
      title: isActive ? 'Carte activée' : 'Carte désactivée',
      message: `La carte ${card?.cardholderName || 'sélectionnée'} a été ${isActive ? 'activée' : 'désactivée'}`
    })
    
    await creditCardStore.fetchCards()
    console.log('Card status updated successfully')
  } catch (error) {
    console.error('Error toggling card status:', error)
    const errorMessage = error.response?.data?.message || error.response?.data?.error || 'Erreur lors de la modification du statut de la carte. Veuillez réessayer.'
    
    // Notification d'erreur
    notificationStore.addNotification({
      type: 'error',
      icon: '⚠️',
      title: 'Erreur',
      message: errorMessage
    })
    
    alert(errorMessage)
    // Reload cards to revert UI state
    await creditCardStore.fetchCards()
  } finally {
    togglingCardId.value = null
  }
}

function formatCurrency(amount) {
  return new Intl.NumberFormat('fr-FR', {
    style: 'currency',
    currency: 'MAD',
    minimumFractionDigits: 2
  }).format(amount)
}

function formatCardNumber(cardNumber) {
  if (!cardNumber) return '**** **** **** ****'
  // Show only last 4 digits
  const cleaned = cardNumber.replace(/\s/g, '')
  if (cleaned.length >= 4) {
    return `**** **** **** ${cleaned.slice(-4)}`
  }
  return '**** **** **** ****'
}

onMounted(async () => {
  await loadUserProfile()
  await loadCards()
})
</script>

<style scoped>
.settings-page {
  padding-top: 56px; /* Header height */
}

.page-header {
  @apply mb-8;
}

.page-title {
  @apply text-3xl font-bold text-gray-900 mb-2;
}

.page-subtitle {
  @apply text-gray-600;
}

.settings-section {
  @apply bg-white rounded-lg shadow-sm p-6 mb-8;
}

.section-title {
  @apply text-xl font-semibold text-gray-900 mb-6;
}

.profile-form {
  @apply space-y-6;
}

.form-grid {
  @apply grid grid-cols-1 md:grid-cols-2 gap-6;
}

.form-group {
  @apply flex flex-col gap-1;
}

.form-label {
  @apply flex items-center gap-2 text-sm font-medium text-gray-700 mb-1;
}

.label-icon {
  @apply text-base;
}

.form-input {
  @apply px-3 py-2 border border-gray-300 rounded-md 
         focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent
         transition-all duration-200;
}

.form-input:disabled {
  @apply bg-gray-100 cursor-not-allowed;
}

.form-hint {
  @apply text-sm text-gray-500 mt-1;
}

.form-error {
  @apply bg-red-50 border border-red-200 rounded-md p-4;
}

.form-success {
  @apply bg-green-50 border border-green-200 rounded-md p-4;
}

.form-actions {
  @apply flex justify-end gap-4 mt-6;
}

.btn-secondary {
  @apply px-4 py-2 bg-gray-100 text-gray-700 rounded-md hover:bg-gray-200 transition-colors;
}

.btn-primary {
  @apply px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors font-medium
         disabled:opacity-50 disabled:cursor-not-allowed;
}

.spinner-small {
  @apply w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin;
}

.loading-state {
  @apply text-center py-12;
}

.spinner {
  @apply w-12 h-12 border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin mx-auto mb-4;
}

.error-state {
  @apply bg-red-50 border border-red-200 rounded-lg p-4 mb-6;
}

.cards-list {
  @apply space-y-4;
}

.card-item {
  @apply bg-gray-50 rounded-lg p-4 border border-gray-200 hover:border-blue-300 transition-all;
}

.card-icon {
  @apply text-2xl;
}

.card-name {
  @apply font-semibold text-gray-900;
}

.card-type {
  @apply text-sm text-gray-600;
}

.card-balance {
  @apply text-sm text-gray-700 mt-1;
}

.toggle-container {
  @apply flex items-center gap-3;
}

.toggle-label {
  @apply text-sm font-medium;
}

.toggle-switch {
  @apply relative inline-block w-14 h-8;
}

.toggle-switch input {
  @apply sr-only;
}

.toggle-slider {
  @apply absolute inset-0 rounded-full transition-all duration-300;
}

.toggle-thumb {
  @apply absolute top-1 left-1 w-6 h-6 bg-white rounded-full transition-all duration-300 shadow-md;
}

.empty-state {
  @apply bg-gray-50 rounded-lg p-12 text-center;
}

.empty-icon {
  @apply text-6xl mb-4;
}

.empty-title {
  @apply text-xl font-semibold text-gray-900 mb-2;
}

.empty-text {
  @apply text-gray-600 mb-6;
}
</style>
