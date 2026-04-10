<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Users</h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col
        cols="12"
        md="8"
      >
        <v-text-field
          v-model="search"
          label="Search by name, email, or username"
          prepend-inner-icon="mdi-magnify"
          variant="outlined"
          hide-details
          single-line
        />
      </v-col>
      <v-col
        cols="12"
        md="4"
      >
        <v-select
          v-model="selectedRoles"
          :items="availableRoles"
          item-title="displayName"
          item-value="id"
          label="Filter by roles"
          prepend-inner-icon="mdi-filter"
          variant="outlined"
          multiple
          chips
          closable-chips
          hide-details
          clearable
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-data-table-server
          v-model:items-per-page="itemsPerPage"
          :items="userData?.users"
          :items-length="userData?.total ?? 0"
          :headers="headers"
          :search="search"
          :loading="usersAreLoading"
          @update:options="handleTableUpdate"
        >
          <template #[`item.userRoles`]="{ item }">
            <v-chip
              v-for="role in item.userRoles"
              :key="role.id"
              size="small"
              class="ma-1"
            >
              {{ role.displayName }}
            </v-chip>
            <span v-if="!item.userRoles || item.userRoles.length === 0">-</span>
          </template>

          <template #[`item.actions`]="{ item }">
            <v-btn
              :to="{ name: 'Show User', params: { userId: item.id } }"
              size="x-small"
              icon="mdi-pencil"
              class="mr-2"
            />
            <v-btn
              size="x-small"
              icon="mdi-delete"
              color="error"
              @click="confirmDelete(item)"
            />
          </template>
        </v-data-table-server>
      </v-col>
    </v-row>

    <!-- Delete Confirmation Dialog -->
    <v-dialog
      v-model="deleteDialog"
      max-width="500"
    >
      <v-card>
        <v-card-title class="text-h5"> Delete User </v-card-title>
        <v-card-text>
          Are you sure you want to delete user
          <strong>{{ userToDelete?.displayName }}</strong> ({{ userToDelete?.email }})?
          <br />
          <br />
          This action cannot be undone.
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn @click="deleteDialog = false"> Cancel </v-btn>
          <v-btn
            color="error"
            @click="handleDelete"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { useRequest } from 'alova/client';
import { getUsers, getRoles, deleteUser } from '@/api/alovaMethods/idsrv';
import { useNotificationStore } from '@/stores/notification';
import type { ApplicationUser } from '@/models/idsrv/applicationUser';

const notification = useNotificationStore();

const {
  data: userData,
  loading: usersAreLoading,
  send: fetchUsers,
} = useRequest(
  ({
    page,
    itemsPerPage,
    search,
    roles,
  }: {
    page: number;
    itemsPerPage: number;
    search: string;
    roles?: string[];
  }) => getUsers(page, itemsPerPage, search, roles),
  { immediate: false }
);

const { data: availableRoles, send: loadRoles } = useRequest(() => getRoles(), {
  immediate: false,
});

const { send: sendDelete } = useRequest((userId: string) => deleteUser(userId), {
  immediate: false,
});

const itemsPerPage = ref(10);
const search = ref('');
const selectedRoles = ref<string[]>([]);
const deleteDialog = ref(false);
const userToDelete = ref<ApplicationUser | null>(null);

// Handle table updates and include selectedRoles
function handleTableUpdate(options: { page: number; itemsPerPage: number; search: string }) {
  fetchUsers({
    page: options.page,
    itemsPerPage: options.itemsPerPage,
    search: options.search,
    roles: selectedRoles.value,
  });
}

// Watch for changes in selectedRoles and trigger a refresh
watch(selectedRoles, () => {
  // Trigger table refresh by calling fetchUsers with current options
  if (userData.value) {
    fetchUsers({
      page: 1,
      itemsPerPage: itemsPerPage.value,
      search: search.value,
      roles: selectedRoles.value,
    });
  }
});

function confirmDelete(user: ApplicationUser) {
  userToDelete.value = user;
  deleteDialog.value = true;
}

async function handleDelete() {
  if (!userToDelete.value) return;

  try {
    await sendDelete(userToDelete.value.id);
    notification.showSuccess(`User ${userToDelete.value.displayName} deleted successfully`);
    deleteDialog.value = false;
    userToDelete.value = null;
    // Refresh the table
    handleTableUpdate({
      page: 1,
      itemsPerPage: itemsPerPage.value,
      search: search.value,
    });
  } catch {
    notification.showError('Failed to delete user');
  }
}

onMounted(() => {
  loadRoles();
});

const headers = [
  {
    title: 'ID',
    key: 'id',
    sortable: false,
  },
  {
    title: 'Name',
    key: 'displayName',
    sortable: false,
  },
  {
    title: 'Email',
    key: 'email',
    sortable: false,
  },
  {
    title: 'Roles',
    key: 'userRoles',
    sortable: false,
  },
  {
    title: 'Actions',
    key: 'actions',
    sortable: false,
  },
];
</script>
