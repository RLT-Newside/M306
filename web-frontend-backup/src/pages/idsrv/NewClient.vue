<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Create New Client</h1>
        <p class="text-body-2 text-medium-emphasis mt-2">
          Create a new OAuth2/OIDC client application
        </p>
      </v-col>
    </v-row>

    <v-form ref="form">
      <!-- Basic Information -->
      <v-row>
        <v-col cols="12">
          <p class="text-h6 mb-2">Basic Information</p>
          <v-divider />
        </v-col>

        <v-col
          cols="12"
          md="6"
        >
          <v-text-field
            v-model="newClient.clientName"
            label="Client Name"
            hint="Human-readable name for the client"
            persistent-hint
            :rules="[(v) => !!v || 'Client name is required']"
            required
          />
        </v-col>
      </v-row>

      <!-- Application Type -->
      <v-row class="mt-4">
        <v-col cols="12">
          <p class="text-h6 mb-2">Application Type</p>
          <v-divider />
        </v-col>

        <v-col cols="12">
          <v-radio-group
            v-model="applicationType"
            @update:model-value="onApplicationTypeChange"
          >
            <v-radio
              value="spa"
              label="Single Page Application (SPA)"
            >
              <template #label>
                <div>
                  <div class="font-weight-medium">Single Page Application (SPA)</div>
                  <div class="text-caption text-medium-emphasis">
                    JavaScript app running in browser (React, Vue, Angular)
                  </div>
                </div>
              </template>
            </v-radio>

            <v-radio
              value="web"
              label="Web Application"
            >
              <template #label>
                <div>
                  <div class="font-weight-medium">Web Application</div>
                  <div class="text-caption text-medium-emphasis">
                    Server-side web app with backend (ASP.NET, Node.js, PHP)
                  </div>
                </div>
              </template>
            </v-radio>

            <v-radio
              value="native"
              label="Native/Mobile Application"
            >
              <template #label>
                <div>
                  <div class="font-weight-medium">Native/Mobile Application</div>
                  <div class="text-caption text-medium-emphasis">
                    Desktop or mobile app (iOS, Android, Electron)
                  </div>
                </div>
              </template>
            </v-radio>

            <v-radio
              value="machine"
              label="Machine-to-Machine"
            >
              <template #label>
                <div>
                  <div class="font-weight-medium">Machine-to-Machine (M2M)</div>
                  <div class="text-caption text-medium-emphasis">
                    CLI, daemon, or backend service
                  </div>
                </div>
              </template>
            </v-radio>
          </v-radio-group>
        </v-col>
      </v-row>

      <!-- Security Settings -->
      <v-row class="mt-4">
        <v-col cols="12">
          <p class="text-h6 mb-2">Security Settings</p>
          <v-divider />
        </v-col>

        <v-col cols="12">
          <v-switch
            v-model="newClient.requirePkce"
            :disabled="!supportsInteractiveFlow"
            color="primary"
            label="Require PKCE"
            hint="Proof Key for Code Exchange - Recommended for public clients"
            persistent-hint
          />

          <v-switch
            v-model="newClient.requireClientSecret"
            :disabled="applicationType === 'spa' || applicationType === 'native'"
            color="primary"
            label="Require Client Secret"
            hint="Required for confidential clients with secure backend"
            persistent-hint
            class="mt-4"
          />
        </v-col>
      </v-row>

      <!-- Grant Types (Auto-configured, read-only display) -->
      <v-row class="mt-4">
        <v-col cols="12">
          <p class="text-h6 mb-2">Configured Grant Types</p>
          <v-divider />
        </v-col>

        <v-col cols="12">
          <v-chip
            v-for="grantType in newClient.allowedGrantTypes"
            :key="grantType"
            class="ma-1"
            color="primary"
          >
            {{ getGrantTypeLabel(grantType) }}
          </v-chip>
          <p
            v-if="newClient.allowedGrantTypes.length === 0"
            class="text-body-2 text-medium-emphasis"
          >
            Select an application type to configure grant types
          </p>
        </v-col>
      </v-row>

      <!-- Allowed Scopes -->
      <v-row class="mt-4">
        <v-col cols="12">
          <p class="text-h6 mb-2">Allowed Scopes</p>
          <v-divider />
        </v-col>

        <v-col cols="12">
          <v-select
            v-model="newClient.allowedScopes"
            :items="apiScopes"
            item-title="displayName"
            item-value="name"
            label="Select allowed scopes"
            multiple
            chips
            closable-chips
          />
        </v-col>
      </v-row>

      <!-- URLs -->
      <v-row
        v-if="supportsInteractiveFlow"
        class="mt-4"
      >
        <v-col cols="12">
          <p class="text-h6 mb-2">Redirect URLs</p>
          <v-divider />
        </v-col>

        <v-col cols="12">
          <MultiUriInput
            v-model="newClient.redirectUris"
            label="Redirect URI"
            hint="Where users are redirected after authentication"
            :clear-after-submit="true"
          />
        </v-col>

        <v-col cols="12">
          <MultiUriInput
            v-model="newClient.postLogoutRedirectUris"
            label="Post Logout Redirect URI"
            hint="Where users are redirected after logout"
            :clear-after-submit="true"
          />
        </v-col>
      </v-row>

      <!-- CORS Origins (only for browser-based apps) -->
      <v-row
        v-if="applicationType === 'spa'"
        class="mt-4"
      >
        <v-col cols="12">
          <p class="text-h6 mb-2">CORS Configuration</p>
          <v-divider />
        </v-col>

        <v-col cols="12">
          <MultiUriInput
            v-model="newClient.allowedCorsOrigins"
            label="Allowed CORS Origin"
            hint="Origins allowed to make cross-origin requests"
            :clear-after-submit="true"
          />
        </v-col>
      </v-row>

      <!-- Actions -->
      <v-row class="mt-6">
        <v-col>
          <v-btn @click="router.push({ name: 'List Clients' })"> Cancel </v-btn>
          <v-btn
            color="primary"
            class="ml-2"
            @click="save"
          >
            Create Client
          </v-btn>
        </v-col>
      </v-row>
    </v-form>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { createClient, getApiScopes } from '@/api/alovaMethods/idsrv';
