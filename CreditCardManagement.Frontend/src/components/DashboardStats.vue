<template>
  <div class="dashboard-stats grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
    <div 
      v-for="stat in stats" 
      :key="stat.label"
      class="stat-card"
      :style="{ '--gradient': stat.gradient }"
    >
      <div class="stat-icon">{{ stat.icon }}</div>
      <div class="stat-content">
        <div class="stat-value">{{ stat.value }}</div>
        <div class="stat-label">{{ stat.label }}</div>
      </div>
      <div class="stat-trend" :class="stat.trend">
        <span v-if="stat.trend === 'up'">↑</span>
        <span v-else-if="stat.trend === 'down'">↓</span>
        <span v-if="stat.change">{{ stat.change }}</span>
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
    const [month, year] = c.expirationDate.split('/')
    const expDate = new Date(2000 + parseInt(year), parseInt(month) - 1)
    return expDate < new Date()
  }).length

  return [
    {
      icon: '💳',
      value: totalCards,
      label: 'Total Cards',
      gradient: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
      trend: 'up',
      change: '+2'
    },
    {
      icon: '🔵',
      value: visaCards,
      label: 'Visa Cards',
      gradient: 'linear-gradient(135deg, #1e3c72 0%, #2a5298 100%)',
      trend: 'neutral'
    },
    {
      icon: '🔴',
      value: mastercardCards,
      label: 'MasterCard',
      gradient: 'linear-gradient(135deg, #eb3349 0%, #f45c43 100%)',
      trend: 'neutral'
    },
    {
      icon: '⚠️',
      value: expiredCards,
      label: 'Expired',
      gradient: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
      trend: expiredCards > 0 ? 'down' : 'neutral',
      change: expiredCards > 0 ? 'Action needed' : ''
    }
  ]
})
</script>

<style scoped>
.dashboard-stats {
  margin-bottom: 2rem;
}

.stat-card {
  background: white;
  border-radius: 16px;
  padding: 24px;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07);
  transition: all 0.3s ease;
  cursor: pointer;
}

.stat-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: var(--gradient);
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.15);
}

.stat-card {
  display: flex;
  align-items: center;
  gap: 16px;
}

.stat-icon {
  font-size: 40px;
  line-height: 1;
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 32px;
  font-weight: 700;
  color: #1f2937;
  line-height: 1.2;
}

.stat-label {
  font-size: 14px;
  color: #6b7280;
  margin-top: 4px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.stat-trend {
  font-size: 20px;
  font-weight: 600;
}

.stat-trend.up {
  color: #10b981;
}

.stat-trend.down {
  color: #ef4444;
}

.stat-trend.neutral {
  color: #6b7280;
}
</style>
