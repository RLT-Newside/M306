<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">{{ client?.clientName || 'Edit Client' }}</h1>
        <p class="text-body-2 text-medium-emphasis mt-2">
          {{ client?.clientId || clientId }}
        </p>
      </v-col>
    </v-row>

    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />

    <v-form
      v-else-if="client"
      ref="form"
    >
      <v-tabs
        v-model="activeTab"
        color="primary"
      >
        <v-tab value="basic">Basic</v-tab>
        <v-tab value="tokens">Token Configuration</v-tab>
        <v-tab value="ux">User Experience</v-tab>
        <v-tab value="logout">Logout</v-tab>
        <v-tab value="secrets">Secrets</v-tab>
        <v-tab value="advanced">Advanced</v-tab>
      </v-tabs>

      <v-window v-model="activeTab">
        <!-- Basic Tab -->
        <v-window-item value="basic">
          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Identity</p>
              <v-divider />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model="client.clientId"
                label="Client ID"
                disabled
                hint="Cannot be changed after creation"
                persistent-hint
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model="client.clientName"
                label="Client Name"
                :rules="[(v) => !!v || 'Client name is required']"
                required
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model="client.clientUri"
                label="Client URI"
                hint="URL of the client's home page"
                persistent-hint
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model="client.logoUri"
                label="Logo URI"
                hint="URL of the client's logo"
                persistent-hint
              />
            </v-col>

            <v-col cols="12">
              <v-textarea
                v-model="client.description"
                label="Description"
                rows="3"
              />
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Grant Types</p>
              <v-divider />
            </v-col>

            <v-col cols="12">
              <v-chip
                v-for="grantType in client.allowedGrantTypes"
                :key="grantType"
                class="ma-1"
                color="primary"
              >
                {{ getGrantTypeLabel(grantType) }}
              </v-chip>
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Security Settings</p>
              <v-divider />
            </v-col>

            <v-col cols="12">
              <v-switch
                v-model="client.requirePkce"
                color="primary"
                label="Require PKCE"
                hint="Proof Key for Code Exchange"
                persistent-hint
              />

              <v-switch
                v-model="client.allowPlainTextPkce"
                :disabled="!client.requirePkce"
                color="primary"
                label="Allow Plain Text PKCE"
                hint="Allow plain text code challenge (not recommended)"
                persistent-hint
                class="mt-2"
              />

              <v-switch
                v-model="client.requireClientSecret"
                color="primary"
                label="Require Client Secret"
                hint="Required for confidential clients"
                persistent-hint
                class="mt-2"
              />

              <v-switch
                v-model="client.allowOfflineAccess"
                color="primary"
                label="Allow Offline Access"
                hint="Allow refresh tokens"
                persistent-hint
                class="mt-2"
              />
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Allowed Scopes</p>
              <v-divider />
            </v-col>

            <v-col cols="12">
              <v-select
                v-model="client.allowedScopes"
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

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Redirect URLs</p>
              <v-divider />
            </v-col>

            <v-col cols="12">
              <MultiUriInput
                v-model="client.redirectUris"
                label="Redirect URI"
                hint="Where users are redirected after authentication"
                :clear-after-submit="true"
              />
            </v-col>

            <v-col cols="12">
              <MultiUriInput
                v-model="client.postLogoutRedirectUris"
                label="Post Logout Redirect URI"
                hint="Where users are redirected after logout"
                :clear-after-submit="true"
              />
            </v-col>

            <v-col cols="12">
              <MultiUriInput
                v-model="client.allowedCorsOrigins"
                label="Allowed CORS Origin"
                hint="Origins allowed to make cross-origin requests"
                :clear-after-submit="true"
              />
            </v-col>
          </v-row>
        </v-window-item>

        <!-- Token Configuration Tab -->
        <v-window-item value="tokens">
          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Token Lifetimes</p>
              <v-divider />
              <p class="text-caption text-medium-emphasis mt-2">All lifetimes are in seconds</p>
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model.number="client.identityTokenLifetime"
                label="Identity Token Lifetime"
                type="number"
                suffix="seconds"
                :hint="`${formatDuration(client.identityTokenLifetime)} (default: 5 minutes)`"
                persistent-hint
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model.number="client.accessTokenLifetime"
                label="Access Token Lifetime"
                type="number"
                suffix="seconds"
                :hint="`${formatDuration(client.accessTokenLifetime)} (default: 1 hour)`"
                persistent-hint
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model.number="client.authorizationCodeLifetime"
                label="Authorization Code Lifetime"
                type="number"
                suffix="seconds"
                :hint="`${formatDuration(client.authorizationCodeLifetime)} (default: 5 minutes)`"
                persistent-hint
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model.number="client.deviceCodeLifetime"
                label="Device Code Lifetime"
                type="number"
                suffix="seconds"
                :hint="`${formatDuration(client.deviceCodeLifetime)} (default: 5 minutes)`"
                persistent-hint
              />
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Refresh Token Settings</p>
              <v-divider />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model.number="client.absoluteRefreshTokenLifetime"
                label="Absolute Refresh Token Lifetime"
                type="number"
                suffix="seconds"
                :hint="`${formatDuration(client.absoluteRefreshTokenLifetime)} (default: 30 days)`"
                persistent-hint
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model.number="client.slidingRefreshTokenLifetime"
                label="Sliding Refresh Token Lifetime"
                type="number"
                suffix="seconds"
                :hint="`${formatDuration(client.slidingRefreshTokenLifetime)} (default: 15 days)`"
                persistent-hint
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-select
                v-model="client.refreshTokenUsage"
                :items="[
                  { title: 'Reusable', value: 0 },
                  { title: 'One Time Only', value: 1 },
                ]"
                label="Refresh Token Usage"
                hint="Whether refresh tokens can be reused"
                persistent-hint
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-select
                v-model="client.refreshTokenExpiration"
                :items="[
                  { title: 'Sliding', value: 0 },
                  { title: 'Absolute', value: 1 },
                ]"
                label="Refresh Token Expiration"
                hint="How refresh token expiration is calculated"
                persistent-hint
              />
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Access Token Type</p>
              <v-divider />
            </v-col>

            <v-col cols="12">
              <v-radio-group v-model="client.accessTokenType">
                <v-radio
                  :value="0"
                  label="JWT (JSON Web Token)"
                >
                  <template #label>
                    <div>
                      <div class="font-weight-medium">JWT (JSON Web Token)</div>
                      <div class="text-caption text-medium-emphasis">
                        Self-contained token with claims
                      </div>
                    </div>
                  </template>
                </v-radio>

                <v-radio
                  :value="1"
                  label="Reference Token"
                >
                  <template #label>
                    <div>
                      <div class="font-weight-medium">Reference Token</div>
                      <div class="text-caption text-medium-emphasis">
                        Opaque token requiring introspection endpoint
                      </div>
                    </div>
                  </template>
                </v-radio>
              </v-radio-group>
            </v-col>
          </v-row>
        </v-window-item>

        <!-- User Experience Tab -->
        <v-window-item value="ux">
          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Consent Screen</p>
              <v-divider />
            </v-col>

            <v-col cols="12">
              <v-switch
                v-model="client.requireConsent"
                color="primary"
                label="Require Consent"
                hint="Show consent screen to users"
                persistent-hint
              />

              <v-switch
                v-model="client.allowRememberConsent"
                :disabled="!client.requireConsent"
                color="primary"
                label="Allow Remember Consent"
                hint="Allow users to save their consent choice"
                persistent-hint
                class="mt-2"
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model.number="client.consentLifetime"
                :disabled="!client.requireConsent || !client.allowRememberConsent"
                label="Consent Lifetime"
                type="number"
                suffix="seconds"
                hint="Leave empty for no expiration"
                persistent-hint
                clearable
              />
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Branding</p>
              <v-divider />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model="client.clientUri"
                label="Client URI"
                hint="Link to client's home page"
                persistent-hint
              />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model="client.logoUri"
                label="Logo URI"
                hint="Logo displayed on consent screen"
                persistent-hint
              />
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Device Flow</p>
              <v-divider />
            </v-col>

            <v-col
              cols="12"
              md="6"
            >
              <v-text-field
                v-model="client.userCodeType"
                label="User Code Type"
                hint="Format for device flow user codes"
                persistent-hint
              />
            </v-col>
          </v-row>
        </v-window-item>

        <!-- Logout Tab -->
        <v-window-item value="logout">
          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Front Channel Logout</p>
              <v-divider />
              <p class="text-caption text-medium-emphasis mt-2">
                Front channel logout uses browser redirects
              </p>
            </v-col>

            <v-col cols="12">
              <v-text-field
                v-model="client.frontChannelLogoutUri"
                label="Front Channel Logout URI"
                hint="URI to call for front channel logout"
                persistent-hint
              />
            </v-col>

            <v-col cols="12">
              <v-switch
                v-model="client.frontChannelLogoutSessionRequired"
                color="primary"
                label="Session Required"
                hint="Include session ID in logout request"
                persistent-hint
              />
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Back Channel Logout</p>
              <v-divider />
              <p class="text-caption text-medium-emphasis mt-2">
                Back channel logout uses direct server-to-server communication
              </p>
            </v-col>

            <v-col cols="12">
              <v-text-field
                v-model="client.backChannelLogoutUri"
                label="Back Channel Logout URI"
                hint="URI to call for back channel logout"
                persistent-hint
              />
            </v-col>

            <v-col cols="12">
              <v-switch
                v-model="client.backChannelLogoutSessionRequired"
                color="primary"
                label="Session Required"
                hint="Include session ID in logout request"
                persistent-hint
              />
            </v-col>
          </v-row>
        </v-window-item>

        <!-- Secrets Tab -->
        <v-window-item value="secrets">
          <v-row class="mt-4">
            <v-col cols="12">
              <v-alert
                v-if="!client.requireClientSecret"
                type="info"
                variant="tonal"
                class="mb-4"
              >
                This client does not require a client secret. Enable "Require Client Secret" in
                Basic tab if you need to add secrets.
              </v-alert>

              <div class="d-flex justify-space-between align-center mb-2">
                <p class="text-h6">Client Secrets</p>
                <v-btn
                  v-if="client.requireClientSecret"
                  color="primary"
                  variant="outlined"
                  prepend-icon="mdi-key-plus"
                  @click="openGenerateSecretDialog"
                >
                  Generate New Secret
                </v-btn>
              </div>
              <v-divider />
            </v-col>

            <v-col cols="12">
              <v-progress-linear
                v-if="loadingSecrets"
                indeterminate
                color="primary"
              />

              <v-list v-else-if="secrets.length > 0">
                <v-list-item
                  v-for="(secret, index) in secrets"
                  :key="secret.hash"
                >
                  <template #prepend>
                    <v-icon>mdi-key</v-icon>
                  </template>

                  <v-list-item-title>
                    <span class="font-weight-medium">Secret {{ index + 1 }}</span>
                    <v-chip
                      v-if="secret.description"
                      class="ml-2"
                      size="small"
                      variant="tonal"
                    >
                      {{ secret.description }}
                    </v-chip>
                    <v-tooltip
                      v-if="secret.expiration"
                      location="top"
                    >
                      <template #activator="{ props }">
                        <v-chip
                          v-bind="props"
                          :color="isSecretExpired(secret.expiration) ? 'error' : 'warning'"
                          class="ml-2"
                          size="small"
                          variant="tonal"
                        >
                          {{ isSecretExpired(secret.expiration) ? 'Expired' : 'Expires' }}
                          {{ formatExpirationDate(secret.expiration) }}
                        </v-chip>
                      </template>
                      <span>{{ new Date(secret.expiration).toLocaleString() }}</span>
                    </v-tooltip>
                  </v-list-item-title>

                  <v-list-item-subtitle class="mt-1">
                    <div class="text-caption">
                      <span class="text-medium-emphasis">Hash:</span>
                      <code class="ml-1">{{ secret.hash }}</code>
                    </div>
                    <div class="text-caption mt-1">
                      <span class="text-medium-emphasis">Created:</span>
                      <span class="ml-1">{{ new Date(secret.created).toLocaleString() }}</span>
                    </div>
                    <div
                      v-if="secret.expiration"
                      class="text-caption mt-1"
                    >
                      <span class="text-medium-emphasis">Expiration:</span>
                      <span class="ml-1">{{ new Date(secret.expiration).toLocaleString() }}</span>
                    </div>
                  </v-list-item-subtitle>

                  <template #append>
                    <v-btn
                      icon="mdi-delete"
                      color="error"
                      variant="text"
                      :loading="deletingSecret === secret.hash"
                      @click="deleteSecret(secret.hash)"
                    />
                  </template>
                </v-list-item>
              </v-list>

              <v-alert
                v-else
                type="info"
                variant="tonal"
              >
                No secrets configured for this client.
              </v-alert>
            </v-col>
          </v-row>
        </v-window-item>

        <!-- Advanced Tab -->
        <v-window-item value="advanced">
          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Claims Configuration</p>
              <v-divider />
            </v-col>

            <v-col cols="12">
              <v-switch
                v-model="client.alwaysSendClientClaims"
                color="primary"
                label="Always Send Client Claims"
                hint="Include client claims in every token"
                persistent-hint
              />

              <v-switch
                v-model="client.alwaysIncludeUserClaimsInIdToken"
                color="primary"
                label="Always Include User Claims in ID Token"
                hint="Include all user claims in identity token"
                persistent-hint
                class="mt-2"
              />

              <v-switch
                v-model="client.updateAccessTokenClaimsOnRefresh"
                color="primary"
                label="Update Access Token Claims on Refresh"
                hint="Refresh claims when using refresh token"
                persistent-hint
                class="mt-2"
              />

              <v-switch
                v-model="client.includeJwtId"
                color="primary"
                label="Include JWT ID"
                hint="Include unique JWT identifier in tokens"
                persistent-hint
                class="mt-2"
              />
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Client Claims</p>
              <v-divider />
              <p class="text-caption text-medium-emphasis mt-2">
                Custom claims to include in tokens for this client
              </p>
            </v-col>

            <v-col cols="12">
              <v-row
                v-for="(claim, index) in client.claims"
                :key="index"
                class="mb-2"
              >
                <v-col
                  cols="12"
                  md="5"
                >
                  <v-text-field
                    v-model="claim.type"
                    label="Claim Type"
                    density="compact"
                  />
                </v-col>
                <v-col
                  cols="12"
                  md="5"
                >
                  <v-text-field
                    v-model="claim.value"
                    label="Claim Value"
                    density="compact"
                  />
                </v-col>
                <v-col
                  cols="12"
                  md="2"
                >
                  <v-btn
                    icon="mdi-delete"
                    color="error"
                    variant="text"
                    @click="removeClaim(index)"
                  />
                </v-col>
              </v-row>

              <v-btn
                variant="outlined"
                prepend-icon="mdi-plus"
                @click="addClaim"
              >
                Add Claim
              </v-btn>
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12">
              <p class="text-h6 mb-2">Authentication</p>
              <v-divider />
            </v-col>

            <v-col cols="12">
              <v-switch
                v-model="client.enableLocalLogin"
                color="primary"
                label="Enable Local Login"
                hint="Allow username/password login"
                persistent-hint
              />
            </v-col>
          </v-row>
        </v-window-item>
      </v-window>

      <!-- Actions -->
      <v-row class="mt-6">
        <v-col>
          <v-btn @click="router.push({ name: 'List Clients' })"> Cancel </v-btn>
          <v-btn
            color="primary"
            class="ml-2"
            :loading="saving"
            @click="save"
          >
            Save Changes
          </v-btn>
        </v-col>
      </v-row>
    </v-form>

    <!-- Secret Generation Dialog -->
    <v-dialog
      v-model="showSecretDialog"
      max-width="700"
      persistent
    >
      <v-card v-if="!newGeneratedSecret">
        <v-card-title>Generate New Client Secret</v-card-title>
        <v-card-text>
          <p class="mb-4">
            A new client secret will be generated. You will only be able to see the plain text
            secret once.
          </p>

          <v-text-field
            v-model="secretDescription"
            label="Description (Optional)"
            hint="E.g., 'Production server', 'Development environment'"
            persistent-hint
            class="mb-4"
          />

          <v-menu
            v-model="showDatePicker"
            :close-on-content-click="false"
            transition="scale-transition"
            offset-y
            min-width="auto"
          >
            <template #activator="{ props }">
              <v-text-field
                :model-value="formattedExpirationDate"
                label="Expiration Date (Optional)"
                hint="Leave empty for no expiration"
                persistent-hint
                readonly
                v-bind="props"
                clearable
                @click:clear="clearExpirationDate"
              >
                <template #prepend-inner>
                  <v-icon>mdi-calendar</v-icon>
                </template>
              </v-text-field>
            </template>
            <v-date-picker
              v-model="secretExpirationDate"
              :min="new Date().toISOString().split('T')[0]"
              @update:model-value="showDatePicker = false"
            />
          </v-menu>

          <v-alert
            type="warning"
            variant="tonal"
            class="mt-4"
          >
            Make sure to copy and save the secret securely before closing this dialog.
          </v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            variant="text"
            @click="closeSecretDialog"
          >
            Cancel
          </v-btn>
          <v-btn
            color="primary"
            @click="generateSecret"
          >
            Generate Secret
          </v-btn>
        </v-card-actions>
      </v-card>

      <v-card v-else>
        <v-card-title>Secret Generated Successfully</v-card-title>
        <v-card-text>
          <v-alert
            type="success"
            variant="tonal"
            class="mb-4"
          >
            Your new client secret has been generated. Copy it now as you won't be able to see it
            again.
          </v-alert>

          <v-text-field
            :model-value="newGeneratedSecret"
            label="Client Secret"
            readonly
            variant="outlined"
          >
            <template #append-inner>
              <v-btn
                icon="mdi-content-copy"
                variant="text"
                @click="copyToClipboard(newGeneratedSecret)"
              />
            </template>
          </v-text-field>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            color="primary"
            @click="closeSecretDialog"
          >
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import {
  getClient,
  updateClient,
  getApiScopes,
  getClientSecrets,
  generateClientSecret,
  deleteClientSecret,
} from '@/api/alovaMethods/idsrv';
import MultiUriInput from '@/components/MultiUriInput.vue';
import { Client } from '@/models/idsrv/client';
import { ApiScope } from '@/models/idsrv/apiScope';
import { useNotificationStore } from '@/stores/notification';

