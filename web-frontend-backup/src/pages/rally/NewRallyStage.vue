<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Neue Station zu Rallye hinzufügen</h1>
      </v-col>
    </v-row>

    <v-row v-if="rally">
      <v-col cols="12">
        <p class="text-body-1">
          Wählen Sie die Station aus, welche Sie zur Rallye
          <span class="font-weight-bold">{{ rally!.title }}</span>
          hinzufügen möchten.
        </p>
      </v-col>
    </v-row>

    <v-row v-if="stages">
      <v-col cols="12">
        <v-select
          v-model="stageId"
          label="Stage"
          :items="stageSelectItems"
        />
      </v-col>
    </v-row>

    <v-row justify="center">
      <v-col cols="auto">
        <v-btn
          prepend-icon="mdi-close"
          :to="{ name: 'Show rally', params: { rallyId: rallyId } }"
        >
          Abbrechen
        </v-btn>
      </v-col>
      <v-col cols="auto">
        <v-btn
          prepend-icon="mdi-content-save"
          color="primary"
          @click="submitRallyStage"
        >
          Speichern
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { RallyStage } from '../../models/rally/rally';
import router from '../../plugins/router';

import { addRallyStage, getRally, getStages } from '@/api/alovaMethods/gibzRally';
import { useRequest } from 'alova/client';
import { useNotifications } from '@/composables/useNotifications';

const props = defineProps({
  rallyId: { type: String, required: true },
});

const { showSuccess, showError } = useNotifications();

const { data: stages, onComplete: stagesLoaded } = useRequest(getStages);
const { data: rally } = useRequest(getRally(props.rallyId));
const { send: addNewRallyStage } = useRequest(
  (rallyStage: Partial<RallyStage>) => addRallyStage(props.rallyId, rallyStage),
  { immediate: false }
);

const stageId = ref('0');

const stageSelectItems = computed<Array<{ title: string; value: string }>>(() => {
  return (
    stages.value?.map((stage) => {
      return { title: stage.title, value: stage.id };
    }) ?? []
  );
});

stagesLoaded((event) => {
  if (event.status === 'success' && typeof event.data === 'object' && event.data?.length > 0) {
    stageId.value = event.data[0]!.id;
  }
});

async function submitRallyStage(): Promise<void> {
  try {
    const createdRallyStage = await addNewRallyStage({
      rallyId: props.rallyId,
      stageId: stageId.value,
    });
    showSuccess('Rallye-Station wurde hinzugefügt.');
    router.push({
      name: 'Show rallyStage',
      params: { rallyId: props.rallyId, rallyStageId: createdRallyStage.id },
    });
  } catch (error) {
    showError(error, {
      notFound: 'Die Rallye-Station konnte nicht erstellt werden, da ein Bezug fehlt.',
      forbidden: 'Keine Berechtigung zum Hinzufügen der Rallye-Station.',
      conflict:
        'Diese Station ist bereits in der Rallye enthalten oder kann nicht hinzugefügt werden.',
      fallback: 'Rallye-Station konnte nicht hinzugefügt werden.',
    });
  }
}
</script>
