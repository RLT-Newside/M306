<template>
  <v-container>
    <!-- ── Header ─────────────────────────────────────────────── -->
    <v-row>
      <v-col cols="12">
        <h1 class="text-h3">Klassenübersicht Fitnesstest</h1>
      </v-col>
    </v-row>

    <!-- ── Cohort selector ───────────────────────────────────── -->
    <v-row>
      <v-col cols="12" sm="6" md="4">
        <v-select
          v-model="selectedCohortId"
          :items="cohortItems"
          item-title="label"
          item-value="id"
          label="Klasse auswählen"
          :loading="cohortsLoading"
          variant="outlined"
          density="comfortable"
          hide-details
        />
      </v-col>
    </v-row>

    <!-- ── Loading / error states ────────────────────────────── -->
    <v-row v-if="overviewLoading">
      <v-col class="text-center py-10">
        <v-progress-circular indeterminate color="primary" />
      </v-col>
    </v-row>

    <v-row v-else-if="overviewError">
      <v-col>
        <v-alert type="error" variant="tonal">
          Die Klassenübersicht konnte nicht geladen werden.
        </v-alert>
      </v-col>
    </v-row>

    <!-- ── Class table ───────────────────────────────────────── -->
    <template v-else-if="overview">
      <v-row class="mt-2">
        <v-col>
          <v-card class="rally-table-surface">
            <v-card-text class="pa-0">
              <div style="overflow-x: auto">
                <v-table class="rally-table" fixed-header>
                  <thead>
                    <tr>
                      <th class="text-left sticky-col">Name</th>
                      <th class="text-left">G</th>
                      <th
                        v-for="d in disciplines"
                        :key="d.key"
                        class="text-center discipline-header"
                      >
                        {{ d.label }}
                      </th>
                      <th class="text-right">∑ Punkte</th>
                      <th class="text-right">⌀ Punkte</th>
                      <th class="text-center">Note</th>
                    </tr>
                  </thead>
                  <tbody>
                    <!-- ── Student rows ─── -->
                    <tr v-for="student in overview.students" :key="student.userId">
                      <td class="sticky-col font-weight-medium">
                        {{ student.lastName }}, {{ student.firstName }}
                        <span v-if="!student.firstName && !student.lastName" class="text-caption text-medium-emphasis">
                          {{ student.userId.substring(0, 8) }}…
                        </span>
                      </td>
                      <td class="text-caption">{{ student.gender }}</td>

                      <!-- Discipline cells -->
                      <td
                        v-for="d in disciplines"
                        :key="d.key"
                        class="text-center discipline-cell"
                      >
                        <template v-if="student.disciplines[d.key]?.result !== null && student.disciplines[d.key]?.result !== undefined">
                          <div class="d-flex flex-column align-center">
                            <span class="font-weight-bold text-primary">
                              {{ formatResult(d.key, student.disciplines[d.key].result!) }}
                            </span>
                            <span class="text-caption text-medium-emphasis">
                              {{ student.disciplines[d.key].points }} Pkt.
                            </span>
                          </div>
                        </template>
                        <template v-else>
                          <!-- Missing result -->
                          <v-chip
                            v-if="student.disciplines[d.key]?.annotation"
                            size="x-small"
                            color="orange"
                            variant="tonal"
                          >
                            {{ student.disciplines[d.key].annotation }}
                          </v-chip>
                          <span v-else class="text-caption text-error">–</span>
                        </template>

                        <!-- Edit / annotate button -->
                        <v-btn
                          size="x-small"
                          icon
                          variant="text"
                          class="ml-1"
                          @click="openEditDialog(student, d.key)"
                        >
                          <v-icon size="14">mdi-pencil</v-icon>
                        </v-btn>
                      </td>

                      <td class="text-right font-weight-bold">
                        {{ student.totalPoints ?? '–' }}
                      </td>
                      <td class="text-right">
                        {{ student.averagePoints !== null ? student.averagePoints?.toFixed(1) : '–' }}
                      </td>
                      <td class="text-center">
                        <v-chip
                          v-if="student.rating"
                          size="small"
                          :color="ratingColor(student.rating)"
                          variant="tonal"
                        >
                          {{ student.rating }}
                        </v-chip>
                        <span v-else>–</span>
                      </td>
                    </tr>

                    <!-- ── Class average row ─── -->
                    <tr class="class-avg-row font-weight-bold">
                      <td class="sticky-col text-caption text-uppercase text-medium-emphasis">Klassenschnitt</td>
                      <td></td>
                      <td
                        v-for="d in disciplines"
                        :key="d.key"
                        class="text-center discipline-cell"
                      >
                        <div class="d-flex flex-column align-center" v-if="overview.classAverages[d.key]?.averageResult !== null">
                          <span class="text-caption">
                            {{ formatResult(d.key, overview.classAverages[d.key].averageResult!) }}
                          </span>
                          <span class="text-caption text-medium-emphasis">
                            ⌀ {{ overview.classAverages[d.key].averagePoints?.toFixed(1) }} Pkt.
                          </span>
                        </div>
                        <span v-else class="text-caption text-medium-emphasis">–</span>
                      </td>
                      <td></td>
                      <td></td>
                      <td></td>
                    </tr>
                  </tbody>
                </v-table>
              </div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </template>

    <!-- ── Edit dialog ───────────────────────────────────────── -->
    <v-dialog v-model="editDialog.open" max-width="480">
      <v-card>
        <v-card-title>
          Ergebnis bearbeiten –
          {{ disciplineLabel(editDialog.discipline) }}
        </v-card-title>
        <v-card-subtitle v-if="editDialog.student">
          {{ editDialog.student.lastName }}, {{ editDialog.student.firstName }}
        </v-card-subtitle>
        <v-card-text>
          <v-tabs v-model="editDialog.tab" class="mb-4">
            <v-tab value="result">Ergebnis</v-tab>
            <v-tab value="annotation">Annotation</v-tab>
          </v-tabs>

          <v-window v-model="editDialog.tab">
            <!-- Result tab -->
            <v-window-item value="result">
              <v-text-field
                v-model.number="editDialog.resultValue"
                :label="resultInputLabel(editDialog.discipline)"
                type="number"
                variant="outlined"
                density="comfortable"
              />
              <v-btn
                color="error"
                variant="text"
                class="mt-1"
                :disabled="!hasResult(editDialog)"
                @click="confirmDelete"
              >
                <v-icon start>mdi-delete</v-icon>
                Ergebnis löschen
              </v-btn>
            </v-window-item>

            <!-- Annotation tab -->
            <v-window-item value="annotation">
              <v-select
                v-model="editDialog.annotationValue"
                :items="annotationOptions"
                label="Annotation"
                variant="outlined"
                density="comfortable"
                clearable
              />
            </v-window-item>
          </v-window>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="editDialog.open = false">Abbrechen</v-btn>
          <v-btn
            color="primary"
            :loading="editDialog.saving"
            @click="saveEdit"
          >
            Speichern
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
// import { useRequest } from 'alova/client';
// import {
//   getCohorts,
//   getClassOverview,
//   setResult,
//   deleteResult,
//   setAnnotation,
// } from '@/api/alovaMethods/fitnessCheck';
import type { ClassOverview, ClassOverviewStudent, CohortInfo } from '@/models/sportsTest/classOverview';
import { useNotifications } from '@/composables/useNotifications';