const router = useRouter();
const route = useRoute();
const notification = useNotificationStore();

const clientId = route.params.clientId as string;
const client = ref<Client | null>(null);
const apiScopes = ref<ApiScope[]>([]);
const loading = ref(true);
const saving = ref(false);
const activeTab = ref('basic');

// Secrets management
interface ClientSecret {
  hash: string;
  description?: string;
  created: string;
  expiration?: string;
}

const secrets = ref<ClientSecret[]>([]);
const loadingSecrets = ref(false);
const deletingSecret = ref<string | null>(null);
const showSecretDialog = ref(false);
const showDatePicker = ref(false);
const newGeneratedSecret = ref('');
const secretDescription = ref('');
const secretExpirationDate = ref<Date | null>(null);

const formattedExpirationDate = computed(() => {
  if (!secretExpirationDate.value) return '';
  return new Date(secretExpirationDate.value).toLocaleDateString();
});

const clearExpirationDate = () => {
  secretExpirationDate.value = null;
};

onMounted(async () => {
  try {
    [client.value, apiScopes.value] = await Promise.all([getClient(clientId), getApiScopes()]);

    // Ensure all array fields are initialized (defensive programming)
    if (client.value) {
      client.value.redirectUris = client.value.redirectUris || [];
      client.value.postLogoutRedirectUris = client.value.postLogoutRedirectUris || [];
      client.value.allowedCorsOrigins = client.value.allowedCorsOrigins || [];
      client.value.allowedScopes = client.value.allowedScopes || [];
      client.value.allowedGrantTypes = client.value.allowedGrantTypes || [];
      client.value.claims = client.value.claims || [];
    }

    // Load secrets if client requires them
    if (client.value?.requireClientSecret) {
      await loadSecrets();
    }
  } catch (error) {
    notification.showError('Failed to load client');
    console.error(error);
  } finally {
    loading.value = false;
  }
});

