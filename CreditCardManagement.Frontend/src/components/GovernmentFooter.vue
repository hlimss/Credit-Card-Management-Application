<template>
  <footer 
    class="gov-footer bg-white border-t border-gray-200 mt-auto transition-all duration-300"
    :style="footerStyle"
  >
    <div class="footer-container max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Main Footer Content -->
      <div class="footer-main py-12">
        <div class="footer-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
          <!-- Logo & Description -->
          <div class="footer-section">
            <div class="footer-logo mb-4">
              <div class="flex items-center gap-2">
                <div class="logo-icon text-2xl">💳</div>
                <div>
                  <h3 class="footer-brand-name text-lg font-bold text-gray-900">Token2Pay</h3>
                  <p class="footer-brand-subtitle text-xs text-gray-600">Gestion des Cartes Bancaires</p>
                </div>
              </div>
            </div>
            <p class="footer-description text-sm text-gray-600 mb-4">
              Simplifiez votre vie avec une banque toujours à vos côtés. 
              Gérez vos cartes bancaires en toute sécurité.
            </p>
            <div class="footer-social flex gap-3">
              <a href="#" class="social-link" aria-label="Facebook">
                <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                  <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z"/>
                </svg>
              </a>
              <a href="#" class="social-link" aria-label="Twitter">
                <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                  <path d="M23.953 4.57a10 10 0 01-2.825.775 4.958 4.958 0 002.163-2.723c-.951.555-2.005.959-3.127 1.184a4.92 4.92 0 00-8.384 4.482C7.69 8.095 4.067 6.13 1.64 3.162a4.822 4.822 0 00-.666 2.475c0 1.71.87 3.213 2.188 4.096a4.904 4.904 0 01-2.228-.616v.06a4.923 4.923 0 003.946 4.827 4.996 4.996 0 01-2.212.085 4.936 4.936 0 004.604 3.417 9.867 9.867 0 01-6.102 2.105c-.39 0-.779-.023-1.17-.067a13.995 13.995 0 007.557 2.209c9.053 0 13.998-7.496 13.998-13.985 0-.21 0-.42-.015-.63A9.935 9.935 0 0024 4.59z"/>
                </svg>
              </a>
              <a href="#" class="social-link" aria-label="LinkedIn">
                <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                  <path d="M20.447 20.452h-3.554v-5.569c0-1.328-.027-3.037-1.852-3.037-1.853 0-2.136 1.445-2.136 2.939v5.667H9.351V9h3.414v1.561h.046c.477-.9 1.637-1.85 3.37-1.85 3.601 0 4.267 2.37 4.267 5.455v6.286zM5.337 7.433c-1.144 0-2.063-.926-2.063-2.065 0-1.138.92-2.063 2.063-2.063 1.14 0 2.064.925 2.064 2.063 0 1.139-.925 2.065-2.064 2.065zm1.782 13.019H3.555V9h3.564v11.452zM22.225 0H1.771C.792 0 0 .774 0 1.729v20.542C0 23.227.792 24 1.771 24h20.451C23.2 24 24 23.227 24 22.271V1.729C24 .774 23.2 0 22.222 0h.003z"/>
                </svg>
              </a>
            </div>
          </div>

          <!-- Essentiels -->
          <div class="footer-section">
            <h4 class="footer-title text-base font-semibold text-gray-900 mb-4">Essentiels</h4>
            <ul class="footer-links space-y-2">
              <li>
                <router-link to="/" class="footer-link">Tableau de bord</router-link>
              </li>
              <li>
                <router-link to="/cards" class="footer-link">Mes cartes</router-link>
              </li>
              <li>
                <router-link to="/transactions" class="footer-link">Transactions</router-link>
              </li>
              <li>
                <router-link to="/statements" class="footer-link">Relevés bancaires</router-link>
              </li>
              <li>
                <router-link to="/loans" class="footer-link">Prêts</router-link>
              </li>
            </ul>
          </div>

          <!-- Services -->
          <div class="footer-section">
            <h4 class="footer-title text-base font-semibold text-gray-900 mb-4">Services</h4>
            <ul class="footer-links space-y-2">
              <li>
                <a href="#" @click.prevent="handleServiceClick('transfers')" class="footer-link">Virements</a>
              </li>
              <li>
                <a href="#" @click.prevent="handleServiceClick('payments')" class="footer-link">Paiements</a>
              </li>
              <li>
                <router-link to="/analytics" class="footer-link">Analyses</router-link>
              </li>
              <li>
                <a href="#" class="footer-link">Assistance</a>
              </li>
              <li>
                <a href="#" class="footer-link">Contact</a>
              </li>
            </ul>
          </div>

          <!-- Informations -->
          <div class="footer-section">
            <h4 class="footer-title text-base font-semibold text-gray-900 mb-4">Informations</h4>
            <ul class="footer-links space-y-2">
              <li>
                <a href="#" class="footer-link">À propos</a>
              </li>
              <li>
                <a href="#" class="footer-link">Mentions légales</a>
              </li>
              <li>
                <a href="#" class="footer-link">Politique de confidentialité</a>
              </li>
              <li>
                <a href="#" class="footer-link">Politique des cookies</a>
              </li>
              <li>
                <a href="#" class="footer-link">Accessibilité</a>
              </li>
              <li>
                <a href="#" class="footer-link">Tarification</a>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <!-- Newsletter Section -->
      <div class="footer-newsletter border-t border-gray-200 py-8">
        <div class="newsletter-content max-w-2xl mx-auto text-center">
          <h4 class="newsletter-title text-lg font-semibold text-gray-900 mb-2">
            Newsletter
          </h4>
          <p class="newsletter-description text-sm text-gray-600 mb-4">
            Inscrivez-vous pour ne rien rater de notre actualité
          </p>
          <form @submit.prevent="handleNewsletterSubmit" class="newsletter-form flex gap-2 max-w-md mx-auto">
            <input
              v-model="newsletterEmail"
              type="email"
              placeholder="Adresse e-mail"
              class="newsletter-input flex-1 px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              required
            />
            <button
              type="submit"
              class="newsletter-btn px-6 py-2 bg-blue-900 text-white rounded-md hover:bg-blue-950 transition-colors font-medium"
            >
              M'inscrire
            </button>
          </form>
        </div>
      </div>

      <!-- Bottom Bar -->
      <div class="footer-bottom border-t border-gray-200 py-6">
        <div class="footer-bottom-content flex flex-col md:flex-row justify-between items-center gap-4">
          <div class="footer-copyright text-sm text-gray-600">
            © {{ currentYear }} Token2Pay. Tous droits réservés.
          </div>
          <div class="footer-legal-links flex flex-wrap gap-4 text-sm">
            <a href="#" class="legal-link text-gray-600 hover:text-gray-900">Réglementation</a>
            <a href="#" class="legal-link text-gray-600 hover:text-gray-900">Médiation bancaire</a>
            <a href="#" class="legal-link text-gray-600 hover:text-gray-900">Mobilité bancaire</a>
            <a href="#" class="legal-link text-gray-600 hover:text-gray-900">Comparateur bancaire</a>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'

