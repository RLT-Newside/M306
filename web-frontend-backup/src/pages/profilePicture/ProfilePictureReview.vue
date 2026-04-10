<template>
  <v-container>
    <PageTitle title="Portraitfotos überprüfen" />

    <v-dialog
      v-model="dialog"
      max-width="500px"
    >
      <v-card>
        <v-card-text
          :class="{
            'bg-green accepted': dialogImage?.status === ReviewState.Accepted,
            'bg-red rejected': dialogImage?.status === ReviewState.Rejected,
          }"
        >
          <v-img :src="dialogImage?.url" />
        </v-card-text>
        <v-card-text>
          <p class="text-body-1">
            {{ dialogImage?.subjectFirstName }} {{ dialogImage?.subjectLastName }}
          </p>
          <p class="text-body-1">{{ dialogImage?.cohortDisplayName }}</p>
          <p class="text-body-2 mt-3">
            Upload: {{ dialogImage?.uploadedAt.toLocaleString('de-DE', localeDateOptions) }}
          </p>
        </v-card-text>
        <v-card-actions class="mx-3 mb-2">
          <v-btn
            color="red"
            variant="tonal"
            prepend-icon="mdi-close"
          >
            Ablehnen
            <v-menu
              activator="parent"
              location="bottom center"
            >
              <v-list>
                <v-list-item
                  v-for="rejectReason in rejectReasons"
                  :key="rejectReason.id"
                  @click="closeDialogAndReject(dialogImage!.id, rejectReason.id)"
                >
                  <v-list-item-title>{{ rejectReason.title }}</v-list-item-title>
                </v-list-item>
              </v-list>
            </v-menu>
          </v-btn>
          <v-spacer />
          <v-btn
            color="green"
            variant="tonal"
            prepend-icon="mdi-check"
            @click="closeDialogAndAccept(dialogImage!.id)"
          >
            Akzeptieren
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-row
      v-if="!images || images.length === 0"
      justify="center"
    >
      <v-col
        cols="12"
        class="text-center"
      >
        <p class="mt-5 text-body-2 text-center">
          Aktuell sind keine Portraitfotos für die manuelle Überprüfung verfügbar.
        </p>
        <v-btn
          class="my-4"
          color="primary"
          prepend-icon="mdi-reload"
          @click="fetchImages()"
        >
          Portraitfotos laden
        </v-btn>
      </v-col>
    </v-row>
    <div v-else>
      <div
        v-if="imagesLoading"
        class="my-5"
      >
        <v-row justify="center">
          <v-col class="text-center">
            <v-progress-circular
              indeterminate
              class="my-2"
              color="primary"
              size="30"
            />
            <p class="text-body-2">Die Portraitfotos werden geladen...</p>
          </v-col>
        </v-row>
      </div>

      <div v-else>
        <v-row>
          <v-col>
            <p class="text-body-1">
              Überprüfen Sie die Qualität der eingereichten Portraitfotos. Jedes einzelne Foto kann
              akzeptiert oder mit einem spezifischen Grund abgelehnt werden.
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col
            v-for="image in images"
            :key="image.id"
            cols="6"
            sm="3"
            md="2"
            lg="2"
          >
            <v-card
              class="cursor-pointer"
              :class="{
                'bg-green accepted': image.status === ReviewState.Accepted,
                'bg-red rejected': image.status === ReviewState.Rejected,
              }"
            >
              <v-img
                :src="image.url"
                class="ma-2"
                @click="openImageDialog(image)"
              ></v-img>

              <v-card-actions>
                <v-spacer />
                <v-btn>
                  <v-icon icon="mdi-close" />
                  <v-menu
                    activator="parent"
                    location="bottom center"
                  >
                    <v-list>
                      <v-list-item
                        v-for="rejectReason in rejectReasons"
                        :key="rejectReason.id"
                        :value="rejectReason.id"
                        @click="rejectImage(image.id, rejectReason.id)"
                      >
                        <v-list-item-title>{{ rejectReason.title }}</v-list-item-title>
                      </v-list-item>
                    </v-list>
                  </v-menu>
                </v-btn>

                <v-btn @click="acceptImage(image.id)">
                  <v-icon icon="mdi-check" />
                </v-btn>
                <v-spacer />
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>

        <v-row justify="center">
          <v-col cols="auto">
            <v-tooltip
              location="top left"
              text="Es werden alle Portraitfotos akzeptiert, für welche noch keine Auswahl getroffen wurde."
            >
              <template #activator="{ props }">
                <v-btn
                  variant="tonal"
                  prepend-icon="mdi-check"
                  v-bind="props"
                  @click="acceptAllImages()"
                >
                  Alle akzeptieren
                </v-btn>
              </template>
            </v-tooltip>
          </v-col>
        </v-row>

        <v-row justify="center">
          <v-col cols="auto">
            <v-btn
              class="my-3"
              color="primary"
              :disabled="reviewedImagesCount < 1"
              @click="confirmSelection()"
            >
              Review für {{ reviewedImagesCount }}
              {{ reviewedImagesCount === 1 ? 'Bild' : 'Bilder' }} abschliessen
            </v-btn>
          </v-col>
        </v-row>
      </div>
    </div>
  </v-container>