const loadSecrets = async () => {
  loadingSecrets.value = true;
  try {
    secrets.value = await getClientSecrets(clientId);
  } catch (error) {
    notification.showError('Failed to load client secrets');
    console.error(error);
  } finally {
    loadingSecrets.value = false;
  }
};

const openGenerateSecretDialog = () => {
  showSecretDialog.value = true;
};

const generateSecret = async () => {
  try {
    const payload: { description?: string; expiration?: string } = {};
    if (secretDescription.value) {
      payload.description = secretDescription.value;
    }
    if (secretExpirationDate.value) {
      // Convert to ISO 8601 datetime (set to end of day)
      const expirationDate = new Date(secretExpirationDate.value);
      expirationDate.setHours(23, 59, 59, 999);
      payload.expiration = expirationDate.toISOString();
    }

    const result = await generateClientSecret(clientId, payload);
    newGeneratedSecret.value = result.secret;
    await loadSecrets(); // Reload secrets list
    notification.showSuccess('New secret generated successfully');
  } catch (error) {
    notification.showError('Failed to generate secret');
    console.error(error);
  }
};

const closeSecretDialog = () => {
  showSecretDialog.value = false;
  newGeneratedSecret.value = '';
  secretDescription.value = '';
  secretExpirationDate.value = null;
};

