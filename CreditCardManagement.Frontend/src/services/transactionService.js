import axios from 'axios'

const API_URL = 'http://localhost:5000/api'

const transactionService = {
  async getTransactions(cardId) {
    const token = localStorage.getItem('token')
    const response = await axios.get(`${API_URL}/transactions/card/${cardId}`, {
      headers: { Authorization: `Bearer ${token}` }
    })
    return response.data
  },

  async getCardExpenses(cardId) {
    const token = localStorage.getItem('token')
    const response = await axios.get(`${API_URL}/transactions/card/${cardId}/expenses`, {
      headers: { Authorization: `Bearer ${token}` }
    })
    return response.data
  },

  async getAllTransactions(startDate, endDate) {
    const token = localStorage.getItem('token')
    const params = {}
    if (startDate) params.startDate = startDate
    if (endDate) params.endDate = endDate
    const response = await axios.get(`${API_URL}/transactions`, {
      headers: { Authorization: `Bearer ${token}` },
      params
    })
    return response.data
  },

  async getExpenseAnalytics(startDate, endDate) {
    const token = localStorage.getItem('token')
    const params = {}
    if (startDate) params.startDate = startDate
    if (endDate) params.endDate = endDate
    const response = await axios.get(`${API_URL}/transactions/analytics`, {
      headers: { Authorization: `Bearer ${token}` },
      params
    })
    return response.data
  },

  async createTransaction(transactionData) {
    const token = localStorage.getItem('token')
    const response = await axios.post(`${API_URL}/transactions`, transactionData, {
      headers: { Authorization: `Bearer ${token}` }
    })
    return response.data
  },

  async updateTransaction(id, transactionData) {
    const token = localStorage.getItem('token')
    const response = await axios.put(`${API_URL}/transactions/${id}`, transactionData, {
      headers: { Authorization: `Bearer ${token}` }
    })
    return response.data
  },

  async deleteTransaction(id) {
    const token = localStorage.getItem('token')
    await axios.delete(`${API_URL}/transactions/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    })
  }
}

export default transactionService
