import api from './api'

export const paymentService = {
  async create(paymentData) {
    const response = await api.post('/api/payments', paymentData)
    return response.data
  },

  async getAll(startDate, endDate) {
    const params = {}
    if (startDate) params.startDate = startDate
    if (endDate) params.endDate = endDate
    const response = await api.get('/api/payments', { params })
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/api/payments/${id}`)
    return response.data
  },

  async getPaymentTypes() {
    const response = await api.get('/api/payments/types')
    return response.data
  }
}
