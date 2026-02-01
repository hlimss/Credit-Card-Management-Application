<template>
  <div class="token2pay-logo">
    <div class="logo-container">
      <div class="logo-graphic">
        <div class="graphic-line line-1"></div>
        <div class="graphic-line line-2"></div>
        <div class="graphic-line line-3"></div>
        <div class="graphic-line line-4"></div>
      </div>
      <div class="logo-text">
        <div class="main-headline">
          <span class="text-black">Digital Cards,</span>
          <span class="text-white"> Simplified</span>
        </div>
        <div class="brand-name">Token2Pay</div>
        <div class="tagline">by Hypeepay</div>
      </div>
    </div>
  </div>
</template>

<script setup>
// Logo component for Token2Pay
</script>

<style scoped>
.token2pay-logo {
  display: flex;
  align-items: center;
  justify-content: center;
}

.logo-container {
  position: relative;
  display: flex;
  align-items: center;
  gap: 2rem;
  padding: 1.5rem 2rem;
  background: linear-gradient(135deg, #1e40af 0%, #3b82f6 50%, #60a5fa 100%);
  border-radius: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.logo-graphic {
  position: relative;
  width: 80px;
  height: 80px;
}

.graphic-line {
  position: absolute;
  background: rgba(255, 255, 255, 0.6);
  border-radius: 2px;
}

.line-1 {
  width: 40px;
  height: 2px;
  bottom: 10px;
  left: 0;
  transform: rotate(-45deg);
  transform-origin: left bottom;
}

.line-2 {
  width: 35px;
  height: 2px;
  bottom: 25px;
  left: 5px;
  transform: rotate(-30deg);
  transform-origin: left bottom;
}

.line-3 {
  width: 30px;
  height: 2px;
  bottom: 40px;
  left: 10px;
  transform: rotate(-15deg);
  transform-origin: left bottom;
}

.line-4 {
  width: 25px;
  height: 2px;
  bottom: 55px;
  left: 15px;
  transform: rotate(0deg);
  transform-origin: left bottom;
}

.logo-text {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.main-headline {
  font-size: 1.5rem;
  font-weight: 800;
  line-height: 1.2;
}

.text-black {
  color: #000000;
}

.text-white {
  color: #ffffff;
}

.brand-name {
  font-size: 1.75rem;
  font-weight: 800;
  color: #ffffff;
  letter-spacing: -0.5px;
}

.tagline {
  font-size: 0.875rem;
  font-weight: 400;
  color: rgba(255, 255, 255, 0.9);
  margin-top: 0.125rem;
}

/* Compact version for navbar - Version institutionnelle transparente */
.token2pay-logo.compact .logo-container {
  padding: 0;
  gap: 0.5rem;
  background: transparent;
  box-shadow: none;
  border-radius: 0;
}

.token2pay-logo.compact .logo-graphic {
  width: 20px;
  height: 20px;
  opacity: 0.85;
}

.token2pay-logo.compact .graphic-line {
  background: rgba(30, 58, 138, 0.6);
  height: 1.5px;
}

.token2pay-logo.compact .line-1 {
  width: 12px;
  bottom: 3px;
}

.token2pay-logo.compact .line-2 {
  width: 10px;
  bottom: 8px;
  left: 2px;
}

.token2pay-logo.compact .line-3 {
  width: 8px;
  bottom: 13px;
  left: 4px;
}

.token2pay-logo.compact .line-4 {
  width: 6px;
  bottom: 18px;
  left: 6px;
}

.token2pay-logo.compact .logo-text {
  display: none; /* Cacher le texte dans la version compacte */
}

/* Version institutionnelle - très petit et transparent */
.token2pay-logo.logo-institutional .logo-container {
  padding: 0;
  gap: 0;
  background: transparent;
  box-shadow: none;
  border-radius: 0;
}

.token2pay-logo.logo-institutional .logo-graphic {
  width: 16px;
  height: 16px;
  opacity: 0.75;
}

.token2pay-logo.logo-institutional .graphic-line {
  background: rgba(30, 58, 138, 0.5);
  height: 1px;
}

.token2pay-logo.logo-institutional .line-1 {
  width: 10px;
  bottom: 2px;
}

.token2pay-logo.logo-institutional .line-2 {
  width: 8px;
  bottom: 6px;
  left: 1px;
}

.token2pay-logo.logo-institutional .line-3 {
  width: 6px;
  bottom: 10px;
  left: 2px;
}

.token2pay-logo.logo-institutional .line-4 {
  width: 5px;
  bottom: 14px;
  left: 3px;
}

.token2pay-logo.logo-institutional .logo-text {
  display: none;
}
</style>
