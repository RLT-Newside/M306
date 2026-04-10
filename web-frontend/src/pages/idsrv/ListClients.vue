<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Client Applications</h1>
      </v-col>
    </v-row>

    <v-row v-if="clientStore.createdClient !== null">
      <v-col>
        <v-alert
          closable
          title="New Client Created"
          type="success"
        >
          <p>A new client named {{ clientStore.createdClient.clientName }} was created.</p>
          <p>
            Please note the <span class="font-weight-bold">Client Secret</span>, since it won't be
            recoverable when this hint disapears: <br />
            <code class="font-weight-bold">{{ clientStore.createdClient.clientSecrets[0] }}</code>
          </p>
        </v-alert>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-expansion-panels>
          <v-expansion-panel
            v-for="client in clients"
            :key="client.clientId"
          >
            <v-expansion-panel-title>
              <div class="d-flex align-center justify-space-between w-100">
                <div>
                  <div class="text-h6">{{ client.clientName }}</div>
                  <div class="text-caption text-medium-emphasis">{{ client.clientId }}</div>
                </div>
                <div class="mr-4">
                  <v-chip
                    v-for="grantType in client.allowedGrantTypes"
                    :key="grantType"
                    size="small"
                    class="ma-1"
                  >
                    {{ grantType }}
                  </v-chip>
                </div>
              </div>
            </v-expansion-panel-title>

            <v-expansion-panel-text>
              <v-container>
                <v-row>
                  <v-col
                    cols="12"
                    md="6"
                  >
                    <p class="text-subtitle-2 mb-2">Client ID</p>
                    <p class="text-body-2 mb-4">{{ client.clientId }}</p>

                    <p class="text-subtitle-2 mb-2">Client Name</p>
                    <p class="text-body-2 mb-4">{{ client.clientName }}</p>

                    <p class="text-subtitle-2 mb-2">Grant Types</p>
                    <v-chip
                      v-for="grantType in client.allowedGrantTypes"
                      :key="grantType"
                      size="small"
                      class="ma-1"
                      color="primary"
                    >
                      {{ grantType }}
                    </v-chip>

                    <p class="text-subtitle-2 mb-2 mt-4">Allowed Scopes</p>
                    <v-chip
                      v-for="scope in client.allowedScopes"
                      :key="scope"
                      size="small"
                      class="ma-1"
                      color="secondary"
                    >
                      {{ scope }}
                    </v-chip>
                  </v-col>

                  <v-col
                    cols="12"
                    md="6"
                  >
                    <p class="text-subtitle-2 mb-2">Configuration</p>
                    <v-chip
                      v-if="client.requirePkce"
                      size="small"
                      class="ma-1"
                      color="success"
                    >
                      PKCE Required
                    </v-chip>
                    <v-chip
                      v-if="client.requireClientSecret"
                      size="small"
                      class="ma-1"
                      color="info"
                    >
                      Client Secret Required
                    </v-chip>
                  </v-col>
                </v-row>

                <v-divider class="my-4" />

                <v-row>
                  <v-col
                    cols="12"
                    md="6"
                  >
                    <p class="text-subtitle-2 mb-2">Redirect URIs</p>
                    <div v-if="client.redirectUris.length > 0">
                      <p
                        v-for="(uri, index) in client.redirectUris"
                        :key="index"
                        class="text-body-2 mb-1"
                      >
                        <v-icon
                          size="small"
                          class="mr-2"
                        >
                          mdi-link
                        </v-icon>
                        {{ uri }}
                      </p>
                    </div>
                    <p
                      v-else
                      class="text-body-2 text-medium-emphasis"
                    >
                      No redirect URIs configured
                    </p>

                    <p class="text-subtitle-2 mb-2 mt-4">Post Logout Redirect URIs</p>
                    <div v-if="client.postLogoutRedirectUris.length > 0">
                      <p
                        v-for="(uri, index) in client.postLogoutRedirectUris"
                        :key="index"
                        class="text-body-2 mb-1"
                      >
                        <v-icon
                          size="small"
                          class="mr-2"
                        >
                          mdi-link
                        </v-icon>
                        {{ uri }}
                      </p>
                    </div>
                    <p
                      v-else
                      class="text-body-2 text-medium-emphasis"
                    >
                      No post logout redirect URIs configured
                    </p>
                  </v-col>

                  <v-col
                    cols="12"
                    md="6"
                  >
                    <p class="text-subtitle-2 mb-2">Allowed CORS Origins</p>
                    <div v-if="client.allowedCorsOrigins.length > 0">
                      <p
                        v-for="(origin, index) in client.allowedCorsOrigins"
                        :key="index"
                        class="text-body-2 mb-1"
                      >
                        <v-icon
                          size="small"
                          class="mr-2"
                        >
                          mdi-web
                        </v-icon>
                        {{ origin }}
                      </p>
                    </div>
                    <p
                      v-else
                      class="text-body-2 text-medium-emphasis"
                    >
                      No CORS origins configured
                    </p>
                  </v-col>
                </v-row>

                <v-divider class="my-4" />

                <v-row>
                  <v-col class="text-right">
                    <v-btn
                      color="primary"
                      variant="outlined"
                      :to="{ name: 'Edit Client', params: { clientId: client.clientId } }"
                    >
                      <v-icon
                        start
                        icon="mdi-pencil"
                      />
                      Edit Client
                    </v-btn>
                  </v-col>
                </v-row>
              </v-container>
            </v-expansion-panel-text>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-col>
    </v-row>

    <v-row>
      <v-col class="text-center">
        <v-btn
          :to="{ name: 'New Client' }"
          color="primary"
        >
          Create Client
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRequest } from 'alova/client';
import { getClients } from '@/api/alovaMethods/idsrv';
import { ClientWithSecrets } from '@/models/idsrv/client';
import { useClientStore } from '@/stores/client';

defineProps({
  newClient: { type: ClientWithSecrets, required: false, default: undefined },
});

const clientStore = useClientStore();

const { data: clients, send: loadClients } = useRequest(() => getClients(), { immediate: false });

onMounted(() => {
  loadClients();
});
</script>
