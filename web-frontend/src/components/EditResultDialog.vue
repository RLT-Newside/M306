<script setup lang="ts">
import { ref } from 'vue'
import { api } from '../services/api'
import type { StudentResult, Discipline, DisciplineData, FitnessResult } from '../types'

const props = defineProps<{
  student: StudentResult
  discipline: Discipline
  disciplineData: DisciplineData
  schoolYear: string
}>()

const emit = defineEmits<{
  close: []
}>()

const saving = ref(false)
const errorMsg = ref('')

// New result form
const newValue = ref<string>('')
const newDate = ref(new Date().toISOString().split('T')[0])

// Editing existing result
const editingId = ref<number | null>(null)
const editValue = ref<string>('')
const editDate = ref('')

function startEdit(result: FitnessResult) {
  editingId.value = result.id
  editValue.value = String(result.value)
  editDate.value = result.date
}

function cancelEdit() {
  editingId.value = null
}

async function saveEdit(result: FitnessResult) {
  saving.value = true
  errorMsg.value = ''
  try {
    const val = parseFloat(editValue.value)
    if (isNaN(val)) throw new Error('Ungültiger Wert')
    await api.updateResult(result.id, { value: val, date: editDate.value })
    emit('close')
  } catch (e: any) {
    errorMsg.value = e.message
  } finally {
    saving.value = false
  }
}

async function addResult() {
  saving.value = true
  errorMsg.value = ''
  try {
    const val = parseFloat(newValue.value)
    if (isNaN(val)) throw new Error('Ungültiger Wert')
    await api.createResult({
      student_id: props.student.id,
      discipline_id: props.discipline.id,
      value: val,
      date: newDate.value,
      school_year: props.schoolYear,
    })
    emit('close')
  } catch (e: any) {
    errorMsg.value = e.message
  } finally {
    saving.value = false
  }
}

async function deleteResult(resultId: number) {
  if (!confirm('Resultat wirklich löschen?')) return
  saving.value = true
  errorMsg.value = ''
  try {
    await api.deleteResult(resultId)
    emit('close')
  } catch (e: any) {
    errorMsg.value = e.message
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="dialog-overlay" @click.self="emit('close')">
    <div class="dialog">
      <div class="dialog-header">
        <h3>{{ discipline.name }} – {{ student.last_name }}, {{ student.first_name }}</h3>
        <button class="close-btn" @click="emit('close')">&times;</button>
      </div>

      <div v-if="errorMsg" class="error-banner">{{ errorMsg }}</div>

      <!-- Existing attempts -->
      <div class="section">
        <h4>Vorhandene Versuche</h4>
        <div v-if="disciplineData.attempts.length === 0" class="no-data">
          Keine Resultate vorhanden.
        </div>
        <table v-else class="attempts-table">
          <thead>
            <tr>
              <th>Versuch</th>
              <th>Wert ({{ discipline.unit }})</th>
              <th>Datum</th>
              <th>Bester</th>
              <th>Aktionen</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="attempt in disciplineData.attempts" :key="attempt.id" :class="{ 'best-row': attempt.is_best }">
              <td>{{ attempt.attempt_number }}</td>
              <td>
                <template v-if="editingId === attempt.id">
                  <input v-model="editValue" type="number" step="0.1" class="edit-input" />
                </template>
                <template v-else>
                  {{ attempt.value }}
                </template>
              </td>
              <td>
                <template v-if="editingId === attempt.id">
                  <input v-model="editDate" type="date" class="edit-input" />
                </template>
                <template v-else>
                  {{ attempt.date }}
                </template>
              </td>
              <td>
                <span v-if="attempt.is_best" class="best-marker">&#9733;</span>
              </td>
              <td class="actions">
                <template v-if="editingId === attempt.id">
                  <button class="primary" @click="saveEdit(attempt)" :disabled="saving">Speichern</button>
                  <button @click="cancelEdit">Abbrechen</button>
                </template>
                <template v-else>
                  <button @click="startEdit(attempt)">Bearbeiten</button>
                  <button class="danger" @click="deleteResult(attempt.id)" :disabled="saving">Löschen</button>
                </template>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Add new result -->
      <div class="section">
        <h4>Neues Resultat hinzufügen</h4>
        <div class="add-form">
          <div class="form-group">
            <label>Wert ({{ discipline.unit }}):</label>
            <input v-model="newValue" type="number" step="0.1" placeholder="Wert eingeben" />
          </div>
          <div class="form-group">
            <label>Datum:</label>
            <input v-model="newDate" type="date" />
          </div>
          <button class="primary" @click="addResult" :disabled="saving || !newValue">
            Hinzufügen
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dialog-overlay {
  position: fixed;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
}

.dialog {
  background: white;
  border-radius: 12px;
  width: 600px;
  max-width: 95vw;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

.dialog-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #e2e8f0;
}

.dialog-header h3 {
  margin: 0;
  font-size: 1.1rem;
  color: #1a365d;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #a0aec0;
  cursor: pointer;
  padding: 0;
  line-height: 1;
}

.close-btn:hover {
  color: #4a5568;
}

.error-banner {
  background-color: #fed7d7;
  color: #c53030;
  padding: 0.5rem 1.5rem;
  font-size: 0.85rem;
}

.section {
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #e2e8f0;
}

.section:last-child {
  border-bottom: none;
}

.section h4 {
  margin: 0 0 0.75rem;
  font-size: 0.95rem;
  color: #2d3748;
}

.no-data {
  color: #a0aec0;
  font-size: 0.85rem;
  font-style: italic;
}

.attempts-table {
  font-size: 0.85rem;
}

.attempts-table th {
  background-color: #f7fafc;
  color: #4a5568;
}

.best-row {
  background-color: #f0fff4;
}

.best-marker {
  color: #d69e2e;
  font-size: 1.1rem;
}

.edit-input {
  width: 100px;
}

.actions {
  display: flex;
  gap: 0.25rem;
}

.actions button {
  font-size: 0.75rem;
  padding: 0.2rem 0.5rem;
}

.add-form {
  display: flex;
  align-items: flex-end;
  gap: 1rem;
  flex-wrap: wrap;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.form-group label {
  font-size: 0.8rem;
  font-weight: 600;
  color: #4a5568;
}

.form-group input {
  width: 150px;
}
</style>
