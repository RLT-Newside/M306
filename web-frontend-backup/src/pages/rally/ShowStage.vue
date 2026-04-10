<template>
  <v-container v-if="!stageIsLoading">
    <v-row>
      <v-col>
        <h1 class="text-h3">{{ stage?.title }}</h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <StageInformation :stage-information="stage.preArrivalInformation" />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <StageInformation :stage-information="stage.information" />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <v-card
          class="rally-section-card"
          title="Standorte"
          subtitle="Standorte und Standort-Marker können nicht bearbeitet werden. Löschen Sie fehlerhafte oder alte Objekte und fügen Sie anschliessend neue hinzu."
        >
          <v-card-text>
            <p class="text-body-1">
              Eine Station kann an mehreren, unterschiedlichen physischen Standorten lokalisiert
              sein. Jeder Standort kann wiederum durch unterschiedliche Standort-Marker definiert
              sein. Klicken Sie auf den Namen eines Standortes um die Standort-Marker dieses
              Standortes anzuzeigen.
            </p>

            <v-list>
              <template
                v-for="location in stage?.locations"
                :key="location.id"
              >
                <v-list-group :value="location.id">
                  <template #activator="{ isOpen, props: activatorProps }">
                    <v-list-item
                      v-bind="activatorProps"
                      :title="location.title"
                      :subtitle="location.locationMarkers.length + ' Marker'"
                      :prepend-icon="isOpen ? 'mdi-chevron-down' : 'mdi-chevron-right'"
                    >
                      <template #append>
                        <v-btn
                          class="rally-list-icon-btn"
                          icon
                          variant="text"
                          @click.stop="openDeleteLocationDialog(location)"
                        >
                          <v-icon>mdi-delete</v-icon>
                        </v-btn>
                      </template>
                    </v-list-item>
                  </template>

                  <v-list-item
                    v-for="locationMarker in location.locationMarkers"
                    :key="locationMarker.id"
                    density="compact"
                    :prepend-icon="
                      locationMarker.type === 'QRCodeLocationMarker'
                        ? 'mdi-qrcode'
                        : 'mdi-bluetooth'
                    "
                    :title="
                      locationMarker.type === 'QRCodeLocationMarker'
                        ? (locationMarker as QRCodeLocationMarker).content
                        : (locationMarker as IBeaconLocationMarker).uuid
                    "
                    :subtitle="getLocationMarkerSubtitle(locationMarker)"
                    :value="locationMarker.id"
                  ></v-list-item>
                </v-list-group>
              </template>
            </v-list>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              prepend-icon="mdi-plus"
              @click.stop="showLocationDialog = true"
              >Standort hinzufügen</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <v-card
          title="Aktivitäten"
          class="rally-section-card"
        >
          <v-card-text>
            <p class="text-body-1">
              Aktivitäten sind Fragen oder Aufgaben, welche die Teilnehmer/-innen nach der Ankunft
              an einer Station bearbeiten können. Durch die Bearbeitung der Aktivität können die
              Teilnehmer/-innen Punkte sammeln.
            </p>
            <v-table class="rally-table">
              <thead>
                <tr>
                  <th>Titel</th>
                  <th>Aktivitätstyp</th>
                  <th># Antworten</th>
                  <th>Maximalpunktezahl</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="activity in stageActivities"
                  :key="activity.id"
                >
                  <td>{{ activity.title }}</td>
                  <td>{{ getActivityTypeName(activity.type) }}</td>
                  <td>{{ activity.answers.length }}</td>
                  <td>{{ activity.maxPoints }}</td>
                  <td class="text-right">
                    <v-btn
                      class="rally-list-icon-btn"
                      variant="text"
                      icon="mdi-pencil"
                      :to="{
                        name: 'Edit stage activity',
                        params: { stageId: stageId, activityId: activity.id },
                      }"
                    />
                    <v-btn
                      class="rally-list-icon-btn"
                      variant="text"
                      icon="mdi-delete"
                      @click="openDeleteStageActivityDialog(activity)"
                    />
                  </td>
                </tr>
              </tbody>
            </v-table>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              prepend-icon="mdi-plus"
              :to="{ name: 'New stage activity' }"
              >Aktivität hinzufügen</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <v-row justify="end">
      <v-col cols="auto">
        <StageActivityDialog
          v-model="showStageActivityDialog"
          :initial-stage-activity="stageActivityForDialog"
          @save-stage-activity="handleSaveStageActivity"
        />
      </v-col>
    </v-row>

    <v-row justify="center">
      <v-col cols="auto">
        <v-btn
          :to="{ name: 'List stages' }"
          prepend-icon="mdi-chevron-left"
          >zurück zur Liste</v-btn
        >
      </v-col>
      <v-col cols="auto">
        <v-btn
          :to="{ name: 'Edit stage', params: { stageId: stageId } }"
          prepend-icon="mdi-pencil"
          >Station bearbeiten</v-btn
        >
      </v-col>
      <v-col cols="auto">
        <v-btn
          color="error"
          prepend-icon="mdi-delete"
          @click="openDeleteStageDialog"
          >Station löschen</v-btn
        >
      </v-col>
    </v-row>
  </v-container>

  <DeleteConfirmDialog
    v-model="showDeleteLocationDialog"
    title="Standort löschen"
    message="Möchten Sie diesen Standort löschen?"
    @confirm="handleDeleteLocation"
  >
    <template #message>
      <p>
        Möchten Sie den Standort
        <strong>{{ locationToDelete?.title }}</strong>
        endgültig löschen?
      </p>
    </template>
    <p class="mt-3 mb-0">Zugehörige Standort-Marker werden ebenfalls gelöscht.</p>
  </DeleteConfirmDialog>

  <DeleteConfirmDialog
    v-model="showDeleteStageActivityDialog"
    title="Aktivität löschen"
    message=""
    :confirm-disabled="stageActivityUsage.isUsed === true"
    @confirm="handleDeleteStageActivity"
  >
    <template #message>
      <p>
        Möchten Sie die Aktivität
        <strong>{{ stageActivityToDelete?.title || 'ohne Titel' }}</strong>
        endgültig löschen?
      </p>
    </template>
    <p class="mt-3 mb-0">Zugehörige Antworten und Resultate werden ebenfalls gelöscht.</p>
    <v-alert
      v-if="stageActivityUsage.isUsed === true"
      type="warning"
      variant="tonal"
      class="mt-3"
    >
      Diese Aktivität wird aktuell in
      <strong>{{
        stageActivityUsage.rallyCount === 1
          ? '1 Rallye'
          : `${stageActivityUsage.rallyCount} Rallyes`
      }}</strong>
      verwendet und kann daher nicht gelöscht werden.
    </v-alert>
  </DeleteConfirmDialog>

  <DeleteStageConfirmDialog
    v-model="showDeleteStageDialog"
    :stage-title="stage?.title"
    :stage-usage="stageUsage"
    @confirm="handleDeleteStage"
  />

  <LocationDialog
    v-model="showLocationDialog"
    :initial-location="locationForLocationDialog"
    @show-i-beacon-location-marker-dialog="
      (value) => {
        showIBeaconLocationMarkerDialog = value;
      }
    "
    @show-q-r-code-location-marker-dialog="
      (value) => {
        showQRCodeLocationMarkerDialog = value;
      }
    "
    @save-location="handleSaveLocation"
    @clear-data="clearLocationMarkers"
  />
  <IBeaconLocationMarkerDialog
    v-model="showIBeaconLocationMarkerDialog"
    @update:location-marker="addLocationMarker"
  />
  <QRCodeLocationMarkerDialog
    v-model="showQRCodeLocationMarkerDialog"
    @update:location-marker="addLocationMarker"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import LocationDialog from '../../components/rally/LocationDialog.vue';
