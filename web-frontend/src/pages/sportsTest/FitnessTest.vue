<template>
  <div class="ft-page">

    <!-- ── Header ──────────────────────────────────────────────── -->
    <header class="ft-header">
      <div class="ft-header-inner">
        <div class="ft-brand">
          <v-icon color="primary" size="26">mdi-trophy-outline</v-icon>
          <div>
            <div class="ft-brand-title">GIBZ Fitnesstest</div>
            <div class="ft-brand-sub">Gewerblich-industrielles Bildungszentrum Bern</div>
          </div>
        </div>
        <nav class="ft-nav">
          <button
            :class="['ft-nav-btn', activeView === 'leaderboard' && 'ft-nav-btn--active']"
            @click="activeView = 'leaderboard'"
          >
            <v-icon size="16">mdi-trophy</v-icon>
            Globale Bestenliste
          </button>
          <button
            v-if="isTeacher"
            :class="['ft-nav-btn', activeView === 'class-management' && 'ft-nav-btn--active']"
            @click="activeView = 'class-management'"
          >
            <v-icon size="16">mdi-account-group</v-icon>
            Klassen-Verwaltung
          </button>
        </nav>
      </div>
    </header>

    <!-- ── Globale Bestenliste ──────────────────────────────────── -->
    <main v-if="activeView === 'leaderboard'" class="ft-content">
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
            <option v-for="d in allDisciplines" :key="d" :value="d">{{ d }}</option>
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

    <!-- ── Klassen-Verwaltung ───────────────────────────────────── -->
    <main v-else-if="activeView === 'class-management'" class="ft-content">
      <h1 class="ft-page-title">Klassen-Verwaltung</h1>
      <p class="ft-page-sub">Alle Einträge der Klasse verwalten und bearbeiten</p>

      <div class="ft-controls">
        <div>
          <div class="ft-label">Klasse auswählen</div>
          <select v-model="selectedClass" class="ft-select ft-select--wide">
            <option v-for="c in allClasses" :key="c" :value="c">{{ c }}</option>
          </select>
        </div>
        <button class="ft-btn-primary" @click="openNewDialog">
          + &nbsp;Neuer Eintrag
        </button>
      </div>

      <div class="ft-card">
        <table class="ft-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Geschlecht</th>
              <th>Disziplin</th>
              <th>Wert</th>
              <th>Punkte</th>
              <th>Note</th>
              <th>Datum</th>
              <th>Schuljahr</th>
              <th class="ft-th-actions">Aktionen</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="entry in classEntries" :key="entry.id">
              <td class="ft-td-name">{{ entry.studentName }}</td>
              <td>
                <span :class="['ft-gender-chip', entry.gender === 'male' ? 'ft-gender-chip--male' : 'ft-gender-chip--female']">
                  {{ entry.gender === 'male' ? 'Männlich' : 'Weiblich' }}
                </span>
              </td>
              <td>{{ entry.discipline }}</td>
              <td class="ft-td-value">{{ entry.value }} {{ entry.unit }}</td>
              <td><span class="ft-points-badge">{{ entry.points }} Pkt</span></td>
              <td><span class="ft-note-badge">{{ computeNote(entry.points) }}</span></td>
              <td class="ft-td-muted">{{ formatDate(entry.date) }}</td>
              <td class="ft-td-muted">{{ entry.schoolYear }}</td>
              <td class="ft-td-actions">
                <button class="ft-icon-btn ft-icon-btn--edit" title="Bearbeiten" @click="openEditDialog(entry)">
                  <v-icon size="17">mdi-pencil</v-icon>
                </button>
                <button class="ft-icon-btn ft-icon-btn--delete" title="Löschen" @click="openDeleteDialog(entry)">
                  <v-icon size="17">mdi-delete</v-icon>
                </button>
              </td>
            </tr>
            <tr v-if="classEntries.length === 0">
              <td colspan="9" class="ft-empty-cell">Keine Einträge für diese Klasse.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </main>

    <!-- ── Edit Dialog ──────────────────────────────────────────── -->
    <v-dialog v-model="editDialog.open" max-width="440">
      <v-card rounded="lg">
        <v-card-title class="pt-5 px-6">Ergebnis bearbeiten</v-card-title>
        <v-card-text class="px-6">
          <div class="mb-1 text-body-2 text-medium-emphasis">
            {{ editDialog.entry?.studentName }} — {{ editDialog.entry?.discipline }}
          </div>
          <v-text-field
            v-model.number="editDialog.value"
            :label="`Wert (${editDialog.entry?.unit ?? ''})`"
            type="number"
            variant="outlined"
            density="comfortable"
            class="mt-3"
          />
        </v-card-text>
        <v-card-actions class="px-6 pb-5">
          <v-spacer />
          <v-btn variant="text" @click="editDialog.open = false">Abbrechen</v-btn>
          <v-btn variant="flat" color="primary" :disabled="editDialog.value === null" @click="saveEdit">
            Speichern
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- ── Delete Dialog ────────────────────────────────────────── -->
    <v-dialog v-model="deleteDialog.open" max-width="400">
      <v-card rounded="lg">
        <v-card-title class="pt-5 px-6">Eintrag löschen</v-card-title>
        <v-card-text class="px-6">
          Soll der Eintrag von <strong>{{ deleteDialog.entry?.studentName }}</strong>
          ({{ deleteDialog.entry?.discipline }}) wirklich gelöscht werden?
        </v-card-text>
        <v-card-actions class="px-6 pb-5">
          <v-spacer />
          <v-btn variant="text" @click="deleteDialog.open = false">Abbrechen</v-btn>
          <v-btn variant="flat" color="error" @click="confirmDelete">Löschen</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- ── New Entry Dialog ─────────────────────────────────────── -->
    <v-dialog v-model="newDialog.open" max-width="480">
      <v-card rounded="lg">
        <v-card-title class="pt-5 px-6">Neuer Eintrag</v-card-title>
        <v-card-text class="px-6">
          <v-text-field
            v-model="newDialog.studentName"
            label="Name"
            variant="outlined"
            density="comfortable"
            class="mb-3"
          />
          <v-select
            v-model="newDialog.gender"
            :items="[{ title: 'Männlich', value: 'male' }, { title: 'Weiblich', value: 'female' }]"
            label="Geschlecht"
            variant="outlined"
            density="comfortable"
            class="mb-3"
          />
          <v-select
            v-model="newDialog.discipline"
            :items="allDisciplines"
            label="Disziplin"
            variant="outlined"
            density="comfortable"
            class="mb-3"
          />
          <v-text-field
            v-model.number="newDialog.value"
            :label="newDialog.discipline ? `Wert (${unitForDiscipline(newDialog.discipline)})` : 'Wert'"
            type="number"
            variant="outlined"
            density="comfortable"
            class="mb-3"
          />
          <v-text-field
            v-model="newDialog.date"
            label="Datum"
            type="date"
            variant="outlined"
            density="comfortable"
            class="mb-3"
          />
          <v-text-field
            v-model.number="newDialog.points"
            label="Punkte (0–100)"
            type="number"
            variant="outlined"
            density="comfortable"
          />
        </v-card-text>
        <v-card-actions class="px-6 pb-5">
          <v-spacer />
          <v-btn variant="text" @click="newDialog.open = false">Abbrechen</v-btn>
          <v-btn variant="flat" color="primary" :disabled="!newDialogValid" @click="saveNewEntry">
            Speichern
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useAuthenticationStore } from '@/stores/authentication';
import type { FitnessAttempt } from '@/models/sportsTest/fitnessAttempt';
import { mockAttempts } from '@/mocks/fitnessAttempts';

