import api from './api'

export const statementService = {
  async generate(statementData) {
    const response = await api.post('/api/statements', statementData)
    return response.data
  },

  async getAll(creditCardId) {
    const params = creditCardId ? { creditCardId } : {}
    const response = await api.get('/api/statements', { params })
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/api/statements/${id}`)
    return response.data
  }
}
