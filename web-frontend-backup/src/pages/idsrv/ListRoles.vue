<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">User Roles</h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-table fixed-header>
          <thead>
            <tr>
              <th class="text-left">ID</th>
              <th class="text-left">Name</th>
              <th class="text-left">Display Name</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="role in roles"
              :key="role.id"
            >
              <td>{{ role.id }}</td>
              <td>{{ role.name }}</td>
              <td>{{ role.displayName }}</td>
            </tr>
          </tbody>
        </v-table>
      </v-col>
    </v-row>

    <v-row>
      <v-col class="text-center">
        <v-btn
          :to="{ name: 'New Role' }"
          color="primary"
          >Create Role</v-btn
        >
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRequest } from 'alova/client';
import { getRoles } from '@/api/alovaMethods/idsrv';

const { data: roles, send: loadRoles } = useRequest(() => getRoles(), { immediate: false });

onMounted(() => {
  loadRoles();
});
</script>