const authStore = useAuthenticationStore();
const isTeacher = computed(() => authStore.userRoles.includes('physicalEducationTeacher'));

const activeView = ref<'leaderboard' | 'class-management'>('leaderboard');

// ── Mutable in-memory dataset ───────────────────────────────────
const attempts = ref<FitnessAttempt[]>([...mockAttempts]);

// ── Derived lists ───────────────────────────────────────────────
const allDisciplines = computed(() => {
  const set = new Set(attempts.value.map((a) => a.discipline));
  return [...set].sort();
});

const allClasses = computed(() => {
  const set = new Set(attempts.value.map((a) => a.classOrProfession));
  return [...set].sort();
});

// ── Leaderboard state ───────────────────────────────────────────
const rankGender = ref<'male' | 'female'>('male');
const rankDiscipline = ref(allDisciplines.value[0] ?? '');

const leaderboardEntries = computed(() =>
  attempts.value
    .filter((a) => a.discipline === rankDiscipline.value && a.gender === rankGender.value)
    .sort((a, b) => b.value - a.value)
    .slice(0, 10)
    .map((a, i) => ({ ...a, rank: i + 1 }))
);

// ── Class management state ──────────────────────────────────────
const selectedClass = ref(allClasses.value[0] ?? '');

const classEntries = computed(() =>
  attempts.value
    .filter((a) => a.classOrProfession === selectedClass.value)
    .sort((a, b) =>
      a.discipline.localeCompare(b.discipline) || b.value - a.value
    )
);

