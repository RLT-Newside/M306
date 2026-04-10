<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">
          Station <span class="font-weight-bold">{{ rallyStageTitle }}</span> für die Rallye
          <span class="font-weight-bold">{{ rally?.title }}</span>
        </h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <p class="text-body-1">
          Die Einschränkungen einer Rallye-Station steuern den Ablauf während der Durchführung einer
          Rallye.
        </p>
        <br />
        <p class="text-body-1">
          Grundsätzlich werden die verschiedenen Rallye-Stationen innerhalb einer Rallye in
          zufälliger Reihenfolge durchlaufen. Auf eine Wiederholung gleicher Rallye-Stationen wird
          verzichtet - jedes Team absolviert jede Station also genau ein Mal. Die Rallye gilt als
          beendet, sobald alle Stationen einmal durchlaufen wurden.
        </p>
        <br />
        <p class="text-body-1">
          Mit Einschränkungen können spezifische Start- oder Endpunkte für die Rallye definiert
          werden. Zudem können Einschränkungen basierend auf bereits absolvierten Rallye-Stationen
          definiert werden.
        </p>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <v-card
          title="Einschränkungen"
          class="rally-section-card"
        >
          <v-card-text>
            <v-table
              v-if="rallyStage?.constraints.length > 0"
              class="rally-table"
            >
              <thead>
                <tr>
                  <th>Typ</th>
                  <th>Zusätzliche Informationen</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="constraint in rallyStage?.constraints"
                  :key="constraint.id"
                >
                  <td>{{ getConstraintTypeLabel(constraint) }}</td>
                  <td>{{ getAdditionalInformation(constraint) }}</td>
                  <td class="text-align-right">
                    <v-btn
                      class="rally-list-icon-btn"
                      variant="text"
                      icon="mdi-delete"
                      @click.stop="openDeleteConstraintDialog(constraint)"
                    ></v-btn>
                  </td>
                </tr>
              </tbody>
            </v-table>
            <p
              v-else
              class="text-body-1 font-italic"
            >
              Aktuell sind für diese Rallye-Station keine Einschränkungen definiert.
            </p>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              variant="text"
              prepend-icon="mdi-plus"
              @click="dialogModel = true"
              >Einschränkung hinzufügen</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <v-row justify="center">
      <v-col cols="auto">
        <v-btn
          prepend-icon="mdi-arrow-left"
          :to="{
            name: 'Show rally',
            params: { rallyId: props.rallyId },
          }"
        >
          Zurück zur Rallye
        </v-btn>
      </v-col>
    </v-row>
  </v-container>

  <DeleteConfirmDialog
    v-model="showDeleteConstraintDialog"
    title="Einschränkung löschen"
    message=""
    @confirm="handleDeleteConstraint"
  >
    <template #message>
      <p>
        Möchten Sie die Einschränkung vom Typ
        <strong>{{ constraintToDelete ? getConstraintTypeLabel(constraintToDelete) : '' }}</strong>
        endgültig löschen?
      </p>
    </template>
  </DeleteConfirmDialog>

  <v-dialog
    v-model="dialogModel"
    width="850px"
  >
    <v-form @submit.prevent="saveConstraint">
      <v-card title="Einschränkung hinzufügen">
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <p class="text-body-1">
                  Wählen Sie die Art der Einschränkung aus. Für die Einschränkungen vom Typ
                  <span class="font-italic">Vorgänger</span> und
                  <span class="font-italic">Nachfolger</span> müssen zusätzliche Angaben erfasst
                  werden.
                </p>
              </v-col>
              <v-col cols="12">
                <v-select
                  v-model="constraintCreationValues.type"
                  :items="constraintTypes"
                >
                  <template #item="{ props: itemProps, item }">
                    <v-list-item
                      v-bind="itemProps"
                      :prepend-icon="item.raw.icon"
                      :subtitle="item.raw.description"
                    ></v-list-item>
                  </template>

                  <template #selection="{ item }">
                    <v-list-item
                      density="compact"
                      :title="item.title"
                      :prepend-icon="item.raw.icon"
                    />
                  </template>
                </v-select>
              </v-col>
            </v-row>

            <v-row>
              <v-col
                v-if="
                  constraintCreationValues.type == 'predecessor' ||
                  constraintCreationValues.type == 'successor'
                "
                cols="12"
              >
                <v-select
                  v-model="constraintCreationValues.rallyStageId"
                  :label="
                    constraintCreationValues.type == 'predecessor'
                      ? 'Vorgänger Rallye-Station'
                      : 'Nachfolger Rallye-Station'
                  "
                  :items="constraintSelectItems"
                />
                <v-checkbox
                  v-if="constraintCreationValues.type == 'predecessor'"
                  v-model="constraintCreationValues.immediate"
                  :label="
                    'Die ausgewählte Rallye-Station (' +
                    getStageTitle(constraintCreationValues.rallyStageId) +
                    ') muss ein direkter Vorgänger sein'
                  "
                />
              </v-col>

              <v-col
                v-if="constraintCreationValues.type === 'entryPoint'"
                cols="12"
              >
                <p>
                  Die Rallye-Station
                  <span class="font-weight-bold">{{ rallyStageTitle }}</span> wird als
                  <span class="font-weight-bold">Startpunkt</span> für die Rallye
                  {{ rally.title }} festgelegt.
                </p>
              </v-col>

              <v-col
                v-if="constraintCreationValues.type === 'terminal'"
                cols="12"
              >
                <p>
                  Die Rallye-Station
                  <span class="font-weight-bold">{{ rallyStageTitle }}</span> wird als
                  <span class="font-weight-bold">Endpunkt</span> für die Rallye
                  {{ rally.title }} festgelegt.
                </p>
              </v-col>

              <template v-if="constraintCreationValues.rallyStageId">
                <v-col
                  v-if="constraintCreationValues.type === 'predecessor'"
                  cols="12"
                >
                  <p>
                    Diese Einschränkung definiert, dass die Rallye-Station
                    <span class="font-weight-bold">{{ rallyStageTitle }}</span> nur absolviert
                    werden kann, wenn
                    <span class="font-italic">{{
                      constraintCreationValues.immediate ? 'direkt' : 'irgendwann'
                    }}</span>
                    zuvor die Station
                    <span class="font-weight-bold">{{
                      getStageTitle(constraintCreationValues.rallyStageId)
                    }}</span>
                    besucht wurde.
                  </p>
                </v-col>

                <v-col
                  v-if="constraintCreationValues.type === 'successor'"
                  cols="12"
                >
                  <p>
                    Diese Einschränkung definiert, dass unmittelbar nach der Rallye-Station
                    <span class="font-weight-bold">{{ rallyStageTitle }}</span> die Station
                    <span class="font-weight-bold">{{
                      getStageTitle(constraintCreationValues.rallyStageId)
                    }}</span>
                    absolviert werden soll.
                  </p>
                </v-col>
              </template>
            </v-row>
            <v-row justify="center">
              <v-col cols="auto">
                <v-btn
                  prepend-icon="mdi-close"
                  @click="dialogModel = false"
                  >abbrechen</v-btn
                >
              </v-col>
              <v-col cols="auto">
                <v-btn
                  type="submit"
                  prepend-icon="mdi-content-save"
                  :disabled="
                    constraintCreationValues.type === '' ||
                    ((constraintCreationValues.type === 'predecessor' ||
                      constraintCreationValues.type === 'successor') &&
                      constraintCreationValues.rallyStageId === '')
                  "
                  >Einschränkung hinzufügen</v-btn
                >
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, reactive, ref } from 'vue';
import { Constraint, PredecessorConstraint, SuccessorConstraint } from '../../models/rally/rally';
import DeleteConfirmDialog from '@/components/rally/DeleteConfirmDialog.vue';