const copyToClipboard = (text: string) => {
  navigator.clipboard.writeText(text);
  notification.showSuccess('Secret copied to clipboard');
};

const deleteSecret = async (secretHash: string) => {
  if (
    !confirm(
      'Are you sure you want to delete this secret? Applications using this secret will no longer be able to authenticate.'
    )
  ) {
    return;
  }

  deletingSecret.value = secretHash;
  try {
    await deleteClientSecret(clientId, secretHash);
    notification.showSuccess('Secret deleted successfully');
    await loadSecrets(); // Reload secrets list after successful deletion
  } catch (error) {
    notification.showError('Failed to delete secret');
    console.error(error);
  } finally {
    deletingSecret.value = null;
  }
};

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

const formatDuration = (seconds: number | undefined): string => {
  if (!seconds || seconds === 0) return '0 seconds';

  const days = Math.floor(seconds / 86400);
  const hours = Math.floor((seconds % 86400) / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const secs = seconds % 60;

  const parts: string[] = [];
  if (days > 0) parts.push(`${days} day${days !== 1 ? 's' : ''}`);
  if (hours > 0) parts.push(`${hours} hour${hours !== 1 ? 's' : ''}`);
  if (minutes > 0) parts.push(`${minutes} minute${minutes !== 1 ? 's' : ''}`);
  if (secs > 0 || parts.length === 0) parts.push(`${secs} second${secs !== 1 ? 's' : ''}`);

  return parts.join(', ');
};

const isSecretExpired = (expiration: string): boolean => {
  return new Date(expiration) < new Date();
};

const formatExpirationDate = (expiration: string): string => {
  const date = new Date(expiration);
  const now = new Date();

  if (date < now) {
    // Already expired - show how long ago
    const diff = Math.floor((now.getTime() - date.getTime()) / 1000);
    const days = Math.floor(diff / 86400);
    const hours = Math.floor((diff % 86400) / 3600);

    if (days > 0) return `${days} day${days !== 1 ? 's' : ''} ago`;
    if (hours > 0) return `${hours} hour${hours !== 1 ? 's' : ''} ago`;
    return 'recently';
  } else {
    // Future expiration - show relative time
    const diff = Math.floor((date.getTime() - now.getTime()) / 1000);
    const days = Math.floor(diff / 86400);
    const hours = Math.floor((diff % 86400) / 3600);

    if (days > 0) return `in ${days} day${days !== 1 ? 's' : ''}`;
    if (hours > 0) return `in ${hours} hour${hours !== 1 ? 's' : ''}`;
    return 'soon';
  }
};

const addClaim = () => {
  if (!client.value) return;
  if (!client.value.claims) {
    client.value.claims = [];
  }
  client.value.claims.push({ type: '', value: '' });
};

const removeClaim = (index: number) => {
  if (!client.value || !client.value.claims) return;
  client.value.claims.splice(index, 1);
};

const save = async () => {
  if (!client.value) return;

  // Validation
  if (!client.value.clientName) {
    notification.showError('Client name is required');
    return;
  }

  saving.value = true;
  try {
    await updateClient(clientId, client.value);
    notification.showSuccess('Client updated successfully');
    router.push({ name: 'List Clients' });
  } catch (error) {
    notification.showError('Failed to update client');
    console.error(error);
  } finally {
    saving.value = false;
  }
};
</script>
