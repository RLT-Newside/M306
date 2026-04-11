<template>
  <v-container>
    <!-- ── Header ─────────────────────────────────────────────── -->
    <v-row>
      <v-col cols="12">
        <h1 class="text-h3">Mein Fitnesstest</h1>
      </v-col>
    </v-row>

    <!-- ── Loading / error ───────────────────────────────────── -->
    <v-row v-if="overviewLoading">
      <v-col class="text-center py-10">
        <v-progress-circular indeterminate color="primary" />
      </v-col>
    </v-row>

    <v-row v-else-if="overviewError">
      <v-col>
        <v-alert type="error" variant="tonal">
          Deine Ergebnisse konnten nicht geladen werden.
        </v-alert>
      </v-col>
    </v-row>

    <template v-else-if="overview && overview.length > 0">
      <!-- ── Year tabs ───────────────────────────────────────── -->
      <v-row v-if="overview.length > 1">
        <v-col>
          <v-tabs v-model="selectedYear" color="primary">
            <v-tab
              v-for="y in sortedYears"
              :key="y.schoolYear"
              :value="y.schoolYear"
            >
              {{ y.schoolYear }}/{{ String(y.schoolYear + 1).slice(-2) }}
              <v-chip
                v-if="y.rating"
                size="x-small"
                :color="ratingColor(y.rating)"
                variant="tonal"
                class="ml-2"
              >
                {{ y.rating }}
              </v-chip>
            </v-tab>
          </v-tabs>
        </v-col>
      </v-row>

      <template v-if="currentYearData">
        <!-- ── Summary card ───────────────────────────────────── -->
        <v-row class="mt-2">
          <v-col cols="12" sm="6" md="4">
            <v-card variant="tonal" :color="currentYearData.rating ? ratingColor(currentYearData.rating) : 'primary'">
              <v-card-text>
                <div class="text-caption text-uppercase mb-1">
                  {{ currentYearData.cohort?.classNameVocationalEducation ?? '–' }}
                  <span v-if="currentYearData.cohort">
                    · {{ currentYearData.schoolYear }}/{{ String(currentYearData.schoolYear + 1).slice(-2) }}
                  </span>
                </div>
                <div class="d-flex align-center gap-4">
                  <div>
                    <div class="text-h4 font-weight-bold">
                      {{ currentYearData.totalPoints ?? '–' }}
                    </div>
                    <div class="text-caption">∑ Punkte</div>
                  </div>
                  <v-divider vertical class="mx-3" />
                  <div>
                    <div class="text-h4 font-weight-bold">
                      {{ currentYearData.averagePoints?.toFixed(1) ?? '–' }}
                    </div>
                    <div class="text-caption">⌀ Punkte</div>
                  </div>
                  <v-divider vertical class="mx-3" />
                  <div>
                    <v-chip
                      v-if="currentYearData.rating"
                      size="large"
                      :color="ratingColor(currentYearData.rating)"
                      variant="flat"
                    >
                      {{ currentYearData.rating }}
                    </v-chip>
                    <span v-else class="text-h5">–</span>
                    <div class="text-caption mt-1">Note</div>
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>

        <!-- ── Disciplines table ──────────────────────────────── -->
        <v-row class="mt-2">
          <v-col>
            <v-card>
              <v-card-text class="pa-0">
                <v-table>
                  <thead>
                    <tr>
                      <th class="text-left">Disziplin</th>
                      <th class="text-right">Bestes Ergebnis</th>
                      <th class="text-right">Punkte</th>
                      <th class="text-right">Datum</th>
                      <th class="text-center" style="width: 60px"></th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="d in disciplines" :key="d.key">
                      <td class="font-weight-medium">{{ d.label }}</td>
                      <td class="text-right">
                        <template v-if="getBestAttempt(d.key)">
                          <span class="font-weight-bold text-primary">
                            {{ formatResult(d.key, getBestAttempt(d.key)!.result) }}
                          </span>
                          <template v-if="d.key === 'OneLegStand' && getBestAttempt(d.key)!.leftFootResult !== null">
                            <div class="text-caption text-medium-emphasis">
                              L: {{ getBestAttempt(d.key)!.leftFootResult }} Sek. /
                              R: {{ getBestAttempt(d.key)!.rightFootResult }} Sek.
                            </div>
                          </template>
                        </template>
                        <span v-else class="text-caption text-medium-emphasis">–</span>
                      </td>
                      <td class="text-right">
                        <span v-if="getBestAttempt(d.key)" class="font-weight-medium">
                          {{ getBestAttempt(d.key)!.points }} Pkt.
                        </span>
                        <span v-else class="text-caption text-medium-emphasis">–</span>
                      </td>
                      <td class="text-right text-caption text-medium-emphasis">
                        {{ getBestAttempt(d.key) ? formatDate(getBestAttempt(d.key)!.momentUtc) : '–' }}
                      </td>
                      <td class="text-center">
                        <v-btn
                          v-if="isCurrentYear"
                          size="small"
                          icon
                          variant="text"
                          color="primary"
                          @click="openSubmitDialog(d.key)"
                        >
                          <v-icon>mdi-plus</v-icon>
                        </v-btn>
                      </td>
                    </tr>
                  </tbody>
                </v-table>
              </v-card-text>

              <!-- Empty state inside the card when current year has no attempts -->
              <v-card-text
                v-if="isCurrentYear && currentYearData?.bestAttempts.length === 0"
                class="text-center py-6"
              >
                <v-icon size="36" color="medium-emphasis">mdi-clipboard-text-outline</v-icon>
                <p class="text-medium-emphasis mt-2 mb-4">
                  Noch keine Versuche in diesem Schuljahr.
                </p>
                <v-btn color="primary" @click="openSubmitDialog(disciplines[0].key)">
                  Ersten Versuch einreichen
                </v-btn>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>

        <!-- ── Year comparison ────────────────────────────────── -->
        <v-row v-if="sortedYears.length > 1" class="mt-2">
          <v-col>
            <v-card>
              <v-card-title class="text-subtitle-1 font-weight-medium py-3 px-4">
                Jahresvergleich
              </v-card-title>
              <v-card-text class="pa-0">
                <div style="overflow-x: auto">
                  <v-table density="compact">
                    <thead>
                      <tr>
                        <th class="text-left">Disziplin</th>
                        <th
                          v-for="y in sortedYears"
                          :key="y.schoolYear"
                          class="text-right year-col"
                        >
                          {{ y.schoolYear }}/{{ String(y.schoolYear + 1).slice(-2) }}
                        </th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="d in disciplines" :key="d.key">
                        <td class="text-caption font-weight-medium">{{ d.label }}</td>
                        <td
                          v-for="y in sortedYears"
                          :key="y.schoolYear"
                          class="text-right year-col"
                        >
                          <template v-if="y.bestAttempts.find(a => a.discipline === d.key)">
                            <div class="text-caption font-weight-bold text-primary">
                              {{ formatResult(d.key, y.bestAttempts.find(a => a.discipline === d.key)!.result) }}
                            </div>
                            <div class="text-caption text-medium-emphasis">
                              {{ y.bestAttempts.find(a => a.discipline === d.key)!.points }} Pkt.
                            </div>
                          </template>
                          <span v-else class="text-caption text-medium-emphasis">–</span>
                        </td>
                      </tr>

                      <!-- Summary rows -->
                      <tr class="year-summary-row">
                        <td class="text-caption text-uppercase text-medium-emphasis">∑ Punkte</td>
                        <td
                          v-for="y in sortedYears"
                          :key="y.schoolYear"
                          class="text-right year-col text-caption font-weight-bold"
                        >
                          {{ y.totalPoints ?? '–' }}
                        </td>
                      </tr>
                      <tr class="year-summary-row">
                        <td class="text-caption text-uppercase text-medium-emphasis">Note</td>
                        <td
                          v-for="y in sortedYears"
                          :key="y.schoolYear"
                          class="text-right year-col"
                        >
                          <v-chip
                            v-if="y.rating"
                            size="x-small"
                            :color="ratingColor(y.rating)"
                            variant="tonal"
                          >
                            {{ y.rating }}
                          </v-chip>
                          <span v-else class="text-caption text-medium-emphasis">–</span>
                        </td>
                      </tr>
                    </tbody>
                  </v-table>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </template>
    </template>

    <!-- ── Submit dialog ───────────────────────────────────────── -->
    <v-dialog v-model="submitDialog.open" max-width="440">
      <v-card>
        <v-card-title>
          Versuch einreichen – {{ disciplineLabel(submitDialog.disciplineKey) }}
        </v-card-title>
        <v-card-text>
          <!-- Remaining attempts banner -->
          <v-progress-linear v-if="submitDialog.loadingInfo" indeterminate class="mb-3" />
          <template v-else-if="submitDialog.remainingAttempts !== null">
            <v-alert
              v-if="submitDialog.remainingAttempts === 0"
              type="warning"
              variant="tonal"
              density="compact"
              class="mb-3"
            >
              Keine Versuche mehr verfügbar ({{ submitDialog.maxAttempts }}/{{ submitDialog.maxAttempts }} verbraucht).
            </v-alert>
            <v-alert
              v-else
              type="info"
              variant="tonal"
              density="compact"
              class="mb-3"
            >
              {{ submitDialog.remainingAttempts }} von {{ submitDialog.maxAttempts }} Versuch{{ submitDialog.maxAttempts === 1 ? '' : 'en' }} verbleibend
            </v-alert>
          </template>

          <v-text-field
            v-model.number="submitDialog.value"
            :label="inputLabel(submitDialog.disciplineKey)"
            type="number"
            :step="inputStep(submitDialog.disciplineKey)"
            :min="inputMin(submitDialog.disciplineKey)"
            :max="inputMax(submitDialog.disciplineKey)"
            variant="outlined"
            density="comfortable"
            :disabled="submitDialog.remainingAttempts === 0"
            autofocus
          />

          <!-- Foot selector for OneLegStand -->
          <v-radio-group
            v-if="submitDialog.disciplineKey === 'OneLegStand'"
            v-model="submitDialog.foot"
            inline
            label="Fuss"
            class="mt-2"
            :disabled="submitDialog.remainingAttempts === 0"
          >
            <v-radio label="Links" value="Left" />
            <v-radio label="Rechts" value="Right" />
          </v-radio-group>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="submitDialog.open = false">Abbrechen</v-btn>
          <v-btn
            color="primary"
            :loading="submitDialog.saving"
            :disabled="!isSubmitValid || submitDialog.remainingAttempts === 0"
            @click="submitAttempt"
          >
            Einreichen
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
// import { useRequest } from 'alova/client';
// import {
//   getOverview,
//   getDisciplineInfo,
//   submitCoreStrength,
//   submitMedicineBallPush,
//   submitStandingLongJump,
//   submitShuttleRun,
//   submitTwelveMinutesRun,
//   submitOneLegStand,
// } from '@/api/alovaMethods/fitnessCheck';
import type { BestAttemptResult, YearOverview } from '@/models/sportsTest/myResults';
import { useNotifications } from '@/composables/useNotifications';
// import { getHttpErrorData } from '@/api/httpError';
// import { useNotificationStore } from '@/stores/notification';

