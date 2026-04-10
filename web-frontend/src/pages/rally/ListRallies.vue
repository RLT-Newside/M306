<template>
  <v-container>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h3">Rallyes</h1>
      </v-col>
      <v-col cols="12">
        <p class="text-body-1">
          Eine Rallye besteht aus einer Abfolge von Stationen. Diese Stationen können in einer
          definierten Reihenfolge oder zufällig aufeinander folgen. Der Start- und Endpunkt einer
          Rallye kann explizit festgelegt werden.
        </p>
        <br />
        <p class="text-body-1">
          Um eine Rallye zu erstellen, sollten zuerst die entsprechenden Stationen erfasst werden.
          Danach kann die Rallye aus den vorerfassten Stationen zusammengesetzt werden.
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
                  <th class="text-left">Titel der Rallye</th>
                  <th class="text-left"># Stages</th>
                  <th class="text-left"># Assignments</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="rally in rallies"
                  :key="rally.id"
                >
                  <td>{{ rally.title }}</td>
                  <td>{{ rally.rallyStages.length }}</td>
                  <td>{{ rally.assignments.length }}</td>
                  <td>
                    <v-btn
                      class="rally-list-icon-btn"
                      icon
                      variant="text"
                      :to="{ name: 'Show rally', params: { rallyId: rally.id } }"
                    >
                      <v-icon>mdi-magnify</v-icon>
                    </v-btn>
                    <v-btn
                      class="rally-list-icon-btn"
                      icon
                      variant="text"
                      @click="
                        openDeleteRallyDialog(
                          rally.id,
                          rally.title,
                          rally.assignments.length,
                          rally.rallyStages.length
                        )
                      "
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
          :to="{ name: 'New rally' }"
          color="primary"
          >Rallye erstellen</v-btn
        >
      </v-col>
    </v-row>

    <DeleteRallyConfirmDialog
      v-model="showDeleteRallyDialog"
      v-model:confirmation-text="deleteRallyConfirmationText"
      :rally-title="selectedRally?.title"
      :delete-impact="deleteImpact"
      :confirm-disabled="!isRallyDeleteConfirmed"
      @confirm="handleDeleteRally"
    />
  </v-container>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import DeleteRallyConfirmDialog from '@/components/rally/DeleteRallyConfirmDialog.vue';
import { useRequest } from 'alova/client';
import { deleteRally, getRallies, getRallyDeletionImpact } from '@/api/alovaMethods/gibzRally';
import type { RallyDeletionImpactState } from '@/models/rally/management';
import { useNotifications } from '@/composables/useNotifications';

const { showSuccess, showError } = useNotifications();

const showDeleteRallyDialog = ref(false);
const deleteRallyConfirmationText = ref('');
const selectedRally = ref<{ id: string; title: string } | null>(null);

const deleteImpact = ref<RallyDeletionImpactState>({
  assignmentCount: 0,
  rallyStageCount: 0,
  stageActivityResultCount: null as number | null,
});

const isRallyDeleteConfirmed = computed(
  () => deleteRallyConfirmationText.value.trim() === (selectedRally.value?.title ?? '')
);

const { data: rallies, send: fetchRallies } = useRequest(getRallies());

const { send: executeDeleteRally } = useRequest((rallyId: string) => deleteRally(rallyId), {
  immediate: false,
});

const { send: loadRallyDeletionImpact } = useRequest(
  (rallyId: string) => getRallyDeletionImpact(rallyId),
  { immediate: false }
);

async function openDeleteRallyDialog(
  rallyId: string,
  title: string,
  assignmentCount: number,
  rallyStageCount: number
): Promise<void> {
  selectedRally.value = { id: rallyId, title };
  deleteRallyConfirmationText.value = '';
  deleteImpact.value = {
    assignmentCount,
    rallyStageCount,
    stageActivityResultCount: null,
  };
  showDeleteRallyDialog.value = true;

  try {
    const impact = await loadRallyDeletionImpact(rallyId);
    deleteImpact.value = {
      assignmentCount: impact.assignmentCount,
      rallyStageCount: impact.rallyStageCount,
      stageActivityResultCount: impact.stageActivityResultCount ?? null,
    };
  } catch {
    // Keep fallback values from table data.
  }
}

async function handleDeleteRally(): Promise<void> {
  if (!selectedRally.value || !isRallyDeleteConfirmed.value) {
    return;
  }

  try {
    await executeDeleteRally(selectedRally.value.id);
    showSuccess('Rallye wurde gelöscht.');
    showDeleteRallyDialog.value = false;
    selectedRally.value = null;
    await fetchRallies();
  } catch (error) {
    const status = showError(error, {
      notFound: 'Die Rallye existiert nicht mehr.',
      forbidden: 'Keine Berechtigung zum Löschen der Rallye.',
      conflict: 'Die Rallye kann aktuell nicht gelöscht werden.',
      fallback: 'Rallye konnte nicht gelöscht werden.',
    });

    if (status === 404) {
      showDeleteRallyDialog.value = false;
      selectedRally.value = null;
      await fetchRallies();
    }
  }
}
</script>
