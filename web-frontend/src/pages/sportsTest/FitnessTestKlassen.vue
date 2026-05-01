<template>
  <div class="ft-page">
    <div class="ft-tabs">
      <router-link class="ft-tab" to="/fitnesstest">
        <v-icon size="16">mdi-trophy</v-icon>
        Globale Bestenliste
      </router-link>
      <router-link class="ft-tab ft-tab--active" to="/fitnesstest/klassen">
        <v-icon size="16">mdi-account-group</v-icon>
        Klassen-Verwaltung
      </router-link>
    </div>

    <div class="ft-content">
      <h1 class="ft-page-title">Klassen-Verwaltung</h1>
      <p class="ft-page-sub">Ergebnisse der Klasse verwalten und bearbeiten</p>

      <div class="ft-controls">
        <div>
          <div class="ft-label">Klasse auswählen</div>
          <select v-model="selectedClass" class="ft-select ft-select--wide">
            <option v-for="c in allClasses" :key="c" :value="c">{{ c }}</option>
          </select>
        </div>
        <div>
          <div class="ft-label">Schuljahr</div>
          <select v-model="selectedYear" class="ft-select">
            <option v-for="y in allYears" :key="y" :value="y">{{ y }}</option>
          </select>
        </div>
      </div>

      <!-- Class overview table: one row per student, columns per discipline -->
      <div class="ft-card">
        <div class="ft-table-scroll">
          <table class="ft-table">
            <thead>
              <tr>
                <th class="ft-th-name">Name</th>
                <th class="ft-th-gender">G</th>
                <th v-for="d in DISCIPLINES" :key="d" class="ft-th-disc">
                  <div class="ft-th-disc-label">{{ shortDiscipline(d) }}</div>
                  <div class="ft-th-disc-sub">Wert / Pkt</div>
                </th>
                <th class="ft-th-total">Total</th>
                <th class="ft-th-avg">⌀ Pkt</th>
                <th class="ft-th-note">Note</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="student in studentRows" :key="student.name">
                <td class="ft-td-name">{{ student.name }}</td>
                <td>
                  <span :class="['ft-gender-chip', student.gender === 'male' ? 'ft-gender-chip--male' : 'ft-gender-chip--female']">
                    {{ student.gender === 'male' ? 'M' : 'W' }}
                  </span>
                </td>
                <td v-for="d in DISCIPLINES" :key="d" class="ft-td-disc"
                    @click="openEditDialog(student, d)">
                  <template v-if="student.results[d]">
                    <template v-if="student.results[d].annotation">
                      <span class="ft-annotation">{{ student.results[d].annotation }}</span>
                    </template>
                    <template v-else>
                      <span class="ft-val">{{ formatCellValue(student.results[d]) }}</span>
                      <span :class="['ft-pts', gradeClass(student.results[d].points)]">{{ student.results[d].points }}</span>
                    </template>
                  </template>
                  <template v-else>
                    <span class="ft-missing">—</span>
                  </template>
                </td>
                <td class="ft-td-total">{{ student.totalPoints }}</td>
                <td class="ft-td-avg">{{ student.avgPoints }}</td>
                <td>
                  <span class="ft-note-badge">{{ student.note }}</span>
                </td>
              </tr>
              <tr v-if="studentRows.length === 0">
                <td :colspan="DISCIPLINES.length + 4" class="ft-empty-cell">
                  Keine Einträge für diese Klasse / Schuljahr.
                </td>
              </tr>
            </tbody>
            <tfoot v-if="studentRows.length > 0">
              <tr class="ft-avg-row">
                <td class="ft-td-name">Klassenschnitt</td>
                <td></td>
                <td v-for="d in DISCIPLINES" :key="d" class="ft-td-disc">
                  <span class="ft-val">{{ classAverages[d]?.avgValue ?? '—' }}</span>
                  <span class="ft-pts ft-pts--muted">{{ classAverages[d]?.avgPoints ?? '—' }}</span>
                </td>
                <td class="ft-td-total">{{ classTotalAvg }}</td>
                <td class="ft-td-avg">{{ classPointAvg }}</td>
                <td></td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    </div>

    <!-- Edit dialog -->
    <v-dialog v-model="editDialog.open" max-width="440">
      <v-card rounded="lg">
        <v-card-title class="pt-5 px-6">Ergebnis bearbeiten</v-card-title>
        <v-card-text class="px-6">
          <div class="mb-1 text-body-2 text-medium-emphasis">
            {{ editDialog.studentName }} — {{ editDialog.discipline }}
          </div>
          <v-text-field
            v-model.number="editDialog.value"
            :label="`Wert (${editDialog.unit})`"
            type="number"
            variant="outlined"
            density="comfortable"
            class="mt-3"
          />
          <div v-if="editDialog.value !== null && editDialog.value > 0" class="mt-1 text-body-2">
            Punkte: <strong>{{ editDialog.computedPoints }}</strong> · Bewertung: <strong>{{ editDialog.computedGrade }}</strong>
          </div>
          <v-text-field
            v-model="editDialog.annotation"
            label="Annotation (optional, z.B. verletzt, dispensiert)"
            variant="outlined"
            density="comfortable"
            class="mt-3"
          />
        </v-card-text>
        <v-card-actions class="px-6 pb-5">
          <v-btn v-if="editDialog.existingId" variant="text" color="error" @click="deleteEntry">Löschen</v-btn>
          <v-spacer />
          <v-btn variant="text" @click="editDialog.open = false">Abbrechen</v-btn>
          <v-btn variant="flat" color="primary" @click="saveEdit">Speichern</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import type { FitnessAttempt } from '@/models/sportsTest/fitnessAttempt';
