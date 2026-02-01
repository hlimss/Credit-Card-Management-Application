<template>
  <div class="gov-dashboard-stats">
    <div class="stats-grid">
      <div
        v-for="stat in stats"
        :key="stat.label"
        class="kpi-card"
        :style="{ '--border-color': stat.borderColor }"
      >
        <div class="kpi-card-border"></div>
        <div class="kpi-card-content">
          <div class="kpi-icon" :style="{ 'background': stat.iconBg }">
            <span class="kpi-icon-emoji">{{ stat.icon }}</span>
          </div>
          <div class="kpi-info">
            <div class="kpi-value">{{ stat.value }}</div>
            <div class="kpi-label">{{ stat.label }}</div>
            <div v-if="stat.change" class="kpi-change" :class="stat.changeType">
              <span class="kpi-change-icon">{{ stat.changeType === 'positive' ? '↑' : '↓' }}</span>
              <span class="kpi-change-value">{{ stat.change }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useCreditCardStore } from '../stores/creditCard'

const creditCardStore = useCreditCardStore()

const stats = computed(() => {
  const cards = creditCardStore.cards || []
  const totalCards = cards.length
  const visaCards = cards.filter(c => c.cardType === 'Visa').length
  const mastercardCards = cards.filter(c => c.cardType === 'MasterCard').length
  const expiredCards = cards.filter(c => {
    if (!c.expirationDate) return false
    const [month, year] = c.expirationDate.split('/')
    if (!month || !year) return false
    const expDate = new Date(2000 + parseInt(year), parseInt(month) - 1)
    return expDate < new Date()
  }).length

  return [
    {
      icon: '💳',
      value: totalCards,
      label: 'Total des Cartes',
      borderColor: '#1E3A8A',
      iconBg: 'linear-gradient(135deg, #3B82F6 0%, #1E3A8A 100%)',
      change: totalCards > 0 ? '+2' : null,
      changeType: 'positive'
    },
    {
      icon: '🔵',
      value: visaCards,
      label: 'Cartes Visa',
      borderColor: '#1D4ED8',
      iconBg: 'linear-gradient(135deg, #2563EB 0%, #1D4ED8 100%)',
      change: null,
      changeType: 'positive'
    },
    {
      icon: '🟠',
      value: mastercardCards,
      label: 'Cartes Mastercard',
      borderColor: '#DC2626',
      iconBg: 'linear-gradient(135deg, #EF4444 0%, #DC2626 100%)',
      change: null,
      changeType: 'positive'
    },
    {
      icon: '⚠️',
      value: expiredCards,
      label: 'Cartes Expirées',
      borderColor: '#F59E0B',
      iconBg: 'linear-gradient(135deg, #F59E0B 0%, #D97706 100%)',
      change: expiredCards > 0 ? 'Attention' : null,
      changeType: expiredCards > 0 ? 'warning' : 'positive'
    }
  ]
})
</script>

<style scoped>
.gov-dashboard-stats {
  @apply mb-8;
}

.stats-grid {
  @apply grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6;
}

.kpi-card {
  @apply bg-white rounded-lg shadow-sm border border-gray-200 relative overflow-hidden;
  @apply transition-shadow duration-200 hover:shadow-md;
  min-height: 120px;
}

.kpi-card-border {
  @apply absolute top-0 left-0 right-0 h-1;
  background: var(--border-color, #1E3A8A);
}

.kpi-card-content {
  @apply flex items-start gap-4 p-6;
}

.kpi-icon {
  @apply w-12 h-12 rounded-md flex items-center justify-center flex-shrink-0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.kpi-icon-emoji {
  @apply text-2xl;
}

.kpi-info {
  @apply flex-1 min-w-0;
}

.kpi-value {
  @apply text-3xl font-bold text-gray-900 mb-1;
  line-height: 1.2;
}

.kpi-label {
  @apply text-sm text-gray-600 font-medium mb-2;
}

.kpi-change {
  @apply flex items-center gap-1 text-xs font-semibold;
}

.kpi-change.positive {
  @apply text-green-600;
}

.kpi-change.warning {
  @apply text-yellow-600;
}

.kpi-change-icon {
  @apply text-xs;
}

.kpi-change-value {
  @apply text-xs;
}
</style>
