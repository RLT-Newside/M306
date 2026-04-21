<template>
  <div class="ft-page">
    <!-- In-page tab navigation -->
    <div class="ft-tabs">
      <router-link class="ft-tab" to="/fitnesstest">
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

      <!-- Filters -->
      <div class="ft-filters">
        <div class="ft-filter-item">
          <span class="ft-label">Geschlecht</span>
          <div class="ft-gender-group">
            <button
              :class="['ft-gender-btn', rankGender === 'male' && 'ft-gender-btn--active']"
              :aria-pressed="rankGender === 'male'"
              @click="rankGender = 'male'"
            >Männer</button>
            <button
              :class="['ft-gender-btn', rankGender === 'female' && 'ft-gender-btn--active']"
              :aria-pressed="rankGender === 'female'"
              @click="rankGender = 'female'"
            >Frauen</button>
          </div>
        </div>
        <div class="ft-filter-item">
          <span class="ft-label">Disziplin</span>
          <select v-model="rankDiscipline" class="ft-select">
            <option v-for="d in ALL_DISCIPLINES" :key="d" :value="d">{{ d }}</option>
          </select>
        </div>
      </div>

      <!-- Leaderboard table -->
      <div class="ft-card">
        <table class="ft-table">
          <thead>
            <tr>
              <th class="ft-th-rank" scope="col">Rang</th>
              <th scope="col">Name</th>
              <th scope="col">Wert</th>
              <th scope="col">Punkte</th>
              <th scope="col">Note</th>
              <th scope="col">Datum</th>
              <th scope="col">Schuljahr</th>
              <th scope="col">Klasse</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="entry in leaderboardEntries" :key="entry.id">
              <td class="ft-td-rank">
                <span class="ft-rank-circle" :style="rankCircleStyle(entry.rank)">{{ entry.rank }}</span>
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
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { mockAttempts } from '@/mocks/fitnessAttempts';
import { ALL_DISCIPLINES, computeNote, formatDate } from '@/composables/useFitnessUtils';
import type { Discipline } from '@/models/sportsTest/fitnessAttempt';
import '@/assets/fitnessTest.css';

const rankGender = ref<'male' | 'female'>('male');
const rankDiscipline = ref<Discipline>(ALL_DISCIPLINES[0]);

const leaderboardEntries = computed(() =>
  mockAttempts
    .filter((a) => a.discipline === rankDiscipline.value && a.gender === rankGender.value)
    .sort((a, b) => b.value - a.value)
    .slice(0, 10)
    .map((a, i) => ({ ...a, rank: i + 1 }))
);

function rankCircleStyle(rank: number): Record<string, string> {
  if (rank === 1) return { background: '#FCD34D', color: '#78350F' };
  if (rank === 2) return { background: '#E2E8F0', color: '#475569' };
  if (rank === 3) return { background: '#FED7AA', color: '#9A3412' };
  return { background: '#F1F5F9', color: '#94A3B8' };
}
</script>

<style scoped>
/* ── Filters ───────────────────────────────────────────────────── */
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

/* ── Leaderboard-specific ──────────────────────────────────────── */
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

.ft-td-class { color: #0891B2; font-weight: 500; }
</style>
