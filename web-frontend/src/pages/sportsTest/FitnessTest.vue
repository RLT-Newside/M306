<template>
  <div class="ft-page">
    <!-- ── Standalone Header ──────────────────────────────────── -->
    <div class="ft-header">
      <div class="ft-header-inner">
        <div class="ft-brand">
          <v-icon color="primary" size="32">mdi-trophy-outline</v-icon>
          <div class="ft-brand-text">
            <div class="ft-brand-title">GIBZ Fitnesstest</div>
            <div class="ft-brand-sub">Gewerblich-industrielles Bildungszentrum Bern</div>
          </div>
        </div>
        <div class="ft-nav-buttons">
          <v-btn
            :variant="activeView === 'leaderboard' ? 'flat' : 'outlined'"
            :color="activeView === 'leaderboard' ? 'primary' : 'default'"
            prepend-icon="mdi-trophy"
            @click="activeView = 'leaderboard'"
          >
            Globale Bestenliste
          </v-btn>
          <v-btn
            v-if="isTeacher"
            :variant="activeView === 'class-management' ? 'flat' : 'outlined'"
            :color="activeView === 'class-management' ? 'primary' : 'default'"
            prepend-icon="mdi-account-group"
            @click="activeView = 'class-management'"
          >
            Klassen-Verwaltung
          </v-btn>
        </div>
      </div>
    </div>

    <!-- ── Content ────────────────────────────────────────────── -->
    <div class="ft-content">

      <!-- ══ GLOBALE BESTENLISTE ══════════════════════════════ -->
      <template v-if="activeView === 'leaderboard'">
        <!-- Filters row -->
        <div class="ft-filters">
          <v-select
            v-model="rankDiscipline"
            :items="disciplineItems"
            item-title="label"
            item-value="key"
            label="Disziplin"
            variant="outlined"
            density="comfortable"
            hide-details
            style="max-width: 280px"
          />
          <v-btn-toggle
            v-model="rankGender"
            mandatory
            color="primary"
            variant="outlined"
            divided
          >
            <v-btn value="m" prepend-icon="mdi-gender-male">Männer</v-btn>
            <v-btn value="f" prepend-icon="mdi-gender-female">Frauen</v-btn>
          </v-btn-toggle>
        </div>

        <!-- Loading -->
        <v-progress-linear v-if="rankLoading" indeterminate color="primary" class="my-4" />

        <!-- Error -->
        <v-alert v-else-if="rankError" type="error" variant="tonal" class="my-4">
          Die Bestenliste konnte nicht geladen werden.
        </v-alert>

        <!-- Ranked table -->
        <div v-else-if="ranking" class="ft-card">
          <table class="ft-table">
            <thead>
              <tr>
                <th style="width: 64px">Rang</th>
                <th>Name</th>
                <th>Wert</th>
                <th>Datum</th>
                <th>Schuljahr</th>
                <th>Klasse</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="entry in ranking"
                :key="entry.rank"
                :class="{ 'ft-row-top3': entry.rank <= 3 }"
              >
                <td>
                  <div
                    class="ft-rank-circle"
                    :style="{ background: rankCircleColor(entry.rank) }"
                  >
                    {{ entry.rank }}
                  </div>
                </td>
                <td class="ft-name">{{ entry.name }}</td>
                <td class="ft-result">{{ formatResult(rankDiscipline, entry.result) }}</td>
                <td class="ft-muted">{{ formatDate(entry.momentUtc) }}</td>
                <td :class="entry.schoolYear >= currentSchoolYear ? 'ft-year-current' : 'ft-year-past'">
                  {{ formatSchoolYear(entry.schoolYear) }}
                </td>
                <td>{{ entry.className }}</td>
              </tr>
              <tr v-if="ranking.length === 0">
                <td colspan="6" class="text-center ft-muted py-6">
                  Keine Daten für diese Disziplin vorhanden.
                </td>
              </tr>
            </tbody>
          </table>

          <div class="ft-footnote">
            Hinweis: Diese Bestenliste zeigt die Top 10 Leistungen für die ausgewählte Disziplin
            und Geschlecht. Die Daten umfassen alle durchgeführten GIBZ-Fitnesstest-Einträge.
          </div>
        </div>
      </template>

      <!-- ══ KLASSEN-VERWALTUNG ════════════════════════════════ -->
      <template v-else-if="activeView === 'class-management' && isTeacher">
        <!-- Controls row -->
        <div class="ft-filters">
          <v-select
            v-model="selectedCohortId"
            :items="cohortItems"
            item-title="label"
            item-value="id"
            label="Klasse auswählen"
            variant="outlined"
            density="comfortable"
            hide-details
            :loading="cohortsLoading"
            style="max-width: 260px"
          />
          <v-btn
            color="primary"
            prepend-icon="mdi-plus"
            :disabled="!selectedCohortId"
            @click="openNewEntryDialog"
          >
            Neuer Eintrag
          </v-btn>
        </div>

        <!-- Loading / error -->
        <v-progress-linear v-if="classLoading" indeterminate color="primary" class="my-4" />
        <v-alert v-else-if="classError" type="error" variant="tonal" class="my-4">
          Die Klassenübersicht konnte nicht geladen werden.
        </v-alert>

        <!-- Flat entries table -->
        <div v-else-if="flatEntries.length > 0" class="ft-card">
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
                <th style="width: 80px">Aktionen</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="entry in flatEntries" :key="entry.userId + entry.discipline">
                <td class="ft-name">{{ entry.lastName }}, {{ entry.firstName }}</td>
                <td>
                  <v-chip
                    size="small"
                    :color="entry.gender === 'm' ? 'blue' : 'pink'"
                    variant="tonal"
                  >
                    {{ entry.gender === 'm' ? 'Männlich' : 'Weiblich' }}
                  </v-chip>
                </td>
                <td>{{ disciplineLabel(entry.discipline) }}</td>
                <td class="ft-result">{{ formatResult(entry.discipline, entry.result) }}</td>
                <td>
                  <v-chip
                    size="small"
                    :color="pointsColor(entry.points)"
                    variant="flat"
                    class="text-white font-weight-bold"
                  >
                    {{ entry.points }} Pkt.
                  </v-chip>
                </td>
                <td>{{ computeNote(entry.points) }}</td>
                <td class="ft-muted">{{ formatDate(entry.momentUtc) }}</td>
                <td>{{ formatSchoolYear(entry.schoolYear) }}</td>
                <td>
                  <v-btn
                    size="small"
                    icon
                    variant="text"
                    color="primary"
                    @click="openEditEntry(entry)"
                  >
                    <v-icon size="18">mdi-pencil</v-icon>
                  </v-btn>
                  <v-btn
                    size="small"
                    icon
                    variant="text"
                    color="error"
                    @click="openDeleteEntry(entry)"
                  >
                    <v-icon size="18">mdi-delete</v-icon>
                  </v-btn>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div v-else-if="selectedCohortId && !classLoading" class="ft-card ft-empty">
          <v-icon size="40" color="medium-emphasis">mdi-clipboard-text-outline</v-icon>
          <p class="text-medium-emphasis mt-2">Keine Einträge für diese Klasse.</p>
        </div>
      </template>
    </div>

    <!-- ── Edit dialog ────────────────────────────────────────── -->
    <v-dialog v-model="editDialog.open" max-width="440">
      <v-card>
        <v-card-title>Ergebnis bearbeiten</v-card-title>
        <v-card-subtitle v-if="editDialog.entry">
          {{ editDialog.entry.lastName }}, {{ editDialog.entry.firstName }} –
          {{ disciplineLabel(editDialog.entry.discipline) }}
        </v-card-subtitle>
        <v-card-text>
          <v-text-field
            v-model.number="editDialog.value"
            :label="editInputLabel(editDialog.entry?.discipline ?? '')"
            type="number"
            variant="outlined"
            density="comfortable"
            autofocus
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="editDialog.open = false">Abbrechen</v-btn>
          <v-btn
            color="primary"
            :loading="editDialog.saving"
            :disabled="editDialog.value === null"
            @click="saveEdit"
          >
            Speichern
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- ── Delete confirm dialog ──────────────────────────────── -->
    <v-dialog v-model="deleteDialog.open" max-width="400">
      <v-card>
        <v-card-title>Eintrag löschen?</v-card-title>
        <v-card-text v-if="deleteDialog.entry">
          Möchtest du den Eintrag für
          <strong>{{ deleteDialog.entry.lastName }}, {{ deleteDialog.entry.firstName }}</strong>
          – {{ disciplineLabel(deleteDialog.entry.discipline) }} wirklich löschen?
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="deleteDialog.open = false">Abbrechen</v-btn>
          <v-btn
            color="error"
            :loading="deleteDialog.deleting"
            @click="confirmDelete"
          >
            Löschen
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- ── New entry dialog ───────────────────────────────────── -->
    <v-dialog v-model="newEntryDialog.open" max-width="480">
      <v-card>
        <v-card-title>Neuer Eintrag</v-card-title>
        <v-card-text>
          <v-select
            v-model="newEntryDialog.userId"
            :items="studentItems"
            item-title="label"
            item-value="userId"
            label="Lernende/r"
            variant="outlined"
            density="comfortable"
            class="mb-3"
          />
          <v-select
            v-model="newEntryDialog.discipline"
            :items="disciplineItems"
            item-title="label"
            item-value="key"
            label="Disziplin"
            variant="outlined"
            density="comfortable"
            class="mb-3"
          />
          <v-text-field
            v-model.number="newEntryDialog.value"
            :label="editInputLabel(newEntryDialog.discipline)"
            type="number"
            variant="outlined"
            density="comfortable"
          />
          <v-radio-group v-model="newEntryDialog.gender" inline label="Geschlecht">
            <v-radio label="Männlich" value="m" />
            <v-radio label="Weiblich" value="f" />
          </v-radio-group>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="newEntryDialog.open = false">Abbrechen</v-btn>
          <v-btn
            color="primary"
            :loading="newEntryDialog.saving"
            :disabled="!newEntryDialog.userId || !newEntryDialog.discipline || newEntryDialog.value === null"
            @click="saveNewEntry"
          >
            Speichern
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRequest } from 'alova/client';
import {
  getRanking,
  getCohorts,
  getClassOverview,
  setResult,
  deleteResult,
} from '@/api/alovaMethods/fitnessCheck';
import type { ClassOverviewStudent } from '@/models/sportsTest/classOverview';
import { useAuthenticationStore } from '@/stores/authentication';
import { useNotifications } from '@/composables/useNotifications';
const { showSuccess, showError } = useNotifications();
const authStore = useAuthenticationStore();