const { showSuccess } = useNotifications();

// ── Disciplines config ──────────────────────────────────────────
const disciplines = [
  { key: 'CoreStrength', label: 'Rumpfkraft', unit: 'Sek.' },
  { key: 'MedicineBallPush', label: 'Medizinballstoss', unit: 'cm' },
  { key: 'StandingLongJump', label: 'Standweitsprung', unit: 'cm' },
  { key: 'ShuttleRun', label: 'Pendellauf', unit: 'ms' },
  { key: 'TwelveMinutesRun', label: '12-Minutenlauf', unit: 'Runden' },
  { key: 'OneLegStand', label: 'Einbeinstand', unit: 'Sek.' },
];

const annotationOptions = ['verletzt', 'dispensiert', 'krank', 'entschuldigt'];

// ── Cohort loading ──────────────────────────────────────────────
// MOCK: const { data: cohorts, loading: cohortsLoading } = useRequest(getCohorts());
const cohortsLoading = ref(false);
const mockCohorts: CohortInfo[] = [
  {
    id: 'cohort-im22a',
    profession: 'Informatiker EFZ',
    baccalaureate: false,
    schoolYear: 2024,
    firstSchoolYear: 2022,
    classNameVocationalEducation: 'IM22a',
    classNameBaccalaureate: null,
  },
  {
    id: 'cohort-kv22b',
    profession: 'Kaufmann EFZ',
    baccalaureate: true,
    schoolYear: 2024,
    firstSchoolYear: 2022,
    classNameVocationalEducation: 'KV22b',
    classNameBaccalaureate: 'KV22b-BM',
  },
  {
    id: 'cohort-mm22a',
    profession: 'Mediamatiker EFZ',
    baccalaureate: false,
    schoolYear: 2024,
    firstSchoolYear: 2022,
    classNameVocationalEducation: 'MM22a',
    classNameBaccalaureate: null,
  },
];
const cohorts = ref<CohortInfo[]>(mockCohorts);

