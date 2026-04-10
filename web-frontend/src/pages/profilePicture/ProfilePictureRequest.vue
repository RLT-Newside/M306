<template>
  <div class="profile-picture-wrapper">
    <v-container
      fluid
      class="profile-picture-page"
    >
      <PageTitle
        title="Portraitfoto Anfragen"
        class="mb-6"
      />

      <v-card
        class="mb-3 search-card"
        elevation="0"
        rounded="xl"
      >
        <v-card-text class="pa-6">
          <v-row align="center">
            <v-col
              cols="12"
              md="8"
            >
              <v-text-field
                v-model="search"
                label="Suchen nach GIB-Nr., Kürzel, Vorname, Nachname"
                prepend-inner-icon="mdi-magnify"
                variant="outlined"
                density="comfortable"
                hide-details
                clearable
                @click:clear="search = ''"
              />
            </v-col>
            <v-col
              cols="12"
              md="4"
            >
              <v-chip-group
                v-model="statusFilter"
                column
                filter
              >
                <v-chip
                  value="Pending"
                  color="orange"
                  variant="outlined"
                >
                  Pendent
                </v-chip>
                <v-chip
                  value="Fulfilled"
                  color="primary"
                  variant="outlined"
                >
                  Abgeschlossen
                </v-chip>
                <v-chip
                  value="Expired"
                  color="error"
                  variant="outlined"
                >
                  Abgelaufen
                </v-chip>
              </v-chip-group>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <v-card
        class="data-table-card"
        elevation="0"
        rounded="xl"
      >
        <v-data-table-server
          v-model:items-per-page="itemsPerPage"
          :items="requestData?.requests"
          :items-length="requestData?.total ?? 0"
          :headers="headers"
          :search="search"
          :loading="requestsAreLoading"
          items-per-page-text="Einträge pro Seite"
          hover
          @update:options="handleTableUpdate"
          @click:row="clickRow"
        >
          <!-- eslint-disable vue/valid-v-slot -->
          <template #item.requestSequenceIndex="{ item }">
            <v-chip
              size="small"
              color="primary"
              variant="tonal"
            >
              #{{ getRequestSequenceValue(item) }}
            </v-chip>
          </template>

          <template #item.requestState="{ item }">
            <v-chip
              :color="getRequestStateColor(item.requestState)"
              size="small"
              variant="flat"
            >
              {{ getRequestStateDisplayLabel(item.requestState) }}
            </v-chip>
          </template>

          <template #item.expiryDate="{ item }">
            <span :class="{ 'text-error font-weight-bold': isExpired(item.expiryDate) }">
              {{ getFormattedExpiryDate(item) }}
            </span>
          </template>

          <template #item.person="{ item }">
            <div class="d-flex flex-column">
              <span class="font-weight-medium">
                {{ getPersonName(item) }}
              </span>
              <span class="text-caption text-medium-emphasis">
                {{ getPersonUserName(item) }}
              </span>
            </div>
          </template>

          <template #item.actions="{ item }">
            <v-btn
              icon="mdi-eye"
              size="small"
              variant="text"
              @click.stop="openDetails(item.id)"
            />
          </template>
          <!-- eslint-enable vue/valid-v-slot -->
        </v-data-table-server>
      </v-card>

      <v-expansion-panels class="mt-4">
        <v-expansion-panel>
          <v-expansion-panel-title>
            <div class="d-flex align-center">
              <v-icon
                class="mr-2"
                color="info"
                >mdi-information</v-icon
              >
              <span class="text-subtitle-2">Datenaufbewahrung & Automatische Löschung</span>
            </div>
          </v-expansion-panel-title>
          <v-expansion-panel-text>
            <div class="text-body-2 mb-3">
              <strong>Aktueller Stand ({{ retentionDates.today }}):</strong>
            </div>

            <v-row
              dense
              class="mb-2"
            >
              <v-col
                cols="12"
                md="6"
              >
                <v-card
                  variant="outlined"
                  class="pa-3"
                >
                  <div class="text-subtitle-2 mb-2 d-flex align-center">
                    <v-icon
                      size="small"
                      color="error"
                      class="mr-2"
                      >mdi-calendar-remove</v-icon
                    >
                    Abgelaufene Anfragen
                  </div>
                  <div class="text-body-2 mb-2">Löschfrist: 30 Tage nach Ablauf</div>
                  <v-divider class="my-2" />
                  <div class="text-caption">
                    <div class="mb-1">
                      <v-chip
                        size="x-small"
                        color="error"
                        variant="flat"
                        class="mr-1"
                        >Gelöscht</v-chip
                      >
                      Ablaufdatum <strong>bis {{ retentionDates.expiredCutoff }}</strong>
                    </div>
                    <div>
                      <v-chip
                        size="x-small"
                        color="success"
                        variant="flat"
                        class="mr-1"
                        >Verfügbar</v-chip
                      >
                      Ablaufdatum <strong>ab {{ retentionDates.expiredCutoffNext }}</strong>
                    </div>
                  </div>
                </v-card>
              </v-col>

              <v-col
                cols="12"
                md="6"
              >
                <v-card
                  variant="outlined"
                  class="pa-3"
                >
                  <div class="text-subtitle-2 mb-2 d-flex align-center">
                    <v-icon
                      size="small"
                      color="success"
                      class="mr-2"
                      >mdi-check-circle</v-icon
                    >
                    Abgeschlossene Anfragen
                  </div>
                  <div class="text-body-2 mb-2">Löschfrist: 90 Tage nach Upload</div>
                  <v-divider class="my-2" />
                  <div class="text-caption">
                    <div class="mb-1">
                      <v-chip
                        size="x-small"
                        color="error"
                        variant="flat"
                        class="mr-1"
                        >Gelöscht</v-chip
                      >
                      Upload-Datum <strong>bis {{ retentionDates.fulfilledCutoff }}</strong>
                    </div>
                    <div>
                      <v-chip
                        size="x-small"
                        color="success"
                        variant="flat"
                        class="mr-1"
                        >Verfügbar</v-chip
                      >
                      Upload-Datum <strong>ab {{ retentionDates.fulfilledCutoffNext }}</strong>
                    </div>
                  </div>
                </v-card>
              </v-col>
            </v-row>

            <v-divider class="my-3" />

            <div class="text-body-2">
              <div class="mb-2">
                <strong>Weitere Regelungen:</strong>
              </div>
              <ul class="pl-4 mb-2">
                <li class="mb-1">
                  <strong>Portraitfotos:</strong> Werden automatisch mit ihren Anfragen gelöscht
                </li>
                <li class="mb-1">
                  <strong>Personendaten:</strong> Bleiben nur solange eine zugehörige Anfrage
                  existiert
                </li>
                <li>
                  <strong>Anfrageketten:</strong> Werden als Ganzes gelöscht, sobald alle Anfragen
                  der Kette die Löschkriterien erfüllen
                </li>
              </ul>
            </div>

            <v-alert
              type="info"
              variant="text"
              density="compact"
              class="mt-2 text-caption"
            >
              Die automatische Bereinigung erfolgt stündlich. Alle angezeigten Daten entsprechen den
              aktuellen Löschkriterien.
            </v-alert>
          </v-expansion-panel-text>
        </v-expansion-panel>
      </v-expansion-panels>

      <v-dialog
        v-model="showDialog"
        max-width="1200"
        scrollable
      >
        <v-card
          v-if="!requestDetailsAreLoading && requestDetails"
          elevation="0"
          class="detail-dialog-card"
          rounded="lg"
        >
          <v-toolbar
            color="primary"
            class="detail-toolbar"
            rounded="t-lg"
          >
            <template #prepend>
              <v-avatar
                color="white"
                size="40"
                class="ml-2"
              >
                <v-icon
                  color="primary"
                  size="24"
                >
                  mdi-account-circle
                </v-icon>
              </v-avatar>
            </template>
            <v-toolbar-title>
              <div class="text-h6">
                {{ requestDetails.request.person.lastName }},
                {{ requestDetails.request.person.firstName }}
              </div>
              <div class="text-caption opacity-90">
                {{ requestDetails.request.person.userName }}
              </div>
            </v-toolbar-title>
            <v-spacer />
            <v-chip
              :color="getRequestStateColor(requestDetails.request.requestState)"
              variant="flat"
              class="mr-2"
            >
              {{ getRequestStateDisplayLabel(requestDetails.request.requestState) }}
            </v-chip>
            <v-btn
              icon="mdi-close"
              variant="text"
              @click="showDialog = false"
            />
          </v-toolbar>

          <v-tabs
            v-model="activeTab"
            bg-color="white"
            color="primary"
            class="detail-tabs"
            grow
          >
            <v-tab
              value="overview"
              prepend-icon="mdi-information"
            >
              Übersicht
            </v-tab>
            <v-tab
              value="person"
              prepend-icon="mdi-account"
            >
              Person
            </v-tab>
            <v-tab
              value="picture"
              :disabled="!requestDetails.profilePicture"
              prepend-icon="mdi-image"
            >
              Portraitfoto
            </v-tab>
            <v-tab
              value="review"
              :disabled="!requestDetails.profilePictureReview"
              prepend-icon="mdi-check-circle"
            >
              Review
            </v-tab>
          </v-tabs>

          <v-divider />

          <v-card-text class="pa-6">
            <v-window v-model="activeTab">
              <!-- Overview Tab -->
              <v-window-item
                value="overview"
                transition="fade-transition"
                reverse-transition="fade-transition"
              >
                <v-row>
                  <v-col
                    cols="12"
                    md="6"
                  >
                    <v-card
                      variant="flat"
                      class="detail-info-card"
                      rounded="lg"
                    >
                      <v-card-title class="d-flex align-center pa-4 bg-primary-lighten">
                        <v-icon
                          class="mr-2"
                          color="primary"
                        >
                          mdi-file-document
                        </v-icon>
                        <span class="text-h6">Anfrage Details</span>
                      </v-card-title>
                      <v-card-text class="pa-4">
                        <v-list
                          density="compact"
                          class="detail-list"
                        >
                          <DetailListItem
                            icon="mdi-pound"
                            label="Anfrage-Nr."
                          >
                            <v-chip
                              size="small"
                              color="primary"
                              variant="tonal"
                              class="mt-1"
                            >
                              #{{ requestDetails.request.requestSequenceIndex + 1 }}
                            </v-chip>
                          </DetailListItem>

                          <v-divider class="my-2" />

                          <DetailListItem
                            v-if="requestDetails.request.previousRequestId"
                            icon="mdi-history"
                            label="Vorherige Anfrage"
                          >
                            <div class="d-flex align-center">
                              <span class="text-caption text-truncate mr-2">
                                {{ requestDetails.request.previousRequestId }}
                              </span>
                              <v-btn
                                icon="mdi-content-copy"
                                size="x-small"
                                variant="text"
                                density="compact"
                                @click="copyToClipboard(requestDetails.request.previousRequestId)"
                              />
                              <v-btn
                                icon="mdi-open-in-new"
                                size="x-small"
                                variant="text"
                                density="compact"
                                @click="openRequest(requestDetails.request.previousRequestId)"
                              />
                            </div>
                          </DetailListItem>

                          <v-divider
                            v-if="requestDetails.request.previousRequestId"
                            class="my-2"
                          />

                          <DetailListItem
                            icon="mdi-calendar"
                            label="Gültig bis"
                          >
                            <span
                              :class="{
                                'text-error': isExpired(requestDetails.request.expiryDate),
                              }"
                            >
                              {{ formatDateTime(requestDetails.request.expiryDate, false) }}
                            </span>
                          </DetailListItem>
                        </v-list>
                      </v-card-text>
                    </v-card>
                  </v-col>

                  <v-col
                    cols="12"
                    md="6"
                  >
                    <v-card
                      variant="flat"
                      class="detail-info-card"
                      rounded="lg"
                    >
                      <v-card-title class="d-flex align-center pa-4 bg-primary-lighten">
                        <v-icon
                          class="mr-2"
                          color="primary"
                        >
                          mdi-link
                        </v-icon>
                        <span class="text-h6">Upload Link</span>
                      </v-card-title>
                      <v-card-text class="pa-4 pt-4">
                        <v-text-field
                          :model-value="getUploadUrl(requestDetails.request.token)"
                          readonly
                          variant="outlined"
                          density="comfortable"
                          hide-details
                        >
                          <template #append-inner>
                            <v-btn
                              icon="mdi-content-copy"
                              size="small"
                              variant="text"
                              @click="copyToClipboard(getUploadUrl(requestDetails.request.token))"
                            />
                            <v-btn
                              icon="mdi-open-in-new"
                              size="small"
                              variant="text"
                              @click="openInNewTab(getUploadUrl(requestDetails.request.token))"
                            />
                          </template>
                        </v-text-field>
                      </v-card-text>
                    </v-card>

                    <v-card
                      v-if="requestDetails.request.requestState === RequestState.Pending"
                      variant="flat"
                      class="detail-info-card mt-3"
                      rounded="lg"
                    >
                      <v-card-title class="d-flex align-center pa-4 bg-primary-lighten">
                        <v-icon
                          class="mr-2"
                          color="primary"
                        >
                          mdi-bell-ring
                        </v-icon>
                        <span class="text-h6">Erinnerung</span>
                      </v-card-title>
                      <v-card-text class="pa-4 pt-4">
                        <v-btn
                          :disabled="sendingReminder || reminderSent"
                          :loading="sendingReminder"
                          color="primary"
                          variant="elevated"
                          prepend-icon="mdi-bell-ring"
                          block
                          @click="handleSendReminder"
                        >
                          {{ reminderSent ? 'Erinnerung gesendet' : 'Erinnerung senden' }}
                        </v-btn>
                        <v-alert
                          v-if="reminderSent"
                          type="success"
                          variant="tonal"
                          density="compact"
                          class="mt-3"
                        >
                          Die Erinnerung wurde erfolgreich versendet.
                        </v-alert>
                      </v-card-text>
                    </v-card>

                    <v-card
                      v-if="requestDetails.profilePicture"
                      variant="flat"
                      class="detail-info-card mt-3"
                      rounded="lg"
                    >
                      <v-card-title class="d-flex align-center pa-4 bg-primary-lighten">
                        <v-icon
                          class="mr-2"
                          color="primary"
                        >
                          mdi-clock
                        </v-icon>
                        <span class="text-h6">Timeline</span>
                      </v-card-title>
                      <v-card-text class="pa-4">
                        <v-timeline
                          density="compact"
                          side="end"
                          truncate-line="both"
                        >
                          <v-timeline-item
                            dot-color="primary"
                            size="small"
                            icon="mdi-upload"
                            class="timeline-item-clickable"
                            @click="activeTab = 'picture'"
                          >
                            <template #opposite>
                              <div class="d-flex flex-column align-end">
                                <span class="text-caption font-weight-bold">Upload</span>
                                <v-chip
                                  size="x-small"
                                  color="primary"
                                  variant="tonal"
                                >
                                  Hochgeladen
                                </v-chip>
                              </div>
                            </template>
                            <div class="text-body-2">
                              {{
                                formatDateTime(requestDetails.profilePicture.uploadDateTime, true)
                              }}
                            </div>
                          </v-timeline-item>

                          <v-timeline-item
                            v-if="requestDetails.profilePictureReview"
                            :dot-color="
                              requestDetails.profilePictureReview.reviewState === 2
                                ? 'success'
                                : 'error'
                            "
                            :icon="
                              requestDetails.profilePictureReview.reviewState === 2
                                ? 'mdi-check-circle'
                                : 'mdi-close-circle'
                            "
                            size="small"
                            class="timeline-item-clickable"
                            @click="activeTab = 'review'"
                          >
                            <template #opposite>
                              <div class="d-flex flex-column align-end">
                                <span class="text-caption font-weight-bold">Review</span>
                                <v-chip
                                  size="x-small"
                                  :color="
                                    requestDetails.profilePictureReview.reviewState === 2
                                      ? 'success'
                                      : 'error'
                                  "
                                  variant="tonal"
                                >
                                  {{
                                    requestDetails.profilePictureReview.reviewState === 2
                                      ? 'Akzeptiert'
                                      : 'Abgelehnt'
                                  }}
                                </v-chip>
                              </div>
                            </template>
                            <div class="text-body-2">
                              {{
                                formatDateTime(
                                  requestDetails.profilePictureReview.reviewDateTime,
                                  true
                                )
                              }}
                            </div>
                            <div class="text-caption text-medium-emphasis mt-1">
                              von {{ requestDetails.profilePictureReview.reviewerUserName }}
                            </div>
                          </v-timeline-item>
                        </v-timeline>
                      </v-card-text>
                    </v-card>
                  </v-col>
                </v-row>
              </v-window-item>

              <!-- Person Tab -->
              <v-window-item
                value="person"
                transition="fade-transition"
                reverse-transition="fade-transition"
              >
                <v-row>
                  <v-col
                    cols="12"
                    md="6"
                  >
                    <v-card
                      variant="flat"
                      class="detail-info-card"
                      rounded="lg"
                    >
                      <v-card-title class="d-flex align-center pa-4 bg-primary-lighten">
                        <v-icon
                          class="mr-2"
                          color="primary"
                        >
                          mdi-account
                        </v-icon>
                        <span class="text-h6">Persönliche Daten</span>
                      </v-card-title>
                      <v-card-text class="pa-4">
                        <v-list
                          density="compact"
                          class="detail-list"
                        >
                          <DetailListItem
                            icon="mdi-account"
                            label="Nachname"
                          >
                            {{ requestDetails.request.person.lastName }}
                          </DetailListItem>

                          <v-divider class="my-2" />

                          <DetailListItem
                            icon="mdi-account"
                            label="Vorname"
                          >
                            {{ requestDetails.request.person.firstName }}
                          </DetailListItem>

                          <v-divider class="my-2" />

                          <DetailListItem
                            icon="mdi-at"
                            label="Kürzel"
                          >
                            {{ requestDetails.request.person.userName }}
                          </DetailListItem>

                          <v-divider class="my-2" />

                          <DetailListItem
                            icon="mdi-identifier"
                            label="GIB-Nr."
                          >
                            <span class="mr-2">
                              {{ requestDetails.request.person.publicIdentifier }}
                            </span>
                            <v-btn
                              icon="mdi-content-copy"
                              size="x-small"
                              variant="text"
                              density="compact"
                              @click="
                                copyToClipboard(requestDetails.request.person.publicIdentifier)
                              "
                            />
                          </DetailListItem>
                        </v-list>
                      </v-card-text>
                    </v-card>
                  </v-col>

                  <v-col
                    cols="12"
                    md="6"
                  >
                    <v-card
                      variant="flat"
                      class="detail-info-card"
                      rounded="lg"
                    >
                      <v-card-title class="d-flex align-center pa-4 bg-primary-lighten">
                        <v-icon
                          class="mr-2"
                          color="primary"
                        >
                          mdi-school
                        </v-icon>
                        <span class="text-h6">Klasse</span>
                      </v-card-title>
                      <v-card-text class="pa-4">
                        <v-list
                          density="compact"
                          class="detail-list"
                        >
                          <DetailListItem
                            icon="mdi-school"
                            label="Jahrgang"
                          >
                            {{ requestDetails.request.cohort.uniqueName }}
                          </DetailListItem>

                          <v-divider class="my-2" />

                          <DetailListItem
                            icon="mdi-school"
                            label="Lehrjahr"
                          >
                            {{ requestDetails.request.cohort.displayName }}
                          </DetailListItem>
                        </v-list>
                      </v-card-text>
                    </v-card>
                  </v-col>
                </v-row>
              </v-window-item>

              <!-- Picture Tab -->
              <v-window-item
                value="picture"
                transition="fade-transition"
                reverse-transition="fade-transition"
              >
                <v-row v-if="requestDetails.profilePicture">
                  <v-col
                    cols="12"
                    md="6"
                  >
                    <v-card
                      variant="flat"
                      class="detail-info-card"
                      rounded="lg"
                    >
                      <v-card-title class="d-flex align-center pa-4 bg-primary-lighten">
                        <v-icon
                          class="mr-2"
                          color="primary"
                        >
                          mdi-image
                        </v-icon>
                        <span class="text-h6">Vorschau</span>
                      </v-card-title>
                      <v-card-text class="text-center pa-4">
                        <v-img
                          :src="requestDetails.profilePicture.downloadURL"
                          max-height="400"
                          contain
                          class="rounded-lg"
                        >
                          <template #placeholder>
                            <div class="d-flex align-center justify-center fill-height">
                              <v-progress-circular
                                indeterminate
                                color="primary"
                              />
                            </div>
                          </template>
                        </v-img>
                      </v-card-text>
                    </v-card>
                  </v-col>

                  <v-col
                    cols="12"
                    md="6"
                  >
                    <v-card
                      variant="flat"
                      class="detail-info-card"
                      rounded="lg"
                    >
                      <v-card-title class="d-flex align-center pa-4 bg-primary-lighten">
                        <v-icon
                          class="mr-2"
                          color="primary"
                        >
                          mdi-information
                        </v-icon>
                        <span class="text-h6">Bild Details</span>
                      </v-card-title>
                      <v-card-text class="pa-4">
                        <v-list
                          density="compact"
                          class="detail-list"
                        >
                          <DetailListItem
                            icon="mdi-clock-outline"
                            label="Upload Zeitpunkt"
                          >
                            {{ formatDateTime(requestDetails.profilePicture.uploadDateTime, true) }}
                          </DetailListItem>

                          <v-divider class="my-2" />

                          <DetailListItem
                            icon="mdi-download"
                            label="Download"
                          >
                            <v-btn
                              :href="requestDetails.profilePicture.downloadURL"
                              target="_blank"
                              color="primary"
                              variant="tonal"
                              size="small"
                              prepend-icon="mdi-download"
                            >
                              Bild herunterladen
                            </v-btn>
                          </DetailListItem>
                        </v-list>
                      </v-card-text>
                    </v-card>
                  </v-col>
                </v-row>
              </v-window-item>

              <!-- Review Tab -->
              <v-window-item
                value="review"
                transition="fade-transition"
                reverse-transition="fade-transition"
              >
                <v-row v-if="requestDetails.profilePictureReview">
                  <v-col
                    cols="12"
                    md="6"
                  >
                    <v-card
                      variant="flat"
                      class="detail-info-card"
                      rounded="lg"
                    >
                      <v-card-title class="d-flex align-center pa-4 bg-primary-lighten">
                        <v-icon
                          class="mr-2"
                          color="primary"
                        >
                          mdi-check-circle
                        </v-icon>
                        <span class="text-h6">Review Details</span>
                      </v-card-title>
                      <v-card-text class="pa-4">
                        <v-list
                          density="compact"
                          class="detail-list"
                        >
                          <DetailListItem
                            icon="mdi-clock-check-outline"
                            label="Review Zeitpunkt"
                          >
                            {{
                              formatDateTime(
                                requestDetails.profilePictureReview.reviewDateTime,
                                true
                              )
                            }}
                          </DetailListItem>

                          <v-divider class="my-2" />

                          <DetailListItem
                            icon="mdi-account-circle"
                            label="Review durch"
                          >
                            {{ requestDetails.profilePictureReview.reviewerUserName }}
                          </DetailListItem>

                          <v-divider class="my-2" />

                          <DetailListItem
                            icon="mdi-shield-check"
                            label="Review Status"
                          >
                            <v-chip
                              :color="
                                requestDetails.profilePictureReview.reviewState === 2
                                  ? 'success'
                                  : 'error'
                              "
                              size="small"
                              variant="flat"
                            >
                              {{
                                getReviewStateLabel(requestDetails.profilePictureReview.reviewState)
                              }}
                            </v-chip>
                          </DetailListItem>

                          <template v-if="requestDetails.rejectReason">
                            <v-divider class="my-2" />

                            <DetailListItem
                              icon="mdi-alert-circle"
                              label="Ablehnungsgrund"
                            >
                              <v-chip
                                color="error"
                                size="small"
                                variant="tonal"
                              >
                                {{ requestDetails.rejectReason.title }}
                              </v-chip>
                            </DetailListItem>
                          </template>
                        </v-list>
                      </v-card-text>
                    </v-card>
                  </v-col>
                </v-row>
              </v-window-item>
            </v-window>
          </v-card-text>

          <v-divider />

          <v-card-actions>
            <v-spacer />
            <v-btn
              color="primary"
              variant="text"
              @click="showDialog = false"
            >
              Schliessen
            </v-btn>
          </v-card-actions>
        </v-card>

        <v-card v-else>
          <v-card-text class="text-center py-12">
            <v-progress-circular
              indeterminate
              color="primary"
              size="64"
            />
            <p class="mt-4 text-body-1">Lade Details...</p>
          </v-card-text>
        </v-card>
      </v-dialog>
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import { useRequest } from 'alova/client';
import {
  getRequestDetails,
  getRequests,
  sendReminder,
} from '@/api/alovaMethods/profilePictureReview';
import PageTitle from '@/components/PageTitle.vue';
import DetailListItem from '@/components/profilePicture/DetailListItem.vue';
import type { ProfilePictureRequest } from '@/models/profilePictureReview/profilePicture';
import { getReviewStateLabel } from '@/models/profilePictureReview/ReviewState';
import { RequestState } from '@/models/profilePictureReview/RequestState';