const { showSuccess } = useNotifications();
// const notificationStore = useNotificationStore();

// ── Disciplines config ──────────────────────────────────────────
interface DisciplineConfig {
  key: string;
  label: string;
  inputLabel: string;
  step: number;
  min: number;
  max: number;
}

const disciplines: DisciplineConfig[] = [
  { key: 'CoreStrength',      label: 'Rumpfkraft',       inputLabel: 'Sekunden (0–360)',         step: 1,    min: 0,   max: 360  },
  { key: 'MedicineBallPush',  label: 'Medizinballstoss', inputLabel: 'Zentimeter (250–1000)',    step: 1,    min: 250, max: 1000 },
  { key: 'StandingLongJump',  label: 'Standweitsprung',  inputLabel: 'Zentimeter (50–350)',      step: 1,    min: 50,  max: 350  },
  { key: 'ShuttleRun',        label: 'Pendellauf',        inputLabel: 'Sekunden (7.00–15.00)',    step: 0.01, min: 7,   max: 15   },
  { key: 'TwelveMinutesRun',  label: '12-Minutenlauf',   inputLabel: 'Runden (16–55)',           step: 0.5,  min: 16,  max: 55   },
  { key: 'OneLegStand',       label: 'Einbeinstand',      inputLabel: 'Sekunden (0–120)',         step: 1,    min: 0,   max: 120  },
];

