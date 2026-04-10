<template>
  <v-container>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h3">Neue Aktivität</h1>
      </v-col>
      <v-col
        v-if="stage"
        cols="12"
      >
        <p class="text-body-1">
          Auf dieser Seite kann eine neue Aktivität zur Station
          <span class="font-weight-bold">{{ stage.title }}</span> hinzugefügt werden.
        </p>
        <p class="text-body-1">
          Wenn für eine Station mehrere Aktivitäten existieren, wird für jedes Team eine
          <span class="font-weight-bold">zufällig ausgewählte</span> Aktivität angezeigt.
        </p>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <v-form
          ref="stageActivityForm"
          v-model="isValidForm"
        >
          <StageActivityForm v-model="stageActivity" />
        </v-form>
      </v-col>
    </v-row>

    <v-row
      align="center"
      justify="center"
    >
      <v-col cols="auto">
        <v-btn :to="{ name: 'Show stage', params: { stageId: stageId } }">abbrechen</v-btn>
      </v-col>
      <v-col cols="auto">
        <v-btn
          type="submit"
          color="primary"
          @click="submitStageActivity"
        >
          Speichern
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { StageActivity, StageActivityType } from '@/models/rally/stageActivity';
import StageActivityForm from '@/components/rally/StageActivityForm.vue';
import { useRequest } from 'alova/client';
import { createStageActivity, getStage } from '@/api/alovaMethods/gibzRally';
import { useNotifications } from '@/composables/useNotifications';
import router from '@/plugins/router';

const props = defineProps({
  stageId: { type: String, required: true },
});

const { showSuccess, showError } = useNotifications();

const stageActivityForm = ref<HTMLFormElement | null>(null);
const isValidForm = ref(true);

const stageActivity = ref<StageActivity>(
  new StageActivity('', '', '', 100, StageActivityType.singleChoice, [])
);

const { data: stage } = useRequest(() => getStage(props.stageId));

const { send: saveStageActivity } = useRequest(
  (stageActivity: Partial<StageActivity>) => createStageActivity(props.stageId, stageActivity),
  { immediate: false }
);

async function submitStageActivity() {
  if (stageActivity.value.type === StageActivityType.qrCode) {
    stageActivity.value.answers = [];
  }
  const validationResult = await stageActivityForm.value?.validate();
  if (validationResult.valid) {
    try {
      await saveStageActivity(stageActivity.value);
      showSuccess('Aktivität wurde erstellt.');
      router.push({ name: 'Show stage', params: { stageId: props.stageId } });
    } catch (error) {
      showError(error, {
        notFound: 'Die Aktivität konnte nicht erstellt werden, da die Station nicht existiert.',
        forbidden: 'Keine Berechtigung zum Erstellen der Aktivität.',
        conflict: 'Die Aktivität kann mit den angegebenen Daten nicht erstellt werden.',
        fallback: 'Aktivität konnte nicht erstellt werden.',
      });
    }
  }
}
</script>
