<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Create Api Scope</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-text-field
          v-model="newApiScope.name"
          label="Name"
        />
      </v-col>
      <v-col>
        <v-text-field
          v-model="newApiScope.displayName"
          label="Anzeigename"
        />
      </v-col>
      <v-col>
        <v-textarea
          v-model="newApiScope.description"
          label="Beschreibung"
        />
      </v-col>
      <v-col>
        <v-checkbox
          v-for="apiResource in apiResources"
          :key="apiResource.name"
          v-model="newApiScope.apiResources"
          :label="apiResource.displayName"
          :value="apiResource.name"
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
import { reactive, onMounted } from 'vue';
import { useRequest } from 'alova/client';
import { createApiScope, getApiResources } from '@/api/alovaMethods/idsrv';
import router from '../../plugins/router';
import { ApiScopeCreationRequest } from '@/models/idsrv/apiScopeCreationRequest';

const newApiScope = reactive<ApiScopeCreationRequest>(new ApiScopeCreationRequest('', '', '', []));

const { data: apiResources, send: loadApiResources } = useRequest(() => getApiResources(), {
  immediate: false,
});

const { send: submitApiScope, onComplete } = useRequest(
  (newApiScope: ApiScopeCreationRequest) => createApiScope(newApiScope),
  { immediate: false }
);

onMounted(() => {
  loadApiResources();
});

function save(): void {
  submitApiScope(newApiScope);
}

onComplete(() => {
  router.push({ name: 'List API Scopes' });
});
</script>
