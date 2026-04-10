<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">API Resources</h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-table fixed-header>
          <thead>
            <tr>
              <th class="text-left">Name</th>
              <th class="text-left">Display Name</th>
              <th class="text-left">Scopes</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="apiResource in apiResources"
              :key="apiResource.name"
            >
              <td>{{ apiResource.name }}</td>
              <td>{{ apiResource.displayName }}</td>
              <td>{{ apiResource.scopes?.join(', ') || '-' }}</td>
            </tr>
          </tbody>
        </v-table>
      </v-col>
    </v-row>

    <v-row>
      <v-col class="text-center">
        <v-btn
          :to="{ name: 'New API Resource' }"
          color="primary"
          >Create API Resource</v-btn
        >
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRequest } from 'alova/client';
import { getApiResources } from '@/api/alovaMethods/idsrv';

const { data: apiResources, send: loadApiResources } = useRequest(() => getApiResources(), {
  immediate: false,
});

onMounted(() => {
  loadApiResources();
});
</script>
