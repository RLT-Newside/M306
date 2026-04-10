<template>
  <v-container>
    <PageTitle title="Developer Tools" />

    <v-card class="mt-4">
      <v-card-title>JWT Tokens</v-card-title>
      <v-card-subtitle> View and copy your authentication tokens for API testing </v-card-subtitle>

      <v-card-text>
        <div class="d-flex justify-center mb-6">
          <v-btn
            color="primary"
            :loading="loading"
            :disabled="loading"
            size="large"
            prepend-icon="mdi-reload"
            @click="loadTokens"
          >
            Load Tokens
          </v-btn>
        </div>

        <div
          v-if="error"
          class="mb-4"
        >
          <v-alert
            type="error"
            variant="tonal"
          >
            <strong>Error:</strong> {{ errorMessage }}
          </v-alert>
        </div>

        <div v-if="tokens">
          <!-- Access Token -->
          <v-card
            v-if="tokens.accessToken"
            variant="outlined"
            class="mb-4"
          >
            <v-card-title class="d-flex justify-space-between align-center">
              <span>Access Token</span>
              <div>
                <v-btn
                  size="small"
                  variant="text"
                  :prepend-icon="showDecoded.access ? 'mdi-eye-off' : 'mdi-eye'"
                  @click="toggleDecoded('access')"
                >
                  {{ showDecoded.access ? 'Hide' : 'Decode' }}
                </v-btn>
                <v-btn
                  size="small"
                  variant="text"
                  prepend-icon="mdi-content-copy"
                  @click="copyToClipboard(tokens.accessToken, 'Access Token')"
                >
                  Copy
                </v-btn>
              </div>
            </v-card-title>
            <v-card-text>
              <div class="token-display">
                {{ tokens.accessToken }}
              </div>
              <v-expand-transition>
                <div
                  v-if="showDecoded.access"
                  class="mt-3"
                >
                  <v-divider class="mb-3"></v-divider>
                  <div class="text-subtitle-2 mb-2">Decoded Payload:</div>
                  <pre class="decoded-token">{{ decodeToken(tokens.accessToken) }}</pre>
                </div>
              </v-expand-transition>
            </v-card-text>
          </v-card>

          <!-- ID Token -->
          <v-card
            v-if="tokens.idToken"
            variant="outlined"
            class="mb-4"
          >
            <v-card-title class="d-flex justify-space-between align-center">
              <span>ID Token</span>
              <div>
                <v-btn
                  size="small"
                  variant="text"
                  :prepend-icon="showDecoded.id ? 'mdi-eye-off' : 'mdi-eye'"
                  @click="toggleDecoded('id')"
                >
                  {{ showDecoded.id ? 'Hide' : 'Decode' }}
                </v-btn>
                <v-btn
                  size="small"
                  variant="text"
                  prepend-icon="mdi-content-copy"
                  @click="copyToClipboard(tokens.idToken, 'ID Token')"
                >
                  Copy
                </v-btn>
              </div>
            </v-card-title>
            <v-card-text>
              <div class="token-display">
                {{ tokens.idToken }}
              </div>
              <v-expand-transition>
                <div
                  v-if="showDecoded.id"
                  class="mt-3"
                >
                  <v-divider class="mb-3"></v-divider>
                  <div class="text-subtitle-2 mb-2">Decoded Payload:</div>
                  <pre class="decoded-token">{{ decodeToken(tokens.idToken) }}</pre>
                </div>
              </v-expand-transition>
            </v-card-text>
          </v-card>

          <!-- Refresh Token -->
          <v-card
            v-if="tokens.refreshToken"
            variant="outlined"
            class="mb-4"
          >
            <v-card-title class="d-flex justify-space-between align-center">
              <span>Refresh Token</span>
              <v-btn
                size="small"
                variant="text"
                prepend-icon="mdi-content-copy"
                @click="copyToClipboard(tokens.refreshToken, 'Refresh Token')"
              >
                Copy
              </v-btn>
            </v-card-title>
            <v-card-text>
              <div class="token-display">
                {{ tokens.refreshToken }}
              </div>
            </v-card-text>
          </v-card>

          <v-card
            v-if="!tokens.accessToken && !tokens.idToken && !tokens.refreshToken"
            variant="outlined"
          >
            <v-card-text>
              <v-alert
                type="info"
                variant="tonal"
              >
                No tokens available. This might be because the backend doesn't expose tokens or the
                /bffw/dev-tokens endpoint is not available.
              </v-alert>
            </v-card-text>
          </v-card>
        </div>

        <v-expansion-panels class="mt-6">
          <v-expansion-panel>
            <v-expansion-panel-title>
              <v-icon class="mr-3">mdi-shield-check</v-icon>
              <strong>Token Validation Information</strong>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <div class="py-2">
                <v-alert
                  type="info"
                  variant="tonal"
                  density="compact"
                  class="mb-4"
                >
                  <div class="mb-2">
                    <strong>Usage:</strong> Copy the access token and use it in your API requests
                    with the <code>Authorization: Bearer &lt;token&gt;</code> header.
                  </div>
                  <div>
                    <strong>Security:</strong> These tokens are sensitive. Never share them or
                    commit them to version control.
                  </div>
                </v-alert>

                <v-divider class="my-4"></v-divider>

                <h3 class="text-h6 mb-3">OpenID Connect Discovery</h3>
                <p class="mb-2">
                  The identity provider exposes its configuration at the well-known endpoint:
                </p>
                <div class="endpoint-link mb-4">
                  <a
                    href="https://auth.gibz-app.ch/.well-known/openid-configuration"
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    https://auth.gibz-app.ch/.well-known/openid-configuration
                    <v-icon
                      size="small"
                      class="ml-1"
                      >mdi-open-in-new</v-icon
                    >
                  </a>
                </div>

                <h3 class="text-h6 mb-3">JWKS (JSON Web Key Set)</h3>
                <p class="mb-2">Public keys for token signature verification can be found at:</p>
                <div class="endpoint-link mb-4">
                  <a
                    href="https://auth.gibz-app.ch/.well-known/openid-configuration/jwks"
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    https://auth.gibz-app.ch/.well-known/openid-configuration/jwks
                    <v-icon
                      size="small"
                      class="ml-1"
                      >mdi-open-in-new</v-icon
                    >
                  </a>
                </div>

                <h3 class="text-h6 mb-3">Token Validation</h3>
                <p class="mb-2">When validating tokens, verify:</p>
                <ul class="validation-list mb-3">
                  <li><strong>Signature:</strong> Use the public keys from JWKS endpoint</li>
                  <li>
                    <strong>Issuer (iss):</strong> Should be <code>https://auth.gibz-app.ch</code>
                  </li>
                  <li>
                    <strong>Audience (aud):</strong> Should match your API resource identifier
                  </li>
                  <li><strong>Expiration (exp):</strong> Token must not be expired</li>
                  <li><strong>Not Before (nbf):</strong> Current time must be after this value</li>
                </ul>

                <v-alert
                  type="warning"
                  variant="tonal"
                  density="compact"
                >
                  <strong>Note:</strong> Always validate tokens server-side. Never trust
                  client-provided tokens without verification.
                </v-alert>
              </div>
            </v-expansion-panel-text>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-card-text>
    </v-card>

    <v-snackbar
      v-model="snackbar"
      :timeout="snackbarTimeout"
      :color="snackbarColor"
      location="bottom"
      :multi-line="snackbarMultiLine"
      :min-width="snackbarMultiLine ? '500px' : undefined"
    >
      <div class="d-flex align-center">
        <span>{{ snackbarMessage }}</span>
      </div>
      <template #actions>
        <v-btn
          variant="text"
          @click="snackbar = false"
        >
          Close
        </v-btn>
      </template>
    </v-snackbar>
  </v-container>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useRequest } from 'alova/client';