// Extended type for requests that include person data from API
interface ProfilePictureRequestWithPerson extends ProfilePictureRequest {
  person: {
    lastName: string;
    firstName: string;
    userName: string;
    publicIdentifier: string;
  };
}

const {
  data: requestData,
  loading: requestsAreLoading,
  send: fetchRequests,
} = useRequest(
  ({
    page,
    itemsPerPage,
    search,
    state,
  }: {
    page: number;
    itemsPerPage: number;
    search: string;
    state: string;
  }) => getRequests(page, itemsPerPage, search, state),
  { immediate: false }
);

const {
  data: requestDetails,
  loading: requestDetailsAreLoading,
  send: fetchRequestDetails,
} = useRequest((requestId: string) => getRequestDetails(requestId), { immediate: false });

const itemsPerPage = ref(10);
const search = ref('');
const showDialog = ref(false);
const activeTab = ref('overview');
const statusFilter = ref<string | null>(null);
const sendingReminder = ref(false);
const reminderSent = ref(false);

const uploadLinkBase = computed(() => {
  return window.location.origin + '/profile-picture/';
});

// Calculate retention reference dates
const retentionDates = computed(() => {
  const today = new Date();

  // For expired requests: Requests that expired before (today - 30 days) are being deleted
  const expiredCutoffDate = new Date(today);
  expiredCutoffDate.setDate(expiredCutoffDate.getDate() - 30);

  // Next day after cutoff (first day that's still available)
  const expiredCutoffNextDate = new Date(expiredCutoffDate);
  expiredCutoffNextDate.setDate(expiredCutoffNextDate.getDate() + 1);

  // For fulfilled requests: Requests uploaded before (today - 90 days) are being deleted
  const fulfilledCutoffDate = new Date(today);
  fulfilledCutoffDate.setDate(fulfilledCutoffDate.getDate() - 90);

  // Next day after cutoff (first day that's still available)
  const fulfilledCutoffNextDate = new Date(fulfilledCutoffDate);
  fulfilledCutoffNextDate.setDate(fulfilledCutoffNextDate.getDate() + 1);

  const formatDate = (date: Date) => {
    return date.toLocaleDateString('de-CH', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  };

  return {
    expiredCutoff: formatDate(expiredCutoffDate),
    expiredCutoffNext: formatDate(expiredCutoffNextDate),
    fulfilledCutoff: formatDate(fulfilledCutoffDate),
    fulfilledCutoffNext: formatDate(fulfilledCutoffNextDate),
    today: formatDate(today),
  };
});

function getUploadUrl(token: string): string {
  return uploadLinkBase.value + token;
}

// Watch search changes to trigger table refresh
watch(search, () => {
  const state = statusFilter.value || '';
  fetchRequests({ page: 1, itemsPerPage: itemsPerPage.value, search: search.value, state });
});

// Watch status filter changes to trigger table refresh
watch(statusFilter, () => {
  // Always trigger a request when the filter changes, even if search is empty
  const state = statusFilter.value || '';
  fetchRequests({ page: 1, itemsPerPage: itemsPerPage.value, search: search.value, state });
});

// Handler for table options updates (pagination, sorting, etc.)
function handleTableUpdate(options: { page: number; itemsPerPage: number; search: string }) {
  const state = statusFilter.value || '';
  fetchRequests({ ...options, state });
}

function clickRow(_event: MouseEvent, row: { item: ProfilePictureRequest }) {
  openDetails(row.item.id);
}

function openDetails(requestId: string) {
  fetchRequestDetails(requestId);
  showDialog.value = true;
  activeTab.value = 'overview';
  reminderSent.value = false; // Reset reminder state when opening new details
}

async function handleSendReminder() {
  if (!requestDetails.value?.request.id) return;

  try {
    sendingReminder.value = true;
    await sendReminder(requestDetails.value.request.id);
    reminderSent.value = true;
  } catch (error) {
    console.error('Failed to send reminder:', error);
  } finally {
    sendingReminder.value = false;
  }
}

function copyToClipboard(text: string) {
  navigator.clipboard.writeText(text);
}

function openInNewTab(url: string) {
  window.open(url, '_blank', 'noopener,noreferrer');
}

function openRequest(requestId: string) {
  openDetails(requestId);
}

function getRequestStateColor(state: RequestState): string {
  switch (state) {
    case RequestState.Pending:
      return 'orange';
    case RequestState.Fulfilled:
      return 'success';
    case RequestState.Expired:
      return 'error';
    default:
      return 'grey';
  }
}

function getRequestStateDisplayLabel(state: RequestState): string {
  switch (state) {
    case RequestState.Pending:
      return 'pendent';
    case RequestState.Fulfilled:
      return 'abgeschlossen';
    case RequestState.Expired:
      return 'abgelaufen';
    default:
      return 'unbekannt';
  }
}

function isExpired(dateString: string): boolean {
  return new Date(dateString) < new Date();
}

function formatDateTime(dateString: string, includeTime: boolean): string {
  const date = new Date(dateString);
  if (includeTime) {
    return date.toLocaleString('de-CH', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    });
  }
  return date.toLocaleDateString('de-CH', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });
}

