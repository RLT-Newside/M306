<template>
  <div class="page-wrapper">
    <v-container class="py-8 landing-page">
      <!-- Page Header with gradient background -->
      <v-row justify="center">
        <v-col
          cols="12"
          md="10"
          lg="8"
        >
          <div class="text-center mb-8 header-section">
            <div class="icon-wrapper mb-4">
              <v-icon
                size="100"
                color="primary"
                class="pulse-animation"
              >
                mdi-camera-outline
              </v-icon>
            </div>
            <h1 class="text-h3 text-md-h2 font-weight-bold mb-3 gradient-text">
              Portraitfoto erstellen
            </h1>
            <p
              v-if="!isValidating && tokenData && tokenData.firstName"
              class="text-h5 greeting-text"
            >
              Hallo {{ tokenData.firstName }}!
            </p>
            <p
              v-if="!isValidating && !isExpired && !isAlreadyProcessed && !notFound"
              class="text-body-1 text-medium-emphasis mt-2"
            >
              Folgen Sie diesen einfachen Schritten, um Ihr Portraitfoto zu erstellen
            </p>
          </div>
        </v-col>
      </v-row>

      <!-- Loading state with modern design -->
      <v-row
        v-if="isValidating"
        justify="center"
      >
        <v-col
          cols="12"
          md="6"
          class="text-center py-12"
        >
          <v-card
            class="pa-8 loading-card"
            elevation="0"
            rounded="xl"
          >
            <v-progress-circular
              indeterminate
              color="primary"
              size="80"
              width="6"
            />
            <p class="text-h6 mt-6 mb-2">Anfrage wird überprüft...</p>
            <p class="text-body-2 text-medium-emphasis">Bitte warten Sie einen Moment</p>
          </v-card>
        </v-col>
      </v-row>

      <!-- Error states with improved design -->
      <v-row
        v-else-if="isExpired || isAlreadyProcessed || notFound"
        justify="center"
      >
        <v-col
          cols="12"
          md="8"
          lg="6"
        >
          <RequestStatusCard
            v-if="isExpired"
            title="Anfrage abgelaufen"
            status-color="error"
            status-icon="mdi-clock-alert-outline"
          >
            <p class="mb-3">
              Diese Anfrage zum Erstellen eines Portraitfotos ist am
              <strong>{{
                tokenData?.requestExpiry ? formatExpiryDate(tokenData.requestExpiry) : ''
              }}</strong>
              abgelaufen.
            </p>
            <p>Sie ist nicht mehr gültig und kann nicht mehr verwendet werden.</p>
          </RequestStatusCard>

          <RequestStatusCard
            v-else-if="isAlreadyProcessed"
            title="Anfrage bereits bearbeitet"
            status-color="success"
            status-icon="mdi-check-circle"
          >
            <p>Sie haben für diese Anfrage bereits ein Portraitfoto hochgeladen.</p>
          </RequestStatusCard>

          <RequestStatusCard
            v-else
            title="Anfrage nicht gefunden"
            status-color="warning"
            status-icon="mdi-alert-circle-outline"
          >
            <p>
              Diese Anfrage konnte nicht gefunden werden. Bitte überprüfen Sie, ob Sie den korrekten
              Link verwendet haben.
            </p>
          </RequestStatusCard>
        </v-col>
      </v-row>

      <!-- Valid request - show stepper with enhanced design -->
      <v-row
        v-else
        justify="center"
      >
        <v-col
          cols="12"
          md="10"
          lg="9"
        >
          <!-- Progress indicator -->
          <v-card
            class="mb-6 progress-card"
            elevation="0"
            rounded="lg"
          >
            <v-card-text class="py-4">
              <div class="d-flex align-center">
                <v-icon
                  color="primary"
                  class="mr-3"
                  >mdi-progress-check</v-icon
                >
                <div class="flex-grow-1">
                  <div class="text-caption text-medium-emphasis mb-1">Ihr Fortschritt</div>
                  <v-progress-linear
                    :model-value="(step / 3) * 100"
                    color="primary"
                    height="8"
                    rounded
                  />
                </div>
                <div class="ml-4 text-h6 font-weight-bold">{{ step }}/3</div>
              </div>
            </v-card-text>
          </v-card>

          <!-- Vertical Stepper with enhanced styling -->
          <v-stepper-vertical
            v-model="step"
            class="stepper-enhanced"
          >
            <!-- Step 1: Install App -->
            <v-stepper-vertical-item
              :complete="step > 1"
              :value="1"
              title="GIBZ App installieren"
              icon="mdi-download"
              complete-icon="mdi-check"
              hide-actions
              color="primary"
              class="step-item"
            >
              <v-card
                elevation="0"
                class="step-content-card"
              >
                <v-card-text class="pa-6">
                  <div class="step-header mb-6">
                    <h3 class="text-h5 font-weight-bold mb-3">
                      Installieren Sie die GIBZ App auf Ihrem Smartphone
                    </h3>
                    <p class="text-body-1 text-medium-emphasis">
                      Wählen Sie Ihr Betriebssystem und folgen Sie den Anweisungen:
                    </p>
                  </div>

                  <v-tabs
                    v-model="smartphoneOS"
                    color="primary"
                    align-tabs="center"
                    class="mb-6 os-tabs"
                    grow
                  >
                    <v-tab
                      :value="0"
                      class="tab-enhanced"
                    >
                      <v-icon
                        start
                        size="28"
                        >mdi-apple</v-icon
                      >
                      <span class="font-weight-bold">iPhone</span>
                    </v-tab>
                    <v-tab
                      :value="1"
                      class="tab-enhanced"
                    >
                      <v-icon
                        start
                        size="28"
                        >mdi-android</v-icon
                      >
                      <span class="font-weight-bold">Android</span>
                    </v-tab>
                  </v-tabs>

                  <v-row align="center">
                    <v-col
                      cols="12"
                      lg="7"
                    >
                      <v-list class="installation-list">
                        <InstallationStepCard
                          v-for="(item, index) in installationSteps"
                          :key="index"
                          :title="item.title"
                          :subtitle="item.subtitle"
                          :avatar-color="getAvatarColor(index)"
                          :icon="item.icon ?? undefined"
                          :image="item.image ?? undefined"
                          :needs-padding="item.needsPadding"
                          :is-first-icon="item.isFirstIcon"
                          :is-last-icon="item.isLastIcon"
                          :ios-style="smartphoneOS === 0"
                        />
                      </v-list>
                    </v-col>

                    <v-col
                      cols="12"
                      lg="5"
                      class="text-center qr-column"
                    >
                      <QrCodeCard
                        title="Schnell-Download"
                        :link="storeLink"
                        description="Scannen Sie den QR-Code<br />oder klicken Sie auf den Button"
                        :button-text="`Zum ${appStoreName}`"
                        :button-icon="smartphoneOS == 0 ? 'mdi-apple' : 'mdi-android'"
                        :max-width="380"
                      />
                    </v-col>
                  </v-row>
                </v-card-text>
              </v-card>
            </v-stepper-vertical-item>

            <!-- Step 2: Scan QR Code -->
            <v-stepper-vertical-item
              :complete="step > 2"
              :value="2"
              title="QR-Code scannen"
              icon="mdi-qrcode-scan"
              complete-icon="mdi-check"
              hide-actions
              color="primary"
              class="step-item"
            >
              <v-card
                elevation="0"
                class="step-content-card"
              >
                <v-card-text class="pa-6">
                  <div class="step-header mb-6">
                    <h3 class="text-h5 font-weight-bold mb-3">Scannen Sie den QR-Code</h3>
                    <p class="text-body-1 text-medium-emphasis">
                      Verwenden Sie die Kamera Ihres Smartphones, um den QR-Code zu scannen und die
                      App direkt zu öffnen.
                    </p>
                  </div>

                  <v-row align="center">
                    <v-col
                      cols="12"
                      lg="7"
                    >
                      <v-timeline
                        side="end"
                        density="compact"
                        class="scan-timeline"
                      >
                        <TimelineInstructionItem
                          v-for="(instruction, index) in stepTwoInstructions"
                          :key="index"
                          :text="instruction"
                          :index="index"
                        />
                      </v-timeline>
                    </v-col>

                    <v-col
                      cols="12"
                      lg="5"
                      class="text-center qr-column"
                    >
                      <QrCodeCard
                        title="Ihr persönlicher QR-Code"
                        :link="`https://gibz-app.ch/profile-picture/${token}`"
                        description="Dieser QR-Code ist nur für Sie bestimmt"
                        large
                        :max-width="400"
                      />
                    </v-col>
                  </v-row>
                </v-card-text>
              </v-card>
            </v-stepper-vertical-item>

            <!-- Step 3: Take Photo -->
            <v-stepper-vertical-item
              :complete="step > 3"
              :value="3"
              title="Foto erstellen"
              icon="mdi-camera"
              complete-icon="mdi-check"
              hide-actions
              color="primary"
              class="step-item"
            >
              <v-card
                elevation="0"
                class="step-content-card"
              >
                <v-card-text class="pa-6">
                  <div class="step-header mb-6">
                    <h3 class="text-h5 font-weight-bold mb-3">Erstellen Sie das Portraitfoto</h3>
                    <p class="text-body-1 text-medium-emphasis">
                      Folgen Sie den Anweisungen in der App und beachten Sie diese wichtigen Tipps
                      für ein perfektes Portraitfoto.
                    </p>
                  </div>

                  <v-row>
                    <v-col
                      cols="12"
                      md="6"
                    >
                      <div class="camera-preview mb-4 d-flex justify-center">
                        <v-img
                          :src="cameraImageUrl"
                          max-width="300"
                          class="rounded-lg"
                        />
                      </div>
                    </v-col>

                    <v-col
                      cols="12"
                      md="6"
                    >
                      <div class="tips-list">
                        <TipCard
                          v-for="(advice, index) in stepThreeAdvices"
                          :key="index"
                          :title="advice.title"
                          :subtitle="advice.subtitle"
                        />
                      </div>
                    </v-col>
                  </v-row>

                  <v-alert
                    type="info"
                    variant="tonal"
                    class="mt-6"
                    rounded="lg"
                  >
                    <template #prepend>
                      <v-icon>mdi-lightbulb-on-outline</v-icon>
                    </template>
                    <div class="text-body-2">
                      <strong>Tipp:</strong> Stellen Sie sich vor ein Fenster, um natürliches Licht
                      zu nutzen. Das sorgt für die beste Belichtung!
                    </div>
                  </v-alert>
                </v-card-text>
              </v-card>
            </v-stepper-vertical-item>
          </v-stepper-vertical>
        </v-col>
      </v-row>

      <!-- Sticky Navigation Buttons Footer with modern design -->
      <v-footer
        v-if="!isValidating && !isExpired && !isAlreadyProcessed && !notFound"
        app
        fixed
        elevation="12"
        class="navigation-footer"
      >
        <v-container>
          <v-row justify="center">
            <v-col
              cols="12"
              md="10"
              lg="9"
            >
              <div class="d-flex justify-space-between align-center">
                <v-btn
                  v-if="step > 1"
                  variant="outlined"
                  size="large"
                  rounded="pill"
                  @click="prev"
                >
                  <v-icon start>mdi-arrow-left</v-icon>
                  Zurück
                </v-btn>
                <div
                  v-else
                  style="width: 120px"
                ></div>

                <!-- Center: Step indicator -->
                <v-chip
                  v-if="step < 3"
                  color="primary"
                  variant="tonal"
                  class="d-none d-sm-flex"
                  size="large"
                >
                  Schritt {{ step }} von 3
                </v-chip>
                <div
                  v-else
                  style="width: 120px"
                ></div>

                <!-- Right: Next/Finish button -->
                <div class="d-flex align-center">
                  <v-btn
                    v-if="step < 3"
                    variant="elevated"
                    color="primary"
                    size="x-large"
                    rounded="pill"
                    class="px-8"
                    @click="next"
                  >
                    Weiter
                    <v-icon end>mdi-arrow-right</v-icon>
                  </v-btn>
                  <v-btn
                    v-if="step === 3"
                    variant="elevated"
                    color="success"
                    size="x-large"
                    rounded="pill"
                    class="px-10 pulse-button"
                    @click="finish"
                  >
                    <v-icon
                      start
                      size="28"
                      >mdi-check-circle</v-icon
                    >
                    Fertig!
                  </v-btn>
                </div>
              </div>
            </v-col>
          </v-row>
        </v-container>
      </v-footer>
    </v-container>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, onMounted, watch, nextTick } from 'vue';
