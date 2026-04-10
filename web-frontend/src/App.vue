<template>
  <v-app>
    <v-app-bar v-if="!isPublicPage">
      <template
        v-if="authenticationStore.isAuthenticated || true"
        #prepend
      >
        <v-app-bar-nav-icon @click="navigationStore.toggleNavigation()"></v-app-bar-nav-icon>
      </template>
      <v-app-bar-title>
        <router-link :to="{ name: 'Home' }"> GIBZ App </router-link>
      </v-app-bar-title>
      <v-spacer />
      <v-btn
        v-if="!authenticationStore.isAuthenticated"
        @click="authenticationStore.login"
        >Login</v-btn
      >
      <v-btn
        v-else
        @click="authenticationStore.logout"
        >Logout</v-btn
      >
    </v-app-bar>
    <NavigationDrawer v-if="!isPublicPage" />
    <v-main>
      <RouterView />
    </v-main>

    <!-- Global notification snackbars -->
    <v-snackbar
      v-for="(notification, index) in notificationStore.notifications"
      :key="notification.id"
      :model-value="true"
      :color="getNotificationColor(notification.type)"
      :timeout="notification.timeout"
      location="top right"
      :class="'app-notification-snackbar'"
      :style="{ top: `${16 + index * 72}px` }"
      @update:model-value="notificationStore.remove(notification.id)"
    >
      {{ notification.message }}
      <template #actions>
        <v-btn
          icon="mdi-close"
          variant="text"
          @click="notificationStore.remove(notification.id)"
        />
      </template>
    </v-snackbar>
  </v-app>
</template>

<script setup lang="ts">
import NavigationDrawer from '@/components/NavigationDrawer.vue';
import { useAuthenticationStore } from './stores/authentication';
import { useNotificationStore } from './stores/notification';
import type { NotificationType } from './stores/notification';
import { computed, onMounted } from 'vue';
import { useNavigationStore } from './stores/navigation';
import { useRoute } from 'vue-router';

const route = useRoute();
const navigationStore = useNavigationStore();
const authenticationStore = useAuthenticationStore();
const notificationStore = useNotificationStore();

const isPublicPage = computed(() => route.meta.isPublicPage === true);

onMounted(() => {
  authenticationStore.loadUserData();
});

function getNotificationColor(type: NotificationType): string {
  const colors = {
    success: 'green',
    error: 'red',
    warning: 'orange',
    info: 'blue',
  };
  return colors[type];
}
</script>

<style lang="scss">
.v-toolbar-title a {
  text-decoration: none;
  color: inherit;
}

.v-container {
  max-width: 1200px;
}

.rally-table-surface {
  border: 1px solid rgba(15, 23, 42, 0.08);
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 10px 24px rgba(15, 23, 42, 0.06);
}

.rally-section-card {
  border: 1px solid rgba(15, 23, 42, 0.08);
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 10px 24px rgba(15, 23, 42, 0.06);
}

.rally-section-card .v-card-item {
  background: #ffffff;
  border-bottom: 1px solid rgba(15, 23, 42, 0.08);
  padding: 10px 16px;
}

.rally-section-card .v-card-title {
  font-size: 1rem;
  font-weight: 700;
  letter-spacing: normal;
  text-transform: none;
  color: rgba(30, 41, 59, 0.95);
}

.rally-section-card .v-card-subtitle {
  margin-top: 2px;
  font-size: 0.875rem;
  line-height: 1.35;
  color: rgba(51, 65, 85, 0.9);
}

.rally-section-card .v-card-text {
  padding-top: 14px;
}

.rally-table {
  --rally-header-bg: #ffffff;
}

.rally-table > .v-table__wrapper > table > thead > tr > th {
  background: var(--rally-header-bg);
  color: rgba(30, 41, 59, 0.95);
  font-size: 0.78rem;
  font-weight: 700;
  letter-spacing: 0.04em;
  text-transform: uppercase;
  border-bottom: 1px solid rgba(15, 23, 42, 0.08);
}

.rally-table > .v-table__wrapper > table > thead > tr > th,
.rally-table > .v-table__wrapper > table > tbody > tr > td {
  padding: 14px 16px;
}

.rally-table > .v-table__wrapper > table > tbody > tr:nth-child(even) {
  background-color: rgba(241, 245, 249, 0.45);
}

.rally-table > .v-table__wrapper > table > tbody > tr:hover > td {
  background-color: rgba(99, 102, 241, 0.08);
  transition: background-color 0.15s ease-in-out;
}

.rally-table > .v-table__wrapper > table > tbody > tr > td:last-child {
  white-space: nowrap;
  text-align: right;
}

.rally-table > .v-table__wrapper > table > tbody > tr > td:last-child .v-btn + .v-btn {
  margin-left: 4px;
}

.rally-list-icon-btn {
  width: 32px !important;
  height: 32px !important;
  min-width: 32px !important;
  border-radius: 8px !important;
  color: rgba(51, 65, 85, 0.85) !important;
}

.rally-list-icon-btn .v-icon {
  font-size: 18px;
}

.rally-list-icon-btn:hover {
  background-color: rgba(148, 163, 184, 0.14);
  color: rgba(30, 41, 59, 0.95) !important;
}

.app-notification-snackbar {
  margin: 0;
}
</style>