import MultiUriInput from '@/components/MultiUriInput.vue';
import { Client } from '@/models/idsrv/client';
import { ApiScope } from '@/models/idsrv/apiScope';
import { useNotificationStore } from '@/stores/notification';
import { useClientStore } from '@/stores/client';

const router = useRouter();
const notification = useNotificationStore();
const clientStore = useClientStore();

const newClient = reactive(new Client('', '', [], [], [], [], [], false, true));
const apiScopes = ref<ApiScope[]>([]);

// Application type controls which OAuth2 flows are configured
const applicationType = ref<'spa' | 'web' | 'native' | 'machine' | ''>('');

// Computed property to check if the app supports interactive flows (user login)
const supportsInteractiveFlow = computed(() => {
  return applicationType.value !== 'machine';
});

onMounted(async () => {
  try {
    apiScopes.value = await getApiScopes();
  } catch (error) {
    notification.showError('Failed to load API scopes');
    console.error(error);
  }
});

// Handle application type changes and auto-configure appropriate settings
const onApplicationTypeChange = (type: string | null) => {
  if (!type) return;

  // Clear previous configuration
  newClient.allowedGrantTypes = [];
  newClient.requirePkce = false;
  newClient.requireClientSecret = false;
  newClient.redirectUris = [];
  newClient.postLogoutRedirectUris = [];
  newClient.allowedCorsOrigins = [];

  switch (type) {
    case 'spa':
      // Single Page Application: Authorization Code + PKCE
      newClient.allowedGrantTypes = ['authorization_code'];
      newClient.requirePkce = true; // PKCE is mandatory for public clients
      newClient.requireClientSecret = false; // SPAs cannot securely store secrets
      break;

    case 'web':
      // Web Application: Authorization Code with optional client secret
      newClient.allowedGrantTypes = ['authorization_code'];
      newClient.requirePkce = false; // Optional but recommended
      newClient.requireClientSecret = true; // Server-side apps can use secrets
      break;

    case 'native':
      // Native/Mobile: Authorization Code + PKCE
      newClient.allowedGrantTypes = ['authorization_code'];
      newClient.requirePkce = true; // PKCE is mandatory for native apps
      newClient.requireClientSecret = false; // Native apps cannot securely store secrets
      break;

    case 'machine':
      // Machine-to-Machine: Client Credentials only
      newClient.allowedGrantTypes = ['client_credentials'];
      newClient.requirePkce = false; // Not applicable
      newClient.requireClientSecret = true; // M2M always requires secret
      break;
  }
};

// Map grant type values to human-readable labels
const getGrantTypeLabel = (grantType: string): string => {
  const labels: Record<string, string> = {
    implicit: 'Implicit (Deprecated)',
    hybrid: 'Hybrid',
    authorization_code: 'Authorization Code',
    client_credentials: 'Client Credentials',
    password: 'Resource Owner Password (Deprecated)',
    device_code: 'Device Code',
  };
  return labels[grantType] || grantType;
};

const save = async () => {
  // Validation
  if (!newClient.clientName) {
    notification.showError('Client name is required');
    return;
  }

  if (newClient.allowedGrantTypes.length === 0) {
    notification.showError('Please select an application type');
    return;
  }

  if (supportsInteractiveFlow.value && newClient.redirectUris.length === 0) {
    notification.showError('At least one redirect URI is required for interactive flows');
    return;
  }

  if (newClient.allowedScopes.length === 0) {
    notification.showError('At least one scope must be selected');
    return;
  }

  try {
    // Generate URL-safe client ID from name
    // 1. Convert to lowercase
    // 2. Replace spaces and special chars with hyphens
    // 3. Remove consecutive hyphens
    // 4. Trim hyphens from start/end
    newClient.clientId = newClient.clientName
      .toLowerCase()
      .replace(/[^a-z0-9]+/g, '-')
      .replace(/-+/g, '-')
      .replace(/^-|-$/g, '');

    const createdClient = await createClient(newClient);
    notification.showSuccess('Client created successfully');

    // Store the created client with secrets in the store for display
    clientStore.createdClient = createdClient;

    // Redirect to edit page for further configuration
    router.push({ name: 'Edit Client', params: { clientId: createdClient.clientId } });
  } catch (error) {
    notification.showError('Failed to create client');
    console.error(error);
  }
};
</script>
