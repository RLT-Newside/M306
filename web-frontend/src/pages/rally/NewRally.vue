<template>
  <v-container>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h3">Neue Rallye erstellen</h1>
      </v-col>
      <v-col cols="12">
        <p class="text-body-1">
          Der Titel der Rallye wird nirgends öffentlich sichtbar angezeigt. Er dient somit nur für
          die interne Erkennung und Unterscheidung unterschiedlicher Rallyes.
        </p>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-form
          ref="rallyTitleForm"
          v-model="formValidationModel"
        >
          <v-text-field
            v-model="title"
            label="Titel der Rallye"
            required
            :rules="[(v: string) => !!v || 'Der Titel der neuen Rallye darf nicht leer sein']"
          ></v-text-field>
        </v-form>
      </v-col>
    </v-row>

    <v-row justify="center">
      <v-col cols="auto">
        <v-btn :to="{ name: 'List rallies' }"> Abbrechen </v-btn>
      </v-col>
      <v-col cols="auto">
        <v-btn
          color="primary"
          @click="submitRally"
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
import { createRally } from '@/api/alovaMethods/gibzRally';
import { useNotifications } from '@/composables/useNotifications';
import router from '../../plugins/router';
import type { Rally } from '@/models/rally/rally';

const title = ref('');
const rallyTitleForm = ref<HTMLFormElement | null>(null);
const formValidationModel = ref(true);
const { showSuccess, showError } = useNotifications();

const { send: createNewRally } = useRequest((newRally: Partial<Rally>) => createRally(newRally), {
  immediate: false,
});

async function submitRally() {
  const validationResult = await rallyTitleForm.value?.validate();
  if (validationResult.valid) {
    try {
      const createdRally = await createNewRally({ title: title.value });
      showSuccess('Rallye wurde erstellt.');
      router.push({ name: 'Show rally', params: { rallyId: createdRally.id } });
    } catch (error) {
      showError(error, {
        notFound: 'Die Rallye konnte nicht erstellt werden, da ein Bezug fehlt.',
        forbidden: 'Keine Berechtigung zum Erstellen der Rallye.',
        conflict: 'Die Rallye kann mit den angegebenen Daten nicht erstellt werden.',
        fallback: 'Rallye konnte nicht erstellt werden.',
      });
    }
  }
}
</script>
