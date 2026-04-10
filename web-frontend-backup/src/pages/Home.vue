<template>
  <div class="home-wrapper">
    <v-container class="py-8 home-page">
      <!-- Hero Section -->
      <v-row
        justify="center"
        class="mb-3 mt-8"
      >
        <v-col
          cols="12"
          md="10"
          lg="8"
        >
          <div class="text-center hero-section">
            <div class="hero-content">
              <div class="app-icon-wrapper mb-5">
                <img
                  src="@/assets/images/gibzAppIcon.png"
                  alt="GIBZ App Icon"
                  class="app-icon"
                />
              </div>
              <h1 class="text-h3 text-md-h2 font-weight-bold mb-3">GIBZ App</h1>
              <p class="text-h6 text-primary font-weight-medium">Von Lernenden für Lernende</p>
            </div>
          </div>
        </v-col>
      </v-row>

      <!-- Download Section -->
      <v-row
        justify="center"
        class="mb-3"
      >
        <v-col
          cols="12"
          md="10"
          lg="8"
        >
          <v-card
            elevation="0"
            rounded="xl"
            class="download-card pa-8"
          >
            <div class="text-center mb-6">
              <h2 class="text-h4 font-weight-bold mb-3">Jetzt herunterladen</h2>
              <p class="text-body-1 text-medium-emphasis">
                Die GIBZ App ist kostenlos für iPhones und Android-Smartphones verfügbar.
              </p>
            </div>

            <div class="d-flex justify-center align-center flex-wrap ga-6">
              <a
                href="https://apps.apple.com/de/app/gibz/id1519809150?itsct=apps_box_badge&amp;itscg=30200"
                target="_blank"
                class="store-link"
              >
                <div class="store-badge-wrapper">
                  <img
                    id="ios-badge"
                    src="@/assets/images/app_store_badge.svg"
                    alt="Download on the App Store"
                  />
                </div>
              </a>
              <a
                href="https://play.google.com/store/apps/details?id=ch.gibz&pcampaignid=pcampaignidMKT-Other-global-all-co-prtnr-py-PartBadge-Mar2515-1"
                target="_blank"
                class="store-link"
              >
                <div class="store-badge-wrapper">
                  <img
                    id="android-badge"
                    alt="Jetzt bei Google Play"
                    src="@/assets/images/play_store_badge.svg"
                  />
                </div>
              </a>
            </div>
          </v-card>
        </v-col>
      </v-row>

      <!-- Features Section -->
      <v-row
        justify="center"
        class="mb-8"
      >
        <v-col
          cols="12"
          md="10"
          lg="8"
        >
          <v-row>
            <v-col
              cols="12"
              md="6"
              class="mb-4"
            >
              <v-card
                elevation="0"
                rounded="xl"
                class="feature-card pa-8 h-100"
              >
                <div class="text-center mb-4">
                  <v-avatar
                    color="primary"
                    size="80"
                    class="feature-icon"
                  >
                    <v-icon
                      size="45"
                      color="white"
                      >mdi-account-group</v-icon
                    >
                  </v-avatar>
                </div>
                <div class="text-center">
                  <h3 class="text-h5 font-weight-bold mb-4">Von Lernenden entwickelt</h3>
                  <p class="text-body-1 text-medium-emphasis">
                    Die GIBZ App wird von lernenden Applikationsentwicklerinnen und
                    Applikationsentwickler im Rahmen des Berufskundeunterrichts oder auf
                    freiwilliger Basis entwickelt.
                  </p>
                </div>
              </v-card>
            </v-col>

            <v-col
              cols="12"
              md="6"
              class="mb-4"
            >
              <v-card
                elevation="0"
                rounded="xl"
                class="feature-card pa-8 h-100"
              >
                <div class="text-center mb-4">
                  <v-avatar
                    color="secondary"
                    size="80"
                    class="feature-icon"
                  >
                    <v-icon
                      size="45"
                      color="white"
                      >mdi-cog-outline</v-icon
                    >
                  </v-avatar>
                </div>
                <div class="text-center">
                  <h3 class="text-h5 font-weight-bold mb-4">Administrations-Tool</h3>
                  <p class="text-body-1 text-medium-emphasis">
                    Diese Webseite wird für die Administration der verschiedenen Inhalte der GIBZ
                    App genutzt. Sie wird deshalb primär durch Mitarbeitende des GIBZ genutzt und
                    erfordert ein Login mit entsprechenden Berechtigungen.
                  </p>
                </div>
              </v-card>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup lang="ts">
defineOptions({
  name: 'HomePage',
});
</script>

<style scoped lang="scss">
.home-wrapper {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #e8eef5 100%);
}

.home-page {
  background: transparent;
  padding-top: 3rem;
}

.hero-section {
  animation: fadeInUp 0.8s ease-out;
}

.hero-content {
  display: inline-block;
  width: 100%;
  max-width: 800px;
  padding: 3rem 4rem;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95) 0%, rgba(255, 255, 255, 0.8) 100%);
  border-radius: 32px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  backdrop-filter: blur(10px);
}

.app-icon-wrapper {
  display: inline-block;
  position: relative;
}

.app-icon {
  width: 120px;
  height: 120px;
  border-radius: 24px;
  box-shadow:
    0 8px 24px rgba(25, 118, 210, 0.4),
    0 4px 8px rgba(0, 0, 0, 0.15);
  animation: breatheIcon 3s ease-in-out infinite;
  transition: all 0.3s ease;

  &:hover {
    transform: scale(1.15) rotate(5deg);
    box-shadow:
      0 12px 32px rgba(25, 118, 210, 0.5),
      0 6px 12px rgba(0, 0, 0, 0.2);
  }
}

@keyframes breatheIcon {
  0%,
  100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.05);
  }
}

@keyframes floatIcon {
  0%,
  100% {
    transform: translateY(0px);
  }
  50% {
    transform: translateY(-10px);
  }
}

.hero-avatar {
  box-shadow: 0 6px 20px rgba(25, 118, 210, 0.4);
  animation: pulse 2s ease-in-out infinite;
}

.icon-wrapper {
  display: inline-block;
}

.pulse-animation {
  animation: pulse 2s ease-in-out infinite;
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

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.download-card {
  border: 1px solid rgba(0, 0, 0, 0.12);
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;

  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
  }
}

.store-link {
  text-decoration: none;
  display: inline-flex;
  align-items: center;
}

.store-badge-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;

  &:hover {
    transform: scale(1.05);
  }

  img {
    display: block;
    height: 60px;
    width: auto;
  }
}

#ios-badge {
  border-radius: 13px;
}

.feature-card {
  border: 1px solid rgba(0, 0, 0, 0.12);
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  height: 100%;
  background: linear-gradient(135deg, #ffffff 0%, #fafbfc 100%);

  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
  }
}

.feature-icon {
  box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
  transition: all 0.3s ease;
}

.feature-card:hover .feature-icon {
  transform: scale(1.1);
  box-shadow: 0 6px 16px rgba(25, 118, 210, 0.4);
}

// Responsive adjustments
@media (max-width: 600px) {
  .hero-section {
    h1 {
      font-size: 2rem;
    }

    .icon-wrapper :deep(.v-icon) {
      font-size: 80px !important;
    }
  }

  .store-badge-wrapper img {
    max-width: 200px;
  }
}
</style>