const mockOverviewByClass: Record<string, ClassOverview> = {
  'cohort-im22a': {
    cohort: mockCohorts[0],
    students: [
      {
        userId: 'u-001', firstName: 'Max', lastName: 'Muster', gender: 'M',
        disciplines: {
          CoreStrength:     { result: 142, points: 9,  momentUtc: '2025-03-10T09:00:00Z', annotation: null },
          MedicineBallPush: { result: 810, points: 10, momentUtc: '2025-03-10T09:10:00Z', annotation: null },
          StandingLongJump: { result: 262, points: 9,  momentUtc: '2025-03-10T09:20:00Z', annotation: null },
          ShuttleRun:       { result: 9050, points: 8, momentUtc: '2025-03-10T09:30:00Z', annotation: null },
          TwelveMinutesRun: { result: 33, points: 7,   momentUtc: '2025-03-10T09:40:00Z', annotation: null },
          OneLegStand:      { result: 88, points: 9,   momentUtc: '2025-03-10T09:50:00Z', annotation: null },
        },
        totalPoints: 52, averagePoints: 8.67, rating: 'UED',
      },
      {
        userId: 'u-002', firstName: 'Anna', lastName: 'Beispiel', gender: 'F',
        disciplines: {
          CoreStrength:     { result: 98,  points: 7,  momentUtc: '2025-03-10T10:00:00Z', annotation: null },
          MedicineBallPush: { result: 590, points: 7,  momentUtc: '2025-03-10T10:10:00Z', annotation: null },
          StandingLongJump: { result: 198, points: 7,  momentUtc: '2025-03-10T10:20:00Z', annotation: null },
          ShuttleRun:       { result: null, points: 0, momentUtc: null,                   annotation: 'verletzt' },
          TwelveMinutesRun: { result: 28, points: 6,   momentUtc: '2025-03-10T10:40:00Z', annotation: null },
          OneLegStand:      { result: 65, points: 7,   momentUtc: '2025-03-10T10:50:00Z', annotation: null },
        },
        totalPoints: 34, averagePoints: 6.8, rating: 'EE',
      },
      {
        userId: 'u-003', firstName: 'Lukas', lastName: 'Grüter', gender: 'M',
        disciplines: {
          CoreStrength:     { result: 175, points: 10, momentUtc: '2025-03-11T09:00:00Z', annotation: null },
          MedicineBallPush: { result: 750, points: 9,  momentUtc: '2025-03-11T09:10:00Z', annotation: null },
          StandingLongJump: { result: null, points: 0, momentUtc: null,                   annotation: 'dispensiert' },
          ShuttleRun:       { result: 8780, points: 9, momentUtc: '2025-03-11T09:30:00Z', annotation: null },
          TwelveMinutesRun: { result: 40, points: 9,   momentUtc: '2025-03-11T09:40:00Z', annotation: null },
          OneLegStand:      { result: 102, points: 10, momentUtc: '2025-03-11T09:50:00Z', annotation: null },
        },
        totalPoints: 47, averagePoints: 9.4, rating: 'UED',
      },
      {
        userId: 'u-004', firstName: 'Sara', lastName: 'Huber', gender: 'F',
        disciplines: {
          CoreStrength:     { result: null, points: 0, momentUtc: null, annotation: null },
          MedicineBallPush: { result: null, points: 0, momentUtc: null, annotation: null },
          StandingLongJump: { result: null, points: 0, momentUtc: null, annotation: null },
          ShuttleRun:       { result: null, points: 0, momentUtc: null, annotation: null },
          TwelveMinutesRun: { result: null, points: 0, momentUtc: null, annotation: null },
          OneLegStand:      { result: null, points: 0, momentUtc: null, annotation: null },
        },
        totalPoints: null, averagePoints: null, rating: null,
      },
    ],
    classAverages: {
      CoreStrength:     { averageResult: 138, averagePoints: 8.7 },
      MedicineBallPush: { averageResult: 717, averagePoints: 8.7 },
      StandingLongJump: { averageResult: 230, averagePoints: 8.0 },
      ShuttleRun:       { averageResult: 8915, averagePoints: 8.5 },
      TwelveMinutesRun: { averageResult: 33.7, averagePoints: 7.3 },
      OneLegStand:      { averageResult: 85, averagePoints: 8.7 },
    },
  },
  'cohort-kv22b': {
    cohort: mockCohorts[1],
    students: [
      {
        userId: 'u-101', firstName: 'Elena', lastName: 'Vogel', gender: 'F',
        disciplines: {
          CoreStrength:     { result: 115, points: 8,  momentUtc: '2025-03-12T09:00:00Z', annotation: null },
          MedicineBallPush: { result: 610, points: 8,  momentUtc: '2025-03-12T09:10:00Z', annotation: null },
          StandingLongJump: { result: 210, points: 8,  momentUtc: '2025-03-12T09:20:00Z', annotation: null },
          ShuttleRun:       { result: 10200, points: 6, momentUtc: '2025-03-12T09:30:00Z', annotation: null },
          TwelveMinutesRun: { result: 30, points: 6,   momentUtc: '2025-03-12T09:40:00Z', annotation: null },
          OneLegStand:      { result: 55, points: 6,   momentUtc: '2025-03-12T09:50:00Z', annotation: null },
        },
        totalPoints: 42, averagePoints: 7.0, rating: 'UEE',
      },
      {
        userId: 'u-102', firstName: 'Tom', lastName: 'Baumann', gender: 'M',
        disciplines: {
          CoreStrength:     { result: 88,  points: 5,  momentUtc: '2025-03-12T10:00:00Z', annotation: null },
          MedicineBallPush: { result: 620, points: 7,  momentUtc: '2025-03-12T10:10:00Z', annotation: null },
          StandingLongJump: { result: 220, points: 7,  momentUtc: '2025-03-12T10:20:00Z', annotation: null },
          ShuttleRun:       { result: null, points: 0, momentUtc: null,                   annotation: 'krank' },
          TwelveMinutesRun: { result: 26, points: 5,   momentUtc: '2025-03-12T10:40:00Z', annotation: null },
          OneLegStand:      { result: 42, points: 5,   momentUtc: '2025-03-12T10:50:00Z', annotation: null },
        },
        totalPoints: 29, averagePoints: 5.8, rating: 'TE',
      },
    ],
    classAverages: {
      CoreStrength:     { averageResult: 101, averagePoints: 6.5 },
      MedicineBallPush: { averageResult: 615, averagePoints: 7.5 },
      StandingLongJump: { averageResult: 215, averagePoints: 7.5 },
      ShuttleRun:       { averageResult: null, averagePoints: null },
      TwelveMinutesRun: { averageResult: 28, averagePoints: 5.5 },
      OneLegStand:      { averageResult: 48, averagePoints: 5.5 },
    },
  },
  'cohort-mm22a': {
    cohort: mockCohorts[2],
    students: [
      {
        userId: 'u-201', firstName: 'Lena', lastName: 'Zimmermann', gender: 'F',
        disciplines: {
          CoreStrength:     { result: 130, points: 9,  momentUtc: '2025-03-13T09:00:00Z', annotation: null },
          MedicineBallPush: { result: 640, points: 8,  momentUtc: '2025-03-13T09:10:00Z', annotation: null },
          StandingLongJump: { result: 225, points: 8,  momentUtc: '2025-03-13T09:20:00Z', annotation: null },
          ShuttleRun:       { result: 9800, points: 7, momentUtc: '2025-03-13T09:30:00Z', annotation: null },
          TwelveMinutesRun: { result: 34, points: 8,   momentUtc: '2025-03-13T09:40:00Z', annotation: null },
          OneLegStand:      { result: 78, points: 8,   momentUtc: '2025-03-13T09:50:00Z', annotation: null },
        },
        totalPoints: 48, averagePoints: 8.0, rating: 'UEE',
      },
    ],
    classAverages: {
      CoreStrength:     { averageResult: 130, averagePoints: 9.0 },
      MedicineBallPush: { averageResult: 640, averagePoints: 8.0 },
      StandingLongJump: { averageResult: 225, averagePoints: 8.0 },
      ShuttleRun:       { averageResult: 9800, averagePoints: 7.0 },
      TwelveMinutesRun: { averageResult: 34, averagePoints: 8.0 },
      OneLegStand:      { averageResult: 78, averagePoints: 8.0 },
    },
  },
};

