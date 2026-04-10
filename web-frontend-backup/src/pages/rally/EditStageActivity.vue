<template>
  <v-container v-if="!stageActivityLoading">
    <v-row>
      <v-col>
        <h1 class="text-h3">{{ stageActivity.title }}</h1>
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

    <v-row>
      <v-col>
        <v-btn
          variant="text"
          :to="{ name: 'Show stage', params: { stageId: stageId } }"
          >abbrechen</v-btn
        >
        <v-btn
          type="submit"
          color="primary"
          variant="text"
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
import { useRequest } from 'alova/client';
import { getStageActivity, updateStageActivity } from '@/api/alovaMethods/gibzRally';
import StageActivityForm from '@/components/rally/StageActivityForm.vue';
import { StageActivity, StageActivityType } from '@/models/rally/stageActivity';
import { useNotifications } from '@/composables/useNotifications';
import router from '@/plugins/router';

const props = defineProps({
  stageId: { type: String, required: true },
  activityId: { type: String, required: true },
});

const { showSuccess, showError } = useNotifications();

const stageActivityForm = ref<HTMLFormElement | null>(null);
const isValidForm = ref(true);

const { data: stageActivity, loading: stageActivityLoading } = useRequest(
  getStageActivity(props.stageId, props.activityId)
);

const { send: saveStageActivity } = useRequest(
  (stageActivity: Partial<StageActivity>) =>
    updateStageActivity(props.stageId, props.activityId, stageActivity),
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
      showSuccess('Aktivität wurde gespeichert.');
      router.push({ name: 'Show stage', params: { stageId: props.stageId } });
    } catch (error) {
      showError(error, {
        notFound: 'Die Aktivität existiert nicht mehr.',
        forbidden: 'Keine Berechtigung zum Bearbeiten der Aktivität.',
        conflict: 'Die Aktivität kann mit den angegebenen Daten nicht gespeichert werden.',
        fallback: 'Aktivität konnte nicht gespeichert werden.',
      });
    }
  }
}
</script>