import { getDevTokens } from '@/api/alovaMethods/devTools';
import PageTitle from '@/components/PageTitle.vue';

interface TokenResponse {
  accessToken?: string;
  idToken?: string;
  refreshToken?: string;
  AccessToken?: string;
  IdToken?: string;
  RefreshToken?: string;
}

const tokens = ref<{
  accessToken?: string;
  idToken?: string;
  refreshToken?: string;
} | null>(null);

const snackbar = ref(false);
const snackbarMessage = ref('');
const snackbarColor = ref('success');
const snackbarTimeout = ref(3000);
const snackbarMultiLine = ref(false);
const errorMessage = ref('');
const showDecoded = ref({
  access: false,
  id: false,
});

const {
  loading,
  error,
  data,
  send: loadTokens,
} = useRequest(getDevTokens, {
  immediate: false,
});

// Watch for data changes and map to tokens
watch(data, (newData) => {
  if (newData) {
    const response = newData as TokenResponse;
    tokens.value = {
      accessToken: response.accessToken || response.AccessToken,
      idToken: response.idToken || response.IdToken,
      refreshToken: response.refreshToken || response.RefreshToken,
    };

    // Show warning toast (no auto-dismiss)
    snackbarColor.value = 'warning';
    snackbarMessage.value =
      '⚠️ Security Warning: These tokens are sensitive credentials. Never share them or commit them to version control!';
    snackbarTimeout.value = -1; // No auto-dismiss
    snackbarMultiLine.value = true;
    snackbar.value = true;
  }
});

