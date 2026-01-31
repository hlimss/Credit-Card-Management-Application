import { defineStore } from 'pinia'
import { authService } from '../services/authService'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || null,
    user: JSON.parse(localStorage.getItem('user') || 'null')
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    userEmail: (state) => state.user?.email || null,
    userFullName: (state) => state.user?.fullName || state.user?.firstName || null,
    userFirstName: (state) => state.user?.firstName || null,
    userLastName: (state) => state.user?.lastName || null
  },

  actions: {
    async register(email, password, confirmPassword, firstName, lastName, phoneNumber) {
      try {
        const response = await authService.register(email, password, confirmPassword, firstName, lastName, phoneNumber)
        this.setAuth(response)
        return { success: true }
      } catch (error) {
        console.error('Registration error:', error)
        let errorMessage = 'Registration failed'
        
        if (error.response) {
          // Backend returned an error response
          errorMessage = error.response.data?.message || 
                        error.response.data?.error || 
                        error.response.statusText ||
                        'Registration failed'
          
          // Check for validation errors
          if (error.response.data?.errors) {
            const validationErrors = Object.values(error.response.data.errors).flat()
            errorMessage = validationErrors.join(', ') || errorMessage
          }
        } else if (error.request) {
          // Request was made but no response received
          errorMessage = 'Cannot connect to server. Please check if the backend is running.'
        } else {
          // Error in request setup
          errorMessage = error.message || 'Registration failed'
        }
        
        return {
          success: false,
          error: errorMessage
        }
      }
    },

    async login(email, password) {
      try {
        const response = await authService.login(email, password)
        this.setAuth(response)
        // Fetch full user details after login
        await this.fetchUserDetails()
        return { success: true }
      } catch (error) {
        return {
          success: false,
          error: error.response?.data?.message || 'Login failed'
        }
      }
    },

    async logout() {
      try {
        await authService.logout()
      } catch (error) {
        console.error('Logout error:', error)
      } finally {
        this.clearAuth()
      }
    },

    setAuth(authData) {
      this.token = authData.token
      this.user = {
        email: authData.email,
        userId: authData.userId,
        firstName: authData.firstName,
        lastName: authData.lastName,
        fullName: authData.fullName || `${authData.firstName} ${authData.lastName}`
      }
      localStorage.setItem('token', authData.token)
      localStorage.setItem('user', JSON.stringify(this.user))
    },

    async fetchUserDetails() {
      try {
        const response = await authService.getCurrentUser()
        if (response.fullName) {
          this.user = {
            ...this.user,
            firstName: response.firstName,
            lastName: response.lastName,
            fullName: response.fullName
          }
          localStorage.setItem('user', JSON.stringify(this.user))
        }
      } catch (error) {
        console.error('Error fetching user details:', error)
      }
    },

    clearAuth() {
      this.token = null
      this.user = null
      localStorage.removeItem('token')
      localStorage.removeItem('user')
    }
  }
})

