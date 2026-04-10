<script setup lang="ts">
import { ref } from 'vue'
import { api } from '../services/api'
import type { StudentResult, Discipline, DisciplineData } from '../types'

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

const commonAnnotations = ['verletzt', 'dispensiert', 'krank', 'abwesend']
const customNote = ref(props.disciplineData.annotation?.note || '')

async function saveAnnotation(note: string) {
  saving.value = true
  errorMsg.value = ''
  try {
    await api.createAnnotation({
      student_id: props.student.id,
      discipline_id: props.discipline.id,
      school_year: props.schoolYear,
      note,
    })
    emit('close')
  } catch (e: any) {
    errorMsg.value = e.message
  } finally {
    saving.value = false
  }
}

async function deleteAnnotation() {
  if (!props.disciplineData.annotation) return
  saving.value = true
  errorMsg.value = ''
  try {
    await api.deleteAnnotation(props.disciplineData.annotation.id)
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
        <h3>Annotation – {{ student.last_name }}, {{ student.first_name }}</h3>
        <button class="close-btn" @click="emit('close')">&times;</button>
      </div>

      <div class="dialog-body">
        <p class="discipline-info">{{ discipline.name }}</p>

        <div v-if="errorMsg" class="error-banner">{{ errorMsg }}</div>

        <div v-if="disciplineData.annotation" class="current-annotation">
          <strong>Aktuelle Annotation:</strong>
          <span class="badge badge-annotated">{{ disciplineData.annotation.note }}</span>
          <button class="danger" @click="deleteAnnotation" :disabled="saving" style="margin-left: 0.5rem;">
            Entfernen
          </button>
        </div>

        <div class="section">
          <h4>Häufige Annotationen</h4>
          <div class="annotation-buttons">
            <button
              v-for="note in commonAnnotations"
              :key="note"
              @click="saveAnnotation(note)"
              :disabled="saving"
              class="annotation-btn"
            >
              {{ note }}
            </button>
          </div>
        </div>

        <div class="section">
          <h4>Benutzerdefiniert</h4>
          <div class="custom-form">
            <input
              v-model="customNote"
              type="text"
              placeholder="Annotation eingeben..."
              @keyup.enter="saveAnnotation(customNote)"
            />
            <button
              class="primary"
              @click="saveAnnotation(customNote)"
              :disabled="saving || !customNote.trim()"
            >
              Speichern
            </button>
          </div>
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
  width: 450px;
  max-width: 95vw;
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

.dialog-body {
  padding: 1.5rem;
}

.discipline-info {
  font-size: 0.9rem;
  color: #718096;
  margin-bottom: 1rem;
}

.error-banner {
  background-color: #fed7d7;
  color: #c53030;
  padding: 0.5rem;
  border-radius: 4px;
  font-size: 0.85rem;
  margin-bottom: 1rem;
}

.current-annotation {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem;
  background-color: #fefcbf;
  border-radius: 6px;
  margin-bottom: 1rem;
  font-size: 0.9rem;
}

.section {
  margin-bottom: 1.25rem;
}

.section h4 {
  margin: 0 0 0.5rem;
  font-size: 0.9rem;
  color: #2d3748;
}

.annotation-buttons {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.annotation-btn {
  padding: 0.4rem 1rem;
  border-radius: 20px;
  background-color: #edf2f7;
  border: 1px solid #e2e8f0;
  font-size: 0.85rem;
}

.annotation-btn:hover {
  background-color: #fefcbf;
  border-color: #d69e2e;
}

.custom-form {
  display: flex;
  gap: 0.5rem;
}

.custom-form input {
  flex: 1;
}
</style>
