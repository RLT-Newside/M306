<template>
  <DeleteConfirmDialog
    :model-value="modelValue"
    title="Rallye löschen"
    message="Diese Aktion kann nicht rückgängig gemacht werden. Die zugehörigen Daten werden ebenfalls gelöscht:"
    confirm-text="Endgültig löschen"
    :confirm-disabled="confirmDisabled"
    @update:model-value="(value) => emit('update:modelValue', value)"
    @confirm="emit('confirm')"
  >
    <ul class="delete-impact-list">
      <li>Zuweisungen: {{ deleteImpact.assignmentCount }}</li>
      <li>Rallye-Stationen: {{ deleteImpact.rallyStageCount }}</li>
      <li>Resultate aus Aktivitäten: {{ deleteImpact.stageActivityResultCount ?? 'unbekannt' }}</li>
    </ul>
    <p class="mb-2">
      Geben Sie zur Bestätigung den exakten Titel ein:
      <strong>{{ rallyTitle }}</strong>
    </p>
    <v-text-field
      :model-value="confirmationText"
      label="Rallye-Titel"
      variant="outlined"
      density="comfortable"
      hide-details
      @update:model-value="(value) => emit('update:confirmationText', String(value ?? ''))"
    />
  </DeleteConfirmDialog>
</template>

<script setup lang="ts">
import DeleteConfirmDialog from '@/components/rally/DeleteConfirmDialog.vue';
import type { RallyDeletionImpactState } from '@/models/rally/management';

defineProps<{
  modelValue: boolean;
  rallyTitle?: string;
  confirmationText: string;
  confirmDisabled: boolean;
  deleteImpact: RallyDeletionImpactState;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'update:confirmationText', value: string): void;
  (e: 'confirm'): void;
}>();
</script>

<style scoped>
.delete-impact-list {
  margin: 0.5rem 0 1rem;
  padding-inline-start: 1.5rem;
}
</style>