// ── Edit dialog ─────────────────────────────────────────────────
const editDialog = ref({
  open: false,
  entry: null as (FitnessAttempt & { rank?: number }) | null,
  value: null as number | null,
});

function openEditDialog(entry: FitnessAttempt) {
  editDialog.value = { open: true, entry, value: entry.value };
}

function saveEdit() {
  if (!editDialog.value.entry || editDialog.value.value === null) return;
  const idx = attempts.value.findIndex((a) => a.id === editDialog.value.entry!.id);
  if (idx !== -1) attempts.value[idx] = { ...attempts.value[idx], value: editDialog.value.value };
  editDialog.value.open = false;
}

// ── Delete dialog ───────────────────────────────────────────────
const deleteDialog = ref({
  open: false,
  entry: null as FitnessAttempt | null,
});

function openDeleteDialog(entry: FitnessAttempt) {
  deleteDialog.value = { open: true, entry };
}

function confirmDelete() {
  if (!deleteDialog.value.entry) return;
  attempts.value = attempts.value.filter((a) => a.id !== deleteDialog.value.entry!.id);
  deleteDialog.value.open = false;
}

// ── New entry dialog ────────────────────────────────────────────
const newDialog = ref({
  open: false,
  studentName: '',
  gender: 'male' as 'male' | 'female',
  discipline: '',
  value: null as number | null,
  date: new Date().toISOString().slice(0, 10),
  points: null as number | null,
});

const newDialogValid = computed(() =>
  !!newDialog.value.studentName &&
  !!newDialog.value.discipline &&
  newDialog.value.value !== null &&
  newDialog.value.points !== null
);

function openNewDialog() {
  newDialog.value = {
    open: true,
    studentName: '',
    gender: 'male',
    discipline: allDisciplines.value[0] ?? '',
    value: null,
    date: new Date().toISOString().slice(0, 10),
    points: null,
  };
}

function saveNewEntry() {
  if (!newDialogValid.value) return;
  const d = newDialog.value;
  const dateStr = d.date;
  const year = new Date(dateStr).getFullYear();
  const month = new Date(dateStr).getMonth();
  const schoolYear = month >= 7 ? `${year}/${year + 1}` : `${year - 1}/${year}`;
  attempts.value.push({
    id: Date.now().toString(),
    studentName: d.studentName,
    gender: d.gender,
    discipline: d.discipline,
    value: d.value!,
    unit: unitForDiscipline(d.discipline),
    date: dateStr,
    schoolYear,
    classOrProfession: selectedClass.value,
    points: d.points!,
  });
  newDialog.value.open = false;
}

// ── Helpers ─────────────────────────────────────────────────────
function unitForDiscipline(discipline: string): string {
  if (discipline === '12-Minutenlauf') return 'Runden';
  if (discipline === 'Standweitsprung' || discipline === 'Rumpfbeuge') return 'cm';
  if (discipline === 'Ballwurf') return 'm';
  return 'Anzahl';
}

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
/* ── Page ──────────────────────────────────────────────────────── */
.ft-page {
  min-height: 100vh;
  background: #F8FAFC;
  font-family: inherit;
}