import type {
  IBeaconLocationMarker,
  LocationMarker,
  QRCodeLocationMarker,
} from '../../models/rally/locationMarker';
import QRCodeLocationMarkerDialog from '../../components/rally/QRCodeLocationMarkerDialog.vue';
import IBeaconLocationMarkerDialog from '../../components/rally/IBeaconLocationMarkerDialog.vue';
import { Location } from '../../models/rally/location';
import StageActivityDialog from '../../components/rally/StageActivityDialog.vue';
import { StageActivity, StageActivityType } from '../../models/rally/stageActivity';
import DeleteConfirmDialog from '@/components/rally/DeleteConfirmDialog.vue';
import DeleteStageConfirmDialog from '@/components/rally/DeleteStageConfirmDialog.vue';
import { useRequest } from 'alova/client';
import {
  deleteStage,
  createLocation,
  createStageActivity,
  getStageActivityUsage,
  getStage,
  getStageActivities,
  getStageUsage,
  removeLocation,
  removeStageActivity,
} from '@/api/alovaMethods/gibzRally';
import type { StageActivityUsageState, StageUsageState } from '@/models/rally/management';
import StageInformation from '@/components/rally/StageInformation.vue';
import { useNotifications } from '@/composables/useNotifications';

const showLocationDialog = ref(false);
const showIBeaconLocationMarkerDialog = ref(false);
const showQRCodeLocationMarkerDialog = ref(false);
const showDeleteLocationDialog = ref(false);
const showStageActivityDialog = ref(false);
const showDeleteStageActivityDialog = ref(false);
const showDeleteStageDialog = ref(false);

