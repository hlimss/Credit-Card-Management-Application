<template>
  <div class="credit-card-3d-container" @mouseenter="isHovered = true" @mouseleave="isHovered = false">
    <div 
      class="credit-card-3d" 
      :class="[`card-${cardType.toLowerCase()}`, { 'hovered': isHovered }]"
      :style="cardStyle"
    >
      <div class="card-inner">
        <!-- Card Chip -->
        <div class="card-chip">
          <div class="chip-line"></div>
          <div class="chip-line"></div>
          <div class="chip-line"></div>
          <div class="chip-line"></div>
        </div>

        <!-- Card Number -->
        <div class="card-number">
          {{ formattedCardNumber }}
        </div>

        <!-- Card Info -->
        <div class="card-info">
          <div class="card-holder">
            <span class="label">CARDHOLDER</span>
            <span class="value">{{ cardholderName }}</span>
          </div>
          <div class="card-expiry">
            <span class="label">EXPIRES</span>
            <span class="value">{{ expirationDate }}</span>
          </div>
        </div>

        <!-- Card Type Logo -->
        <div class="card-logo">
          <div v-if="cardType === 'Visa'" class="visa-logo">VISA</div>
          <div v-else-if="cardType === 'MasterCard'" class="mastercard-logo">
            <div class="mc-circle mc-circle-1"></div>
            <div class="mc-circle mc-circle-2"></div>
          </div>
          <div v-else-if="cardType === 'American Express'" class="amex-logo">AMEX</div>
          <div v-else class="default-logo">{{ cardType }}</div>
        </div>

        <!-- Holographic Effect -->
        <div class="holographic-effect"></div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  cardNumber: String,
  cardholderName: String,
  expirationDate: String,
  cardType: String,
  cvv: String
})

const isHovered = ref(false)

const formattedCardNumber = computed(() => {
  if (!props.cardNumber) return '•••• •••• •••• ••••'
  return props.cardNumber.replace(/(.{4})/g, '$1 ').trim()
})

const cardStyle = computed(() => {
  const baseGradient = getCardGradient(props.cardType)
  return {
    background: baseGradient,
    transform: isHovered.value 
      ? 'perspective(1000px) rotateY(5deg) rotateX(-5deg) translateZ(20px)' 
      : 'perspective(1000px) rotateY(0deg) rotateX(0deg) translateZ(0px)'
  }
})

function getCardGradient(cardType) {
  const gradients = {
    'Visa': 'linear-gradient(135deg, #1e3c72 0%, #2a5298 100%)',
    'MasterCard': 'linear-gradient(135deg, #eb3349 0%, #f45c43 100%)',
    'American Express': 'linear-gradient(135deg, #006fcf 0%, #00a8ff 100%)',
    'Discover': 'linear-gradient(135deg, #ff6b6b 0%, #ee5a6f 100%)',
    'default': 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
  }
  return gradients[cardType] || gradients.default
}
</script>

<style scoped>
.credit-card-3d-container {
  perspective: 1000px;
  width: 100%;
  max-width: 400px;
  height: 250px;
  margin: 0 auto;
}

.credit-card-3d {
  width: 100%;
  height: 100%;
  position: relative;
  transition: transform 0.6s cubic-bezier(0.23, 1, 0.32, 1);
  transform-style: preserve-3d;
  cursor: pointer;
}

.credit-card-3d.hovered {
  transform: perspective(1000px) rotateY(5deg) rotateX(-5deg) translateZ(20px);
}

.card-inner {
  width: 100%;
  height: 100%;
  border-radius: 20px;
  padding: 30px;
  position: relative;
  overflow: hidden;
  box-shadow: 
    0 20px 60px rgba(0, 0, 0, 0.3),
    0 0 0 1px rgba(255, 255, 255, 0.1) inset;
  backdrop-filter: blur(10px);
  color: white;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.card-chip {
  width: 50px;
  height: 40px;
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  border-radius: 8px;
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.chip-line {
  width: 80%;
  height: 2px;
  background: rgba(0, 0, 0, 0.2);
  margin: 2px 0;
  border-radius: 1px;
}

.card-number {
  font-size: 24px;
  font-weight: 600;
  letter-spacing: 3px;
  font-family: 'Courier New', monospace;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
  margin: 20px 0;
}

.card-info {
  display: flex;
  justify-content: space-between;
  margin-top: auto;
}

.card-holder,
.card-expiry {
  display: flex;
  flex-direction: column;
}

.label {
  font-size: 10px;
  letter-spacing: 1px;
  opacity: 0.8;
  margin-bottom: 4px;
  text-transform: uppercase;
}

.value {
  font-size: 14px;
  font-weight: 600;
  letter-spacing: 1px;
}

.card-logo {
  position: absolute;
  top: 30px;
  right: 30px;
  font-weight: bold;
  font-size: 18px;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.visa-logo {
  color: white;
  font-weight: 900;
  letter-spacing: 2px;
}

.mastercard-logo {
  position: relative;
  width: 50px;
  height: 50px;
}

.mc-circle {
  position: absolute;
  border-radius: 50%;
  width: 30px;
  height: 30px;
}

.mc-circle-1 {
  background: #eb001b;
  left: 0;
  z-index: 1;
}

.mc-circle-2 {
  background: #f79e1b;
  right: 0;
  z-index: 2;
}

.amex-logo {
  color: white;
  font-weight: 900;
  letter-spacing: 1px;
}

.holographic-effect {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(
    45deg,
    transparent 30%,
    rgba(255, 255, 255, 0.1) 50%,
    transparent 70%
  );
  opacity: 0;
  transition: opacity 0.3s;
  pointer-events: none;
}

.credit-card-3d:hover .holographic-effect {
  opacity: 1;
  animation: shimmer 2s infinite;
}

@keyframes shimmer {
  0% { transform: translateX(-100%) translateY(-100%) rotate(45deg); }
  100% { transform: translateX(100%) translateY(100%) rotate(45deg); }
}

/* Card Type Specific Styles */
.card-visa .card-inner {
  background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
}

.card-mastercard .card-inner {
  background: linear-gradient(135deg, #eb3349 0%, #f45c43 100%);
}

.card-american express .card-inner {
  background: linear-gradient(135deg, #006fcf 0%, #00a8ff 100%);
}
</style>