import { mockAttempts } from '@/mocks/fitnessAttempts';
import {
  calculatePoints,
  getGrade,
  computeSwissNote,
  DISCIPLINES,
  unitForDiscipline,
  getDisciplineConfig,
} from '@/utils/fitnessCalculation';

const attempts = ref<FitnessAttempt[]>([...mockAttempts]);

const allClasses = computed(() => [...new Set(attempts.value.map((a) => a.classOrProfession))].sort());
const allYears = computed(() => [...new Set(attempts.value.map((a) => a.schoolYear))].sort().reverse());

const selectedClass = ref(allClasses.value[0] ?? '');
const selectedYear = ref(allYears.value[0] ?? '');

interface StudentResult {
  attempt: FitnessAttempt;
  points: number;
  grade: string;
  annotation?: string;
}

interface StudentRow {
  name: string;
  gender: 'male' | 'female';
  results: Record<string, StudentResult>;
  totalPoints: number;
  avgPoints: string;
  note: string;
}

const studentRows = computed<StudentRow[]>(() => {
  const filtered = attempts.value.filter(
    (a) => a.classOrProfession === selectedClass.value && a.schoolYear === selectedYear.value
  );

  const grouped = new Map<string, { gender: 'male' | 'female'; results: Record<string, StudentResult> }>();
  for (const a of filtered) {
    if (!grouped.has(a.studentName)) {
      grouped.set(a.studentName, { gender: a.gender, results: {} });
    }
    const points = a.annotation ? 0 : calculatePoints(a.discipline, a.gender, a.value);
    grouped.get(a.studentName)!.results[a.discipline] = {
      attempt: a,
      points,
      grade: getGrade(points),
      annotation: a.annotation,
    };
  }

  return [...grouped.entries()]
    .map(([name, data]) => {
      const validResults = Object.values(data.results).filter((r) => !r.annotation);
      const total = validResults.reduce((s, r) => s + r.points, 0);
      const count = validResults.length || 1;
      const avg = total / count;
      return {
        name,
        gender: data.gender,
        results: data.results,
        totalPoints: total,
        avgPoints: avg.toFixed(1),
        note: computeSwissNote(avg),
      };
    })
    .sort((a, b) => a.name.localeCompare(b.name));
});