const isTeacher = computed(() => authStore.userRoles.includes('physicalEducationTeacher'));

const activeView = ref<'leaderboard' | 'class-management'>(
  isTeacher.value ? 'class-management' : 'leaderboard'
);

// ── Discipline config ───────────────────────────────────────────
interface DisciplineConfig {
  key: string;
  label: string;
  unit: string;
}

const disciplines: DisciplineConfig[] = [
  { key: 'CoreStrength',     label: 'Rumpfkraft',       unit: 'Sek.' },
  { key: 'MedicineBallPush', label: 'Medizinballstoss', unit: 'cm' },
  { key: 'StandingLongJump', label: 'Standweitsprung',  unit: 'cm' },
  { key: 'ShuttleRun',       label: 'Pendellauf',        unit: 'Sek.' },
  { key: 'TwelveMinutesRun', label: '12-Minutenlauf',   unit: 'Runden' },
  { key: 'OneLegStand',      label: 'Einbeinstand',      unit: 'Sek.' },
];

const disciplineItems = disciplines.map((d) => ({ key: d.key, label: d.label }));

// ── Globale Bestenliste ─────────────────────────────────────────
const rankDiscipline = ref('TwelveMinutesRun');
const rankGender = ref<'m' | 'f'>('m');

const {
  data: ranking,
  loading: rankLoading,
  error: rankError,
  send: reloadRanking,
} = useRequest(
  (discipline: string, gender: 'm' | 'f') => getRanking(discipline, gender, 10),
  { immediate: false }
);

