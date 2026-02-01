import api from './api'

export const authService = {
  async register(email, password, confirmPassword, firstName, lastName, phoneNumber) {
    const response = await api.post('/api/auth/register', {
      email,
      password,
      confirmPassword,
      firstName,
      lastName,
      phoneNumber
    })
    return response.data
  },

  async login(email, password) {
    const response = await api.post('/api/auth/login', {
      email,
      password
    })
    return response.data
  },

  async logout() {
    try {
      await api.post('/api/auth/logout')
    } catch (error) {
      console.error('Logout error:', error)
    }
  },

  async getCurrentUser() {
    const response = await api.get('/api/auth/me')
    return response.data
  },

  async updateProfile(profileData) {
    const response = await api.put('/api/auth/me', profileData)
    return response.data
  }
}

