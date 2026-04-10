<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { api } from '../services/api'
import type { ClassOverviewResponse, StudentResult, Discipline, DisciplineData } from '../types'
import EditResultDialog from '../components/EditResultDialog.vue'
import AnnotationDialog from '../components/AnnotationDialog.vue'

const route = useRoute()
const classId = computed(() => Number(route.params.id))

const data = ref<ClassOverviewResponse | null>(null)
const loading = ref(true)
const error = ref('')

// Dialog state
const editDialog = ref<{
  visible: boolean
  student: StudentResult | null
  discipline: Discipline | null
  disciplineData: DisciplineData | null
  schoolYear: string
}>({ visible: false, student: null, discipline: null, disciplineData: null, schoolYear: '' })

const annotationDialog = ref<{
  visible: boolean
  student: StudentResult | null
  discipline: Discipline | null
  disciplineData: DisciplineData | null
  schoolYear: string
}>({ visible: false, student: null, discipline: null, disciplineData: null, schoolYear: '' })

async function loadData() {
  loading.value = true
  error.value = ''
  try {
    data.value = await api.getClassOverview(classId.value)
  } catch (e: any) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

onMounted(loadData)

function formatValue(value: number, discipline: Discipline): string {
  if (discipline.unit === 'Sekunden') {
    return value.toFixed(1) + 's'
  }
  if (Number.isInteger(value)) return String(value)
  return value.toFixed(1)
}

function getCellClass(discData: DisciplineData): string {
  if (discData.missing) return 'cell-missing'
  if (discData.annotation && !discData.bestResult) return 'cell-annotated'
  return ''
}

function getPointsClass(points: number | null): string {
  if (points === null) return ''
  if (points >= 5) return 'points-excellent'
  if (points >= 4) return 'points-good'
  if (points >= 3) return 'points-average'
  return 'points-poor'
}

function getGradeClass(grade: number | null): string {
  if (grade === null) return ''
  if (grade >= 5.0) return 'grade-excellent'
  if (grade >= 4.0) return 'grade-good'
  return 'grade-poor'
}

function openEditDialog(student: StudentResult, discipline: Discipline) {
  if (!data.value) return
  editDialog.value = {
    visible: true,
    student,
    discipline,
    disciplineData: student.disciplines[discipline.id],
    schoolYear: data.value.classInfo.school_year,
  }
}

function openAnnotationDialog(student: StudentResult, discipline: Discipline) {
  if (!data.value) return
  annotationDialog.value = {
    visible: true,
    student,
    discipline,
    disciplineData: student.disciplines[discipline.id],
    schoolYear: data.value.classInfo.school_year,
  }
}

async function handleDialogClose() {
  editDialog.value.visible = false
  annotationDialog.value.visible = false
  await loadData()
}
</script>

<template>
  <div>
    <div v-if="loading" class="loading">Daten werden geladen...</div>
    <div v-else-if="error" class="error">{{ error }}</div>

    <template v-else-if="data">
      <div class="page-header">
        <div>
          <router-link to="/" class="back-link">&larr; Zurück</router-link>
          <h2>{{ data.classInfo.name }} – {{ data.classInfo.school_year }}</h2>
          <p class="class-profession">{{ data.classInfo.profession }}</p>
        </div>
      </div>

      <div class="table-wrapper">
        <table class="results-table">
          <thead>
            <tr>
              <th class="sticky-col col-name">Lernende/r</th>
              <th class="col-gender">G</th>
              <template v-for="disc in data.disciplines" :key="disc.id">
                <th class="col-value">{{ disc.name }}<br><span class="unit">({{ disc.unit }})</span></th>
                <th class="col-points">Pkt</th>
              </template>
              <th class="col-total">Total</th>
              <th class="col-avg">&empty; Pkt</th>
              <th class="col-grade">Note</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="student in data.students" :key="student.id">
              <td class="sticky-col col-name">
                <strong>{{ student.last_name }}</strong>, {{ student.first_name }}
              </td>
              <td class="col-gender">
                <span :class="['badge', student.gender === 'm' ? 'badge-male' : 'badge-female']">
                  {{ student.gender === 'm' ? 'M' : 'W' }}
                </span>
              </td>
              <template v-for="disc in data.disciplines" :key="disc.id">
                <td
                  :class="['col-value', getCellClass(student.disciplines[disc.id])]"
                  @click="openEditDialog(student, disc)"
                  class="clickable"
                >
                  <template v-if="student.disciplines[disc.id].bestResult">
                    {{ formatValue(student.disciplines[disc.id].bestResult!.value, disc) }}
                    <span class="attempt-count" v-if="student.disciplines[disc.id].attempts.length > 1">
                      ({{ student.disciplines[disc.id].attempts.length }})
                    </span>
                  </template>
                  <template v-else-if="student.disciplines[disc.id].annotation">
                    <span
                      class="badge badge-annotated annotation-badge"
                      @click.stop="openAnnotationDialog(student, disc)"
                    >
                      {{ student.disciplines[disc.id].annotation!.note }}
                    </span>
                  </template>
                  <template v-else>
                    <span class="missing-indicator" @click.stop="openAnnotationDialog(student, disc)">
                      —
                    </span>
                  </template>
                </td>
                <td :class="['col-points', getPointsClass(student.disciplines[disc.id].points)]">
                  {{ student.disciplines[disc.id].points ?? '–' }}
                </td>
              </template>
              <td class="col-total"><strong>{{ student.totalPoints || '–' }}</strong></td>
              <td class="col-avg">{{ student.avgPoints ? student.avgPoints.toFixed(2) : '–' }}</td>
              <td :class="['col-grade', getGradeClass(student.grade)]">
                <strong>{{ student.grade?.toFixed(1) ?? '–' }}</strong>
              </td>
            </tr>
          </tbody>
          <tfoot>
            <tr class="avg-row">
              <td class="sticky-col col-name"><strong>Klassenschnitt</strong></td>
              <td></td>
              <template v-for="disc in data.disciplines" :key="disc.id">
                <td class="col-value">
                  {{ data.classAverages[disc.id]?.avgValue?.toFixed(1) ?? '–' }}
                </td>
                <td class="col-points">
                  {{ data.classAverages[disc.id]?.avgPoints?.toFixed(1) ?? '–' }}
                </td>
              </template>
              <td></td>
              <td></td>
              <td></td>
            </tr>
          </tfoot>
        </table>
      </div>

      <div class="legend">
        <h3>Legende</h3>
        <div class="legend-items">
          <span class="legend-item"><span class="cell-missing legend-swatch"></span> Fehlend (keine Daten, keine Annotation)</span>
          <span class="legend-item"><span class="badge badge-annotated">Beispiel</span> Annotiert (z.B. verletzt, dispensiert)</span>
          <span class="legend-item"><span class="attempt-count">(3)</span> Anzahl Versuche</span>
        </div>
        <p class="legend-hint">Klicken Sie auf einen Wert, um ihn zu bearbeiten. Klicken Sie auf «—» oder eine Annotation, um eine Annotation hinzuzufügen/zu ändern.</p>
      </div>

      <!-- Edit Result Dialog -->
      <EditResultDialog
        v-if="editDialog.visible"
        :student="editDialog.student!"
        :discipline="editDialog.discipline!"
        :discipline-data="editDialog.disciplineData!"
        :school-year="editDialog.schoolYear"
        @close="handleDialogClose"
      />

      <!-- Annotation Dialog -->
      <AnnotationDialog
        v-if="annotationDialog.visible"
        :student="annotationDialog.student!"
        :discipline="annotationDialog.discipline!"
        :discipline-data="annotationDialog.disciplineData!"
        :school-year="annotationDialog.schoolYear"
        @close="handleDialogClose"
      />
    </template>
  </div>
</template>

<style scoped>
.page-header {
  margin-bottom: 1.5rem;
}

.page-header h2 {
  margin: 0.25rem 0 0;
  font-size: 1.5rem;
  color: #1a365d;
}

.back-link {
  font-size: 0.85rem;
  color: #4a5568;
}

.class-profession {
  color: #718096;
  font-size: 0.9rem;
  margin-top: 0.25rem;
}

.table-wrapper {
  overflow-x: auto;
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.results-table {
  min-width: 1200px;
  font-size: 0.85rem;
}

.results-table th {
  position: sticky;
  top: 0;
  z-index: 1;
  background-color: #1a365d;
  color: white;
  text-transform: none;
  font-size: 0.8rem;
  padding: 0.6rem 0.5rem;
}

.results-table th .unit {
  font-weight: 400;
  font-size: 0.7rem;
  opacity: 0.8;
}

.sticky-col {
  position: sticky;
  left: 0;
  z-index: 2;
  background-color: inherit;
}

th.sticky-col {
  z-index: 3;
}

.col-name {
  min-width: 160px;
  background-color: white;
}

tr:hover .col-name {
  background-color: #f7fafc;
}

.col-gender { text-align: center; width: 40px; }
.col-value { text-align: right; min-width: 80px; }
.col-points { text-align: center; width: 40px; font-size: 0.8rem; }
.col-total { text-align: center; width: 50px; }
.col-avg { text-align: center; width: 60px; }
.col-grade { text-align: center; width: 50px; }

.clickable {
  cursor: pointer;
  transition: background-color 0.15s;
}

.clickable:hover {
  background-color: #ebf8ff !important;
}

.cell-missing {
  background-color: #fed7d7;
}

.cell-annotated {
  background-color: #fefcbf;
}

.annotation-badge {
  cursor: pointer;
}

.missing-indicator {
  color: #e53e3e;
  font-weight: bold;
  cursor: pointer;
}

.attempt-count {
  font-size: 0.7rem;
  color: #a0aec0;
  margin-left: 2px;
}

.points-excellent { color: #276749; font-weight: 600; }
.points-good { color: #2b6cb0; }
.points-average { color: #975a16; }
.points-poor { color: #c53030; }

.grade-excellent { background-color: #c6f6d5; }
.grade-good { background-color: #fefcbf; }
.grade-poor { background-color: #fed7d7; }

.avg-row {
  background-color: #edf2f7;
  font-weight: 600;
}

.avg-row .col-name {
  background-color: #edf2f7;
}

.legend {
  margin-top: 1.5rem;
  padding: 1rem 1.5rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.legend h3 {
  margin: 0 0 0.75rem;
  font-size: 1rem;
  color: #1a365d;
}

.legend-items {
  display: flex;
  flex-wrap: wrap;
  gap: 1.5rem;
  margin-bottom: 0.5rem;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.85rem;
}

.legend-swatch {
  display: inline-block;
  width: 20px;
  height: 16px;
  border-radius: 3px;
  border: 1px solid #e2e8f0;
}

.legend-hint {
  font-size: 0.8rem;
  color: #718096;
  margin-top: 0.5rem;
}
</style>