import confetti from 'canvas-confetti';
import QrCodeCard from '@/components/profilePicture/QrCodeCard.vue';
import InstallationStepCard from '@/components/profilePicture/InstallationStepCard.vue';
import TipCard from '@/components/profilePicture/TipCard.vue';
import TimelineInstructionItem from '@/components/profilePicture/TimelineInstructionItem.vue';
import RequestStatusCard from '@/components/profilePicture/RequestStatusCard.vue';
import {
  validateRequestToken,
  type TokenValidationResponse,
} from '@/api/alovaMethods/profilePictureReview';

defineOptions({
  name: 'DeeplinkLandingPage',
});

const { token } = defineProps({
  token: { type: String, required: true },
});

const stepTwoInstructions = [
  'Öffnen Sie die Kamera-App auf Ihrem Smartphone.',
  'Richten Sie die Kamera auf den nebenstehenden QR-Code.',
  'Bestätigen Sie das Öffnen des Links in der GIBZ App.',
];

const stepThreeAdvices = [
  {
    title: 'Gleichmässige Belichtung',
    subtitle:
      'Achten Sie darauf, dass Ihr Gesicht gleichmässig beleuchtet ist und keine Schatten auf Ihr Gesicht fallen. Stellen Sie sich dafür beispielsweise frontal vor ein Fenster.',
  },
  {
    title: 'Smartphone mit beiden Händen halten',
    subtitle: 'Halten Sie Ihr Smartphone mit beiden Händen auf Augenhöhe.',
  },
  {
    title: 'Keine Kopfbedeckung',
    subtitle: 'Caps oder Mützen sind auf dem Foto nicht erlaubt.',
  },
];