const classAverages = computed(() => {
  const avgs: Record<string, { avgValue: string; avgPoints: string }> = {};
  for (const d of DISCIPLINES) {
    const values: number[] = [];
    const points: number[] = [];
    for (const s of studentRows.value) {
      const r = s.results[d];
      if (r && !r.annotation) {
        values.push(r.attempt.value);
        points.push(r.points);
      }
    }
    if (values.length > 0) {
      const config = getDisciplineConfig(d);
      let avgVal = values.reduce((a, b) => a + b, 0) / values.length;
      let formatted: string;
      if (config?.key === 'MedicineBallPush') {
        formatted = (avgVal / 100).toFixed(2);
      } else if (config?.key === 'ShuttleRun') {
        formatted = (avgVal / 1000).toFixed(2);
      } else {
        formatted = avgVal.toFixed(1);
      }
      avgs[d] = {
        avgValue: formatted,
        avgPoints: (points.reduce((a, b) => a + b, 0) / points.length).toFixed(1),
      };
    }
  }
  return avgs;
});

const classTotalAvg = computed(() => {
  if (studentRows.value.length === 0) return '—';
  const total = studentRows.value.reduce((s, r) => s + r.totalPoints, 0);
  return (total / studentRows.value.length).toFixed(1);
});

const classPointAvg = computed(() => {
  if (studentRows.value.length === 0) return '—';
  const total = studentRows.value.reduce((s, r) => s + parseFloat(r.avgPoints), 0);
  return (total / studentRows.value.length).toFixed(1);
});

function shortDiscipline(name: string): string {
  const map: Record<string, string> = {
    '12-Minutenlauf': '12-Min',
    'Standweitsprung': 'Weitsp.',
    'Rumpfkraft': 'Rumpfk.',
    'Einbeinstand': 'Einbein.',
    'Shuttle-Run': 'Shuttle',
    'Medizinballstoss': 'Med.ball',
  };
  return map[name] ?? name;
}

function formatCellValue(result: StudentResult): string {
  const a = result.attempt;
  const config = getDisciplineConfig(a.discipline);
  if (config?.key === 'MedicineBallPush') return (a.value / 100).toFixed(1);
  if (config?.key === 'ShuttleRun') return (a.value / 1000).toFixed(1);
  return String(a.value);
}

function gradeClass(points: number): string {
  const g = getGrade(points).toLowerCase();
  return `ft-pts--${g}`;
}

// Edit dialog
const editDialog = ref({
  open: false,
  studentName: '',
  discipline: '',
  gender: 'male' as 'male' | 'female',
  unit: '',
  value: null as number | null,
  annotation: '',
  existingId: null as string | null,
  computedPoints: 0,
  computedGrade: '',
});

watch(
  () => editDialog.value.value,
  (val) => {
    if (val !== null && val > 0 && !editDialog.value.annotation) {
      editDialog.value.computedPoints = calculatePoints(
        editDialog.value.discipline,
        editDialog.value.gender,
        val
      );
      editDialog.value.computedGrade = getGrade(editDialog.value.computedPoints);
    } else {
      editDialog.value.computedPoints = 0;
      editDialog.value.computedGrade = '';
    }
  }
);

function openEditDialog(student: StudentRow, discipline: string) {
  const existing = student.results[discipline];
  editDialog.value = {
    open: true,
    studentName: student.name,
    discipline,
    gender: student.gender,
    unit: unitForDiscipline(discipline),
    value: existing ? existing.attempt.value : null,
    annotation: existing?.annotation ?? '',
    existingId: existing?.attempt.id ?? null,
    computedPoints: existing ? existing.points : 0,
    computedGrade: existing ? existing.grade : '',
  };
}

function saveEdit() {
  const d = editDialog.value;
  if (d.existingId) {
    const idx = attempts.value.findIndex((a) => a.id === d.existingId);
    if (idx !== -1) {
      attempts.value[idx] = {
        ...attempts.value[idx],
        value: d.value ?? 0,
        annotation: d.annotation || undefined,
      };
    }
  } else if (d.value !== null) {
    const year = selectedYear.value;
    attempts.value.push({
      id: Date.now().toString(),
      studentName: d.studentName,
      gender: d.gender,
      discipline: d.discipline,
      value: d.value,
      unit: d.unit,
      date: new Date().toISOString().slice(0, 10),
      schoolYear: year,
      classOrProfession: selectedClass.value,
      annotation: d.annotation || undefined,
    });
  }
  editDialog.value.open = false;
}

