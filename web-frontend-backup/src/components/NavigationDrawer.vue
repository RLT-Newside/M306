<template>
  <v-navigation-drawer
    v-model="navigationStore.isNavigationDrawerOpen"
    permanent
  >
    <v-list nav>
      <v-list-item
        v-if="userRoles.includes('studentsManualEditor')"
        router-link
        :to="{ name: 'Students manual' }"
        title="Handbuch für Lernende"
        value="showManual"
      ></v-list-item>

      <v-list-group v-if="userRoles.includes('gibzRallyManager')">
        <template #activator="{ props }">
          <v-list-item
            v-bind="props"
            title="GIBZ Rallye"
            value="rally"
          />
        </template>

        <v-list-item
          router-link
          :to="{ name: 'List stages' }"
          title="Stationen"
          value="listStages"
        />

        <v-list-item
          router-link
          :to="{ name: 'List rallies' }"
          title="Rallyes"
          value="listRallies"
        />
      </v-list-group>

      <v-list-group v-if="userRoles.includes('idsrvAdmin')">
        <template #activator="{ props }">
          <v-list-item
            v-bind="props"
            title="Identity Server"
            value="idsrv"
          />
        </template>

        <v-list-item
          router-link
          :to="{ name: 'List Roles' }"
          title="Roles"
          value="idRoles"
        ></v-list-item>

        <v-list-item
          router-link
          :to="{ name: 'List Users' }"
          title="Users"
          value="idUsers"
        ></v-list-item>

        <v-list-item
          router-link
          :to="{ name: 'List Clients' }"
          title="Clients"
          value="idsrvClients"
        ></v-list-item>

        <v-list-item
          router-link
          :to="{ name: 'List API Resources' }"
          title="API Resources"
          value="idsrvApiResources"
        ></v-list-item>

        <v-list-item
          router-link
          :to="{ name: 'List API Scopes' }"
          title="API Scopes"
          value="idsrvApiScopes"
        ></v-list-item>
      </v-list-group>

      <v-list-group v-if="userRoles.includes('profilePictureReviewer')">
        <template #activator="{ props }">
          <v-list-item
            v-bind="props"
            title="Portraitfoto"
            value="profilePicture"
          />
        </template>

        <v-list-item
          router-link
          :to="{ name: 'Profile Picture Request Generation' }"
          title="Anfrage generieren"
          value="manualRequestGeneration"
        />

        <v-list-item
          router-link
          :to="{ name: 'Profile Picture Requests' }"
          title="Anfragen"
          value="profilePictureRequests"
        />

        <v-list-item
          router-link
          :to="{ name: 'Profile Picture Review' }"
          title="Review"
          value="profilePictureReview"
        />
      </v-list-group>

      <v-list-group v-if="userRoles.includes('pushNotificationDeveloper')">
        <template #activator="{ props }">
          <v-list-item
            v-bind="props"
            title="Push Notifications"
            value="pushNotifications"
          />
        </template>

        <v-list-item
          router-link
          :to="{ name: 'Push Notification Development Sender' }"
          title="Development Sender"
          value="sendToSelf"
        />
      </v-list-group>

      <v-list-group v-if="userRoles.includes('developer')">
        <template #activator="{ props }">
          <v-list-item
            v-bind="props"
            title="Developer"
            value="developer"
          />
        </template>

        <v-list-item
          router-link
          :to="{ name: 'Developer Tools' }"
          title="Tools"
          value="devTools"
        />
      </v-list-group>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { useAuthenticationStore } from '@/stores/authentication';
import { useNavigationStore } from '@/stores/navigation';
import { storeToRefs } from 'pinia';

const navigationStore = useNavigationStore();
const authenticationStore = useAuthenticationStore();

const { userRoles } = storeToRefs(authenticationStore);
</script>
