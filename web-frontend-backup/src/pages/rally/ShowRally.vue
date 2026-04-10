<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">{{ rally?.title }}</h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-card
          title="Rallye-Stationen"
          class="rally-section-card"
        >
          <v-card-text>
            <v-table
              v-if="stages && rally?.rallyStages.length > 0"
              class="rally-table"
            >
              <thead>
                <tr>
                  <th>Titel</th>
                  <th>Information</th>
                  <th># Standorte</th>
                  <th># Einschränkungen</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="rallyStage in rally?.rallyStages"
                  :key="rallyStage.id"
                >
                  <td>{{ stageWithId(rallyStage.stageId)?.title }}</td>
                  <td>{{ stageWithId(rallyStage.stageId)?.information.title }}</td>
                  <td>{{ stageWithId(rallyStage.stageId)?.locations.length }}</td>
                  <td>{{ rallyStage.constraints.length }}</td>
                  <td class="text-right">
                    <v-btn
                      class="rally-list-icon-btn"
                      variant="text"
                      icon="mdi-source-commit-local"
                      :to="{
                        name: 'Show rallyStage',
                        params: { rallyId: props.rallyId, rallyStageId: rallyStage.id },
                      }"
                    />
                    <v-btn
                      class="rally-list-icon-btn"
                      variant="text"
                      icon="mdi-delete"
                      @click="openDeleteRallyStageDialog(rallyStage.id)"
                    />
                  </td>
                </tr>
              </tbody>
            </v-table>
            <p
              v-else
              class="text-body-2 text-center"
            >
              Es sind noch keine Rallye-Stationen für die Rallye
              <span class="font-italic">{{ rally?.title }}</span> erfasst.
            </p>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              variant="text"
              :to="{ name: 'Add rallyStage', params: { rallyId: props.rallyId } }"
              prepend-icon="mdi-plus"
              >Rallye-Station hinzufügen</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-card
          title="Zuweisungen"
          class="rally-section-card"
        >
          <v-card-text>
            <v-table
              v-if="rally?.assignments.length > 0"
              class="rally-table"
            >
              <thead>
                <tr>
                  <th>Kohorte</th>
                  <th>Teilnahme-Code</th>
                  <th># Teilnahmen</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="assignment in rally?.assignments"
                  :key="assignment.id"
                >
                  <td>{{ assignment.audience.title }}</td>
                  <td>{{ formattedJoiningCode(assignment.joiningCode) }}</td>
                  <td>{{ assignment.audience.participatingParties.length }}</td>
                  <td>
                    <v-btn
                      class="rally-list-icon-btn"
                      variant="text"
                      icon="mdi-delete"
                      @click="openDeleteAssignmentDialog(assignment.id, assignment.audience.title)"
                    />
                  </td>
                </tr>
              </tbody>
            </v-table>
            <p
              v-else
              class="text-body-2 text-center"
            >
              Es sind noch keine Zuweisungen für die Rallye
              <span class="font-italic">{{ rally?.title }}</span> erfasst.
            </p>
            <new-assignment
              :rally-id="props.rallyId"
              @added-new-assignment="fetchRally"
            />
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              variant="text"
              prepend-icon="mdi-content-copy"
              @click="assignmentsToClipboard"
              >In die Zwischenablage kopieren</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col class="d-flex justify-end">
        <v-btn
          color="error"
          prepend-icon="mdi-delete"
          @click="openDeleteRallyDialog"
          >Rallye löschen</v-btn
        >
      </v-col>
    </v-row>

    <DeleteConfirmDialog
      v-model="showDeleteRallyStageDialog"
      title="Rallye-Station entfernen"
      message="Möchten Sie diese Rallye-Station aus der Rallye entfernen?"
      @confirm="handleDeleteRallyStage"
    />

    <DeleteConfirmDialog
      v-model="showDeleteAssignmentDialog"
      title="Zuweisung löschen"
      message="Möchten Sie diese Zuweisung endgültig löschen?"
      @confirm="handleDeleteAssignment"
    >
      <template #message>
        <p>
          Möchten Sie die Zuweisung für die Kohorte
          <strong>{{ selectedAssignment?.cohortTitle }}</strong>
          endgültig löschen?
        </p>
      </template>
    </DeleteConfirmDialog>

    <DeleteRallyConfirmDialog
      v-model="showDeleteRallyDialog"
      v-model:confirmation-text="deleteRallyConfirmationText"
      :rally-title="rally?.title"
      :delete-impact="deleteImpact"
      :confirm-disabled="!isRallyDeleteConfirmed"
      @confirm="handleDeleteRally"
    />
  </v-container>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';
import type { Stage } from '../../models/rally/stage';
import NewAssignment from './NewAssignment.vue';
import DeleteConfirmDialog from '@/components/rally/DeleteConfirmDialog.vue';
import DeleteRallyConfirmDialog from '@/components/rally/DeleteRallyConfirmDialog.vue';
import { useRequest } from 'alova/client';
import {
  deleteAssignment,
  deleteRally,
  deleteRallyStage,
  getRallyDeletionImpact,
  getRally,
  getStages,
} from '@/api/alovaMethods/gibzRally';
import type { RallyDeletionImpactState } from '@/models/rally/management';
import { useNotifications } from '@/composables/useNotifications';

const props = defineProps({
  rallyId: { type: String, required: true },
});

const router = useRouter();
const { showSuccess, showError } = useNotifications();

const showDeleteRallyDialog = ref(false);
const showDeleteRallyStageDialog = ref(false);
const showDeleteAssignmentDialog = ref(false);
const deleteRallyConfirmationText = ref('');
const selectedRallyStageId = ref<string | null>(null);
const selectedAssignment = ref<{ id: string; cohortTitle: string } | null>(null);

