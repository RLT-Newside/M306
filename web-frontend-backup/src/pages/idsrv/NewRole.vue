<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Neue Rolle erfassen</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-text-field
          v-model="name"
          label="Role Name (Technical)"
        />
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-text-field
          v-model="displayName"
          label="Display Name (Human Readable)"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-btn>Abbrechen</v-btn>
        <v-btn
          color="primary"
          @click="save"
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
import { useRouter } from 'vue-router';
import { useNotificationStore } from '@/stores/notification';
import { createRole } from '@/api/alovaMethods/idsrv';

const router = useRouter();
const notification = useNotificationStore();

const name = ref('');
const displayName = ref('');

const { send: submitRole } = useRequest(
  (roleName: string, roleDisplayName: string) =>
    createRole({ name: roleName, displayName: roleDisplayName }),
  { immediate: false }
);

async function save(): Promise<void> {
  try {
    await submitRole(name.value, displayName.value);
    notification.showSuccess('Role created successfully');
    router.push({ name: 'List Roles' });
  } catch {
    notification.showError('Failed to create role');
  }
}
</script>
