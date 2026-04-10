<template>
  <v-dialog
    v-model="showDialog"
    :retain-focus="false"
    persistent
    width="auto"
  >
    <v-form
      ref="locationForm"
      v-model="isValidForm"
      @submit.prevent="submitLocation"
    >
      <v-card width="750px">
        <v-card-title>
          <span class="text-h5">Neuer Standort</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="location.title"
                  label="Titel"
                  required
                  :rules="[(v: string) => !!v || 'Der Titel des Standortes darf nicht leer sein.']"
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <span class="text-h6">Standort-Marker</span>
              </v-col>
              <v-col cols="12">
                <p
                  v-if="location.locationMarkers.length == 0"
                  class="text-center"
                >
                  Für diesen Standort wurden noch keine Standort-Marker erfasst.
                </p>
                <v-list
                  v-else
                  density="compact"
                >
                  <v-list-item
                    v-for="(locationMarker, index) in location.locationMarkers"
                    :key="index"
                    :prepend-icon="
                      locationMarker.hasOwnProperty('uuid') ? 'mdi-bluetooth' : 'mdi-qrcode'
                    "
                  >
                    <v-list-item-title v-if="locationMarker.hasOwnProperty('uuid')">
                      <code>{{ (locationMarker as IBeaconLocationMarker).uuid }}</code>
                    </v-list-item-title>
                    <v-list-item-title v-else>
                      <code>{{ (locationMarker as QRCodeLocationMarker).content }}</code>
                    </v-list-item-title>

                    <v-list-item-subtitle v-if="locationMarker.hasOwnProperty('uuid')">
                      Major:
                      {{
                        (locationMarker as IBeaconLocationMarker).major
                      }}&nbsp;&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;&nbsp; Minor:
                      {{ (locationMarker as IBeaconLocationMarker).minor }}
                    </v-list-item-subtitle>
                  </v-list-item>
                </v-list>
              </v-col>
            </v-row>
          </v-container>
          <v-container class="location-marker-form">
            <v-row>
              <v-col
                class="d-flex justify-center align-baseline"
                style="gap: 1rem"
              >
                <v-btn
                  variant="outlined"
                  size="small"
                  prepend-icon="mdi-plus"
                  @click="$emit('showIBeaconLocationMarkerDialog', true)"
                >
                  iBeacon Marker
                </v-btn>
                <v-btn
                  variant="outlined"
                  size="small"
                  prepend-icon="mdi-plus"
                  @click="$emit('showQRCodeLocationMarkerDialog', true)"
                >
                  QR Code Marker
                </v-btn>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>

        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            variant="text"
            @click="cancel"
          >
            Abbrechen
          </v-btn>
          <v-btn
            type="submit"
            color="primary"
            variant="text"
            @click="submitLocation"
          >
            Speichern
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import type { PropType } from 'vue';
import { Location } from '../../models/rally/location';
import type {
  IBeaconLocationMarker,
  QRCodeLocationMarker,
} from '../../models/rally/locationMarker';

const props = defineProps({
  value: { type: Boolean, required: false, default: false },
  initialLocation: {
    type: Object as PropType<Location>,
    required: false,
    default: new Location('', '', []),
  },
});

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'clearData', value: null): void;
  (e: 'showIBeaconLocationMarkerDialog', value: boolean): void;
  (e: 'showQRCodeLocationMarkerDialog', value: boolean): void;
  (e: 'saveLocation', location: Location): void;
}>();

const showDialog = computed({
  get() {
    return props.value;
  },
  set(value) {
    emit('update:modelValue', value);
  },
});

watch(
  () => props.initialLocation,
  (newInitialLocation: Location) => {
    location.value = newInitialLocation;
  }
);

const locationForm = ref<HTMLFormElement | null>(null);
const isValidForm = ref(true);

const location = ref<Location>(props.initialLocation);

function cancel() {
  emit('clearData', null);
  emit('update:modelValue', false);
}
async function submitLocation(event: Event) {
  event.preventDefault();
  const validationResult = await locationForm.value?.validate();
  if (validationResult.valid) {
    emit('saveLocation', location.value);
    emit('clearData', null);
  }
}
</script>
