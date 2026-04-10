<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { api } from '../services/api'
import type { LeaderboardResponse, Discipline } from '../types'

const data = ref<LeaderboardResponse | null>(null)
const loading = ref(true)
const error = ref('')

const selectedGender = ref<string>('all')
const selectedDiscipline = ref<number | 0>(0)

async function loadData() {
  loading.value = true
  error.value = ''
  try {
    const params: { gender?: string; discipline_id?: number } = {}
    if (selectedGender.value !== 'all') params.gender = selectedGender.value
    if (selectedDiscipline.value) params.discipline_id = selectedDiscipline.value
    data.value = await api.getLeaderboard(params)
  } catch (e: any) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

onMounted(loadData)

watch([selectedGender, selectedDiscipline], loadData)

const disciplines = computed(() => data.value?.disciplines || [])

const displayDisciplines = computed(() => {
  if (!data.value) return []
  if (selectedDiscipline.value) {
    return disciplines.value.filter(d => d.id === selectedDiscipline.value)
  }
  return disciplines.value
})

const gendersToShow = computed(() => {
  if (selectedGender.value !== 'all') return [selectedGender.value]
  return ['m', 'f']
})

function genderLabel(g: string): string {
  return g === 'm' ? 'Männer' : 'Frauen'
}

function formatValue(value: number, disc: Discipline): string {
  if (disc.unit === 'Sekunden') return value.toFixed(1) + 's'
  if (Number.isInteger(value)) return String(value)
  return value.toFixed(1)
}

function formatDate(dateStr: string): string {
  const d = new Date(dateStr)
  return d.toLocaleDateString('de-CH')
}

function getMedalClass(rank: number): string {
  if (rank === 1) return 'medal-gold'
  if (rank === 2) return 'medal-silver'
  if (rank === 3) return 'medal-bronze'
  return ''
}
</script>

<template>
  <div>
    <div class="page-header">
      <h2>Globale Bestenliste</h2>
      <div class="filters">
        <div class="filter-group">
          <label>Geschlecht:</label>
          <select v-model="selectedGender">
            <option value="all">Alle</option>
            <option value="m">Männer</option>
            <option value="f">Frauen</option>
          </select>
        </div>
        <div class="filter-group">
          <label>Disziplin:</label>
          <select v-model="selectedDiscipline">
            <option :value="0">Alle Disziplinen</option>
            <option v-for="disc in disciplines" :key="disc.id" :value="disc.id">
              {{ disc.name }}
            </option>
          </select>
        </div>
      </div>
    </div>

    <div v-if="loading" class="loading">Bestenliste wird geladen...</div>
    <div v-else-if="error" class="error">{{ error }}</div>

    <template v-else-if="data">
      <div v-for="disc in displayDisciplines" :key="disc.id" class="discipline-section">
        <div class="discipline-header">
          <h3>{{ disc.name }}</h3>
          <span class="unit-info">{{ disc.unit }} · {{ disc.higher_is_better ? 'Höher ist besser' : 'Niedriger ist besser' }}</span>
        </div>

        <div class="gender-tables">
          <div v-for="g in gendersToShow" :key="g" class="gender-table-wrapper">
            <h4 :class="['gender-title', g === 'm' ? 'gender-male' : 'gender-female']">
              {{ genderLabel(g) }}
            </h4>

            <table class="leaderboard-table">
              <thead>
                <tr>
                  <th class="col-rank">Rang</th>
                  <th class="col-name">Name</th>
                  <th class="col-value">Wert</th>
                  <th class="col-date">Datum</th>
                  <th class="col-year">Schuljahr</th>
                  <th class="col-class">Klasse / Beruf</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="record in data.leaderboard[disc.id]?.records[g] || []"
                  :key="record.id"
                  :class="getMedalClass(record.rank)"
                >
                  <td class="col-rank">
                    <span class="rank-number">{{ record.rank }}</span>
                  </td>
                  <td class="col-name">
                    <strong>{{ record.last_name }}</strong>, {{ record.first_name }}
                  </td>
                  <td class="col-value">
                    <strong>{{ formatValue(record.value, disc) }}</strong>
                  </td>
                  <td class="col-date">{{ formatDate(record.date) }}</td>
                  <td class="col-year">{{ record.school_year }}</td>
                  <td class="col-class">
                    <span class="class-name">{{ record.class_name }}</span>
                    <span class="profession">{{ record.profession }}</span>
                  </td>
                </tr>
                <tr v-if="!data.leaderboard[disc.id]?.records[g]?.length">
                  <td colspan="6" class="no-data">Keine Daten vorhanden</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.page-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.page-header h2 {
  margin: 0;
  font-size: 1.5rem;
  color: #1a365d;
}

.filters {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.filter-group label {
  font-weight: 600;
  color: #4a5568;
  font-size: 0.85rem;
}

.discipline-section {
  margin-bottom: 2rem;
}

.discipline-header {
  display: flex;
  align-items: baseline;
  gap: 1rem;
  margin-bottom: 1rem;
  padding-bottom: 0.5rem;
  border-bottom: 2px solid #1a365d;
}

.discipline-header h3 {
  margin: 0;
  font-size: 1.25rem;
  color: #1a365d;
}

.unit-info {
  font-size: 0.8rem;
  color: #a0aec0;
}

.gender-tables {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(450px, 1fr));
  gap: 1.5rem;
}

.gender-table-wrapper {
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.gender-title {
  margin: 0;
  padding: 0.75rem 1rem;
  font-size: 1rem;
}

.gender-male {
  background-color: #ebf8ff;
  color: #2a4365;
  border-bottom: 2px solid #90cdf4;
}

.gender-female {
  background-color: #fff5f7;
  color: #97266d;
  border-bottom: 2px solid #fbb6ce;
}

.leaderboard-table {
  font-size: 0.85rem;
}

.leaderboard-table th {
  background-color: #f7fafc;
  color: #4a5568;
  font-size: 0.75rem;
}

.col-rank { text-align: center; width: 50px; }
.col-name { min-width: 140px; }
.col-value { text-align: right; width: 80px; }
.col-date { width: 90px; }
.col-year { width: 90px; }
.col-class { min-width: 120px; }

.rank-number {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  font-weight: 700;
  font-size: 0.85rem;
}

.medal-gold {
  background-color: #fffff0;
}

.medal-gold .rank-number {
  background-color: #f6e05e;
  color: #744210;
}

.medal-silver {
  background-color: #fafafa;
}

.medal-silver .rank-number {
  background-color: #e2e8f0;
  color: #4a5568;
}

.medal-bronze {
  background-color: #fffaf0;
}

.medal-bronze .rank-number {
  background-color: #ed8936;
  color: white;
}

.class-name {
  font-weight: 600;
  display: block;
}

.profession {
  font-size: 0.75rem;
  color: #a0aec0;
}

.no-data {
  text-align: center;
  color: #a0aec0;
  font-style: italic;
  padding: 1.5rem;
}
</style>