const step = ref(1);
const smartphoneOS = ref(-1);
const isValidating = ref(true);
const isExpired = ref(false);
const isAlreadyProcessed = ref(false);
const notFound = ref(false);
const tokenData = ref<TokenValidationResponse | null>(null);
const errorMessage = ref<string>('');

// Asset URLs
const appStoreImageUrl = new URL('@/assets/images/appstore.svg', import.meta.url).href;
const playStoreImageUrl = new URL('@/assets/images/playstore.svg', import.meta.url).href;
const gibzAppIconUrl = new URL('@/assets/images/gibzAppIcon.png', import.meta.url).href;

// Helper function for avatar colors
function getAvatarColor(index: number): string {
  if (index === 0) {
    // First step: grey for better contrast with both icons
    return smartphoneOS.value === 0 ? 'primary' : '#607D8B'; // grey for Android
  }
  if (index === 1) return 'secondary';
  return 'primary'; // Changed from 'success' to 'primary' for consistency
}

// Computed property for dynamic installation steps
const installationSteps = computed(() => [
  {
    title: `Öffnen Sie die App ${appStoreName.value}`,
    subtitle: 'Tippen Sie auf das Icon auf Ihrem Smartphone',
    image: smartphoneOS.value === 0 ? appStoreImageUrl : playStoreImageUrl,
    icon: null,
    needsPadding: smartphoneOS.value === 1, // Add padding for Play Store icon
    isFirstIcon: true, // Mark as first icon for special styling
  },
  {
    title: 'Suchen Sie nach "GIBZ"',
    subtitle: 'Geben Sie im Suchfeld den Begriff ein',
    image: null,
    icon: 'mdi-magnify',
    needsPadding: false,
    isFirstIcon: false,
  },
  {
    title: 'Installieren Sie die GIBZ App',
    subtitle: 'Tippen Sie auf "Laden" oder "Installieren"',
    image: gibzAppIconUrl,
    icon: null,
    needsPadding: false,
    isFirstIcon: false,
    isLastIcon: true, // Mark as last icon for special styling
  },
]);