/* ── Header ────────────────────────────────────────────────────── */
.ft-header {
  background: #fff;
  border-bottom: 1px solid #E2E8F0;
  padding: 0 32px;
  height: 60px;
  display: flex;
  align-items: center;
}

.ft-header-inner {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.ft-brand {
  display: flex;
  align-items: center;
  gap: 10px;
}

.ft-brand-title {
  font-size: 1rem;
  font-weight: 700;
  color: #0F172A;
  line-height: 1.2;
}

.ft-brand-sub {
  font-size: 0.72rem;
  color: #64748B;
}

.ft-nav {
  display: flex;
  gap: 8px;
}

.ft-nav-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 7px 16px;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  border: 1px solid transparent;
  background: transparent;
  color: #64748B;
  transition: all 0.15s;
}

.ft-nav-btn:hover {
  background: #F1F5F9;
  color: #1E293B;
}

.ft-nav-btn--active {
  background: #2563EB;
  color: #fff;
  border-color: #2563EB;
}

.ft-nav-btn--active:hover {
  background: #1D4ED8;
}

/* ── Content ───────────────────────────────────────────────────── */
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

/* ── Filters ───────────────────────────────────────────────────── */
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
  letter-spacing: 0.01em;
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

.ft-gender-btn:first-child {
  border-radius: 6px 0 0 6px;
}

.ft-gender-btn:last-child {
  border-radius: 0 6px 6px 0;
  border-left: none;
}

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
  appearance: auto;
}

.ft-select--wide {
  min-width: 280px;
  max-width: 400px;
}

/* ── Controls (class management) ──────────────────────────────── */
.ft-controls {
  display: flex;
  align-items: flex-end;
  gap: 16px;
  margin-bottom: 24px;
  flex-wrap: wrap;
}

.ft-btn-primary {
  padding: 9px 20px;
  background: #2563EB;
  color: #fff;
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  white-space: nowrap;
  transition: background 0.15s;
}

.ft-btn-primary:hover {
  background: #1D4ED8;
}

/* ── Card / Table ──────────────────────────────────────────────── */
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

.ft-table thead tr {
  border-bottom: 1px solid #E2E8F0;
}

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

.ft-table tbody tr:last-child td {
  border-bottom: none;
}

.ft-table tbody tr:hover td {
  background: #FAFBFC;
}

.ft-th-rank { width: 72px; }
.ft-th-actions { width: 90px; }

/* ── Rank circle ───────────────────────────────────────────────── */
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

/* ── Cell variants ─────────────────────────────────────────────── */
.ft-td-name { font-weight: 500; color: #0F172A; }
.ft-td-value { font-weight: 700; color: #0F172A; }
.ft-td-muted { color: #64748B; }
.ft-td-class { color: #0891B2; font-weight: 500; }
.ft-td-actions { white-space: nowrap; }

/* ── Points badge ──────────────────────────────────────────────── */
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

/* ── Note badge ────────────────────────────────────────────────── */
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

/* ── Gender chip ───────────────────────────────────────────────── */
.ft-gender-chip {
  display: inline-block;
  padding: 3px 10px;
  border-radius: 20px;
  font-size: 0.78rem;
  font-weight: 500;
}

.ft-gender-chip--male {
  background: #DBEAFE;
  color: #1D4ED8;
}

.ft-gender-chip--female {
  background: #FCE7F3;
  color: #BE185D;
}

/* ── Action icon buttons ───────────────────────────────────────── */
.ft-icon-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 30px;
  height: 30px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  background: transparent;
  transition: background 0.15s;
}

.ft-icon-btn + .ft-icon-btn {
  margin-left: 4px;
}

.ft-icon-btn--edit { color: #2563EB; }
.ft-icon-btn--edit:hover { background: #EFF6FF; }
.ft-icon-btn--delete { color: #DC2626; }
.ft-icon-btn--delete:hover { background: #FEF2F2; }

/* ── Empty state ───────────────────────────────────────────────── */
.ft-empty-cell {
  text-align: center;
  color: #94A3B8;
  padding: 48px 16px !important;
  font-size: 0.9rem;
}
</style>