const cohortItems = computed(() =>
  (cohorts.value ?? []).map((c) => ({
    id: c.id,
    label: `${c.classNameVocationalEducation} (${c.firstSchoolYear}/${String(c.firstSchoolYear + 1).slice(-2)})`,
  }))
);

const selectedCohortId = ref<string | null>(null);

// ── Overview loading ────────────────────────────────────────────
// MOCK: const { data: overview, loading: overviewLoading, error: overviewError, send: reloadOverview } = useRequest(...);
const overviewLoading = ref(false);
const overviewError = ref(null);
const overview = ref<ClassOverview | null>(null);
const reloadOverview = async (cohortId: string) => {
  overviewLoading.value = true;
  await new Promise((r) => setTimeout(r, 300));
  overview.value = mockOverviewByClass[cohortId] ?? null;
  overviewLoading.value = false;
};

watch(selectedCohortId, (id) => {
  if (id) reloadOverview(id);
});

// ── Edit dialog ─────────────────────────────────────────────────
const editDialog = ref({
  open: false,
  tab: 'result' as 'result' | 'annotation',
  student: null as ClassOverviewStudent | null,
  discipline: '',
  resultValue: null as number | null,
  annotationValue: '' as string,
  saving: false,
});

function openEditDialog(student: ClassOverviewStudent, disciplineKey: string): void {
  const dr = student.disciplines[disciplineKey];
  editDialog.value = {
    open: true,
    tab: 'result',
    student,
    discipline: disciplineKey,
    resultValue: dr?.result ?? null,
    annotationValue: dr?.annotation ?? '',
    saving: false,
  };
}

