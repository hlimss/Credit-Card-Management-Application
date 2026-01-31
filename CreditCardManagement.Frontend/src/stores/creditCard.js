import { defineStore } from 'pinia'
import { creditCardService } from '../services/creditCardService'

export const useCreditCardStore = defineStore('creditCard', {
  state: () => ({
    cards: [],
    loading: false,
    error: null
  }),

  actions: {
    async fetchCards() {
      this.loading = true
      this.error = null
      try {
        this.cards = await creditCardService.getAll()
      } catch (error) {
        this.error = error.response?.data?.message || 'Failed to fetch credit cards'
        throw error
      } finally {
        this.loading = false
      }
    },

    async createCard(cardData) {
      this.loading = true
      this.error = null
      try {
        const newCard = await creditCardService.create(cardData)
        this.cards.unshift(newCard)
        return { success: true, card: newCard }
      } catch (error) {
        const errorMessage = error.response?.data?.message || 
                           error.response?.data?.error || 
                           error.message || 
                           'Failed to create credit card'
        this.error = errorMessage
        console.error('Create card error:', error.response?.data || error)
        return { success: false, error: errorMessage }
      } finally {
        this.loading = false
      }
    },

    async updateCard(id, cardData) {
      this.loading = true
      this.error = null
      try {
        const updatedCard = await creditCardService.update(id, cardData)
        const index = this.cards.findIndex(c => c.id === id)
        if (index !== -1) {
          this.cards[index] = updatedCard
        }
        return { success: true, card: updatedCard }
      } catch (error) {
        this.error = error.response?.data?.message || 'Failed to update credit card'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    },

    async deleteCard(id) {
      this.loading = true
      this.error = null
      try {
        await creditCardService.delete(id)
        this.cards = this.cards.filter(c => c.id !== id)
        return { success: true }
      } catch (error) {
        this.error = error.response?.data?.message || 'Failed to delete credit card'
        return { success: false, error: this.error }
      } finally {
        this.loading = false
      }
    }
  }
})

