// Luhn algorithm for card number validation
export function validateCardNumber(cardNumber) {
  const cleaned = cardNumber.replace(/[\s-]/g, '')
  
  if (!/^\d+$/.test(cleaned)) {
    return { valid: false, message: 'Card number must contain only digits' }
  }

  if (cleaned.length < 13 || cleaned.length > 19) {
    return { valid: false, message: 'Card number must be between 13 and 19 digits' }
  }

  // Luhn algorithm
  let sum = 0
  let alternate = false

  for (let i = cleaned.length - 1; i >= 0; i--) {
    let digit = parseInt(cleaned[i])

    if (alternate) {
      digit *= 2
      if (digit > 9) {
        digit -= 9
      }
    }

    sum += digit
    alternate = !alternate
  }

  if (sum % 10 !== 0) {
    return { valid: false, message: 'Invalid card number' }
  }

  return { valid: true }
}

export function validateExpirationDate(expirationDate) {
  if (!/^(0[1-9]|1[0-2])\/([0-9]{2})$/.test(expirationDate)) {
    return { valid: false, message: 'Expiration date must be in MM/YY format' }
  }

  const [month, year] = expirationDate.split('/')
  const fullYear = 2000 + parseInt(year)
  const expiration = new Date(fullYear, parseInt(month), 0)

  if (expiration < new Date()) {
    return { valid: false, message: 'Expiration date must be in the future' }
  }

  return { valid: true }
}

export function validateCVV(cvv) {
  if (!/^[0-9]{3,4}$/.test(cvv)) {
    return { valid: false, message: 'CVV must be 3 or 4 digits' }
  }
  return { valid: true }
}

export function detectCardType(cardNumber) {
  const cleaned = cardNumber.replace(/[\s-]/g, '')

  if (/^4/.test(cleaned)) return 'Visa'
  if (/^5[1-5]/.test(cleaned) || /^2[2-7]/.test(cleaned)) return 'MasterCard'
  if (/^3[47]/.test(cleaned)) return 'American Express'
  if (/^6011/.test(cleaned) || /^65/.test(cleaned) || /^64[4-9]/.test(cleaned)) return 'Discover'

  return 'Unknown'
}

export function formatCardNumber(cardNumber) {
  const cleaned = cardNumber.replace(/[\s-]/g, '')
  const groups = cleaned.match(/.{1,4}/g) || []
  return groups.join(' ')
}