function hasResult(dialog: typeof editDialog.value): boolean {
  return dialog.student?.disciplines[dialog.discipline]?.result !== null &&
    dialog.student?.disciplines[dialog.discipline]?.result !== undefined;
}

async function saveEdit(): Promise<void> {
  const { student, discipline, tab, resultValue, annotationValue } = editDialog.value;
  if (!student || !selectedCohortId.value) return;

  editDialog.value.saving = true;
  // MOCK: simulate save delay
  await new Promise((r) => setTimeout(r, 400));
  if (tab === 'result' && resultValue !== null) {
    student.disciplines[discipline] = { ...student.disciplines[discipline], result: resultValue };
    showSuccess('Ergebnis wurde gespeichert.');
  } else if (tab === 'annotation') {
    student.disciplines[discipline] = { ...student.disciplines[discipline], annotation: annotationValue || null };
    showSuccess(annotationValue ? 'Annotation gesetzt.' : 'Annotation entfernt.');
  }
  editDialog.value.open = false;
  editDialog.value.saving = false;
  // try {
  //   if (tab === 'result' && resultValue !== null) {
  //     await setResult(selectedCohortId.value, student.userId, discipline, { result: resultValue, gender: student.gender }).send();
  //     showSuccess('Ergebnis wurde gespeichert.');
  //   } else if (tab === 'annotation') {
  //     await setAnnotation(selectedCohortId.value, student.userId, discipline, annotationValue ?? '').send();
  //     showSuccess(annotationValue ? 'Annotation gesetzt.' : 'Annotation entfernt.');
  //   }
  //   editDialog.value.open = false;
  //   await reloadOverview(selectedCohortId.value);
  // } catch (err) {
  //   showError(err, { fallback: 'Speichern fehlgeschlagen.' });
  // } finally {
  //   editDialog.value.saving = false;
  // }
}