// ── Overview loading ────────────────────────────────────────────
// MOCK: const { data: overview, loading: overviewLoading, error: overviewError, send: reloadOverview } = useRequest(getOverview());
const overviewLoading = ref(false);
const overviewError = ref(null);
const overview = ref<YearOverview[]>([
  {
    schoolYear: 2024,
    cohort: {
      id: 'cohort-1',
      profession: 'Informatiker EFZ',
      baccalaureate: false,
      schoolYear: 2024,
      firstSchoolYear: 2022,
      classNameVocationalEducation: 'IM22a',
      classNameBaccalaureate: null,
    },
    bestAttempts: [
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-1',
        discipline: 'CoreStrength',
        result: 135,
        points: 8,
        momentUtc: '2025-03-10T09:00:00Z',
        leftFootResult: null,
        rightFootResult: null,
      },
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-1',
        discipline: 'MedicineBallPush',
        result: 720,
        points: 9,
        momentUtc: '2025-03-10T09:15:00Z',
        leftFootResult: null,
        rightFootResult: null,
      },
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-1',
        discipline: 'StandingLongJump',
        result: 255,
        points: 9,
        momentUtc: '2025-03-10T09:30:00Z',
        leftFootResult: null,
        rightFootResult: null,
      },
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-1',
        discipline: 'ShuttleRun',
        result: 9250,
        points: 7,
        momentUtc: '2025-03-10T09:45:00Z',
        leftFootResult: null,
        rightFootResult: null,
      },
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-1',
        discipline: 'OneLegStand',
        result: 72,
        points: 8,
        momentUtc: '2025-03-10T10:00:00Z',
        leftFootResult: 65,
        rightFootResult: 72,
      },
    ],
    totalPoints: 41,
    averagePoints: 8.2,
    rating: 'UEE',
  },
  {
    schoolYear: 2023,
    cohort: {
      id: 'cohort-0',
      profession: 'Informatiker EFZ',
      baccalaureate: false,
      schoolYear: 2023,
      firstSchoolYear: 2022,
      classNameVocationalEducation: 'IM22a',
      classNameBaccalaureate: null,
    },
    bestAttempts: [
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-0',
        discipline: 'CoreStrength',
        result: 105,
        points: 6,
        momentUtc: '2024-03-14T09:00:00Z',
        leftFootResult: null,
        rightFootResult: null,
      },
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-0',
        discipline: 'MedicineBallPush',
        result: 680,
        points: 7,
        momentUtc: '2024-03-14T09:15:00Z',
        leftFootResult: null,
        rightFootResult: null,
      },
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-0',
        discipline: 'StandingLongJump',
        result: 235,
        points: 7,
        momentUtc: '2024-03-14T09:30:00Z',
        leftFootResult: null,
        rightFootResult: null,
      },
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-0',
        discipline: 'ShuttleRun',
        result: 10100,
        points: 6,
        momentUtc: '2024-03-14T09:45:00Z',
        leftFootResult: null,
        rightFootResult: null,
      },
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-0',
        discipline: 'TwelveMinutesRun',
        result: 29,
        points: 6,
        momentUtc: '2024-03-14T10:00:00Z',
        leftFootResult: null,
        rightFootResult: null,
      },
      {
        userId: 'mock-user-1',
        cohortId: 'cohort-0',
        discipline: 'OneLegStand',
        result: 58,
        points: 6,
        momentUtc: '2024-03-14T10:15:00Z',
        leftFootResult: 50,
        rightFootResult: 58,
      },
    ],
    totalPoints: 38,
    averagePoints: 6.3,
    rating: 'EE',
  },
]);
const reloadOverview = async () => { /* MOCK: no-op */ };

