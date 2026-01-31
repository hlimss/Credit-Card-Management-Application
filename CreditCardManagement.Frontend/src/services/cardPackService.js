import { creditCardService } from './creditCardService'

// Numéros de cartes de test valides pour chaque type
const testCardNumbers = {
  visa: '4111111111111111',
  mastercard: '5555555555554444',
  amex: '378282246310005',
  discover: '6011111111111117'
}

// Générer un numéro de carte valide basé sur le type
function generateCardNumber(type) {
  return testCardNumbers[type.toLowerCase()] || testCardNumbers.visa
}

// Générer une date d'expiration future
function generateExpirationDate() {
  const now = new Date()
  const year = now.getFullYear() + 3
  const month = String(now.getMonth() + 1).padStart(2, '0')
  return `${month}/${String(year).slice(-2)}`
}

// Générer un CVV
function generateCVV() {
  return String(Math.floor(Math.random() * 900) + 100)
}

export const cardPackService = {
  // Définir les packs de cartes avec leurs configurations
  cardPacks: {
    attijari: {
      'attijari-premium': {
        name: 'Pack Premium',
        cards: [
          {
            cardNumber: generateCardNumber('visa'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Premium',
            tags: 'Premium,Attijari,Cashback',
            balance: 50000
          }
        ],
        description: 'Carte premium avec avantages exclusifs'
      },
      'attijari-classic': {
        name: 'Pack Classic',
        cards: [
          {
            cardNumber: generateCardNumber('visa'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Classic',
            tags: 'Classic,Attijari',
            balance: 20000
          }
        ],
        description: 'Carte classique pour usage quotidien'
      },
      'attijari-business': {
        name: 'Pack Business',
        cards: [
          {
            cardNumber: generateCardNumber('mastercard'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Business',
            tags: 'Business,Attijari,Entreprise',
            balance: 100000
          }
        ],
        description: 'Carte professionnelle avec avantages entreprise'
      }
    },
    bmce: {
      'bmce-gold': {
        name: 'Pack Gold',
        cards: [
          {
            cardNumber: generateCardNumber('visa'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Gold',
            tags: 'Gold,BMCE',
            balance: 40000
          }
        ],
        description: 'Carte Gold avec privilèges'
      },
      'bmce-platinum': {
        name: 'Pack Platinum',
        cards: [
          {
            cardNumber: generateCardNumber('mastercard'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Platinum',
            tags: 'Platinum,BMCE,Premium',
            balance: 75000
          }
        ],
        description: 'Carte Platinum premium'
      }
    },
    cih: {
      'cih-standard': {
        name: 'Pack Standard',
        cards: [
          {
            cardNumber: generateCardNumber('visa'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Standard',
            tags: 'Standard,CIH',
            balance: 15000
          }
        ],
        description: 'Carte standard CIH'
      },
      'cih-premium': {
        name: 'Pack Premium',
        cards: [
          {
            cardNumber: generateCardNumber('mastercard'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Premium',
            tags: 'Premium,CIH',
            balance: 60000
          }
        ],
        description: 'Carte premium CIH'
      }
    },
    bmci: {
      'bmci-classic': {
        name: 'Pack Classic',
        cards: [
          {
            cardNumber: generateCardNumber('visa'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Classic',
            tags: 'Classic,BMCI',
            balance: 25000
          }
        ],
        description: 'Carte classique BMCI'
      }
    },
    credit: {
      'credit-standard': {
        name: 'Pack Standard',
        cards: [
          {
            cardNumber: generateCardNumber('visa'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Standard',
            tags: 'Standard,Credit du Maroc',
            balance: 20000
          }
        ],
        description: 'Carte standard Crédit du Maroc'
      }
    },
    sgmb: {
      'sgmb-classic': {
        name: 'Pack Classic',
        cards: [
          {
            cardNumber: generateCardNumber('visa'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Classic',
            tags: 'Classic,SGMB',
            balance: 20000
          }
        ],
        description: 'Carte classique SGMB'
      }
    },
    banque: {
      'banque-populaire': {
        name: 'Pack Populaire',
        cards: [
          {
            cardNumber: generateCardNumber('visa'),
            cardholderName: '',
            expirationDate: generateExpirationDate(),
            cvv: generateCVV(),
            category: 'Populaire',
            tags: 'Populaire,Banque Populaire',
            balance: 30000
          }
        ],
        description: 'Carte Banque Populaire'
      }
    }
  },

  // Appliquer un pack de cartes (créer les cartes automatiquement)
  async applyCardPack(bankId, packId, userFullName) {
    const bankPacks = this.cardPacks[bankId]
    if (!bankPacks || !bankPacks[packId]) {
      throw new Error('Pack de cartes non trouvé')
    }

    const pack = bankPacks[packId]
    const createdCards = []

    for (const cardTemplate of pack.cards) {
      const cardData = {
        ...cardTemplate,
        cardholderName: userFullName || 'Cardholder Name'
      }

      try {
        const card = await creditCardService.create(cardData)
        createdCards.push(card)
      } catch (error) {
        console.error('Erreur lors de la création de la carte:', error)
        throw error
      }
    }

    return {
      success: true,
      pack: pack.name,
      cards: createdCards,
      message: `${createdCards.length} carte(s) créée(s) avec succès!`
    }
  },

  // Obtenir les packs disponibles pour une banque
  getPacksForBank(bankId) {
    return this.cardPacks[bankId] || {}
  }
}
