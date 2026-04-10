<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Create API Resource</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-text-field
          v-model="newApiResource.name"
          label="Name"
        />
      </v-col>
      <v-col>
        <v-textarea
          v-model="newApiResource.displayName"
          label="Beschreibung"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-btn>Abbrechen</v-btn>
        <v-btn
          color="primary"
          @click="save"
          >Speichern</v-btn
        >
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { reactive } from 'vue';
import { useRequest } from 'alova/client';
import { createApiResource } from '@/api/alovaMethods/idsrv';
import router from '../../plugins/router';
import { ApiResource } from '@/models/idsrv/apiResource';

const newApiResource = reactive<ApiResource>(new ApiResource('', '', []));

const { send: submitApiResource, onComplete } = useRequest(
  (newApiResource: ApiResource) => createApiResource(newApiResource),
  { immediate: false }
);

function save(): void {
  submitApiResource(newApiResource);
}

onComplete(() => {
  router.push({ name: 'List API Resources' });
});
</script>
