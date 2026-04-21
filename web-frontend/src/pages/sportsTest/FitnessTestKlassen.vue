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
      <h1 class="ft-page-title">Klassen-Verwaltung</h1>
      <p class="ft-page-sub">Alle Einträge der Klasse verwalten und bearbeiten</p>

      <!-- Controls -->
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

      <!-- Class table -->
      <div class="ft-card">
        <table class="ft-table">
          <thead>
            <tr>
              <th scope="col">Name</th>
              <th scope="col">Geschlecht</th>
              <th scope="col">Disziplin</th>
              <th scope="col">Wert</th>
              <th scope="col">Punkte</th>
              <th scope="col">Note</th>
              <th scope="col">Datum</th>
              <th scope="col">Schuljahr</th>
              <th class="ft-th-actions" scope="col">Aktionen</th>
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
    </div>

    <!-- Edit dialog -->
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

    <!-- Delete dialog -->
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

    <!-- New entry dialog -->
    <v-dialog v-model="newDialog.open" max-width="480">
      <v-card rounded="lg">
        <v-card-title class="pt-5 px-6">Neuer Eintrag</v-card-title>
        <v-card-text class="px-6">
          <v-text-field v-model="newDialog.studentName" label="Name" variant="outlined" density="comfortable" class="mb-3" />
          <v-select
            v-model="newDialog.gender"
            :items="[{ title: 'Männlich', value: 'male' }, { title: 'Weiblich', value: 'female' }]"
            label="Geschlecht" variant="outlined" density="comfortable" class="mb-3"
          />
          <v-select v-model="newDialog.discipline" :items="ALL_DISCIPLINES" label="Disziplin" variant="outlined" density="comfortable" class="mb-3" />
          <v-text-field
            v-model.number="newDialog.value"
            :label="newDialog.discipline ? `Wert (${unitForDiscipline(newDialog.discipline)})` : 'Wert'"
            type="number" variant="outlined" density="comfortable" class="mb-3"
          />
          <v-text-field v-model="newDialog.date" label="Datum" type="date" variant="outlined" density="comfortable" class="mb-3" />
          <v-text-field v-model.number="newDialog.points" label="Punkte (0–100)" type="number" variant="outlined" density="comfortable" />
        </v-card-text>
        <v-card-actions class="px-6 pb-5">
          <v-spacer />
          <v-btn variant="text" @click="newDialog.open = false">Abbrechen</v-btn>
          <v-btn variant="flat" color="primary" :disabled="!newDialogValid" @click="saveNewEntry">Speichern</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import type { FitnessAttempt, Discipline } from '@/models/sportsTest/fitnessAttempt';
import { mockAttempts } from '@/mocks/fitnessAttempts';
import {
  ALL_DISCIPLINES,
  computeNote,
  formatDate,
  unitForDiscipline,
  schoolYearFromDate,
} from '@/composables/useFitnessUtils';
import '@/assets/fitnessTest.css';

const attempts = ref<FitnessAttempt[]>([...mockAttempts]);

const allClasses = computed(() => [...new Set(attempts.value.map((a) => a.classOrProfession))].sort());
const selectedClass = ref(allClasses.value[0] ?? '');

const classEntries = computed(() =>
  attempts.value
    .filter((a) => a.classOrProfession === selectedClass.value)
    .sort((a, b) => a.discipline.localeCompare(b.discipline) || b.value - a.value)
);

// Edit
const editDialog = ref({ open: false, entry: null as FitnessAttempt | null, value: null as number | null });

function openEditDialog(entry: FitnessAttempt) {
  editDialog.value = { open: true, entry, value: entry.value };
}

function saveEdit() {
  if (!editDialog.value.entry || editDialog.value.value === null) return;
  const idx = attempts.value.findIndex((a) => a.id === editDialog.value.entry!.id);
  if (idx !== -1) attempts.value[idx] = { ...attempts.value[idx], value: editDialog.value.value };
  editDialog.value.open = false;
}

// Delete
const deleteDialog = ref({ open: false, entry: null as FitnessAttempt | null });

function openDeleteDialog(entry: FitnessAttempt) {
  deleteDialog.value = { open: true, entry };
}

function confirmDelete() {
  if (!deleteDialog.value.entry) return;
  attempts.value = attempts.value.filter((a) => a.id !== deleteDialog.value.entry!.id);
  deleteDialog.value.open = false;
}

// New entry
const newDialog = ref({
  open: false,
  studentName: '',
  gender: 'male' as 'male' | 'female',
  discipline: ALL_DISCIPLINES[0],
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
    discipline: ALL_DISCIPLINES[0],
    value: null,
    date: new Date().toISOString().slice(0, 10),
    points: null,
  };
}

function saveNewEntry() {
  if (!newDialogValid.value) return;
  const d = newDialog.value;
  attempts.value.push({
    id: Date.now().toString(),
    studentName: d.studentName,
    gender: d.gender,
    discipline: d.discipline as Discipline,
    value: d.value!,
    unit: unitForDiscipline(d.discipline as Discipline),
    date: d.date,
    schoolYear: schoolYearFromDate(d.date),
    classOrProfession: selectedClass.value,
    points: d.points!,
  });
  newDialog.value.open = false;
}
</script>

<style scoped>
/* ── Controls ──────────────────────────────────────────────────── */
.ft-controls {
  display: flex;
  align-items: flex-end;
  gap: 16px;
  margin-bottom: 20px;
  flex-wrap: wrap;
}

.ft-select--wide {
  min-width: 260px;
  max-width: 380px;
}

.ft-btn-primary {
  padding: 8px 18px;
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

.ft-btn-primary:hover { background: #1D4ED8; }

/* ── Table actions ─────────────────────────────────────────────── */
.ft-th-actions { width: 90px; }
.ft-td-actions { white-space: nowrap; }

/* ── Gender chip ───────────────────────────────────────────────── */
.ft-gender-chip {
  display: inline-block;
  padding: 3px 10px;
  border-radius: 20px;
  font-size: 0.78rem;
  font-weight: 500;
}

.ft-gender-chip--male   { background: #DBEAFE; color: #1D4ED8; }
.ft-gender-chip--female { background: #FCE7F3; color: #BE185D; }

/* ── Icon buttons ──────────────────────────────────────────────── */
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

.ft-icon-btn + .ft-icon-btn { margin-left: 4px; }
.ft-icon-btn--edit          { color: #2563EB; }
.ft-icon-btn--edit:hover    { background: #EFF6FF; }
.ft-icon-btn--delete        { color: #DC2626; }
.ft-icon-btn--delete:hover  { background: #FEF2F2; }
</style>
