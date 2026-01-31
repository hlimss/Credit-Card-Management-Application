<template>
  <div class="currency-converter-section">
    <div class="section-header">
      <h2 class="section-title">
        <span class="title-icon">💱</span>
        Currency Exchange & Converter
      </h2>
      <p class="section-subtitle">
        Real-time exchange rates (MAD - Moroccan Dirham)
        <span v-if="lastUpdate" class="update-time">
          • Last updated: {{ formatTime(lastUpdate) }}
        </span>
        <button 
          @click="fetchExchangeRates" 
          class="refresh-btn"
          :disabled="loading"
          title="Refresh rates"
        >
          {{ loading ? '⏳' : '🔄' }}
        </button>
      </p>
    </div>

    <!-- Exchange Rates Display -->
    <div class="exchange-rates">
      <div v-if="loading && !exchangeRatesData" class="loading-rates">
        <div class="spinner-small"></div>
        <span>Loading exchange rates...</span>
      </div>
      <div v-else-if="exchangeRates.length > 0" class="rates-grid">
        <div
          v-for="rate in exchangeRates"
          :key="rate.code"
          class="rate-card"
          :class="{ 'highlight': rate.code === 'USD' || rate.code === 'EUR' }"
        >
          <div class="rate-flag">{{ rate.flag }}</div>
          <div class="rate-info">
            <div class="rate-code">{{ rate.code }}</div>
            <div class="rate-name">{{ rate.name }}</div>
          </div>
          <div class="rate-value">
            <span class="rate-amount">{{ formatRate(rate.rate) }}</span>
            <span class="rate-label">MAD</span>
          </div>
        </div>
      </div>
      <div v-else class="error-rates">
        <span>⚠️ Unable to load exchange rates. Using fallback values.</span>
      </div>
    </div>

    <!-- Currency Converter -->
    <div class="converter-container">
      <div class="converter-card">
        <h3 class="converter-title">Currency Converter</h3>
        <div class="converter-form">
          <div class="converter-row">
            <div class="converter-input-group">
              <label class="converter-label">From</label>
              <select v-model="fromCurrency" class="converter-select">
                <option v-for="currency in currencies" :key="currency.code" :value="currency.code">
                  {{ currency.flag }} {{ currency.code }} - {{ currency.name }}
                </option>
              </select>
              <input
                v-model.number="fromAmount"
                type="number"
                placeholder="0.00"
                class="converter-input"
                @input="calculateConversion"
              />
            </div>

            <div class="swap-button-container">
              <button @click="swapCurrencies" class="swap-button" title="Swap currencies">
                ⇄
              </button>
            </div>

            <div class="converter-input-group">
              <label class="converter-label">To</label>
              <select v-model="toCurrency" class="converter-select">
                <option v-for="currency in currencies" :key="currency.code" :value="currency.code">
                  {{ currency.flag }} {{ currency.code }} - {{ currency.name }}
                </option>
              </select>
              <input
                v-model.number="toAmount"
                type="number"
                placeholder="0.00"
                class="converter-input"
                @input="calculateReverseConversion"
                readonly
              />
            </div>
          </div>

          <div class="conversion-result">
            <div class="result-text">
              <span class="result-amount">{{ formatCurrency(fromAmount, fromCurrency) }}</span>
              <span class="result-equals">=</span>
              <span class="result-amount highlight">{{ formatCurrency(toAmount, toCurrency) }}</span>
            </div>
            <div class="result-rate">
              Rate: 1 {{ fromCurrency }} = {{ getExchangeRate(fromCurrency, toCurrency) }} {{ toCurrency }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { currencyService } from '../services/currencyService'

const fromCurrency = ref('USD')
const toCurrency = ref('MAD')
const fromAmount = ref(1)
const toAmount = ref(0)
const loading = ref(false)
const lastUpdate = ref(null)
const exchangeRatesData = ref(null)

const currencies = [
  { code: 'MAD', name: 'Moroccan Dirham', flag: '🇲🇦' },
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

const exchangeRates = computed(() => {
  if (!exchangeRatesData.value) return []
  
  return currencies
    .filter(c => c.code !== 'MAD' && exchangeRatesData.value.rates[c.code])
    .map(currency => ({
      ...currency,
      rate: exchangeRatesData.value.rates[currency.code].toFixed(4)
    }))
    .sort((a, b) => {
      // Sort by importance: USD, EUR first, then others
      if (a.code === 'USD') return -1
      if (b.code === 'USD') return 1
      if (a.code === 'EUR') return -1
      if (b.code === 'EUR') return 1
      return a.code.localeCompare(b.code)
    })
})

async function fetchExchangeRates() {
  loading.value = true
  try {
    const data = await currencyService.getExchangeRates()
    exchangeRatesData.value = data
    lastUpdate.value = new Date()
    // Recalculate conversion with new rates
    await calculateConversion()
  } catch (error) {
    console.error('Error fetching exchange rates:', error)
  } finally {
    loading.value = false
  }
}

function formatRate(rate) {
  return parseFloat(rate).toFixed(4)
}

function getExchangeRate(from, to) {
  if (!exchangeRatesData.value) return '0.0000'
  
  const rates = exchangeRatesData.value.rates
  
  if (from === 'MAD') {
    return rates[to] ? rates[to].toFixed(4) : '0.0000'
  } else if (to === 'MAD') {
    return rates[from] ? (1 / rates[from]).toFixed(4) : '0.0000'
  } else {
    // Convert from -> MAD -> to
    if (!rates[from] || !rates[to]) return '0.0000'
    return ((1 / rates[from]) * rates[to]).toFixed(4)
  }
}

async function calculateConversion() {
  if (!fromAmount.value || fromAmount.value <= 0) {
    toAmount.value = 0
    return
  }
  
  if (!exchangeRatesData.value) {
    await fetchExchangeRates()
    return
  }
  
  try {
    const result = await currencyService.convertCurrency(
      fromAmount.value,
      fromCurrency.value,
      toCurrency.value
    )
    toAmount.value = parseFloat(result.toFixed(2))
  } catch (error) {
    console.error('Conversion error:', error)
    toAmount.value = 0
  }
}

function calculateReverseConversion() {
  // This is readonly, so we don't calculate here
}

function swapCurrencies() {
  const temp = fromCurrency.value
  fromCurrency.value = toCurrency.value
  toCurrency.value = temp
  
  const tempAmount = fromAmount.value
  fromAmount.value = toAmount.value
  toAmount.value = tempAmount
}

function formatCurrency(amount, currency) {
  if (!amount || amount === 0) return '0.00'
  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

function formatTime(date) {
  if (!date) return ''
  return new Intl.DateTimeFormat('en-US', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  }).format(date)
}

// Watch for currency changes
watch([fromCurrency, toCurrency], () => {
  calculateConversion()
})

onMounted(async () => {
  await fetchExchangeRates()
  // Auto-refresh every 5 minutes
  setInterval(fetchExchangeRates, 5 * 60 * 1000)
})
</script>

<style scoped>
.currency-converter-section {
  background: white;
  border-radius: 20px;
  padding: 2rem;
  margin-bottom: 2rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07);
}

.section-header {
  margin-bottom: 2rem;
  text-align: center;
}

.section-title {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  font-size: 1.75rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.title-icon {
  font-size: 2rem;
}

.section-subtitle {
  color: #6b7280;
  font-size: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.update-time {
  color: #10b981;
  font-weight: 500;
}

.refresh-btn {
  padding: 0.25rem 0.5rem;
  background: #f3f4f6;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 1rem;
}

.refresh-btn:hover:not(:disabled) {
  background: #667eea;
  border-color: #667eea;
  transform: rotate(180deg);
}

.refresh-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.exchange-rates {
  margin-bottom: 2rem;
}

.loading-rates {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  padding: 2rem;
  color: #6b7280;
}

.spinner-small {
  width: 20px;
  height: 20px;
  border: 2px solid #e5e7eb;
  border-top-color: #667eea;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

.error-rates {
  text-align: center;
  padding: 1rem;
  background: #fef2f2;
  border: 1px solid #fecaca;
  border-radius: 12px;
  color: #991b1b;
}

.rates-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
}

.rate-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: #f9fafb;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  transition: all 0.3s;
}

.rate-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  border-color: #667eea;
}

.rate-card.highlight {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-color: transparent;
}

.rate-flag {
  font-size: 2rem;
  line-height: 1;
}

.rate-info {
  flex: 1;
}

.rate-code {
  font-size: 1rem;
  font-weight: 700;
  color: inherit;
}

.rate-name {
  font-size: 0.75rem;
  opacity: 0.8;
  color: inherit;
}

.rate-value {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.rate-amount {
  font-size: 1.25rem;
  font-weight: 700;
  color: inherit;
}

.rate-label {
  font-size: 0.75rem;
  opacity: 0.8;
  color: inherit;
}

.converter-container {
  margin-top: 2rem;
}

.converter-card {
  background: linear-gradient(135deg, #f9fafb 0%, #ffffff 100%);
  border: 2px solid #e5e7eb;
  border-radius: 16px;
  padding: 2rem;
}

.converter-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 1.5rem;
  text-align: center;
}

.converter-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.converter-row {
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  gap: 1rem;
  align-items: end;
}

.converter-input-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.converter-label {
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
}

.converter-select {
  padding: 0.875rem;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  font-size: 1rem;
  background: white;
  cursor: pointer;
  transition: all 0.3s;
}

.converter-select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.converter-input {
  padding: 0.875rem;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  font-size: 1.25rem;
  font-weight: 600;
  transition: all 0.3s;
  background: white;
}

.converter-input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.converter-input[readonly] {
  background: #f9fafb;
  cursor: not-allowed;
}

.swap-button-container {
  display: flex;
  align-items: center;
  justify-content: center;
  padding-bottom: 0.5rem;
}

.swap-button {
  width: 50px;
  height: 50px;
  border: 2px solid #e5e7eb;
  background: white;
  border-radius: 12px;
  font-size: 1.5rem;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.swap-button:hover {
  background: #667eea;
  border-color: #667eea;
  color: white;
  transform: rotate(180deg);
}

.conversion-result {
  text-align: center;
  padding: 1.5rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 12px;
  color: white;
}

.result-text {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  margin-bottom: 0.5rem;
}

.result-amount {
  font-size: 1.5rem;
  font-weight: 700;
}

.result-amount.highlight {
  font-size: 2rem;
}

.result-equals {
  font-size: 1.5rem;
  opacity: 0.9;
}

.result-rate {
  font-size: 0.875rem;
  opacity: 0.9;
}

@media (max-width: 768px) {
  .converter-row {
    grid-template-columns: 1fr;
  }

  .swap-button-container {
    padding: 0.5rem 0;
  }

  .rates-grid {
    grid-template-columns: 1fr;
  }
}
</style>
