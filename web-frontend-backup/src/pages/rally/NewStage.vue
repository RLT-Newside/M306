<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Neue Station erfassen</h1>
      </v-col>
    </v-row>

    <StageForm v-model="stage" />

    <v-row justify="center">
      <v-col cols="auto">
        <v-btn :to="{ name: 'List stages' }">Abbrechen</v-btn>
      </v-col>
      <v-col cols="auto">
        <v-btn
          color="primary"
          @click="submitStage"
          >Speichern</v-btn
        >
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import router from '@/plugins/router';
import { useRequest } from 'alova/client';
import { createStage as createStageRequest } from '@/api/alovaMethods/gibzRally';
import { useNotifications } from '@/composables/useNotifications';
import { Stage, StageInformation } from '../../models/rally/stage';
import StageForm from '@/components/rally/StageForm.vue';

const stage = ref<Stage>(
  new Stage('', '', new StageInformation('', '', '', []), new StageInformation('', '', '', []), [])
);
const { showSuccess, showError } = useNotifications();

const { send: createStage } = useRequest((stage: Partial<Stage>) => createStageRequest(stage), {
  immediate: false,
});

async function submitStage(): Promise<void> {
  try {
    const createdStage = await createStage(stage.value);
    showSuccess('Station wurde erstellt.');
    router.push({ name: 'Show stage', params: { stageId: createdStage.id } });
  } catch (error) {
    showError(error, {
      notFound: 'Die Station konnte nicht erstellt werden, da ein Bezug fehlt.',
      forbidden: 'Keine Berechtigung zum Erstellen der Station.',
      conflict: 'Die Station kann mit den angegebenen Daten nicht erstellt werden.',
      fallback: 'Station konnte nicht erstellt werden.',
    });
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