const locationForLocationDialog = ref<Location>(new Location('', '', []));
const locationToDelete = ref<Location | null>(null);
const stageActivityForDialog = ref<StageActivity>(
  new StageActivity('', '', '', 0, StageActivityType.singleChoice, [])
);
const stageActivityToDelete = ref<StageActivity | null>(null);
const stageActivityUsage = ref<StageActivityUsageState>({
  isUsed: null,
  rallyCount: null,
});
const stageUsage = ref<StageUsageState>({
  isUsed: null,
  rallyCount: null,
  rallyTitles: [],
});

const props = defineProps({
  stageId: { type: String, required: true },
});

const router = useRouter();
const { showSuccess, showError } = useNotifications();

const {
  data: stage,
  send: fetchStage,
  loading: stageIsLoading,
} = useRequest(getStage(props.stageId), { immediate: true });

const { data: stageActivities, send: fetchStageActivities } = useRequest(
  getStageActivities(props.stageId),
  { immediate: true }
);

const { send: saveLocation, onComplete: saveLocationCompleted } = useRequest(
  (location: Partial<Location>) => createLocation(props.stageId, location),
  { immediate: false }
);

const { send: deleteLocation } = useRequest(
  (locationId: string) => removeLocation(props.stageId, locationId),
  { immediate: false }
);

const { send: saveStageActivity, onComplete: saveStageActivityCompleted } = useRequest(
  (stageActivity: Partial<StageActivity>) => createStageActivity(props.stageId, stageActivity),
  { immediate: false }
);

const { send: deleteStageActivity, onComplete: deleteStageActivityCompleted } = useRequest(
  (stageActivityId: string) => removeStageActivity(props.stageId, stageActivityId),
  { immediate: false }
);

const { send: executeDeleteStage } = useRequest(() => deleteStage(props.stageId), {
  immediate: false,
});

const { send: loadStageActivityUsage } = useRequest(
  (stageActivityId: string) => getStageActivityUsage(props.stageId, stageActivityId),
  { immediate: false }
);

const { send: loadStageUsage } = useRequest(() => getStageUsage(props.stageId), {
  immediate: false,
});

saveLocationCompleted((event) => {
  if (event.status === 'success') {
    showSuccess('Standort wurde gespeichert.');
    showLocationDialog.value = false;
    fetchStage();
  }
});

saveStageActivityCompleted((event) => {
  if (event.status === 'success') {
    showSuccess('Aktivität wurde gespeichert.');
    showStageActivityDialog.value = false;
    fetchStageActivities();
  }
});

async function handleSaveLocation(location: Partial<Location>): Promise<void> {
  try {
    await saveLocation(location);
  } catch (error) {
    showError(error, {
      notFound: 'Der Standort konnte nicht erstellt werden, da die Station nicht existiert.',
      forbidden: 'Keine Berechtigung zum Erstellen des Standorts.',
      conflict: 'Der Standort kann mit den angegebenen Daten nicht erstellt werden.',
      fallback: 'Standort konnte nicht erstellt werden.',
    });
  }
}

async function handleSaveStageActivity(stageActivity: Partial<StageActivity>): Promise<void> {
  try {
    await saveStageActivity(stageActivity);
  } catch (error) {
    showError(error, {
      notFound: 'Die Aktivität konnte nicht erstellt werden, da die Station nicht existiert.',
      forbidden: 'Keine Berechtigung zum Erstellen der Aktivität.',
      conflict: 'Die Aktivität kann mit den angegebenen Daten nicht erstellt werden.',
      fallback: 'Aktivität konnte nicht erstellt werden.',
    });
  }
}

deleteStageActivityCompleted(() => {
  fetchStageActivities();
});

async function openDeleteStageActivityDialog(activity: StageActivity): Promise<void> {
  stageActivityToDelete.value = activity;
  stageActivityUsage.value = { isUsed: null, rallyCount: null };
  showDeleteStageActivityDialog.value = true;

  try {
    const usage = await loadStageActivityUsage(activity.id);
    stageActivityUsage.value = {
      isUsed: usage.isUsed,
      rallyCount: usage.rallyCount ?? null,
    };
  } catch {
    // Fallback to delete endpoint validation when usage endpoint is unavailable.
  }
}

