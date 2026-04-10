<template>
  <v-container>
    <PageTitle title="Push Notification: Development Sender" />

    <v-row>
      <v-col>
        <p class="text-body-1">
          On this page, developers may send push notification for testing purposes. Be aware, the
          test messages will actually be sent to the selected recipients.
        </p>
      </v-col>
    </v-row>

    <v-form
      ref="messageEditForm"
      @submit.prevent="submitMessage"
    >
      <v-row>
        <v-col cols="12">
          <v-text-field
            v-model="notificationRequest.userId"
            label="Username of recipient"
            required
            :rules="[rules.required, rules.userId]"
          />
        </v-col>
        <v-col cols="12">
          <v-text-field
            v-model="notificationRequest.title"
            label="Title of message"
            required
            :rules="[rules.required]"
          />
          <v-textarea
            v-model="notificationRequest.body"
            label="Message"
            rows="4"
            required
            :rules="[rules.required]"
          />
        </v-col>
        <v-col cols="12">
          <p class="text-body-1">
            Use the JSON editor below to pass additional data to the push notification service.
          </p>

          <JsonEditorVue
            v-model="notificationRequest.data"
            :mode="Mode.text"
            :main-menu-bar="true"
            :ask-to-format="true"
            :status-bar="true"
          />
        </v-col>
        <v-col cols="12">
          <v-text-field
            v-model="endpoint"
            label="Push Notification API Endpoint"
            required
            :rules="[rules.required]"
          />
        </v-col>
      </v-row>
      <v-row justify="center">
        <v-col cols="auto">
          <v-btn
            type="submit"
            color="primary"
            prepend-icon="mdi-send"
            :disabled="!isFormValid"
            >Send Notification</v-btn
          >
        </v-col>
      </v-row>
    </v-form>

    <v-snackbar
      v-model="snackbar"
      location="top center"
      :color="snackbarColor"
    >
      {{ snackbarText }}
      <template #actions>
        <v-btn
          variant="text"
          @click="snackbar = false"
        >
          Schliessen
        </v-btn>
      </template>
    </v-snackbar>
  </v-container>
</template>
<script setup lang="ts">
import PageTitle from '@/components/PageTitle.vue';
import { useRequest } from 'alova/client';
import { onMounted, reactive, ref } from 'vue';
import JsonEditorVue from 'json-editor-vue';
import { Mode } from 'vanilla-jsoneditor';
import { useAuthenticationStore } from '@/stores/authentication';
import { storeToRefs } from 'pinia';
import { NotificationRequest } from '@/models/pushNotification/notificationRequest';
import { sendPushNotificationRequest } from '@/api/alovaMethods/pushNotification';

const auth = useAuthenticationStore();
const { userData, isAuthenticated, userRoles } = storeToRefs(auth);
// methods are not refs; take them directly from the store
const { loadUserData } = auth;

const notificationRequest = reactive(new NotificationRequest('', '', '', {}));
const endpoint = ref('');

const personForm = ref<HTMLFormElement | null>(null);
const isFormValid = ref(true);

const snackbar = ref(false);
const snackbarText = ref('');
const snackbarColor = ref('green');

const { send: submitMessage, loading: submissionInProgress } = sendPushNotificationRequest(
  endpoint.value,
  notificationRequest
);

onMounted(async () => {
  // your store sets { immediate: false } so you need to load once
  if (!userData.value) await loadUserData();

  // Preset email if available
  if (userData.value) {
    const emailClaim = userData.value.find((claim) => claim.type === 'upn');
    if (emailClaim) {
      notificationRequest.userId = emailClaim.value;
    }
  }
});

const rules = {
  required: (value: string) => !!value || 'Das Feld darf nicht leer sein',
  gib: (value: string) => {
    const gibPattern = /(\bGIB\d{6})\b/;
    return gibPattern.test(value) || 'Geben Sie eine gültige GIB-Nummer ein';
  },
  userId: (value: string) => {
    const userNamePattern = /^[A-Za-z]{2,}\d{0,2}@(?:online\.)?gibz\.ch$/;
    return (
      userNamePattern.test(value) ||
      'Geben Sie einen gültigen Benutzernamen ein (z.B. fmuster@gibz.ch)'
    );
  },
};

function showSnackbar(content: string, color: string = 'green') {
  snackbarText.value = content;
  snackbarColor.value = color;
  snackbar.value = true;
}
</script>