// Debug watcher
watch(step, (newVal, oldVal) => {
  console.log('Step changed from', oldVal, 'to', newVal);
});

onMounted(async () => {
  try {
    const response = await validateRequestToken(token);
    tokenData.value = response;

    console.log('Token validation response:', response);

    // Check if this is a 404 error response (has status property = 404)
    if ('status' in response && (response as { status: number }).status === 404) {
      notFound.value = true;
      errorMessage.value = 'Diese Anfrage wurde nicht gefunden.';
    }
    // Check for status codes in the response
    else if (response.statusCode === 12) {
      // Expired (status code 12)
      isExpired.value = true;
      errorMessage.value = response.statusMessage || 'Diese Anfrage ist abgelaufen.';
    } else if (response.statusCode === 11) {
      // Already processed (status code 11)
      isAlreadyProcessed.value = true;
      errorMessage.value = response.statusMessage || 'Diese Anfrage wurde bereits bearbeitet.';
    } else if (!response.firstName || !response.lastName) {
      // Missing required fields - might be an error response
      if (response.statusCode) {
        isExpired.value = true;
        errorMessage.value = response.statusMessage || 'Ein Fehler ist aufgetreten.';
      }
    }
    // Otherwise it's valid (200 OK) - firstName and lastName are present
  } catch (error: unknown) {
    console.error('Failed to validate token:', error);

    const err = error as {
      response?: { status?: number; data?: unknown };
      statusCode?: number;
      status?: number;
      data?: unknown;
    };
    console.log('Error details:', {
      status: err?.response?.status,
      statusCode: err?.statusCode,
      data: err?.data,
      responseData: err?.response?.data,
      fullError: error,
    });

    // Check HTTP status codes - try multiple possible error structures
    const status = err?.response?.status || err?.statusCode || err?.status;
    const responseData = err?.response?.data || err?.data;

    if (status === 404) {
      // Not Found
      notFound.value = true;
      errorMessage.value = 'Diese Anfrage wurde nicht gefunden.';
    } else if (status === 410) {
      // Gone (Expired)
      isExpired.value = true;
      if (responseData && typeof responseData === 'object') {
        tokenData.value = responseData as TokenValidationResponse;
        errorMessage.value =
          (responseData as TokenValidationResponse).statusMessage ||
          'Diese Anfrage ist abgelaufen.';
      } else {
        errorMessage.value = 'Diese Anfrage ist abgelaufen.';
      }
    } else if (status === 409) {
      // Conflict (Already Processed)
      isAlreadyProcessed.value = true;
      if (responseData && typeof responseData === 'object') {
        tokenData.value = responseData as TokenValidationResponse;
        errorMessage.value =
          (responseData as TokenValidationResponse).statusMessage ||
          'Diese Anfrage wurde bereits bearbeitet.';
      } else {
        errorMessage.value = 'Diese Anfrage wurde bereits bearbeitet.';
      }
    } else {
      // Other errors
      isExpired.value = true;
      errorMessage.value = 'Ein Fehler ist aufgetreten. Bitte versuchen Sie es später erneut.';
    }
  } finally {
    isValidating.value = false;
  }
});