const props = defineProps({
  isCollapsed: {
    type: Boolean,
    default: false
  },
  isMobileOpen: {
    type: Boolean,
    default: false
  }
})

const router = useRouter()
const newsletterEmail = ref('')
const currentYear = new Date().getFullYear()

const windowWidth = ref(typeof window !== 'undefined' ? window.innerWidth : 1024)

const updateWindowWidth = () => {
  if (typeof window !== 'undefined') {
    windowWidth.value = window.innerWidth
  }
}

const footerStyle = computed(() => {
  // Sur mobile, le sidebar est en overlay, donc pas de décalage
  if (windowWidth.value < 1024) {
    return {}
  }
  
  // Sur desktop, décaler selon l'état collapsed/expanded
  if (props.isCollapsed) {
    return { marginLeft: '72px' } // Largeur sidebar collapsed
  } else {
    return { marginLeft: '256px' } // Largeur sidebar expanded
  }
})

function handleNewsletterSubmit() {
  if (newsletterEmail.value) {
    alert(`Merci pour votre inscription ! Un email de confirmation a été envoyé à ${newsletterEmail.value}`)
    newsletterEmail.value = ''
  }
}

function handleServiceClick(service) {
  // Emit event for transfer/payment modals
  window.dispatchEvent(new CustomEvent('open-modal', { detail: service }))
}

onMounted(() => {
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
</script>

<style scoped>
.gov-footer {
  @apply bg-white border-t border-gray-200 mt-auto;
}

.footer-container {
  @apply max-w-7xl mx-auto px-4 sm:px-6 lg:px-8;
}

.footer-main {
  @apply py-12;
}

.footer-grid {
  @apply grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8;
}

.footer-section {
  @apply flex flex-col;
}

.footer-logo {
  @apply mb-4;
}

.logo-icon {
  @apply text-2xl;
}

.footer-brand-name {
  @apply text-lg font-bold text-gray-900;
}

.footer-brand-subtitle {
  @apply text-xs text-gray-600;
}

.footer-description {
  @apply text-sm text-gray-600 mb-4;
}

.footer-social {
  @apply flex gap-3;
}

.social-link {
  @apply w-10 h-10 flex items-center justify-center rounded-full bg-gray-100 text-gray-600 
         hover:bg-blue-900 hover:text-white transition-colors duration-200;
}

.footer-title {
  @apply text-base font-semibold text-gray-900 mb-4;
}

.footer-links {
  @apply space-y-2;
}

.footer-link {
  @apply text-sm text-gray-600 hover:text-blue-900 transition-colors duration-200 block;
}

.footer-newsletter {
  @apply border-t border-gray-200 py-8;
}

.newsletter-content {
  @apply max-w-2xl mx-auto text-center;
}

.newsletter-title {
  @apply text-lg font-semibold text-gray-900 mb-2;
}

.newsletter-description {
  @apply text-sm text-gray-600 mb-4;
}

.newsletter-form {
  @apply flex gap-2 max-w-md mx-auto;
}

.newsletter-input {
  @apply flex-1 px-4 py-2 border border-gray-300 rounded-md 
         focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent;
}

.newsletter-btn {
  @apply px-6 py-2 bg-blue-900 text-white rounded-md 
         hover:bg-blue-950 transition-colors font-medium;
}

.footer-bottom {
  @apply border-t border-gray-200 py-6;
}

.footer-bottom-content {
  @apply flex flex-col md:flex-row justify-between items-center gap-4;
}

.footer-copyright {
  @apply text-sm text-gray-600;
}

.footer-legal-links {
  @apply flex flex-wrap gap-4 text-sm;
}

.legal-link {
  @apply text-gray-600 hover:text-gray-900 transition-colors duration-200;
}

/* Responsive */
@media (max-width: 768px) {
  .footer-grid {
    @apply grid-cols-1 gap-6;
  }
  
  .footer-bottom-content {
    @apply flex-col text-center;
  }
}
</style>