watch([rankDiscipline, rankGender], ([d, g]) => reloadRanking(d, g), { immediate: true });

// ── Klassen-Verwaltung ──────────────────────────────────────────
const { data: cohorts, loading: cohortsLoading } = useRequest(getCohorts());

const cohortItems = computed(() =>
  (cohorts.value ?? []).map((c) => ({
    id: c.id,
    label: `${c.classNameVocationalEducation} (${c.firstSchoolYear}/${String(c.firstSchoolYear + 1).slice(-2)})`,
  }))
);

const selectedCohortId = ref<string | null>(null);

const {
  data: classOverview,
  loading: classLoading,
  error: classError,
  send: reloadClass,
} = useRequest((id: string) => getClassOverview(id), { immediate: false });

watch(selectedCohortId, (id) => { if (id) reloadClass(id); });

// Flatten: one row per student × discipline that has a result
interface FlatEntry {
  userId: string;
  firstName: string;
  lastName: string;
  gender: string;
  discipline: string;
  result: number;
  points: number;
  momentUtc: string;
  schoolYear: number;
}

const flatEntries = computed<FlatEntry[]>(() => {
  if (!classOverview.value) return [];
  const rows: FlatEntry[] = [];
  for (const student of classOverview.value.students) {
    for (const [discipline, dr] of Object.entries(student.disciplines)) {
      if (dr.result !== null && dr.result !== undefined && dr.momentUtc) {
        rows.push({
          userId: student.userId,
          firstName: student.firstName,
          lastName: student.lastName,
          gender: student.gender,
          discipline,
          result: dr.result,
          points: dr.points,
          momentUtc: dr.momentUtc,
          schoolYear: getSchoolYear(dr.momentUtc),
        });
      }
    }
  }
  // Sort by discipline then by lastName
  return rows.sort((a, b) =>
    a.discipline.localeCompare(b.discipline) || a.lastName.localeCompare(b.lastName)
  );
});

