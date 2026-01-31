<template>
  <div class="oauth-callback-page min-h-screen bg-gradient-to-br from-blue-50 via-purple-50 to-pink-50 flex items-center justify-center">
    <div class="callback-container">
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p class="loading-text">Completing authentication...</p>
      </div>
      <div v-else-if="error" class="error-state">
        <div class="error-icon">⚠️</div>
        <h2 class="error-title">Authentication Failed</h2>
        <p class="error-message">{{ error }}</p>
        <router-link to="/login" class="back-btn">Back to Login</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const loading = ref(true)
const error = ref('')

onMounted(async () => {
  try {
    const token = route.query.token
    const email = route.query.email
    const firstName = route.query.firstName
    const lastName = route.query.lastName

    if (!token || !email) {
      error.value = 'Missing authentication data'
      loading.value = false
      return
    }

    // Set authentication data
    authStore.setAuth({
      token,
      email,
      firstName,
      lastName,
      fullName: `${firstName} ${lastName}`,
      userId: null // Will be extracted from token if needed
    })

    // Redirect to home
    setTimeout(() => {
      router.push('/')
    }, 1000)
  } catch (err) {
    console.error('OAuth callback error:', err)
    error.value = 'An error occurred during authentication'
    loading.value = false
  }
})
</script>

<style scoped>
.oauth-callback-page {
  padding: 2rem;
}

.callback-container {
  text-align: center;
  max-width: 400px;
  width: 100%;
}

.loading-state,
.error-state {
  background: white;
  border-radius: 24px;
  padding: 3rem 2rem;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
}

.spinner {
  width: 50px;
  height: 50px;
  border: 4px solid #e5e7eb;
  border-top-color: #667eea;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1.5rem;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.loading-text {
  color: #6b7280;
  font-size: 1.1rem;
}

.error-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
}

.error-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.error-message {
  color: #6b7280;
  margin-bottom: 1.5rem;
}

.back-btn {
  display: inline-block;
  padding: 0.75rem 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 12px;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.3s;
}

.back-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}
</style>
