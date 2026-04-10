<template>
  <v-container v-if="!stageLoading">
    <v-row>
      <v-col>
        <h1 class="text-h3">{{ stage.title }}</h1>
      </v-col>
    </v-row>

    <StageForm v-model="stage" />

    <v-row justify="center">
      <v-col cols="auto">
        <v-btn
          :to="{ name: 'Show stage', params: { stageId: stageId } }"
          prepend-icon="mdi-close"
          >Abbrechen</v-btn
        >
      </v-col>
      <v-col cols="auto">
        <v-btn
          color="primary"
          prepend-icon="mdi-content-save"
          @click="submitStage"
          >Speichern</v-btn
        >
      </v-col>
      <v-col cols="auto">
        <v-btn
          color="error"
          prepend-icon="mdi-delete"
          @click="openDeleteStageDialog"
          >Station löschen</v-btn
        >
      </v-col>
    </v-row>

    <DeleteStageConfirmDialog
      v-model="showDeleteStageDialog"
      :stage-title="stage?.title"
      :stage-usage="stageUsage"
      @confirm="handleDeleteStage"
    />
  </v-container>
</template>

<script setup lang="ts">
import router from '@/plugins/router';
import { ref } from 'vue';
import { useRequest } from 'alova/client';
import {
  deleteStage,
  getStage,
  getStageUsage,
  updateStage as updateStageRequest,
} from '@/api/alovaMethods/gibzRally';
import type { StageUsageState } from '@/models/rally/management';
import { Stage } from '../../models/rally/stage';
import StageForm from '@/components/rally/StageForm.vue';
import DeleteStageConfirmDialog from '@/components/rally/DeleteStageConfirmDialog.vue';
import { useNotifications } from '@/composables/useNotifications';

const props = defineProps({
  stageId: { type: String, required: true },
});

const { showSuccess, showError } = useNotifications();

const showDeleteStageDialog = ref(false);
const stageUsage = ref<StageUsageState>({
  isUsed: null,
  rallyCount: null,
  rallyTitles: [],
});

const { data: stage, loading: stageLoading } = useRequest(getStage(props.stageId));

const { send: updateStage } = useRequest(
  (stage: Partial<Stage>) => updateStageRequest(props.stageId, stage),
  { immediate: false }
);

const { send: executeDeleteStage } = useRequest(() => deleteStage(props.stageId), {
  immediate: false,
});

const { send: loadStageUsage } = useRequest(() => getStageUsage(props.stageId), {
  immediate: false,
});

async function submitStage(): Promise<void> {
  try {
    const updatedStage = await updateStage(stage.value);
    showSuccess('Station wurde gespeichert.');
    router.push({ name: 'Show stage', params: { stageId: updatedStage.id } });
  } catch (error) {
    showError(error, {
      notFound: 'Die Station existiert nicht mehr.',
      forbidden: 'Keine Berechtigung zum Bearbeiten der Station.',
      conflict: 'Die Station kann mit den angegebenen Daten nicht gespeichert werden.',
      fallback: 'Station konnte nicht gespeichert werden.',
    });
  }
}

async function openDeleteStageDialog(): Promise<void> {
  stageUsage.value = { isUsed: null, rallyCount: null, rallyTitles: [] };
  showDeleteStageDialog.value = true;

  try {
    const usage = await loadStageUsage();
    const rallyTitles = usage.rallyTitles ?? [];
    stageUsage.value = {
      isUsed: usage.isUsed,
      rallyCount: usage.rallyCount ?? null,
      rallyTitles,
    };
  } catch {
    // Fallback to delete endpoint validation when usage endpoint is unavailable.
  }
}

async function handleDeleteStage(): Promise<void> {
  try {
    await executeDeleteStage();
    showSuccess('Station wurde gelöscht.');
    showDeleteStageDialog.value = false;
    router.push({ name: 'List stages' });
  } catch (error) {
    const status = showError(error, {
      notFound: 'Die Station existiert nicht mehr.',
      forbidden: 'Keine Berechtigung zum Löschen der Station.',
      conflict: 'Diese Station wird in einer Rallye verwendet und kann nicht gelöscht werden.',
      fallback: 'Station konnte nicht gelöscht werden.',
    });

    if (status === 409) {
      stageUsage.value.isUsed = true;
      if (stageUsage.value.rallyCount === null) {
        stageUsage.value.rallyCount = 1;
      }
    }

    if (status === 404) {
      showDeleteStageDialog.value = false;
      router.push({ name: 'List stages' });
    }
  }
}
</script>

<style lang="scss">
.markdown-preview {
  padding: 15px;
  border: 1px solid black;
  border-radius: 5px;

  list-style-position: inside;

  & ul ul {
    margin-left: 1.5em;
  }
}
</style>
