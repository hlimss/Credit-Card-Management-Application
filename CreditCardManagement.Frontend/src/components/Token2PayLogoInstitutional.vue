<template>
  <div class="logo-institutional-container">
    <svg width="32" height="32" viewBox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg">
      <!-- Carte bancaire stylisée -->
      <rect x="4" y="8" width="24" height="16" rx="2" fill="url(#cardGradient)" opacity="0.9"/>
      <rect x="4" y="8" width="24" height="16" rx="2" stroke="rgba(30, 58, 138, 0.3)" stroke-width="0.5"/>
      
      <!-- Puce -->
      <rect x="6" y="12" width="6" height="4" rx="0.5" fill="rgba(251, 191, 36, 0.8)"/>
      
      <!-- Lignes de la carte -->
      <line x1="6" y1="18" x2="20" y2="18" stroke="rgba(255, 255, 255, 0.4)" stroke-width="1"/>
      <line x1="6" y1="20" x2="16" y2="20" stroke="rgba(255, 255, 255, 0.3)" stroke-width="1"/>
      
      <defs>
        <linearGradient id="cardGradient" x1="0%" y1="0%" x2="100%" y2="100%">
          <stop offset="0%" style="stop-color:#1E3A8A;stop-opacity:0.95" />
          <stop offset="100%" style="stop-color:#3B82F6;stop-opacity:0.85" />
        </linearGradient>
      </defs>
    </svg>
  </div>
</template>

<script setup>
// Logo institutionnel minimaliste et transparent
</script>

<style scoped>
.logo-institutional-container {
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0.9;
  transition: opacity 0.2s ease;
}

.logo-institutional-container:hover {
  opacity: 1;
}
</style>
