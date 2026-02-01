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
        console.error('Create card error:', error)
        console.error('Error response:', error.response?.data)
        
        let errorMessage = 'Failed to create credit card'
        
        // Vérifier d'abord les erreurs de validation ModelState
        if (error.response?.data) {
          const data = error.response.data
          
          // Si c'est un objet ModelState avec des erreurs de validation
          if (typeof data === 'object' && !data.message && !data.error) {
            const errors = []
            for (const key in data) {
              if (Array.isArray(data[key])) {
                errors.push(...data[key])
              } else if (typeof data[key] === 'string') {
                errors.push(data[key])
              }
            }
            if (errors.length > 0) {
              errorMessage = errors.join(', ')
            }
          } else if (data.message) {
            errorMessage = data.message
          } else if (data.error) {
            errorMessage = data.error
          } else if (typeof data === 'string') {
            errorMessage = data
          }
        } else if (error.message) {
          errorMessage = error.message
        }
        
        this.error = errorMessage
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