const appStoreName = computed(() => {
  return smartphoneOS.value == 0 ? 'App Store' : 'Play Store';
});

const storeLink = computed(() => {
  return smartphoneOS.value === 0
    ? 'https://apps.apple.com/ch/app/gibz/id1519809150?itsct=apps_box_link&itscg=30200'
    : 'https://play.google.com/store/apps/details?id=ch.gibz';
});

const cameraImageUrl = new URL('@/assets/images/camera.svg', import.meta.url).href;

function prev() {
  console.log('prev() called, current step:', step.value);
  if (step.value > 1) {
    step.value--;
    console.log('After prev(), step is now:', step.value);
  } else {
    console.log('Cannot go back, already at step 1');
  }
}

function next() {
  console.log('next() called, current step:', step.value);
  if (step.value < 3) {
    step.value++;
    console.log('After next(), step is now:', step.value);

    // Smooth scroll to bottom after step change
    // Use nextTick and longer timeout to ensure content is rendered
    nextTick(() => {
      setTimeout(() => {
        window.scrollTo({
          top: document.documentElement.scrollHeight,
          behavior: 'smooth',
        });
      }, 300);
    });
  } else {
    console.log('Cannot go forward, already at step 3');
  }
}

function formatExpiryDate(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('de-CH', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });
}

function finish() {
  const count = 200;
  const defaults = {
    origin: { y: 0.7 },
  };

  function fire(particleRatio: number, opts: confetti.Options) {
    confetti({
      ...defaults,
      ...opts,
      particleCount: Math.floor(count * particleRatio),
    });
  }

  fire(0.25, {
    spread: 26,
    startVelocity: 55,
  });
  fire(0.2, {
    spread: 60,
  });
  fire(0.35, {
    spread: 100,
    decay: 0.91,
    scalar: 0.8,
  });
  fire(0.1, {
    spread: 120,
    startVelocity: 25,
    decay: 0.92,
    scalar: 1.2,
  });
  fire(0.1, {
    spread: 120,
    startVelocity: 45,
  });
}
</script>

<style scoped lang="scss">
.page-wrapper {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #e8eef5 100%);
}

.landing-page {
  background: transparent;
}

