<template>
  <div class="gov-credit-card-container">
    <div 
      class="gov-credit-card"
      :class="[`card-${cardType.toLowerCase()}`, { 'card-revealed': showDetails }]"
      :style="cardStyle"
    >
      <div class="card-inner">
        <!-- Card Header -->
        <div class="card-header">
          <div class="card-chip">
            <div class="chip-lines">
              <div class="chip-line"></div>
              <div class="chip-line"></div>
              <div class="chip-line"></div>
              <div class="chip-line"></div>
            </div>
          </div>
          <div class="card-logo">
            <div v-if="cardType === 'Visa'" class="visa-logo">VISA</div>
            <div v-else-if="cardType === 'MasterCard'" class="mastercard-logo">
              <div class="mc-circle mc-circle-1"></div>
              <div class="mc-circle mc-circle-2"></div>
            </div>
            <div v-else-if="cardType === 'American Express'" class="amex-logo">AMEX</div>
            <div v-else class="default-logo">{{ cardType }}</div>
          </div>
        </div>

        <!-- Card Number -->
        <div class="card-number-section">
          <div class="card-number-label">Numéro de carte</div>
          <div class="card-number">
            <button
              v-if="!showDetails"
              @click="toggleDetails"
              class="card-reveal-btn"
              aria-label="Afficher le numéro de carte"
            >
              <span class="card-number-masked">{{ maskedCardNumber }}</span>
              <svg class="w-4 h-4 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
              </svg>
            </button>
            <span v-else class="card-number-visible">{{ formattedCardNumber }}</span>
          </div>
        </div>

        <!-- Card Info -->
        <div class="card-info-section">
          <div class="card-holder">
            <div class="card-info-label">Titulaire</div>
            <div class="card-info-value">{{ cardholderName.toUpperCase() }}</div>
          </div>
          <div class="card-expiry">
            <div class="card-info-label">Expiration</div>
            <div class="card-info-value">
              <button
                v-if="!showDetails"
                @click="toggleDetails"
                class="card-reveal-btn-inline"
                aria-label="Afficher la date d'expiration"
              >
                <span>••/••</span>
                <svg class="w-3 h-3 ml-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
              </button>
              <span v-else>{{ expirationDate }}</span>
            </div>
          </div>
          <div v-if="cvv" class="card-cvv">
            <div class="card-info-label">CVV</div>
            <div class="card-info-value">
              <button
                v-if="!showDetails"
                @click="toggleDetails"
                class="card-reveal-btn-inline"
                aria-label="Afficher le CVV"
              >
                <span>•••</span>
                <svg class="w-3 h-3 ml-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
              </button>
              <span v-else>{{ cvv }}</span>
            </div>
          </div>
        </div>

        <!-- Balance (si disponible) -->
        <div v-if="balance !== undefined" class="card-balance">
          <div class="card-balance-label">Solde disponible</div>
          <div class="card-balance-value">{{ formatBalance(balance) }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  cardNumber: String,
  cardholderName: String,
  expirationDate: String,
  cardType: String,
  cvv: String,
  balance: Number
})

const showDetails = ref(false)

const formattedCardNumber = computed(() => {
  if (!props.cardNumber) return '•••• •••• •••• ••••'
  return props.cardNumber.replace(/(.{4})/g, '$1 ').trim()
})

const maskedCardNumber = computed(() => {
  if (!props.cardNumber) return '•••• •••• •••• ••••'
  const last4 = props.cardNumber.slice(-4)
  return `•••• •••• •••• ${last4}`
})

const cardStyle = computed(() => {
  return {
    background: getCardGradient(props.cardType)
  }
})

function getCardGradient(cardType) {
  const gradients = {
    'Visa': 'linear-gradient(135deg, #1E3A8A 0%, #3B82F6 100%)',
    'MasterCard': 'linear-gradient(135deg, #1E293B 0%, #475569 100%)',
    'American Express': 'linear-gradient(135deg, #0F172A 0%, #1E293B 100%)',
    'Discover': 'linear-gradient(135deg, #1E3A8A 0%, #2563EB 100%)',
    'default': 'linear-gradient(135deg, #1E3A8A 0%, #64748B 100%)'
  }
  return gradients[cardType] || gradients.default
}

function toggleDetails() {
  showDetails.value = !showDetails.value
}

function formatBalance(balance) {
  if (balance === null || balance === undefined) return 'N/A'
  return new Intl.NumberFormat('fr-FR', {
    style: 'currency',
    currency: 'MAD'
  }).format(balance)
}
</script>

<style scoped>
.gov-credit-card-container {
  @apply w-full max-w-md mx-auto;
  perspective: 1000px;
}

.gov-credit-card {
  @apply relative w-full h-36 rounded-lg overflow-hidden text-white;
  background: linear-gradient(135deg, #1E3A8A 0%, #3B82F6 100%);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.gov-credit-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.2);
}

.card-inner {
  @apply p-3 h-full flex flex-col justify-between;
}

.card-header {
  @apply flex items-start justify-between mb-4;
}

.card-chip {
  @apply w-9 h-7 bg-gradient-to-br from-yellow-400 to-yellow-600 rounded-md 
         flex items-center justify-center relative overflow-hidden;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.chip-lines {
  @apply w-full h-full flex flex-col justify-center gap-1 px-2;
}

.chip-line {
  @apply h-0.5 bg-yellow-800 rounded-full;
}

.card-logo {
  @apply flex items-center justify-end;
}

.visa-logo {
  @apply text-white font-bold text-lg tracking-wider;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}

.mastercard-logo {
  @apply relative w-12 h-12;
}

.mc-circle {
  @apply absolute rounded-full;
  width: 24px;
  height: 24px;
}

.mc-circle-1 {
  @apply bg-red-500 left-0;
}

.mc-circle-2 {
  @apply bg-orange-500 right-0;
}

.amex-logo {
  @apply text-white font-bold text-sm tracking-wider;
}

.default-logo {
  @apply text-white font-semibold text-xs;
}

.card-number-section {
  @apply mb-4;
}

.card-number-label {
  @apply text-xs text-white/70 mb-1 font-medium;
}

.card-number {
  @apply text-sm font-mono font-semibold tracking-wider;
}

.card-number-masked {
  @apply font-mono;
}

.card-reveal-btn {
  @apply flex items-center text-white hover:text-white/80 transition-colors;
}

.card-number-visible {
  @apply font-mono tracking-wider;
}

.card-info-section {
  @apply flex items-end justify-between gap-4;
}

.card-holder,
.card-expiry,
.card-cvv {
  @apply flex-1;
}

.card-info-label {
  @apply text-xs text-white/70 mb-1 font-medium;
}

.card-info-value {
  @apply text-sm font-semibold;
}

.card-reveal-btn-inline {
  @apply flex items-center text-white hover:text-white/80 transition-colors;
}

.card-balance {
  @apply mt-4 pt-4 border-t border-white/20;
}

.card-balance-label {
  @apply text-xs text-white/70 mb-1 font-medium;
}

.card-balance-value {
  @apply text-lg font-bold;
}

/* Responsive */
@media (max-width: 640px) {
  .gov-credit-card {
    @apply h-52;
  }
  
  .card-inner {
    @apply p-4;
  }
  
  .card-number {
    @apply text-lg;
  }
}
</style>
