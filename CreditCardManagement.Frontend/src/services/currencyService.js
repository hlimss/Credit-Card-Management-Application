import api from './api'

// Using ExchangeRate-API (free, no API key needed for basic usage)
// Alternative APIs:
// - https://api.exchangerate-api.com/v4/latest/MAD (Free, no key)
// - https://api.frankfurter.app/latest?from=MAD (Free, no key)
// - https://api.exchangerate.host/latest?base=MAD (Free, no key)
const EXCHANGE_RATE_API = 'https://api.exchangerate-api.com/v4/latest/MAD'
const BACKUP_API = 'https://api.frankfurter.app/latest?from=MAD'

export const currencyService = {
  async getExchangeRates() {
    try {
      // Try the free ExchangeRate-API first
      const response = await fetch(EXCHANGE_RATE_API, {
        method: 'GET',
        headers: {
          'Accept': 'application/json'
        }
      })
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }
      
      const data = await response.json()
      
      // Transform the data to match our expected format
      if (data.rates) {
        return {
          base: data.base || 'MAD',
          date: data.date || new Date().toISOString().split('T')[0],
          rates: data.rates
        }
      }
      
      throw new Error('Invalid API response format')
    } catch (error) {
      console.error('Error fetching exchange rates from primary API:', error)
      
      // Try backup API
      try {
        const backupResponse = await fetch(BACKUP_API)
        if (backupResponse.ok) {
          const backupData = await backupResponse.json()
          if (backupData.rates) {
            // Frankfurter API returns rates with MAD as base
            return {
              base: 'MAD',
              date: backupData.date || new Date().toISOString().split('T')[0],
              rates: backupData.rates
            }
          }
        }
      } catch (backupError) {
        console.error('Backup API also failed:', backupError)
      }
      
      // Return fallback rates if all APIs fail
      console.warn('Using fallback exchange rates')
      return this.getFallbackRates()
    }
  },

  getFallbackRates() {
    // Fallback rates (approximate values)
    return {
      base: 'MAD',
      date: new Date().toISOString().split('T')[0],
      rates: {
        USD: 0.1,
        EUR: 0.092,
        GBP: 0.079,
        JPY: 14.5,
        CAD: 0.135,
        AUD: 0.15,
        CHF: 0.09,
        CNY: 0.72,
        AED: 0.367,
        SAR: 0.375,
        TND: 0.31,
        DZD: 13.5
      }
    }
  },

  async convertCurrency(amount, fromCurrency, toCurrency) {
    try {
      const rates = await this.getExchangeRates()
      
      if (fromCurrency === 'MAD') {
        // Converting from MAD to another currency
        const rate = rates.rates[toCurrency]
        if (!rate) throw new Error(`Rate not found for ${toCurrency}`)
        return amount * rate
      } else if (toCurrency === 'MAD') {
        // Converting to MAD from another currency
        const rate = rates.rates[fromCurrency]
        if (!rate) throw new Error(`Rate not found for ${fromCurrency}`)
        return amount / rate
      } else {
        // Converting between two non-MAD currencies (via MAD)
        const fromRate = rates.rates[fromCurrency]
        const toRate = rates.rates[toCurrency]
        if (!fromRate || !toRate) {
          throw new Error(`Rates not found for conversion`)
        }
        // Convert: from -> MAD -> to
        const madAmount = amount / fromRate
        return madAmount * toRate
      }
    } catch (error) {
      console.error('Conversion error:', error)
      throw error
    }
  }
}