async function handleDeleteStageActivity(): Promise<void> {
  if (!stageActivityToDelete.value) {
    return;
  }

  try {
    await deleteStageActivity(stageActivityToDelete.value.id);
    showSuccess('Aktivität wurde gelöscht.');
    showDeleteStageActivityDialog.value = false;
  } catch (error) {
    const status = showError(error, {
      notFound: 'Die Aktivität existiert nicht mehr.',
      forbidden: 'Keine Berechtigung zum Löschen der Aktivität.',
      conflict: 'Diese Aktivität wird in einer Rallye verwendet und kann nicht gelöscht werden.',
      fallback: 'Aktivität konnte nicht gelöscht werden.',
    });

    if (status === 409) {
      stageActivityUsage.value.isUsed = true;
      if (stageActivityUsage.value.rallyCount === null) {
        stageActivityUsage.value.rallyCount = 1;
      }
    }

    if (status === 404) {
      showDeleteStageActivityDialog.value = false;
      await fetchStageActivities();
    }
  }
}

async function openDeleteStageDialog(): Promise<void> {
  stageUsage.value = { isUsed: null, rallyCount: null, rallyTitles: [] };
  showDeleteStageDialog.value = true;

  try {
    const usage = await loadStageUsage();
    const rallyTitles = usage.rallyTitles ?? [];
    stageUsage.value = {
      isUsed: usage.isUsed,
      rallyCount: usage.rallyCount ?? null,
      rallyTitles,
    };
  } catch {
    // Fallback to delete endpoint validation when usage endpoint is unavailable.
  }
}

async function handleDeleteStage(): Promise<void> {
  try {
    await executeDeleteStage();
    showSuccess('Station wurde gelöscht.');
    showDeleteStageDialog.value = false;
    router.push({ name: 'List stages' });
  } catch (error) {
    const status = showError(error, {
      notFound: 'Die Station existiert nicht mehr.',
      forbidden: 'Keine Berechtigung zum Löschen der Station.',
      conflict: 'Diese Station wird in einer Rallye verwendet und kann nicht gelöscht werden.',
      fallback: 'Station konnte nicht gelöscht werden.',
    });

    if (status === 409) {
      stageUsage.value.isUsed = true;
      if (stageUsage.value.rallyCount === null) {
        stageUsage.value.rallyCount = 1;
      }
    }

    if (status === 404) {
      showDeleteStageDialog.value = false;
      router.push({ name: 'List stages' });
    }
  }
}

function getActivityTypeName(activityType: StageActivityType) {
  switch (activityType) {
    case StageActivityType.singleChoice:
      return 'Single Choice';
    case StageActivityType.multipleChoice:
      return 'Multiple Choice';
    case StageActivityType.textInput:
      return 'Texteingabe';
    case StageActivityType.qrCode:
      return 'QR Code';
    default:
      return 'Unbekannt';
  }
}

function addLocationMarker(locationMarker: LocationMarker) {
  locationForLocationDialog.value.locationMarkers.push(locationMarker);
}

function clearLocationMarkers() {
  locationForLocationDialog.value.locationMarkers.splice(
    0,
    locationForLocationDialog.value.locationMarkers.length
  );
  locationForLocationDialog.value.title = '';
}

function openDeleteLocationDialog(location: Location): void {
  locationToDelete.value = location;
  showDeleteLocationDialog.value = true;
}

async function handleDeleteLocation(): Promise<void> {
  if (!locationToDelete.value) {
    return;
  }

  try {
    await deleteLocation(locationToDelete.value.id);
    showSuccess('Standort wurde gelöscht.');
    showDeleteLocationDialog.value = false;
    locationToDelete.value = null;
    await fetchStage();
  } catch (error) {
    const status = showError(error, {
      notFound: 'Der Standort existiert nicht mehr.',
      forbidden: 'Keine Berechtigung zum Löschen des Standorts.',
      conflict: 'Der Standort kann aktuell nicht gelöscht werden.',
      fallback: 'Standort konnte nicht gelöscht werden.',
    });

    if (status === 404) {
      showDeleteLocationDialog.value = false;
      locationToDelete.value = null;
      await fetchStage();
    }
  }
}

function getLocationMarkerSubtitle(locationMarker: LocationMarker): string {
  if (locationMarker.type === 'IBeaconLocationMarker') {
    const iBeaconLocationMarker = locationMarker as IBeaconLocationMarker;
    return `Major: ${iBeaconLocationMarker.major} , Minor: ${iBeaconLocationMarker.minor}`;
  }
  return '';
}
</script>

<style lang="scss">
#markdown-preview {
  padding: 15px;
  border: 1px solid black;
  border-radius: 5px;

  list-style-position: inside;

  & ul ul {
    margin-left: 1.5em;
  }
}
</style>
