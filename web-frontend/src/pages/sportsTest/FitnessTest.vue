<template>
  <div class="ft-page">
    <FitnessHeader />

    <main class="ft-content">
      <h1 class="ft-page-title">GIBZ Fitnesstest Bestenliste</h1>
      <p class="ft-page-sub">Die besten Leistungen nach Disziplin und Geschlecht</p>

      <!-- Filters -->
      <div class="ft-filters">
        <div class="ft-filter-item">
          <span class="ft-label">Geschlecht</span>
          <div class="ft-gender-group">
            <button
              :class="['ft-gender-btn', rankGender === 'male' && 'ft-gender-btn--active']"
              @click="rankGender = 'male'"
            >Männer</button>
            <button
              :class="['ft-gender-btn', rankGender === 'female' && 'ft-gender-btn--active']"
              @click="rankGender = 'female'"
            >Frauen</button>
          </div>
        </div>
        <div class="ft-filter-item">
          <span class="ft-label">Disziplin</span>
          <select v-model="rankDiscipline" class="ft-select">
            <option v-for="d in allDisciplines" :key="d" :value="d">{{ d }}</option>
          </select>
        </div>
      </div>

      <!-- Leaderboard table -->
      <div class="ft-card">
        <table class="ft-table">
          <thead>
            <tr>
              <th class="ft-th-rank">Rang</th>
              <th>Name</th>
              <th>Wert</th>
              <th>Punkte</th>
              <th>Note</th>
              <th>Datum</th>
              <th>Schuljahr</th>
              <th>Klasse</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="entry in leaderboardEntries" :key="entry.id">
              <td class="ft-td-rank">
                <span class="ft-rank-circle" :style="rankCircleStyle(entry.rank)">
                  {{ entry.rank }}
                </span>
              </td>
              <td class="ft-td-name">{{ entry.studentName }}</td>
              <td class="ft-td-value">{{ entry.value }} {{ entry.unit }}</td>
              <td><span class="ft-points-badge">{{ entry.points }} Pkt</span></td>
              <td><span class="ft-note-badge">{{ computeNote(entry.points) }}</span></td>
              <td class="ft-td-muted">{{ formatDate(entry.date) }}</td>
              <td class="ft-td-muted">{{ entry.schoolYear }}</td>
              <td class="ft-td-class">{{ entry.classOrProfession }}</td>
            </tr>
            <tr v-if="leaderboardEntries.length === 0">
              <td colspan="8" class="ft-empty-cell">Keine Daten für diese Auswahl vorhanden.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import FitnessHeader from '@/components/FitnessHeader.vue';
import { mockAttempts } from '@/mocks/fitnessAttempts';

const allDisciplines = [...new Set(mockAttempts.map((a) => a.discipline))].sort();

const rankGender = ref<'male' | 'female'>('male');
const rankDiscipline = ref(allDisciplines[0] ?? '');

const leaderboardEntries = computed(() =>
  mockAttempts
    .filter((a) => a.discipline === rankDiscipline.value && a.gender === rankGender.value)
    .sort((a, b) => b.value - a.value)
    .slice(0, 10)
    .map((a, i) => ({ ...a, rank: i + 1 }))
);

function computeNote(points: number): string {
  return (1 + (points / 100) * 5).toFixed(1);
}

function formatDate(iso: string): string {
  const [y, m, d] = iso.split('-');
  return `${d}.${m}.${y}`;
}

function rankCircleStyle(rank: number): Record<string, string> {
  if (rank === 1) return { background: '#FCD34D', color: '#78350F' };
  if (rank === 2) return { background: '#E2E8F0', color: '#475569' };
  if (rank === 3) return { background: '#FED7AA', color: '#9A3412' };
  return { background: '#F1F5F9', color: '#94A3B8' };
}
</script>

<style scoped>
.ft-page {
  min-height: 100vh;
  background: #F8FAFC;
}

.ft-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 36px 32px 60px;
}

.ft-page-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: #0F172A;
  margin: 0 0 6px;
}

.ft-page-sub {
  font-size: 0.95rem;
  color: #64748B;
  margin: 0 0 28px;
}

.ft-filters {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 24px;
}

.ft-filter-item {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.ft-label {
  font-size: 0.8rem;
  font-weight: 600;
  color: #374151;
}

.ft-gender-group {
  display: flex;
}

.ft-gender-btn {
  padding: 8px 20px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  border: 1px solid #D1D5DB;
  background: #fff;
  color: #374151;
  transition: all 0.15s;
}

.ft-gender-btn:first-child { border-radius: 6px 0 0 6px; }
.ft-gender-btn:last-child  { border-radius: 0 6px 6px 0; border-left: none; }

.ft-gender-btn--active {
  background: #2563EB;
  color: #fff;
  border-color: #2563EB;
  z-index: 1;
}

.ft-select {
  padding: 9px 12px;
  border: 1px solid #D1D5DB;
  border-radius: 6px;
  font-size: 0.875rem;
  color: #1E293B;
  background: #fff;
  cursor: pointer;
  min-width: 220px;
  max-width: 320px;
}

.ft-card {
  background: #fff;
  border: 1px solid #E2E8F0;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 1px 4px rgba(15, 23, 42, 0.06);
}

.ft-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.875rem;
}

.ft-table thead tr { border-bottom: 1px solid #E2E8F0; }

.ft-table th {
  padding: 14px 16px;
  text-align: left;
  font-size: 0.8rem;
  font-weight: 600;
  color: #374151;
  white-space: nowrap;
}

.ft-table td {
  padding: 14px 16px;
  color: #1E293B;
  border-bottom: 1px solid #F1F5F9;
}

.ft-table tbody tr:last-child td { border-bottom: none; }
.ft-table tbody tr:hover td { background: #FAFBFC; }

.ft-th-rank { width: 72px; }

.ft-td-rank { text-align: center; }

.ft-rank-circle {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  font-size: 0.8rem;
  font-weight: 700;
}

.ft-td-name  { font-weight: 500; color: #0F172A; }
.ft-td-value { font-weight: 700; color: #0F172A; }
.ft-td-muted { color: #64748B; }
.ft-td-class { color: #0891B2; font-weight: 500; }

.ft-points-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 4px 10px;
  background: #22C55E;
  color: #fff;
  border-radius: 20px;
  font-size: 0.78rem;
  font-weight: 600;
  white-space: nowrap;
}

.ft-note-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 4px 10px;
  background: #DBEAFE;
  color: #1E40AF;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
  white-space: nowrap;
}

.ft-empty-cell {
  text-align: center;
  color: #94A3B8;
  padding: 48px 16px !important;
  font-size: 0.9rem;
}
</style>
