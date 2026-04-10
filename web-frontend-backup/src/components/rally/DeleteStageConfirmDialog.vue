<template>
  <DeleteConfirmDialog
    :model-value="modelValue"
    title="Station löschen"
    message="Möchten Sie diese Station endgültig löschen?"
    :confirm-disabled="stageUsage.isUsed === true"
    @update:model-value="(value) => emit('update:modelValue', value)"
    @confirm="emit('confirm')"
  >
    <template #message>
      <p>
        Möchten Sie die Station
        <strong>{{ stageTitle || 'ohne Titel' }}</strong>
        endgültig löschen?
      </p>
    </template>

    <v-alert
      v-if="stageUsage.isUsed === true"
      type="warning"
      variant="tonal"
      class="mt-3"
    >
      Diese Station wird aktuell in <strong>{{ stageUsageCountText }}</strong> verwendet und kann
      daher nicht gelöscht werden.

      <div
        v-if="stageUsage.rallyTitles.length > 0"
        class="mt-2"
      >
        Verwendet in folgenden Rallyes:
        <ul class="rally-title-list">
          <li
            v-for="rallyTitle in stageUsage.rallyTitles"
            :key="rallyTitle"
          >
            {{ rallyTitle }}
          </li>
        </ul>
      </div>
    </v-alert>
  </DeleteConfirmDialog>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import DeleteConfirmDialog from '@/components/rally/DeleteConfirmDialog.vue';
import type { StageUsageState } from '@/models/rally/management';

const props = defineProps<{
  modelValue: boolean;
  stageTitle?: string;
  stageUsage: StageUsageState;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'confirm'): void;
}>();

const stageUsageCountText = computed(() => {
  const count = props.stageUsage.rallyCount ?? 1;
  return count === 1 ? '1 Rallye' : `${count} Rallyes`;
});
</script>

<style scoped>
.rally-title-list {
  margin-top: 0.25rem;
  margin-bottom: 0;
  padding-inline-start: 1.25rem;
}
</style>
