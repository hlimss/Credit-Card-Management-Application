<template>
  <div class="login-page min-h-screen bg-gradient-to-br from-blue-50 via-purple-50 to-pink-50 flex items-center justify-center py-4 px-4 sm:px-6 lg:px-8">
    <div class="login-container">
      <!-- Logo Section -->
      <div class="logo-section">
        <Token2PayLogo />
      </div>

      <!-- Login Card with 3D Effect -->
      <div class="login-card-3d">
        <div class="card-inner-3d">
          <div class="card-header">
            <h2 class="card-title">
              <span class="title-icon">🔐</span>
              Welcome Back
            </h2>
            <p class="card-subtitle">Sign in to your Token2Pay account</p>
          </div>

          <form @submit.prevent="handleLogin" class="login-form">
            <!-- Error Display -->
            <div v-if="error" class="form-error-3d">
              <span class="error-icon">⚠️</span>
              <span>{{ error }}</span>
            </div>

            <div class="form-group-3d">
              <label for="email" class="form-label-3d">
                <span class="label-icon">📧</span>
                Email Address
              </label>
              <div class="input-wrapper-3d">
                <input
                  id="email"
                  v-model="email"
                  name="email"
                  type="email"
                  autocomplete="email"
                  required
                  class="form-input-3d"
                  placeholder="your.email@example.com"
                />
                <div class="input-glow"></div>
              </div>
            </div>

            <div class="form-group-3d">
              <label for="password" class="form-label-3d">
                <span class="label-icon">🔒</span>
                Password
              </label>
              <div class="input-wrapper-3d">
                <input
                  id="password"
                  v-model="password"
                  name="password"
                  type="password"
                  autocomplete="current-password"
                  required
                  class="form-input-3d"
                  placeholder="Enter your password"
                />
                <div class="input-glow"></div>
              </div>
            </div>

            <button
              type="submit"
              :disabled="loading"
              class="submit-btn-3d"
            >
              <span v-if="loading" class="btn-spinner"></span>
              <span v-else>Sign In</span>
            </button>

            <!-- OAuth Divider -->
            <div class="oauth-divider">
              <span class="divider-line"></span>
              <span class="divider-text">Or continue with</span>
              <span class="divider-line"></span>
            </div>

            <!-- OAuth Buttons -->
            <div class="oauth-buttons">
              <button
                type="button"
                @click="loginWithGoogle"
                class="oauth-btn google-btn"
              >
                <svg class="oauth-icon" viewBox="0 0 24 24">
                  <path fill="#4285F4" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"/>
                  <path fill="#34A853" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"/>
                  <path fill="#FBBC05" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"/>
                  <path fill="#EA4335" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"/>
                </svg>
                <span>Google</span>
              </button>
              <button
                type="button"
                @click="loginWithFacebook"
                class="oauth-btn facebook-btn"
              >
                <svg class="oauth-icon" viewBox="0 0 24 24" fill="#1877F2">
                  <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z"/>
                </svg>
                <span>Facebook</span>
              </button>
            </div>

            <div class="form-footer">
              <p class="footer-text">
                Don't have an account?
                <router-link to="/register" class="footer-link">
                  Create one now
                </router-link>
              </p>
            </div>
          </form>
        </div>
      </div>

      <!-- Decorative Elements -->
      <div class="floating-shapes">
        <div class="shape shape-1"></div>
        <div class="shape shape-2"></div>
        <div class="shape shape-3"></div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import Token2PayLogo from '../components/Token2PayLogo.vue'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')

// Vérifier les erreurs OAuth dans l'URL
onMounted(() => {
  if (route.query.error === 'google_auth_failed' || route.query.error === 'facebook_auth_failed') {
    const provider = route.query.error === 'google_auth_failed' ? 'Google' : 'Facebook'
    error.value = `${provider} authentication failed. Please try again or use email/password login.`
  }
})

const loginWithGoogle = () => {
  window.location.href = 'http://localhost:5000/api/oauth/google'
}

const loginWithFacebook = () => {
  window.location.href = 'http://localhost:5000/api/oauth/facebook'
}

const handleLogin = async () => {
  error.value = ''
  loading.value = true

  const result = await authStore.login(email.value, password.value)

  if (result.success) {
    router.push('/')
  } else {
    error.value = result.error
  }

  loading.value = false
}
</script>

<style scoped>
.login-page {
  position: relative;
  overflow: hidden;
}

.login-container {
  width: 100%;
  max-width: 500px;
  position: relative;
  z-index: 10;
}

.logo-section {
  margin-bottom: 1.5rem;
  display: flex;
  justify-content: center;
  animation: fadeInDown 0.6s ease-out;
  transform: scale(0.85);
}