const deleteImpact = ref<RallyDeletionImpactState>({
  assignmentCount: 0,
  rallyStageCount: 0,
  stageActivityResultCount: null as number | null,
});

const { data: rally, send: fetchRally } = useRequest(getRally(props.rallyId));
const { data: stages } = useRequest(getStages());

const isRallyDeleteConfirmed = computed(
  () => deleteRallyConfirmationText.value.trim() === (rally.value?.title ?? '')
);

const { send: removeRallyStage, onComplete: removeRallyStageCompleted } = useRequest(
  (rallyStageId: string) => deleteRallyStage(props.rallyId, rallyStageId),
  { immediate: false }
);

const { send: removeAssignment, onComplete: removeAssignmentCompleted } = useRequest(
  (assignmentId: string) => deleteAssignment(props.rallyId, assignmentId),
  { immediate: false }
);

const { send: executeDeleteRally } = useRequest(() => deleteRally(props.rallyId), {
  immediate: false,
});

const { send: loadRallyDeletionImpact } = useRequest(() => getRallyDeletionImpact(props.rallyId), {
  immediate: false,
});

removeRallyStageCompleted(() => fetchRally());
removeAssignmentCompleted(() => fetchRally());

function stageWithId(stageId: string): Stage | null {
  return stages.value?.find((stage) => stage.id == stageId) || null;
}

function formattedJoiningCode(joiningCode: number): string {
  const joiningCodeString = joiningCode.toString();
  return joiningCodeString.substring(0, 3) + '-' + joiningCodeString.substring(3);
}

function openDeleteRallyStageDialog(rallyStageId: string): void {
  selectedRallyStageId.value = rallyStageId;
  showDeleteRallyStageDialog.value = true;
}

function openDeleteAssignmentDialog(assignmentId: string, cohortTitle: string): void {
  selectedAssignment.value = { id: assignmentId, cohortTitle };
  showDeleteAssignmentDialog.value = true;
}

async function handleDeleteRallyStage(): Promise<void> {
  if (!selectedRallyStageId.value) {
    return;
  }

  try {
    await removeRallyStage(selectedRallyStageId.value);
    showSuccess('Rallye-Station wurde entfernt.');
    showDeleteRallyStageDialog.value = false;
    selectedRallyStageId.value = null;
  } catch (error) {
    const status = showError(error, {
      notFound: 'Die Rallye-Station existiert nicht mehr.',
      forbidden: 'Keine Berechtigung zum Entfernen der Rallye-Station.',
      conflict: 'Die Rallye-Station kann aktuell nicht entfernt werden.',
      fallback: 'Rallye-Station konnte nicht entfernt werden.',
    });

    if (status === 404) {
      showDeleteRallyStageDialog.value = false;
      selectedRallyStageId.value = null;
      await fetchRally();
    }
  }
}

async function handleDeleteAssignment(): Promise<void> {
  if (!selectedAssignment.value) {
    return;
  }

  try {
    await removeAssignment(selectedAssignment.value.id);
    showSuccess('Zuweisung wurde gelöscht.');
    showDeleteAssignmentDialog.value = false;
    selectedAssignment.value = null;
  } catch (error) {
    const status = showError(error, {
      notFound: 'Die Zuweisung existiert nicht mehr.',
      forbidden: 'Keine Berechtigung zum Löschen der Zuweisung.',
      conflict: 'Die Zuweisung kann aktuell nicht gelöscht werden.',
      fallback: 'Zuweisung konnte nicht gelöscht werden.',
    });

    if (status === 404) {
      showDeleteAssignmentDialog.value = false;
      selectedAssignment.value = null;
      await fetchRally();
    }
  }
}

async function openDeleteRallyDialog(): Promise<void> {
  deleteRallyConfirmationText.value = '';
  deleteImpact.value = {
    assignmentCount: rally.value?.assignments.length ?? 0,
    rallyStageCount: rally.value?.rallyStages.length ?? 0,
    stageActivityResultCount: null,
  };

  showDeleteRallyDialog.value = true;

  try {
    const impact = await loadRallyDeletionImpact();
    deleteImpact.value = {
      assignmentCount: impact.assignmentCount,
      rallyStageCount: impact.rallyStageCount,
      stageActivityResultCount: impact.stageActivityResultCount ?? null,
    };
  } catch {
    // Keep fallback values from already loaded rally data.
  }
}

async function handleDeleteRally(): Promise<void> {
  if (!isRallyDeleteConfirmed.value) {
    return;
  }

  try {
    await executeDeleteRally();
    showSuccess('Rallye wurde gelöscht.');
    showDeleteRallyDialog.value = false;
    router.push({ name: 'List rallies' });
  } catch (error) {
    const status = showError(error, {
      notFound: 'Die Rallye existiert nicht mehr.',
      forbidden: 'Keine Berechtigung zum Löschen der Rallye.',
      conflict: 'Die Rallye kann aktuell nicht gelöscht werden.',
      fallback: 'Rallye konnte nicht gelöscht werden.',
    });

    if (status === 404) {
      showDeleteRallyDialog.value = false;
      router.push({ name: 'List rallies' });
    }
  }
}

async function assignmentsToClipboard(): Promise<void> {
  const assignments = rally.value.assignments;
  const data = assignments.map(
    (assignment) => assignment.audience.title + '\t' + assignment.joiningCode
  );
  await navigator.clipboard.writeText(data.join('\n'));
}
</script>
