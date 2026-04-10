<template>
  <v-container>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h3">Stationen</h1>
      </v-col>
      <v-col cols="12">
        <p class="text-body-1">
          Jede Rallye besteht aus einer oder mehreren Stationen. Die Stationen können in mehreren
          Rallyes parallel genutzt werden. Dazu können in einem ersten Schritt einzelne Stationen
          erfasst werden, welche danach im nächsten Schritt zu einer oder mehreren Rallyes
          zusammengefügt werden.
        </p>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-card class="rally-table-surface">
          <v-card-text class="pa-0">
            <v-table
              class="rally-table"
              fixed-header
            >
              <thead>
                <tr>
                  <th class="text-left">Titel</th>
                  <th class="text-left">Instruktionen</th>
                  <th class="text-left">Informationen</th>
                  <th class="text-left"># Standorte</th>
                  <th class="text-left"># Aktivitäten</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="stage in stages"
                  :key="stage.id"
                >
                  <td>{{ stage.title }}</td>
                  <td>{{ stage.preArrivalInformation.title }}</td>
                  <td>{{ stage.information.title }}</td>
                  <td>{{ stage.locations.length }}</td>
                  <td>{{ stageActivities[stage.id]?.length ?? 0 }}</td>
                  <td>
                    <v-btn
                      class="rally-list-icon-btn"
                      icon
                      variant="text"
                      :to="{ name: 'Show stage', params: { stageId: stage.id } }"
                    >
                      <v-icon>mdi-magnify</v-icon>
                    </v-btn>
                    <v-btn
                      class="rally-list-icon-btn"
                      icon
                      variant="text"
                      :to="{ name: 'Edit stage', params: { stageId: stage.id } }"
                    >
                      <v-icon>mdi-pencil</v-icon>
                    </v-btn>
                    <v-btn
                      class="rally-list-icon-btn"
                      icon
                      variant="text"
                      @click="openDeleteStageDialog(stage)"
                    >
                      <v-icon>mdi-delete</v-icon>
                    </v-btn>
                  </td>
                </tr>
              </tbody>
            </v-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col class="text-center">
        <v-btn
          :to="{ name: 'New stage' }"
          color="primary"
          prepend-icon="mdi-plus"
          >Station erstellen</v-btn
        >
      </v-col>
    </v-row>

    <DeleteStageConfirmDialog
      v-model="showDeleteStageDialog"
      :stage-title="stageToDelete?.title"
      :stage-usage="stageUsage"
      @confirm="handleDeleteStage"
    />
  </v-container>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import type { Stage } from '../../models/rally/stage';
import { useRequest } from 'alova/client';
import {
  deleteStage,
  getStageActivities,
  getStages,
  getStageUsage,
} from '@/api/alovaMethods/gibzRally';
import type { StageUsageState } from '@/models/rally/management';
import type { StageActivity } from '@/models/rally/stageActivity';
import DeleteStageConfirmDialog from '@/components/rally/DeleteStageConfirmDialog.vue';
import { useNotifications } from '@/composables/useNotifications';

const { data: stages, onComplete: stagesLoaded, send: fetchStages } = useRequest(getStages());
const { showSuccess, showError } = useNotifications();

const showDeleteStageDialog = ref(false);
const stageToDelete = ref<Stage | null>(null);
const stageUsage = ref<StageUsageState>({
  isUsed: null,
  rallyCount: null,
  rallyTitles: [],
});

const { send: fetchStageActivities } = useRequest(
  (stageId: string) => getStageActivities(stageId),
  { immediate: true }
);

const { send: executeDeleteStage } = useRequest((stageId: string) => deleteStage(stageId), {
  immediate: false,
});

const { send: loadStageUsage } = useRequest((stageId: string) => getStageUsage(stageId), {
  immediate: false,
});

const stageActivities: { [id: string]: StageActivity[] } = reactive({});

stagesLoaded(() => {
  loadActivitiesForStages();
});

async function openDeleteStageDialog(stage: Stage): Promise<void> {
  stageToDelete.value = stage;
  stageUsage.value = { isUsed: null, rallyCount: null, rallyTitles: [] };
  showDeleteStageDialog.value = true;

  try {
    const usage = await loadStageUsage(stage.id);
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
  if (!stageToDelete.value) {
    return;
  }

  try {
    await executeDeleteStage(stageToDelete.value.id);
    showSuccess('Station wurde gelöscht.');
    showDeleteStageDialog.value = false;
    await reloadStages();
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
      await reloadStages();
    }
  }
}

async function reloadStages(): Promise<void> {
  await fetchStages();
  loadActivitiesForStages();
}

function loadActivitiesForStages(): void {
  Object.keys(stageActivities).forEach((key) => delete stageActivities[key]);

  stages.value?.forEach((stage) => {
    fetchStageActivities(stage.id).then((activities) => {
      stageActivities[stage.id] = activities;
    });
  });
}
</script>

<style lang="scss"></style>
