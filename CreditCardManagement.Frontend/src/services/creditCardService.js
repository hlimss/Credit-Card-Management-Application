import api from './api'

export const creditCardService = {
  async getAll() {
    const response = await api.get('/api/creditcards')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/api/creditcards/${id}`)
    return response.data
  },

  async create(cardData) {
    const response = await api.post('/api/creditcards', cardData)
    return response.data
  },

  async update(id, cardData) {
    const response = await api.put(`/api/creditcards/${id}`, cardData)
    return response.data
  },

  async delete(id) {
    await api.delete(`/api/creditcards/${id}`)
  }
}

