<template>
  <div v-if="alerts.length > 0" class="expiration-alerts">
    <div class="alerts-header">
      <h3 class="alerts-title">⚠️ Alertes d'Expiration</h3>
      <span class="alerts-count">{{ alerts.length }}</span>
    </div>
    <div class="alerts-list">
      <div
        v-for="alert in alerts"
        :key="alert.cardId"
        class="alert-item"
        :class="{ 'urgent': alert.daysUntilExpiration <= 7 }"
      >
        <div class="alert-icon">⏰</div>
        <div class="alert-content">
          <p class="alert-card">{{ alert.cardholderName }} ({{ alert.cardType }})</p>
          <p class="alert-message">
            Expire dans <strong>{{ alert.daysUntilExpiration }} jour{{ alert.daysUntilExpiration > 1 ? 's' : '' }}</strong>
            • {{ alert.expirationDate }}
          </p>
          <span class="alert-category">{{ alert.category }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const alerts = ref([])
const loading = ref(true)

const loadAlerts = async () => {
  try {
    const token = localStorage.getItem('token')
    const response = await axios.get('http://localhost:5000/api/notifications/expiring-cards', {
      headers: { Authorization: `Bearer ${token}` }
    })
    alerts.value = response.data
  } catch (error) {
    console.error('Error loading expiration alerts:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadAlerts()
  // Refresh alerts every 5 minutes
  setInterval(loadAlerts, 5 * 60 * 1000)
})
</script>

<style scoped>
.expiration-alerts {
  background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
  border: 2px solid #f59e0b;
  border-radius: 16px;
  padding: 1.5rem;
  margin: 2rem 0;
  box-shadow: 0 4px 15px rgba(245, 158, 11, 0.2);
}

.alerts-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.alerts-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #92400e;
  margin: 0;
}

.alerts-count {
  background: #f59e0b;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-weight: 700;
  font-size: 0.875rem;
}

.alerts-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.alert-item {
  background: white;
  border-radius: 12px;
  padding: 1rem;
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  transition: all 0.3s;
  border-left: 4px solid #f59e0b;
}

.alert-item:hover {
  transform: translateX(5px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.alert-item.urgent {
  border-left-color: #ef4444;
  background: #fef2f2;
}

.alert-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
}

.alert-content {
  flex: 1;
}

.alert-card {
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.25rem;
}

.alert-message {
  color: #6b7280;
  font-size: 0.875rem;
  margin-bottom: 0.5rem;
}

.alert-message strong {
  color: #f59e0b;
  font-weight: 700;
}

.alert-item.urgent .alert-message strong {
  color: #ef4444;
}

.alert-category {
  display: inline-block;
  background: #f3f4f6;
  color: #6b7280;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 600;
}

@media (max-width: 768px) {
  .expiration-alerts {
    padding: 1rem;
  }
}
</style>