const selectedYear = ref<number | null>(null);

const sortedYears = computed(() =>
  (overview.value ?? []).slice().sort((a, b) => b.schoolYear - a.schoolYear)
);

const currentYearData = computed(() => {
  if (!overview.value || overview.value.length === 0) return null;
  const year = selectedYear.value ?? sortedYears.value[0]?.schoolYear;
  return overview.value.find((y) => y.schoolYear === year) ?? sortedYears.value[0] ?? null;
});

const isCurrentYear = computed(() => {
  if (!currentYearData.value) return false;
  const now = new Date();
  const currentSchoolYear = now.getMonth() >= 7 ? now.getFullYear() : now.getFullYear() - 1;
  return currentYearData.value.schoolYear === currentSchoolYear;
});

function getBestAttempt(disciplineKey: string): BestAttemptResult | undefined {
  return currentYearData.value?.bestAttempts.find((a) => a.discipline === disciplineKey);
}

// ── Submit dialog ───────────────────────────────────────────────
const submitDialog = ref({
  open: false,
  disciplineKey: '',
  value: null as number | null,
  foot: 'Left' as 'Left' | 'Right',
  saving: false,
  loadingInfo: false,
  remainingAttempts: null as number | null,
  maxAttempts: null as number | null,
});

async function openSubmitDialog(disciplineKey: string): Promise<void> {
  submitDialog.value = {
    open: true,
    disciplineKey,
    value: null,
    foot: 'Left',
    saving: false,
    loadingInfo: true,
    remainingAttempts: null,
    maxAttempts: null,
  };
  // MOCK: simulate network delay + return fake attempt info
  await new Promise((r) => setTimeout(r, 400));
  submitDialog.value.remainingAttempts = 3;
  submitDialog.value.maxAttempts = 5;
  submitDialog.value.loadingInfo = false;
  // try {
  //   const info = await getDisciplineInfo(disciplineKey).send();
  //   submitDialog.value.remainingAttempts = info.remainingAttempts;
  //   submitDialog.value.maxAttempts = info.maxAllowedAttempts;
  // } catch {
  //   submitDialog.value.remainingAttempts = null;
  // } finally {
  //   submitDialog.value.loadingInfo = false;
  // }
}

