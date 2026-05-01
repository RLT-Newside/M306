<template>
  <div class="ft-page">
    <div class="ft-tabs">
      <router-link class="ft-tab ft-tab--active" to="/fitnesstest">
        <v-icon size="16">mdi-trophy</v-icon>
        Globale Bestenliste
      </router-link>
      <router-link class="ft-tab" to="/fitnesstest/klassen">
        <v-icon size="16">mdi-account-group</v-icon>
        Klassen-Verwaltung
      </router-link>
    </div>

    <div class="ft-content">
      <h1 class="ft-page-title">GIBZ Fitnesstest Bestenliste</h1>
      <p class="ft-page-sub">Die besten Leistungen nach Disziplin und Geschlecht</p>

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
            <option v-for="d in DISCIPLINES" :key="d" :value="d">{{ d }}</option>
          </select>
        </div>
      </div>

      <div class="ft-card">
        <table class="ft-table">
          <thead>
            <tr>
              <th class="ft-th-rank">Rang</th>
              <th>Name</th>
              <th>Wert</th>
              <th>Punkte</th>
              <th>Bewertung</th>
              <th>Datum</th>
              <th>Schuljahr</th>
              <th>Klasse</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="entry in leaderboardEntries" :key="entry.id">
              <td class="ft-td-rank">
                <span class="ft-rank-circle" :style="rankCircleStyle(entry.rank)">{{ entry.rank }}</span>
              </td>
              <td class="ft-td-name">{{ entry.studentName }}</td>
              <td class="ft-td-value">{{ formatValue(entry) }}</td>
              <td><span class="ft-points-badge">{{ entry.points }} Pkt</span></td>
              <td><span :class="['ft-grade-badge', 'ft-grade-badge--' + entry.grade.toLowerCase()]">{{ entry.grade }}</span></td>
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
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { mockAttempts } from '@/mocks/fitnessAttempts';
import { calculatePoints, getGrade, DISCIPLINES, getDisciplineConfig } from '@/utils/fitnessCalculation';

const rankGender = ref<'male' | 'female'>('male');
const rankDiscipline = ref(DISCIPLINES[0]);

const leaderboardEntries = computed(() => {
  const config = getDisciplineConfig(rankDiscipline.value);
  if (!config) return [];

  return mockAttempts
    .filter((a) => a.discipline === rankDiscipline.value && a.gender === rankGender.value && !a.annotation)
    .map((a) => {
      const points = calculatePoints(a.discipline, a.gender, a.value);
      return { ...a, points, grade: getGrade(points) };
    })
    .sort((a, b) => b.points - a.points || b.value - a.value)
    .slice(0, 10)
    .map((a, i) => ({ ...a, rank: i + 1 }));
});

function formatValue(entry: { value: number; unit: string; discipline: string }): string {
  const config = getDisciplineConfig(entry.discipline);
  if (config?.key === 'MedicineBallPush') {
    return `${(entry.value / 100).toFixed(2)} m`;
  }
  if (config?.key === 'ShuttleRun') {
    return `${(entry.value / 1000).toFixed(2)} s`;
  }
  return `${entry.value} ${entry.unit}`;
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
  min-height: 100%;
  background: #F8FAFC;
}

.ft-tabs {
  display: flex;
  gap: 4px;
  padding: 16px 24px 0;
  border-bottom: 1px solid #E2E8F0;
  background: #fff;
}

.ft-tab {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 10px 18px;
  font-size: 0.875rem;
  font-weight: 500;
  color: #64748B;
  text-decoration: none;
  border-radius: 6px 6px 0 0;
  border: 1px solid transparent;
  border-bottom: none;
  transition: all 0.15s;
  position: relative;
  bottom: -1px;
}

.ft-tab:hover {
  color: #1E293B;
  background: #F8FAFC;
}

.ft-tab--active {
  color: #2563EB;
  background: #fff;
  border-color: #E2E8F0;
  border-bottom-color: #fff;
  font-weight: 600;
}

.ft-content {
  padding: 28px 24px 48px;
}

.ft-page-title {
  font-size: 1.6rem;
  font-weight: 700;
  color: #0F172A;
  margin: 0 0 4px;
}

.ft-page-sub {
  font-size: 0.9rem;
  color: #64748B;
  margin: 0 0 24px;
}

.ft-filters {
  display: flex;
  flex-direction: column;
  gap: 14px;
  margin-bottom: 20px;
}

.ft-filter-item {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.ft-label {
  font-size: 0.78rem;
  font-weight: 600;
  color: #374151;
  letter-spacing: 0.01em;
}

.ft-gender-group { display: flex; }

.ft-gender-btn {
  padding: 7px 18px;
  font-size: 0.85rem;
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
  padding: 8px 12px;
  border: 1px solid #D1D5DB;
  border-radius: 6px;
  font-size: 0.875rem;
  color: #1E293B;
  background: #fff;
  cursor: pointer;
  min-width: 200px;
  max-width: 300px;
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
  padding: 12px 16px;
  text-align: left;
  font-size: 0.78rem;
  font-weight: 600;
  color: #374151;
  white-space: nowrap;
}

.ft-table td {
  padding: 13px 16px;
  color: #1E293B;
  border-bottom: 1px solid #F1F5F9;
}

.ft-table tbody tr:last-child td { border-bottom: none; }
.ft-table tbody tr:hover td { background: #FAFBFC; }

.ft-th-rank { width: 68px; }
.ft-td-rank { text-align: center; }

.ft-rank-circle {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  font-size: 0.78rem;
  font-weight: 700;
}

.ft-td-name  { font-weight: 500; color: #0F172A; }
.ft-td-value { font-weight: 700; color: #0F172A; }
.ft-td-muted { color: #64748B; }
.ft-td-class { color: #0891B2; font-weight: 500; }

.ft-points-badge {
  display: inline-flex;
  align-items: center;
  padding: 3px 9px;
  background: #22C55E;
  color: #fff;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
  white-space: nowrap;
}

.ft-grade-badge {
  display: inline-flex;
  align-items: center;
  padding: 3px 9px;
  border-radius: 20px;
  font-size: 0.78rem;
  font-weight: 600;
  white-space: nowrap;
}

.ft-grade-badge--te  { background: #FEE2E2; color: #991B1B; }
.ft-grade-badge--ee  { background: #DBEAFE; color: #1E40AF; }
.ft-grade-badge--uee { background: #D1FAE5; color: #065F46; }
.ft-grade-badge--ued { background: #FCD34D; color: #78350F; }

.ft-empty-cell {
  text-align: center;
  color: #94A3B8;
  padding: 40px 16px !important;
  font-size: 0.9rem;
}
</style>