const studentItems = computed(() =>
  (classOverview.value?.students ?? []).map((s: ClassOverviewStudent) => ({
    userId: s.userId,
    label: `${s.lastName}, ${s.firstName}`,
    gender: s.gender,
  }))
);

// ── Edit dialog ─────────────────────────────────────────────────
const editDialog = ref({
  open: false,
  entry: null as FlatEntry | null,
  value: null as number | null,
  saving: false,
});

function openEditEntry(entry: FlatEntry): void {
  editDialog.value = { open: true, entry, value: entry.result, saving: false };
}

async function saveEdit(): Promise<void> {
  const { entry, value } = editDialog.value;
  if (!entry || value === null || !selectedCohortId.value) return;
  editDialog.value.saving = true;
  try {
    await setResult(selectedCohortId.value, entry.userId, entry.discipline, {
      result: value,
      gender: entry.gender,
    }).send();
    showSuccess('Ergebnis gespeichert.');
    editDialog.value.open = false;
    await reloadClass(selectedCohortId.value);
  } catch (err) {
    showError(err, {
      notFound: 'Eintrag nicht gefunden.',
      forbidden: 'Keine Berechtigung.',
      conflict: 'Konflikt beim Speichern.',
      fallback: 'Speichern fehlgeschlagen.',
    });
  } finally {
    editDialog.value.saving = false;
  }
}

// ── Delete dialog ───────────────────────────────────────────────
const deleteDialog = ref({
  open: false,
  entry: null as FlatEntry | null,
  deleting: false,
});

function openDeleteEntry(entry: FlatEntry): void {
  deleteDialog.value = { open: true, entry, deleting: false };
}

async function confirmDelete(): Promise<void> {
  const { entry } = deleteDialog.value;
  if (!entry || !selectedCohortId.value) return;
  deleteDialog.value.deleting = true;
  try {
    await deleteResult(selectedCohortId.value, entry.userId, entry.discipline).send();
    showSuccess('Eintrag gelöscht.');
    deleteDialog.value.open = false;
    await reloadClass(selectedCohortId.value);
  } catch (err) {
    showError(err, {
      notFound: 'Eintrag nicht gefunden.',
      forbidden: 'Keine Berechtigung.',
      conflict: '',
      fallback: 'Löschen fehlgeschlagen.',
    });
  } finally {
    deleteDialog.value.deleting = false;
  }
}

// ── New entry dialog ────────────────────────────────────────────
const newEntryDialog = ref({
  open: false,
  userId: '',
  discipline: 'TwelveMinutesRun',
  value: null as number | null,
  gender: 'm',
  saving: false,
});

function openNewEntryDialog(): void {
  newEntryDialog.value = {
    open: true,
    userId: '',
    discipline: 'TwelveMinutesRun',
    value: null,
    gender: 'm',
    saving: false,
  };
}

async function saveNewEntry(): Promise<void> {
  const { userId, discipline, value, gender } = newEntryDialog.value;
  if (!userId || value === null || !selectedCohortId.value) return;
  newEntryDialog.value.saving = true;
  try {
    await setResult(selectedCohortId.value, userId, discipline, {
      result: value,
      gender,
    }).send();
    showSuccess('Eintrag gespeichert.');
    newEntryDialog.value.open = false;
    await reloadClass(selectedCohortId.value);
  } catch (err) {
    showError(err, {
      notFound: 'Student nicht gefunden.',
      forbidden: 'Keine Berechtigung.',
      conflict: 'Konflikt beim Speichern.',
      fallback: 'Speichern fehlgeschlagen.',
    });
  } finally {
    newEntryDialog.value.saving = false;
  }
}