function deleteEntry() {
  if (editDialog.value.existingId) {
    attempts.value = attempts.value.filter((a) => a.id !== editDialog.value.existingId);
  }
  editDialog.value.open = false;
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

.ft-controls {
  display: flex;
  align-items: flex-end;
  gap: 16px;
  margin-bottom: 20px;
  flex-wrap: wrap;
}

.ft-label {
  font-size: 0.78rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 5px;
}

.ft-select {
  padding: 8px 12px;
  border: 1px solid #D1D5DB;
  border-radius: 6px;
  font-size: 0.875rem;
  color: #1E293B;
  background: #fff;
  cursor: pointer;
  min-width: 160px;
}

.ft-select--wide {
  min-width: 260px;
}

.ft-card {
  background: #fff;
  border: 1px solid #E2E8F0;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 1px 4px rgba(15, 23, 42, 0.06);
}

.ft-table-scroll {
  overflow-x: auto;
}

.ft-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.8rem;
}

.ft-table thead tr { border-bottom: 1px solid #E2E8F0; }

.ft-table th {
  padding: 10px 8px;
  text-align: center;
  font-size: 0.72rem;
  font-weight: 600;
  color: #374151;
  white-space: nowrap;
}

.ft-table td {
  padding: 10px 8px;
  color: #1E293B;
  border-bottom: 1px solid #F1F5F9;
  text-align: center;
}

.ft-table tbody tr:last-child td { border-bottom: none; }
.ft-table tbody tr:hover td { background: #FAFBFC; }

.ft-th-name { text-align: left; min-width: 140px; }
.ft-th-gender { width: 36px; }
.ft-th-disc { min-width: 80px; }
.ft-th-disc-label { font-size: 0.72rem; }
.ft-th-disc-sub { font-size: 0.62rem; color: #94A3B8; font-weight: 400; }
.ft-th-total { width: 55px; }
.ft-th-avg { width: 55px; }
.ft-th-note { width: 50px; }

.ft-td-name { text-align: left; font-weight: 500; color: #0F172A; }
.ft-td-total { font-weight: 700; color: #0F172A; }
.ft-td-avg { font-weight: 600; color: #475569; }

.ft-td-disc {
  cursor: pointer;
  transition: background 0.15s;
}

.ft-td-disc:hover {
  background: #EFF6FF !important;
}

.ft-val {
  display: block;
  font-size: 0.78rem;
  color: #1E293B;
}

.ft-pts {
  display: inline-block;
  font-size: 0.68rem;
  font-weight: 700;
  padding: 1px 5px;
  border-radius: 8px;
  margin-top: 2px;
}

.ft-pts--te  { background: #FEE2E2; color: #991B1B; }
.ft-pts--ee  { background: #DBEAFE; color: #1E40AF; }
.ft-pts--uee { background: #D1FAE5; color: #065F46; }
.ft-pts--ued { background: #FCD34D; color: #78350F; }
.ft-pts--muted { background: #F1F5F9; color: #64748B; }

.ft-annotation {
  font-size: 0.7rem;
  color: #DC2626;
  font-style: italic;
}

.ft-missing {
  color: #CBD5E1;
}

.ft-note-badge {
  display: inline-flex;
  align-items: center;
  padding: 3px 9px;
  background: #DBEAFE;
  color: #1E40AF;
  border-radius: 20px;
  font-size: 0.78rem;
  font-weight: 600;
  white-space: nowrap;
}

.ft-gender-chip {
  display: inline-block;
  padding: 2px 6px;
  border-radius: 10px;
  font-size: 0.7rem;
  font-weight: 600;
}

.ft-gender-chip--male   { background: #DBEAFE; color: #1D4ED8; }
.ft-gender-chip--female { background: #FCE7F3; color: #BE185D; }

.ft-empty-cell {
  text-align: center;
  color: #94A3B8;
  padding: 40px 16px !important;
  font-size: 0.9rem;
}

.ft-avg-row td {
  background: #F8FAFC;
  border-top: 2px solid #E2E8F0;
  font-weight: 600;
  color: #475569;
}
</style>