.header-section {
  .icon-wrapper {
    display: inline-block;
    padding: 20px;
    background: linear-gradient(135deg, rgba(25, 118, 210, 0.1) 0%, rgba(25, 118, 210, 0.05) 100%);
    border-radius: 50%;
  }

  .gradient-text {
    background: linear-gradient(135deg, #1976d2 0%, #0d47a1 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }

  .greeting-text {
    color: #1976d2;
    font-weight: 500;
    animation: fadeInUp 0.6s ease-out;
  }
}

@keyframes pulse {
  0%,
  100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.05);
  }
}

.pulse-animation {
  animation: pulse 2s ease-in-out infinite;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.loading-card {
  background: linear-gradient(135deg, #ffffff 0%, #f8f9fa 100%);
  border: 2px solid rgba(25, 118, 210, 0.1);
  animation: fadeInUp 0.5s ease-out;
}

.progress-card {
  background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%);
  color: white;

  :deep(.v-progress-linear) {
    background-color: rgba(255, 255, 255, 0.3) !important;
  }

  .text-caption,
  .text-h6 {
    color: white !important;
  }
}

.stepper-enhanced {
  background: transparent !important;

  :deep(.v-stepper-vertical-item) {
    background: transparent !important;
    box-shadow: none !important;

    .v-stepper-vertical-item__avatar {
      background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%) !important;
      box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
    }

    &.v-stepper-vertical-item--complete .v-stepper-vertical-item__avatar {
      box-shadow: 0 4px 12px rgba(25, 118, 210, 0.4);
    }
  }
}

.step-content-card {
  background: linear-gradient(135deg, #ffffff 0%, #f8f9fa 100%);
  border: 2px solid rgba(25, 118, 210, 0.08);
  border-radius: 16px !important;
  transition: all 0.3s ease;

  &:hover {
    border-color: rgba(25, 118, 210, 0.15);
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.06);
  }
}

.step-header h3 {
  color: #1976d2;
}

.os-tabs :deep(.v-tab) {
  transition: all 0.3s ease;
  border-radius: 12px;

  &.v-tab--selected {
    background: linear-gradient(135deg, rgba(25, 118, 210, 0.1) 0%, rgba(25, 118, 210, 0.05) 100%);
    font-weight: 600;
  }
}

.installation-list {
  padding: 4px;
  background: transparent !important;
}

.scan-timeline :deep(.v-timeline-item) {
  padding-bottom: 24px;

  .v-timeline-divider__dot {
    box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
  }
}

.camera-preview :deep(.v-img) {
  filter: drop-shadow(0 8px 24px rgba(0, 0, 0, 0.12));
  transition: all 0.3s ease;

  &:hover {
    transform: scale(1.03);
    filter: drop-shadow(0 12px 32px rgba(0, 0, 0, 0.16));
  }
}

.navigation-footer {
  background: linear-gradient(
    180deg,
    rgba(255, 255, 255, 0.95) 0%,
    rgba(255, 255, 255, 0.98) 100%
  ) !important;
  backdrop-filter: blur(12px);
  border-top: 2px solid rgba(25, 118, 210, 0.1);

  .v-btn {
    font-weight: 600;
    letter-spacing: 0.5px;
    text-transform: none;
    transition: all 0.3s ease;

    &.v-btn--variant-outlined {
      border-width: 2px;

      &:hover {
        transform: scale(1.05);
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
      }
    }

    &.v-btn--variant-elevated {
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);

      &:hover {
        transform: scale(1.05);
        box-shadow: 0 6px 20px rgba(0, 0, 0, 0.2);
      }
    }
  }

  .pulse-button {
    animation: pulse 2s ease-in-out infinite;

    &:hover {
      animation: none;
    }
  }
}

:deep(.v-stepper-vertical),
:deep(.v-stepper-vertical-item),
:deep(.v-stepper-vertical-item__content),
:deep(.v-expansion-panel),
:deep(.v-expansion-panel-title),
:deep(.v-expansion-panel-text),
:deep(.v-expansion-panels) {
  box-shadow: none !important;
  border: none !important;
  background: transparent !important;
}

:deep(.v-expansion-panel__shadow) {
  display: none !important;
}

@media (max-width: 960px) {
  .header-section {
    .icon-wrapper {
      padding: 15px;
    }

    .gradient-text {
      font-size: 2rem;
    }
  }
}
</style>