// ── Helpers ─────────────────────────────────────────────────────
const currentSchoolYear = computed(() => {
  const now = new Date();
  return now.getMonth() >= 7 ? now.getFullYear() : now.getFullYear() - 1;
});

function getSchoolYear(isoDate: string): number {
  const d = new Date(isoDate);
  return d.getMonth() >= 7 ? d.getFullYear() : d.getFullYear() - 1;
}

function formatResult(disciplineKey: string, result: number): string {
  switch (disciplineKey) {
    case 'CoreStrength':
    case 'OneLegStand':
      return `${result} Sek.`;
    case 'MedicineBallPush':
    case 'StandingLongJump':
      return `${result} cm`;
    case 'ShuttleRun':
      return `${(result / 1000).toFixed(2)} Sek.`;
    case 'TwelveMinutesRun':
      return `${result} Runden`;
    default:
      return String(result);
  }
}

function formatDate(isoDate: string): string {
  return new Date(isoDate).toLocaleDateString('de-CH', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });
}

function formatSchoolYear(year: number): string {
  return `${year}/${String(year + 1).slice(-2)}`;
}

function disciplineLabel(key: string): string {
  return disciplines.find((d) => d.key === key)?.label ?? key;
}

function editInputLabel(key: string): string {
  const d = disciplines.find((dd) => dd.key === key);
  return d ? `Wert (${d.unit})` : 'Wert';
}

function rankCircleColor(rank: number): string {
  if (rank === 1) return '#FFD700';
  if (rank === 2) return '#C0C0C0';
  if (rank === 3) return '#CD7F32';
  return '#E2E8F0';
}

function pointsColor(points: number): string {
  if (points >= 22) return 'green';
  if (points >= 16) return 'teal';
  if (points >= 7) return 'orange';
  return 'red';
}

function computeNote(points: number): string {
  const note = 1 + (points / 25) * 5;
  return note.toFixed(1);
}
</script>

<style scoped>
.ft-page {
  min-height: 100vh;
  background: #f8fafc;
}

/* Header */
.ft-header {
  background: #fff;
  border-bottom: 1px solid #e2e8f0;
  padding: 12px 24px;
}

.ft-header-inner {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}

.ft-brand {
  display: flex;
  align-items: center;
  gap: 12px;
}

.ft-brand-title {
  font-size: 1.15rem;
  font-weight: 700;
  color: #1e293b;
  line-height: 1.2;
}

.ft-brand-sub {
  font-size: 0.75rem;
  color: #64748b;
}

.ft-nav-buttons {
  display: flex;
  gap: 8px;
}

/* Content */
.ft-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 24px;
}

/* Filters row */
.ft-filters {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 24px;
  flex-wrap: wrap;
}

/* Card */
.ft-card {
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  overflow: hidden;
}

.ft-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 48px 24px;
}

/* Table */
.ft-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.ft-table thead tr {
  border-bottom: 1px solid #e2e8f0;
}

.ft-table thead th {
  padding: 12px 16px;
  text-align: left;
  font-size: 0.78rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  color: #64748b;
  background: #fff;
}

.ft-table tbody tr {
  border-bottom: 1px solid #f1f5f9;
  transition: background 0.12s;
}

.ft-table tbody tr:hover {
  background: #f8fafc;
}

.ft-table tbody tr.ft-row-top3 {
  background: #fffbeb;
}

.ft-table tbody tr.ft-row-top3:hover {
  background: #fef3c7;
}

.ft-table tbody td {
  padding: 14px 16px;
  color: #334155;
}

/* Rank circle */
.ft-rank-circle {
  width: 34px;
  height: 34px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 0.85rem;
  color: #475569;
}

/* Specific cell styles */
.ft-name { font-weight: 600; color: #1e293b; }
.ft-result { font-weight: 700; color: #1e293b; }
.ft-muted { color: #64748b; }
.ft-year-current { color: #0ea5e9; font-weight: 500; }
.ft-year-past { color: #64748b; }

/* Footnote */
.ft-footnote {
  padding: 12px 16px;
  font-size: 0.78rem;
  color: #94a3b8;
  border-top: 1px solid #f1f5f9;
}
</style>
