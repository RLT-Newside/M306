<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { api } from '../services/api'
import type { ClassInfo } from '../types'

const classes = ref<ClassInfo[]>([])
const loading = ref(true)
const error = ref('')
const selectedYear = ref('')

onMounted(async () => {
  try {
    classes.value = await api.getClasses()
    if (classes.value.length > 0) {
      const years = [...new Set(classes.value.map(c => c.school_year))]
      selectedYear.value = years[0]
    }
  } catch (e: any) {
    error.value = e.message
  } finally {
    loading.value = false
  }
})

const schoolYears = computed(() => {
  return [...new Set(classes.value.map(c => c.school_year))].sort().reverse()
})

const filteredClasses = computed(() => {
  if (!selectedYear.value) return classes.value
  return classes.value.filter(c => c.school_year === selectedYear.value)
})
</script>

<template>
  <div>
    <div class="page-header">
      <h2>Klassenübersicht</h2>
      <div class="filter-bar" v-if="schoolYears.length > 0">
        <label>Schuljahr:</label>
        <select v-model="selectedYear">
          <option v-for="year in schoolYears" :key="year" :value="year">{{ year }}</option>
        </select>
      </div>
    </div>

    <div v-if="loading" class="loading">Daten werden geladen...</div>
    <div v-else-if="error" class="error">{{ error }}</div>

    <div v-else class="class-grid">
      <router-link
        v-for="cls in filteredClasses"
        :key="cls.id"
        :to="`/class/${cls.id}`"
        class="class-card"
      >
        <div class="class-name">{{ cls.name }}</div>
        <div class="class-meta">{{ cls.profession }}</div>
        <div class="class-meta">{{ cls.school_year }}</div>
        <div class="class-count">{{ cls.student_count }} Lernende</div>
      </router-link>
    </div>
  </div>
</template>

<style scoped>
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.5rem;
}

.page-header h2 {
  margin: 0;
  font-size: 1.5rem;
  color: #1a365d;
}

.filter-bar {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.filter-bar label {
  font-weight: 600;
  color: #4a5568;
}

.class-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1rem;
}

.class-card {
  background: white;
  border-radius: 8px;
  padding: 1.25rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  text-decoration: none;
  color: inherit;
  transition: all 0.2s;
  border: 2px solid transparent;
}

.class-card:hover {
  border-color: #2b6cb0;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  text-decoration: none;
}

.class-name {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1a365d;
  margin-bottom: 0.5rem;
}

.class-meta {
  color: #718096;
  font-size: 0.85rem;
  margin-bottom: 0.25rem;
}

.class-count {
  margin-top: 0.75rem;
  font-weight: 600;
  color: #2b6cb0;
}
</style>