// Helper functions for table data
function getPersonName(item: ProfilePictureRequest): string {
  const request = item as ProfilePictureRequestWithPerson;
  return request.person ? `${request.person.lastName}, ${request.person.firstName}` : 'N/A';
}

function getPersonUserName(item: ProfilePictureRequest): string {
  const request = item as ProfilePictureRequestWithPerson;
  return request.person?.userName || 'N/A';
}

function getRequestSequenceValue(item: ProfilePictureRequest): string {
  return (Number(item.requestSequenceIndex) + 1).toString();
}

function getFormattedExpiryDate(item: ProfilePictureRequest): string {
  return formatDateTime(item.expiryDate, false);
}

const headers = [
  {
    title: 'Person',
    key: 'person',
    sortable: false,
  },
  {
    title: 'GIB-Nr.',
    key: 'publicIdentifier',
    value: 'person.publicIdentifier',
    sortable: false,
  },
  {
    title: 'Anfrage',
    key: 'requestSequenceIndex',
    sortable: false,
  },
  {
    title: 'Status',
    key: 'requestState',
    sortable: false,
  },
  {
    title: 'Gültig bis',
    key: 'expiryDate',
    sortable: false,
  },
  {
    title: 'Aktionen',
    key: 'actions',
    sortable: false,
    align: 'end' as const,
  },
];
</script>

