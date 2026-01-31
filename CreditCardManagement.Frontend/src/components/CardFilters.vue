<template>
  <div class="card-filters">
    <div class="search-box">
      <input
        v-model="searchQuery"
        type="text"
        placeholder="Search cards..."
        class="search-input"
        @input="$emit('search', searchQuery)"
      />
      <span class="search-icon">🔍</span>
    </div>

    <div class="filter-buttons">
      <button
        v-for="filter in filters"
        :key="filter.value"
        @click="selectFilter(filter.value)"
        :class="['filter-btn', { active: selectedFilter === filter.value }]"
      >
        {{ filter.label }}
      </button>
    </div>

    <div class="sort-options">
      <select v-model="sortBy" @change="$emit('sort', sortBy)" class="sort-select">
        <option value="newest">Newest First</option>
        <option value="oldest">Oldest First</option>
        <option value="name">Name A-Z</option>
        <option value="type">Card Type</option>
      </select>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const emit = defineEmits(['search', 'filter', 'sort'])

const searchQuery = ref('')
const selectedFilter = ref('all')
const sortBy = ref('newest')

const filters = [
  { label: 'All', value: 'all' },
  { label: 'Visa', value: 'Visa' },
  { label: 'MasterCard', value: 'MasterCard' },
  { label: 'Amex', value: 'American Express' },
  { label: 'Expired', value: 'expired' }
]

function selectFilter(value) {
  selectedFilter.value = value
  emit('filter', value)
}
</script>

<style scoped>
.card-filters {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  margin-bottom: 24px;
  align-items: center;
}

.search-box {
  position: relative;
  flex: 1;
  min-width: 200px;
}

.search-input {
  width: 100%;
  padding: 12px 16px 12px 44px;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  font-size: 14px;
  transition: all 0.3s;
  background: white;
}

.search-input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.search-icon {
  position: absolute;
  left: 16px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 18px;
}

.filter-buttons {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.filter-btn {
  padding: 10px 20px;
  border: 2px solid #e5e7eb;
  background: white;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s;
  color: #6b7280;
}

.filter-btn:hover {
  border-color: #667eea;
  color: #667eea;
}

.filter-btn.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-color: transparent;
  color: white;
}

.sort-select {
  padding: 10px 16px;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  font-size: 14px;
  background: white;
  cursor: pointer;
  transition: all 0.3s;
}

.sort-select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}
</style>