@keyframes fadeInDown {
  from {
    opacity: 0;
    transform: translateY(-30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.login-card-3d {
  perspective: 1000px;
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.card-inner-3d {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 24px;
  padding: 2rem;
  box-shadow: 
    0 20px 60px rgba(0, 0, 0, 0.15),
    0 0 0 1px rgba(255, 255, 255, 0.5) inset;
  transform-style: preserve-3d;
  transition: transform 0.6s cubic-bezier(0.23, 1, 0.32, 1);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.login-card-3d:hover .card-inner-3d {
  transform: perspective(1000px) rotateY(2deg) rotateX(-2deg) translateZ(10px);
}

.card-header {
  text-align: center;
  margin-bottom: 1.5rem;
}

.card-title {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-size: 1.5rem;
  font-weight: 800;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin-bottom: 0.25rem;
}

.title-icon {
  font-size: 1.5rem;
  -webkit-text-fill-color: initial;
}

.card-subtitle {
  color: #6b7280;
  font-size: 0.875rem;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group-3d {
  position: relative;
}

.form-label-3d {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.8rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
}

.label-icon {
  font-size: 1.1rem;
}

.input-wrapper-3d {
  position: relative;
}

.form-input-3d {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  font-size: 0.9rem;
  background: white;
  transition: all 0.3s;
  position: relative;
  z-index: 1;
}

.form-input-3d:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 
    0 0 0 4px rgba(102, 126, 234, 0.1),
    0 4px 12px rgba(102, 126, 234, 0.15);
  transform: translateY(-2px);
}

.input-glow {
  position: absolute;
  inset: -2px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 12px;
  opacity: 0;
  transition: opacity 0.3s;
  z-index: 0;
  filter: blur(8px);
}

.form-input-3d:focus ~ .input-glow {
  opacity: 0.3;
}

.form-error-3d {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem 1.25rem;
  background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
  border: 2px solid #f87171;
  border-radius: 12px;
  color: #991b1b;
  font-weight: 500;
  animation: shake 0.5s;
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-10px); }
  75% { transform: translateX(10px); }
}

.error-icon {
  font-size: 1.25rem;
}

.submit-btn-3d {
  width: 100%;
  padding: 0.875rem 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.3s;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.submit-btn-3d::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.3);
  transform: translate(-50%, -50%);
  transition: width 0.6s, height 0.6s;
}

.submit-btn-3d:hover:not(:disabled)::before {
  width: 300px;
  height: 300px;
}

.submit-btn-3d:hover:not(:disabled) {
  transform: translateY(-3px);
  box-shadow: 0 8px 25px rgba(102, 126, 234, 0.5);
}

.submit-btn-3d:active:not(:disabled) {
  transform: translateY(-1px);
}

.submit-btn-3d:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-spinner {
  width: 20px;
  height: 20px;
  border: 3px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.form-footer {
  text-align: center;
  margin-top: 1rem;
}

.footer-text {
  color: #6b7280;
  font-size: 0.9rem;
}

.footer-link {
  color: #667eea;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.3s;
  margin-left: 0.25rem;
}

.footer-link:hover {
  color: #764ba2;
  text-decoration: underline;
}

/* OAuth Styles */
.oauth-divider {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin: 1.5rem 0;
}

.divider-line {
  flex: 1;
  height: 1px;
  background: linear-gradient(90deg, transparent, #e5e7eb, transparent);
}

.divider-text {
  color: #9ca3af;
  font-size: 0.875rem;
  font-weight: 500;
}

.oauth-buttons {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
}

.oauth-btn {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  padding: 0.875rem 1.5rem;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  background: white;
  font-weight: 600;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.3s;
  color: #374151;
}

.oauth-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
}

.oauth-btn.google-btn:hover {
  border-color: #4285F4;
  background: #f8f9fa;
}

.oauth-btn.facebook-btn:hover {
  border-color: #1877F2;
  background: #f8f9fa;
}

.oauth-icon {
  width: 20px;
  height: 20px;
  flex-shrink: 0;
}

/* Floating Shapes */
.floating-shapes {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
  overflow: hidden;
}

.shape {
  position: absolute;
  border-radius: 50%;
  background: linear-gradient(135deg, rgba(102, 126, 234, 0.1) 0%, rgba(118, 75, 162, 0.1) 100%);
  animation: float 20s infinite ease-in-out;
}

.shape-1 {
  width: 300px;
  height: 300px;
  top: -150px;
  right: -150px;
  animation-delay: 0s;
}

.shape-2 {
  width: 200px;
  height: 200px;
  bottom: -100px;
  left: -100px;
  animation-delay: 5s;
}

.shape-3 {
  width: 150px;
  height: 150px;
  top: 50%;
  left: 10%;
  animation-delay: 10s;
}

@keyframes float {
  0%, 100% {
    transform: translate(0, 0) rotate(0deg);
  }
  33% {
    transform: translate(30px, -30px) rotate(120deg);
  }
  66% {
    transform: translate(-20px, 20px) rotate(240deg);
  }
}

/* Responsive */
@media (max-width: 640px) {
  .card-inner-3d {
    padding: 2rem 1.5rem;
  }

  .card-title {
    font-size: 1.5rem;
  }
}
</style>