<style scoped>
/* Disable text truncation in list items */
:deep(.v-list-item-title),
:deep(.v-list-item-subtitle) {
  white-space: normal !important;
  overflow: visible !important;
  text-overflow: clip !important;
  line-height: 1.5 !important;
  line-clamp: unset !important;
  -webkit-line-clamp: unset !important;
  -webkit-box-orient: unset !important;
  display: block !important;
}

/* Allow ripple effect and chips to overflow list items and lists */
:deep(.v-list-item) {
  overflow: visible !important;
  min-height: 56px !important;
}

:deep(.v-list-item__content) {
  overflow: visible !important;
}

:deep(.v-list) {
  overflow: visible !important;
}

:deep(.v-card) {
  overflow: visible !important;
}

:deep(.v-card-text) {
  overflow: visible !important;
}

/* Ensure buttons and chips don't get clipped */
:deep(.v-btn) {
  position: relative;
  z-index: 1;
}

:deep(.v-chip) {
  position: relative;
  z-index: 1;
}

/* Make sure all containers allow overflow */
:deep(.v-container),
:deep(.v-row),
:deep(.v-col) {
  overflow: visible !important;
}

/* Disable text truncation in card titles - Safari specific */
:deep(.v-card-title) {
  white-space: normal !important;
  overflow: visible !important;
  text-overflow: clip !important;
  line-clamp: unset !important;
  -webkit-line-clamp: unset !important;
  -webkit-box-orient: unset !important;
  display: flex !important;
}