import {
  createConstraint,
  deleteConstraint,
  getRally,
  getRallyStage,
  getStages,
} from '@/api/alovaMethods/gibzRally';
import { useRequest } from 'alova/client';
import { useNotifications } from '@/composables/useNotifications';

const props = defineProps({
  rallyId: { type: String, required: true },
  rallyStageId: { type: String, required: true },
});

const constraintTypes = [
  {
    title: 'Startpunkt',
    value: 'entryPoint',
    icon: 'mdi-source-commit-start',
    description: 'Ein Startpunkt legt die erste Rallye-Station der Rallye fest.',
  },
  {
    title: 'Vorgänger',
    value: 'predecessor',
    icon: 'mdi-source-commit-start-next-local',
    description:
      'Diese Einschränkung definiert einen Vorgänger, welcher zwingend zuvor absolviert werden muss.',
  },
  {
    title: 'Nachfolger',
    value: 'successor',
    icon: 'mdi-source-commit-end-local',
    description:
      'Diese Einschränkung definiert einen Nachfolger, welcher unmittelbar nach dieser Rallye-Station folgt.',
  },
  {
    title: 'Endpunkt',
    value: 'terminal',
    icon: 'mdi-source-commit-end',
    description: 'Beim Erreichen eines Endpunktes gilt die Rallye als beendet.',
  },
];