const toggleDecoded = (tokenType: 'access' | 'id') => {
  showDecoded.value[tokenType] = !showDecoded.value[tokenType];
};

const copyToClipboard = async (text: string | undefined, label: string) => {
  if (!text) return;

  try {
    await navigator.clipboard.writeText(text);
    snackbarColor.value = 'success';
    snackbarMessage.value = `${label} copied to clipboard!`;
    snackbarTimeout.value = 2000; // Auto-dismiss for success
    snackbarMultiLine.value = false;
    snackbar.value = true;
  } catch {
    snackbarColor.value = 'error';
    snackbarMessage.value = 'Failed to copy to clipboard';
    snackbarTimeout.value = 3000;
    snackbarMultiLine.value = false;
    snackbar.value = true;
  }
};

const decodeToken = (token: string | undefined): string => {
  if (!token) return '';

  try {
    const parts = token.split('.');
    if (parts.length !== 3) return 'Invalid token format';

    const part = parts[1];
    if (!part) return 'Invalid token format';

    const payload = JSON.parse(atob(part));
    return JSON.stringify(payload, null, 2);
  } catch {
    return 'Failed to decode token';
  }
};
</script>

<style scoped>
.token-display {
  font-family: 'Courier New', monospace;
  font-size: 0.875rem;
  padding: 12px;
  background-color: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
  word-break: break-all;
  line-height: 1.5;
}

.decoded-token {
  font-family: 'Courier New', monospace;
  font-size: 0.875rem;
  padding: 12px;
  background-color: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
  overflow-x: auto;
  white-space: pre-wrap;
  word-wrap: break-word;
  margin: 0;
}

code {
  background-color: rgba(0, 0, 0, 0.05);
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 0.875em;
}

.endpoint-link {
  font-family: 'Courier New', monospace;
  font-size: 0.875rem;
  padding: 8px 12px;
  background-color: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
  display: inline-flex;
  align-items: center;
}

.endpoint-link a {
  text-decoration: none;
  color: #1976d2;
  word-break: break-all;
}

.endpoint-link a:hover {
  text-decoration: underline;
}

.validation-list {
  padding-left: 24px;
  margin-left: 0;
}

.validation-list li {
  margin-bottom: 8px;
}
</style>