const isSubmitValid = computed(() => {
  const { disciplineKey, value, foot, loadingInfo } = submitDialog.value;
  if (loadingInfo) return false;
  if (value === null || isNaN(value)) return false;
  if (disciplineKey === 'OneLegStand' && !foot) return false;
  const d = disciplines.find((dd) => dd.key === disciplineKey);
  if (!d) return false;
  return value >= d.min && value <= d.max;
});

async function submitAttempt(): Promise<void> {
  const { value } = submitDialog.value;
  if (value === null) return;

  submitDialog.value.saving = true;
  // MOCK: simulate submission delay
  await new Promise((r) => setTimeout(r, 600));
  showSuccess('Versuch wurde gespeichert.');
  submitDialog.value.open = false;
  submitDialog.value.saving = false;

  // try {
  //   switch (disciplineKey) {
  //     case 'CoreStrength':      await submitCoreStrength(value).send(); break;
  //     case 'MedicineBallPush':  await submitMedicineBallPush(value).send(); break;
  //     case 'StandingLongJump':  await submitStandingLongJump(value).send(); break;
  //     case 'ShuttleRun':        await submitShuttleRun(Math.round(value * 1000)).send(); break;
  //     case 'TwelveMinutesRun':  await submitTwelveMinutesRun(value).send(); break;
  //     case 'OneLegStand':       await submitOneLegStand(value, foot).send(); break;
  //   }
  //   showSuccess('Versuch wurde gespeichert.');
  //   submitDialog.value.open = false;
  //   await reloadOverview();
  // } catch (err) {
  //   const status = (err as { status?: number })?.status;
  //   if (status === 400) {
  //     const msg = getHttpErrorData<string>(err);
  //     notificationStore.showWarning(msg ?? 'Ungültige Eingabe.');
  //   } else {
  //     showError(err, { ... });
  //   }
  // } finally {
  //   submitDialog.value.saving = false;
  // }
}

// ── Helpers ─────────────────────────────────────────────────────
function disciplineLabel(key: string): string {
  return disciplines.find((d) => d.key === key)?.label ?? key;
}

function inputLabel(key: string): string {
  return disciplines.find((d) => d.key === key)?.inputLabel ?? 'Wert';
}

function inputStep(key: string): number {
  return disciplines.find((d) => d.key === key)?.step ?? 1;
}

function inputMin(key: string): number {
  return disciplines.find((d) => d.key === key)?.min ?? 0;
}

function inputMax(key: string): number {
  return disciplines.find((d) => d.key === key)?.max ?? 9999;
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

function formatDate(isoDate: string): string {
  return new Date(isoDate).toLocaleDateString('de-CH', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });
}

function ratingColor(rating: string): string {
  const map: Record<string, string> = { UED: 'green', UEE: 'blue', EE: 'orange', TE: 'red' };
  return map[rating] ?? 'grey';
}
</script>

<style scoped>
.year-col {
  min-width: 90px;
}

.year-summary-row {
  background: rgba(99, 102, 241, 0.05);
  border-top: 1px solid rgba(0, 0, 0, 0.08);
}
</style>
