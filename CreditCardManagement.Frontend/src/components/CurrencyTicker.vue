<template>
  <div class="currency-ticker-container">
    <div class="ticker-label">
      <span class="ticker-icon">💱</span>
      <span>Live Rates (MAD):</span>
    </div>
    <div class="ticker-wrapper">
      <div class="ticker-content" :style="{ animationDuration: `${tickerSpeed}s` }">
        <!-- Duplicate content for seamless loop -->
        <div
          v-for="rate in [...displayRates, ...displayRates]"
          :key="`${rate.code}-${rate.index}`"
          class="ticker-item"
        >
          <span class="ticker-flag">{{ rate.flag }}</span>
          <span class="ticker-code">{{ rate.code }}</span>
          <span class="ticker-rate">{{ formatRate(rate.rate) }}</span>
        </div>
      </div>
    </div>
    <button
      @click="refreshRates"
      class="ticker-refresh"
      :disabled="loading"
      title="Refresh rates"
    >
      {{ loading ? '⏳' : '🔄' }}
    </button>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { currencyService } from '../services/currencyService'

const loading = ref(false)
const exchangeRatesData = ref(null)
const lastUpdate = ref(null)

const currencies = [
  { code: 'USD', name: 'US Dollar', flag: '🇺🇸' },
  { code: 'EUR', name: 'Euro', flag: '🇪🇺' },
  { code: 'GBP', name: 'British Pound', flag: '🇬🇧' },
  { code: 'JPY', name: 'Japanese Yen', flag: '🇯🇵' },
  { code: 'CAD', name: 'Canadian Dollar', flag: '🇨🇦' },
  { code: 'AUD', name: 'Australian Dollar', flag: '🇦🇺' },
  { code: 'CHF', name: 'Swiss Franc', flag: '🇨🇭' },
  { code: 'CNY', name: 'Chinese Yuan', flag: '🇨🇳' },
  { code: 'AED', name: 'UAE Dirham', flag: '🇦🇪' },
  { code: 'SAR', name: 'Saudi Riyal', flag: '🇸🇦' },
  { code: 'TND', name: 'Tunisian Dinar', flag: '🇹🇳' },
  { code: 'DZD', name: 'Algerian Dinar', flag: '🇩🇿' }
]

const displayRates = computed(() => {
  if (!exchangeRatesData.value) return []
  
  return currencies
    .filter(c => exchangeRatesData.value.rates && exchangeRatesData.value.rates[c.code])
    .map((currency, index) => ({
      ...currency,
      rate: exchangeRatesData.value.rates[currency.code],
      index
    }))
    .sort((a, b) => {
      if (a.code === 'USD') return -1
      if (b.code === 'USD') return 1
      if (a.code === 'EUR') return -1
      if (b.code === 'EUR') return 1
      return a.code.localeCompare(b.code)
    })
})

const tickerSpeed = computed(() => {
  return Math.max(20, displayRates.value.length * 3) // Adjust speed based on number of rates
})

async function refreshRates() {
  loading.value = true
  try {
    const data = await currencyService.getExchangeRates()
    exchangeRatesData.value = data
    lastUpdate.value = new Date()
  } catch (error) {
    console.error('Error fetching exchange rates:', error)
  } finally {
    loading.value = false
  }
}

function formatRate(rate) {
  if (!rate) return '0.0000'
  return parseFloat(rate).toFixed(4)
}

let refreshInterval = null

onMounted(async () => {
  await refreshRates()
  // Auto-refresh every 5 minutes
  refreshInterval = setInterval(refreshRates, 5 * 60 * 1000)
})

onUnmounted(() => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
  }
})
</script>

<style scoped>
.currency-ticker-container {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: linear-gradient(135deg, #1E3A8A 0%, #3B82F6 100%);
  padding: 0.5rem 1rem;
  border-radius: 8px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  width: 100%;
  min-width: 0;
  height: 36px;
}

.ticker-label {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  color: white;
  font-weight: 600;
  font-size: 0.8rem;
  white-space: nowrap;
  flex-shrink: 0;
}

.ticker-icon {
  font-size: 1rem;
}

.ticker-wrapper {
  flex: 1;
  overflow: hidden;
  position: relative;
  height: 32px;
}

.ticker-content {
  display: flex;
  gap: 2rem;
  align-items: center;
  height: 100%;
  animation: scroll-horizontal linear infinite;
  white-space: nowrap;
}

@keyframes scroll-horizontal {
  0% {
    transform: translateX(0);
  }
  100% {
    transform: translateX(-50%);
  }
}

.ticker-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: white;
  font-size: 0.8rem;
  padding: 0.4rem 0.85rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 6px;
  white-space: nowrap;
  flex-shrink: 0;
  backdrop-filter: blur(10px);
  transition: all 0.3s;
}

.ticker-item:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: scale(1.05);
}

.ticker-flag {
  font-size: 1.2rem;
  line-height: 1;
}

.ticker-code {
  font-weight: 700;
  font-size: 1rem;
}

.ticker-rate {
  font-weight: 600;
  color: rgba(255, 255, 255, 0.95);
  font-family: 'Courier New', monospace;
}

.ticker-refresh {
  padding: 0.35rem;
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 6px;
  color: white;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 0.85rem;
  flex-shrink: 0;
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.ticker-refresh:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.3);
  transform: rotate(180deg);
}

.ticker-refresh:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Pause animation on hover */
.currency-ticker-container:hover .ticker-content {
  animation-play-state: paused;
}

@media (max-width: 768px) {
  .currency-ticker-container {
    flex-direction: column;
    gap: 0.75rem;
    padding: 1rem;
  }

  .ticker-label {
    font-size: 0.8rem;
  }

  .ticker-wrapper {
    width: 100%;
  }

  .ticker-item {
    font-size: 0.8rem;
    padding: 0.4rem 0.8rem;
  }
}
</style>
