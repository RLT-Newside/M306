<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">{{ user?.displayName ?? 'Loading user...' }}</h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="6">
        <p class="text-subtitle-2">ID</p>
        <p class="text-body-1">{{ user?.id }}</p>
      </v-col>
      <v-col cols="6">
        <p class="text-subtitle-2">User Name</p>
        <p class="text-body-1">{{ user?.userName }}</p>
      </v-col>
      <v-col cols="6">
        <p class="text-subtitle-2">Display Name</p>
        <p class="text-body-1">{{ user?.displayName }}</p>
      </v-col>
      <v-col cols="6">
        <p class="text-subtitle-2">Email</p>
        <p class="text-body-1">{{ user?.email }}</p>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-select
          v-model="selected.roles"
          label="Select Roles"
          :items="roles"
          item-title="displayName"
          return-object
          multiple
          chips
          closable-chips
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col class="text-center">
        <v-btn
          color="primary"
          @click="save"
          >Save</v-btn
        >
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { useRequest } from 'alova/client';
import { useRouter } from 'vue-router';
import { useNotificationStore } from '@/stores/notification';
import { getRoles, getUser, updateUserRoles } from '@/api/alovaMethods/idsrv';
import type { RoleModificationRequest } from '@/models/idsrv/roleModificationRequest';
import type { IdentityRole } from '@/models/idsrv/identityRole';

const router = useRouter();
const notification = useNotificationStore();

const props = defineProps({
  userId: { type: String, required: true },
});

const {
  data: roles,
  send: loadRoles,
  onComplete: rolesLoaded,
} = useRequest(() => getRoles(), { immediate: false });

const {
  data: user,
  send: loadUser,
  onComplete: userLoaded,
} = useRequest((userId: string) => getUser(userId), { immediate: false });

const { send: sendUpdate } = useRequest(
  (userId: string, roleModificationRequest: RoleModificationRequest) =>
    updateUserRoles(userId, roleModificationRequest),
  { immediate: false }
);

const selected = reactive({
  roles: [] as IdentityRole[],
});

onMounted(() => {
  loadRoles();
});

rolesLoaded(() => {
  loadUser(props.userId);
});

userLoaded(() => {
  user.value.userRoles?.forEach((userRole) => {
    const role = roles.value.find((role) => role.id === userRole.id);
    if (role !== undefined) {
      selected.roles.push(role);
    }
  });
});

async function save() {
  let originalUserRoles: string[];
  if (user.value!.userRoles === null) {
    user.value!.userRoles = [];
    originalUserRoles = [];
  } else {
    originalUserRoles = user.value!.userRoles.map((role) => role.name) ?? [];
  }

  const newUserRoles = selected.roles.map((role) => role.name);

  // calculate roles to be added
  const rolesToBeAdded = newUserRoles.filter((role) => !originalUserRoles.includes(role));

  // calculate roles to be removed
  const rolesToBeRemoved = originalUserRoles.filter((role) => !newUserRoles.includes(role));

  const modificationRequest: RoleModificationRequest = {
    userId: user.value!.id,
    rolesToBeAdded,
    rolesToBeRemoved,
  };

  try {
    await sendUpdate(user.value!.id, modificationRequest);
    notification.showSuccess('User roles updated successfully');
    // Redirect back to users list after successful update
    router.push({ name: 'List Users' });
  } catch {
    notification.showError('Failed to update user roles');
  }
}
</script>