/* Disable text truncation in tabs */
:deep(.v-tab) {
  white-space: normal !important;
  overflow: visible !important;
  text-overflow: clip !important;
  line-clamp: unset !important;
  -webkit-line-clamp: unset !important;
  -webkit-box-orient: unset !important;
}

/* Make timeline items clickable */
.timeline-item-clickable {
  cursor: pointer;
  transition: opacity 0.2s ease;
}

.timeline-item-clickable:hover {
  opacity: 0.8;
}

.timeline-item-clickable :deep(.v-timeline-item__body) {
  cursor: pointer;
}

.profile-picture-wrapper {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #e8eef5 100%);
}

.profile-picture-page {
  background: transparent;
  padding-top: 2rem;
}

.search-card,
.data-table-card {
  border: 1px solid rgba(0, 0, 0, 0.12);
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  background: white;

  &:hover {
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
  }
}

.detail-dialog-card {
  background: white !important;
}

.detail-toolbar {
  border-bottom: 1px solid rgba(0, 0, 0, 0.08);
}

.detail-tabs {
  background: white !important;
  border-bottom: 1px solid rgba(0, 0, 0, 0.08);
}

.detail-info-card {
  background: white;
  border: 1px solid rgba(0, 0, 0, 0.08);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
  transition: all 0.3s ease;

  &:hover {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }
}

.bg-primary-lighten {
  background-color: rgba(25, 118, 210, 0.08) !important;
}

.primary-lighten {
  background-color: rgba(25, 118, 210, 0.12) !important;
}

.error-lighten {
  background-color: rgba(211, 47, 47, 0.12) !important;
}

.detail-list {
  background: transparent !important;
}

.detail-list-item {
  border-radius: 8px;
  padding: 8px 0;
  transition: all 0.2s ease;

  &:hover {
    background-color: rgba(0, 0, 0, 0.02);
  }
}
</style>