async function confirmDelete(): Promise<void> {
  const { student, discipline } = editDialog.value;
  if (!student || !selectedCohortId.value) return;

  editDialog.value.saving = true;
  // MOCK: simulate delete delay
  await new Promise((r) => setTimeout(r, 400));
  student.disciplines[discipline] = { ...student.disciplines[discipline], result: null };
  showSuccess('Ergebnis wurde gelöscht.');
  editDialog.value.open = false;
  editDialog.value.saving = false;
  // try {
  //   await deleteResult(selectedCohortId.value, student.userId, discipline).send();
  //   showSuccess('Ergebnis wurde gelöscht.');
  //   editDialog.value.open = false;
  //   await reloadOverview(selectedCohortId.value);
  // } catch (err) {
  //   showError(err, { fallback: 'Löschen fehlgeschlagen.' });
  // } finally {
  //   editDialog.value.saving = false;
  // }
}

// ── Formatting ──────────────────────────────────────────────────
function disciplineLabel(key: string): string {
  return disciplines.find((d) => d.key === key)?.label ?? key;
}

function resultInputLabel(key: string): string {
  const d = disciplines.find((dd) => dd.key === key);
  return d ? `Wert (${d.unit})` : 'Wert';
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
      return `${result} Rd.`;
    default:
      return String(result);
  }
}

function ratingColor(rating: string): string {
  const map: Record<string, string> = { UED: 'green', UEE: 'blue', EE: 'orange', TE: 'red' };
  return map[rating] ?? 'grey';
}
</script>

<style lang="scss">
.sticky-col {
  position: sticky;
  left: 0;
  background: var(--v-theme-surface);
  z-index: 1;
}

.discipline-header {
  min-width: 110px;
}

.discipline-cell {
  white-space: nowrap;
}

.class-avg-row {
  background: rgba(99, 102, 241, 0.06) !important;
  border-top: 2px solid rgba(15, 23, 42, 0.1);
}
</style>
