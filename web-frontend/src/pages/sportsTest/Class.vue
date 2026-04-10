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
import { useRequest } from 'alova/client';
import {
  getCohorts,
  getClassOverview,
  setResult,
  deleteResult,
  setAnnotation,
} from '@/api/alovaMethods/fitnessCheck';
import type { ClassOverviewStudent } from '@/models/sportsTest/classOverview';
import { useNotifications } from '@/composables/useNotifications';

const { showSuccess, showError } = useNotifications();

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
const { data: cohorts, loading: cohortsLoading } = useRequest(getCohorts());

const cohortItems = computed(() =>
  (cohorts.value ?? []).map((c) => ({
    id: c.id,
    label: `${c.classNameVocationalEducation} (${c.firstSchoolYear}/${String(c.firstSchoolYear + 1).slice(-2)})`,
  }))
);

const selectedCohortId = ref<string | null>(null);

// ── Overview loading ────────────────────────────────────────────
const {
  data: overview,
  loading: overviewLoading,
  error: overviewError,
  send: reloadOverview,
} = useRequest((cohortId: string) => getClassOverview(cohortId), { immediate: false });

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
  try {
    if (tab === 'result' && resultValue !== null) {
      await setResult(selectedCohortId.value, student.userId, discipline, {
        result: resultValue,
        gender: student.gender,
      }).send();
      showSuccess('Ergebnis wurde gespeichert.');
    } else if (tab === 'annotation') {
      await setAnnotation(selectedCohortId.value, student.userId, discipline, annotationValue ?? '').send();
      showSuccess(annotationValue ? 'Annotation gesetzt.' : 'Annotation entfernt.');
    }
    editDialog.value.open = false;
    await reloadOverview(selectedCohortId.value);
  } catch (err) {
    showError(err, { fallback: 'Speichern fehlgeschlagen.' });
  } finally {
    editDialog.value.saving = false;
  }
}

async function confirmDelete(): Promise<void> {
  const { student, discipline } = editDialog.value;
  if (!student || !selectedCohortId.value) return;

  editDialog.value.saving = true;
  try {
    await deleteResult(selectedCohortId.value, student.userId, discipline).send();
    showSuccess('Ergebnis wurde gelöscht.');
    editDialog.value.open = false;
    await reloadOverview(selectedCohortId.value);
  } catch (err) {
    showError(err, { fallback: 'Löschen fehlgeschlagen.' });
  } finally {
    editDialog.value.saving = false;
  }
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
