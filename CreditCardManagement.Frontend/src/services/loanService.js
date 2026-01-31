import api from './api'

export const loanService = {
  async create(loanData) {
    const response = await api.post('/api/loans', loanData)
    return response.data
  },

  async getAll() {
    const response = await api.get('/api/loans')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/api/loans/${id}`)
    return response.data
  },

  async makePayment(id, amount) {
    const response = await api.post(`/api/loans/${id}/payment`, amount)
    return response.data
  },

  async delete(id) {
    const response = await api.delete(`/api/loans/${id}`)
    return response.data
  }
}
