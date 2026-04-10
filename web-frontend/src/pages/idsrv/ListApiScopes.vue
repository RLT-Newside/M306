<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Api Scopes</h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-table fixed-header>
          <thead>
            <tr>
              <th class="text-left">Name</th>
              <th class="text-left">Display Name</th>
              <th class="text-left">Description</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="apiScope in scopes"
              :key="apiScope.name"
            >
              <td>{{ apiScope.name }}</td>
              <td>{{ apiScope.displayName }}</td>
              <td>{{ apiScope.description }}</td>
            </tr>
          </tbody>
        </v-table>
      </v-col>
    </v-row>

    <v-row>
      <v-col class="text-center">
        <v-btn
          :to="{ name: 'New API Scope' }"
          color="primary"
          >Create Api Scope</v-btn
        >
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRequest } from 'alova/client';
import { getApiScopes } from '@/api/alovaMethods/idsrv';

const { data: scopes, send: loadApiScopes } = useRequest(() => getApiScopes(), {
  immediate: false,
});

onMounted(() => {
  loadApiScopes();
});
</script>