</template>

<script setup lang="ts">
import {
  ReviewedProfilePicture,
  type ProfilePicture,
} from '@/models/profilePictureReview/profilePicture';
import { computed, onMounted, ref } from 'vue';
import { useRequest } from 'alova/client';
import {
  getPendingReviews,
  getRejectReasons,
  reviewProfilePictures,
} from '@/api/alovaMethods/profilePictureReview';
import PageTitle from '@/components/PageTitle.vue';
import { ReviewState } from '@/models/profilePictureReview/ReviewState';

const dialog = ref<boolean>(false);
const dialogImage = ref<ProfilePicture | null>(null);

const localeDateOptions: Intl.DateTimeFormatOptions = {
  year: 'numeric',
  month: '2-digit',
  day: '2-digit',
  hour: '2-digit',
  minute: '2-digit',
};

const { data: rejectReasons, send: fetchRejectReasons } = useRequest(getRejectReasons(), {
  immediate: false,
});

const {
  data: images,
  loading: imagesLoading,
  send: fetchImages,
} = useRequest(getPendingReviews(), {
  immediate: false,
});

const reviewedImagesCount = computed(() => {
  if (images === undefined || images.value.length === 0) {
    return 0;
  }
  return images.value.filter((image) => image.status !== ReviewState.Pending).length;
});

const { send: sendReview, onComplete } = useRequest(
  (reviewedImages: ReviewedProfilePicture[]) => reviewProfilePictures(reviewedImages),
  {
    immediate: false,
  }
);

function openImageDialog(image: ProfilePicture) {
  dialogImage.value = image;
  dialog.value = true;
}

function closeDialogAndReject(imageId: string, rejectReasonId: string) {
  dialog.value = false;
  rejectImage(imageId, rejectReasonId);
}

function closeDialogAndAccept(imageId: string) {
  dialog.value = false;
  acceptImage(imageId);
}

function findImage(imageId: string) {
  // Find image by id in images array
  const image = images.value.find((image) => image.id === imageId);

  if (!image) {
    console.error('Image not found');
  }

  return image;
}

function acceptImage(imageId: string) {
  const image = findImage(imageId);

  if (!image) {
    return;
  }

  // Set or toggle status
  image.status = image.status == ReviewState.Accepted ? ReviewState.Pending : ReviewState.Accepted;
}

function rejectImage(imageId: string, rejectReasonId: string) {
  const image = findImage(imageId);

  if (!image) {
    return;
  }

  // Set or toggle status
  image.rejectReason = rejectReasons.value.find((reason) => reason.id == rejectReasonId) ?? null;
  image.status = ReviewState.Rejected;
}

function acceptAllImages() {
  images.value.forEach((image) => {
    if (image.status === ReviewState.Pending) {
      image.status = ReviewState.Accepted;
    }
  });
}

async function confirmSelection() {
  const reviews = images.value
    .filter((image) => image.status !== ReviewState.Pending)
    .map(
      (image) => new ReviewedProfilePicture(image.id, image.status, image.rejectReason?.id ?? null)
    );

  sendReview(reviews);
}

onComplete(() => {
  fetchImages();
});

onMounted(async () => {
  await fetchImages().then((images) => {
    if (images.length > 0) fetchRejectReasons();
  });
});
</script>