const { data: rally } = useRequest(getRally(props.rallyId));
const { data: stages } = useRequest(getStages);
const { data: rallyStage, send: fetchRallyStage } = useRequest(
  getRallyStage(props.rallyId, props.rallyStageId)
);

const { send: addConstraint } = useRequest(
  (newConstraint: Partial<Constraint>) =>
    createConstraint(props.rallyId, props.rallyStageId, newConstraint),
  { immediate: false }
);

const { send: removeConstraint, onComplete: constraintDeleted } = useRequest(
  (constraintId: string) => deleteConstraint(props.rallyId, props.rallyStageId, constraintId),
  { immediate: false }
);

const dialogModel = ref(false);
const showDeleteConstraintDialog = ref(false);
const constraintToDelete = ref<Constraint | null>(null);

const { showSuccess, showError } = useNotifications();

const constraintCreationValues = reactive({
  type: '',
  rallyStageId: '',
  immediate: false,
});

const rallyStageTitle = computed<string>(() => {
  return getStageTitle(props.rallyStageId);
});

const constraintSelectItems = computed<Array<{ title: string; value: string }>>(() => {
  return (
    rally.value?.rallyStages
      .filter((rallyStage) => rallyStage.id !== props.rallyStageId)
      .map((rallyStage) => {
        const stage = stages.value.find((stage) => stage.id == rallyStage.stageId);
        return {
          title: stage?.title ?? 'Unbekannt',
          value: rallyStage.id,
        };
      }) ?? []
  );
});

function getConstraintTypeLabel(constraint: Constraint): string {
  return constraintTypes.find((type) => type.value === constraint.type)?.title ?? 'unbekannt';
}

function getAdditionalInformation(constraint: Constraint): string {
  if (constraint.type == 'predecessor') {
    const predecessorConstraint = constraint as PredecessorConstraint;
    let output = getStageTitle(predecessorConstraint.predecessorId);
    output += predecessorConstraint.immediate ? ' [immediate]' : '';
    return output;
  }

  if (constraint.type == 'successor') {
    const successorConstraint = constraint as SuccessorConstraint;
    return getStageTitle(successorConstraint.successorId);
  }

  return ' - ';
}

function getStageTitle(rallyStageId: string): string {
  const rallyStage = rally.value?.rallyStages.find((rallyStage) => rallyStage.id == rallyStageId);
  const stage = stages.value?.find((stage) => stage.id == rallyStage?.stageId);
  return stage?.title ?? '';
}

async function saveConstraint(): Promise<void> {
  const payload = Constraint.BuildConstraint(
    constraintCreationValues.type,
    [],
    constraintCreationValues.rallyStageId,
    constraintCreationValues.immediate
  );

  dialogModel.value = false;

  constraintCreationValues.type = '';
  constraintCreationValues.rallyStageId = '';
  constraintCreationValues.immediate = false;

  try {
    await addConstraint(payload);
    showSuccess('Einschränkung wurde erstellt.');
    await fetchRallyStage();
  } catch (error) {
    showError(error, {
      notFound: 'Die Einschränkung konnte nicht erstellt werden, da ein Bezug fehlt.',
      forbidden: 'Keine Berechtigung zum Erstellen der Einschränkung.',
      conflict: 'Diese Einschränkung ist ungültig oder bereits vorhanden.',
      fallback: 'Einschränkung konnte nicht erstellt werden.',
    });
  }
}

constraintDeleted((event) => {
  if (event.status === 'success') {
    fetchRallyStage();
  }
});

function openDeleteConstraintDialog(constraint: Constraint): void {
  constraintToDelete.value = constraint;
  showDeleteConstraintDialog.value = true;
}

async function handleDeleteConstraint(): Promise<void> {
  if (!constraintToDelete.value) {
    return;
  }

  try {
    await removeConstraint(constraintToDelete.value.id);
    showSuccess('Einschränkung wurde gelöscht.');
    showDeleteConstraintDialog.value = false;
    constraintToDelete.value = null;
  } catch (error) {
    showError(error, {
      notFound: 'Die Einschränkung existiert nicht mehr.',
      forbidden: 'Keine Berechtigung zum Löschen der Einschränkung.',
      conflict: 'Die Einschränkung kann aktuell nicht gelöscht werden.',
      fallback: 'Einschränkung konnte nicht gelöscht werden.',
    });
  }
}
</script>
