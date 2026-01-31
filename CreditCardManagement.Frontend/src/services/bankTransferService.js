import api from './api'

export const bankTransferService = {
  async create(transferData) {
    const response = await api.post('/api/banktransfers', transferData)
    return response.data
  },

  async getAll(startDate, endDate) {
    const params = {}
    if (startDate) params.startDate = startDate
    if (endDate) params.endDate = endDate
    const response = await api.get('/api/banktransfers', { params })
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/api/banktransfers/${id}`)
    return response.data
  },

  async cancel(id) {
    const response = await api.post(`/api/banktransfers/${id}/cancel`)
    return response.data
  }
}
