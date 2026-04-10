<template>
  <v-container>
    <PageTitle title="Portraitfoto-Anfrage erfassen" />

    <v-row>
      <v-col>
        <p class="text-body-1">
          Auf dieser Seite können für Personen, welche im schulNetz erfasst sind, Anfragen für die
          Erstellung eines Portraitfotos generiert werden.
        </p>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-form
          ref="searchForm"
          @submit.prevent="getPersonData"
        >
          <v-text-field
            v-model="searchValue"
            label="Suchen nach Kürzel einer Person"
            prepend-inner-icon="mdi-magnify"
            variant="solo"
            :loading="searchInProgress"
            :disabled="searchInProgress"
          >
            <template #append-inner>
              <v-btn
                variant="text"
                :disabled="!searchValue"
                icon="mdi-send"
                @click="getPersonData"
              >
              </v-btn>
            </template>
          </v-text-field>
        </v-form>
      </v-col>
    </v-row>

    <v-form
      ref="personForm"
      v-model="isFormValid"
      @submit.prevent="submitCreationRequest"
    >
      <v-row>
        <v-col
          cols="12"
          md="6"
        >
          <v-text-field
            v-model="creationRequest.gib"
            label="GIB-Nummer"
            :disabled="searchInProgress || submissionInProgress"
            required
            :rules="[rules.required, rules.gib]"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <v-text-field
            v-model="creationRequest.userName"
            label="Kürzel"
            :disabled="searchInProgress || submissionInProgress"
            suffix="@online.gibz.ch"
            required
            :rules="[rules.required, rules.username]"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <v-text-field
            v-model="creationRequest.lastName"
            label="Nachname"
            :disabled="searchInProgress || submissionInProgress"
            required
            :rules="[rules.required]"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <v-text-field
            v-model="creationRequest.firstName"
            label="Vorname"
            :disabled="searchInProgress || submissionInProgress"
            required
            :rules="[rules.required]"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <v-text-field
            v-model="creationRequest.classUniqueName"
            label="Klasse (mit Jahreszahl)"
            :disabled="searchInProgress || submissionInProgress"
            required
            :rules="[rules.classUniqueName]"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <v-text-field
            v-model="creationRequest.classDisplayName"
            label="Klasse (mit Lehrjahr)"
            :disabled="searchInProgress || submissionInProgress"
            required
            :rules="[rules.classDisplayName]"
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
            >Anfrage generieren</v-btn
          >
        </v-col>
      </v-row>
    </v-form>
  </v-container>

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
</template>
<script setup lang="ts">
import { createRequest, searchPerson } from '@/api/alovaMethods/profilePictureRequestGeneration';
import PageTitle from '@/components/PageTitle.vue';
import { PersonSearchResult } from '@/models/profilePictureReview/profilePicture';
import { useRequest } from 'alova/client';
import { reactive, ref } from 'vue';

const searchForm = ref();
const searchValue = ref('');

const personForm = ref<HTMLFormElement | null>(null);
const isFormValid = ref(true);
const creationRequest = reactive(new PersonSearchResult('', '', '', '', '', ''));

const snackbar = ref(false);
const snackbarText = ref('');
const snackbarColor = ref('green');

const {
  send: getPersonData,
  loading: searchInProgress,
  onSuccess: onLoadPersonData,
  onError: onSearchError,
} = useRequest(() => searchPerson('username', searchValue.value), {
  immediate: false,
});

const {
  send: submitCreationRequest,
  loading: submissionInProgress,
  onComplete: onCreationComplete,
} = useRequest(() => createRequest(creationRequest), { immediate: false });

const rules = {
  required: (value: string) => !!value || 'Das Feld darf nicht leer sein',
  gib: (value: string) => {
    const gibPattern = /(\bGIB\d{6})\b/;
    return gibPattern.test(value) || 'Geben Sie eine gültige GIB-Nummer ein';
  },
  username: (value: string) => {
    const userNamePattern = /\b[A-Za-z]{2,}\d{0,2}\b/;
    return userNamePattern.test(value) || 'Geben Sie ein gültiges Kürzel ein';
  },
  classUniqueName: (value: string) => {
    const pattern = /\b[A-Za-z]{2,}20\d{2}[a-z]?\b/;
    return (
      pattern.test(value) || 'Diese Klassenbezeichnung muss eine vierstellige Jahreszahl enthalten'
    );
  },
  classDisplayName: (value: string) => {
    const pattern = /\b[A-Za-z]{2,}[1-4]{1}[a-z]?\b/;
    return (
      pattern.test(value) ||
      'Diese Klassenbezeichnung muss das Lehrjahr als einstellige Zahl enthalten'
    );
  },
};

onSearchError((event) => {
  console.log('ERROR');
  throw event.error;
});

onLoadPersonData((event) => {
  const person = event.data[0];
  if (!person) return;

  creationRequest.gib = person.gib;
  creationRequest.userName = person.userName;
  creationRequest.firstName = person.firstName;
  creationRequest.lastName = person.lastName;
  creationRequest.classUniqueName = person.classUniqueName;
  creationRequest.classDisplayName = person.classDisplayName;
});

onCreationComplete((event) => {
  if (event.status == 'success') {
    searchForm.value?.reset();
    personForm.value?.reset();

    const person = event.data;
    if (person) {
      showSnackbar(
        `Der Request für ${person.firstName} ${person.lastName} wurde erfolgreich erstellt.`
      );
    }
  }
});

function showSnackbar(content: string, color: string = 'green') {
  snackbarText.value = content;
  snackbarColor.value = color;
  snackbar.value = true;
}
</script>
