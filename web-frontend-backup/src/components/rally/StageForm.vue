<template>
  <v-container>
    <v-row>
      <v-col cols="12">
        <v-text-field
          v-model="model.title"
          label="Titel der Station"
        >
          <template #append-inner>
            <v-tooltip>
              <template #activator="{ props }">
                <v-icon v-bind="props">mdi-information</v-icon>
              </template>
              Der Titel der Station wird <span class="font-italic">nicht</span> in der GIBZ App
              angezeigt - er dient nur für die interne Benennung einer Station.
            </v-tooltip>
          </template>
        </v-text-field>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h2 class="text-h4">Instruktion zum Auffinden der Station</h2>
        <p class="text-body-1">
          Die Instruktionen zum Auffinden der Station werden in der GIBZ App nach dem Abschluss
          einer vorhergehenden Station bis zum Eintreffen an dieser Station angezeigt.
        </p>
      </v-col>
      <v-col cols="12">
        <v-text-field
          v-model="model.preArrivalInformation.title"
          label="Titel der Instruktion"
        >
          <template #append-inner>
            <v-tooltip>
              <template #activator="{ props }">
                <v-icon v-bind="props">mdi-information</v-icon>
              </template>
              Der Titel der Instruktion wird als Überschrift in der GIBZ App oberhalb der
              detaillierten Instruktionen angezeigt.
            </v-tooltip>
          </template>
        </v-text-field>
      </v-col>
      <v-col cols="12">
        <p class="text-body-1">
          Die detaillierten Instruktionen zum Auffinden der Station können mit
          <span class="font-italic">Markdown</span> formatiert werden. In der rechten Hälfte des
          Editors wird eine strukturelle Vorschau der Darstellung angezeigt. Die effektive
          Darstellung in der GIBZ App ist auf die Gestaltung der mobilen Anwendung ausgerichtet und
          weicht im Detail von der untenstehenden Vorschau ab.
        </p>
      </v-col>
      <v-col cols="12">
        <MarkdownEditor v-model="model.preArrivalInformation.content" />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h2 class="text-h4">Information für die Station</h2>
        <p class="text-body-1">
          Die Informationen zur Station werden in der GIBZ App nach dem Eintreffen an dieser Station
          angezeigt.
        </p>
      </v-col>
      <v-col cols="12">
        <v-text-field
          v-model="model.information.title"
          label="Titel der Information"
        >
          <template #append-inner>
            <v-tooltip>
              <template #activator="{ props }">
                <v-icon v-bind="props">mdi-information</v-icon>
              </template>
              Der Titel der Information wird als Überschrift in der GIBZ App oberhalb der
              detaillierten Informationen angezeigt.
            </v-tooltip>
          </template>
        </v-text-field>
      </v-col>
      <v-col cols="12">
        <p class="text-body-1">
          Die detaillierten Informationen zur Station können mit
          <span class="font-italic">Markdown</span> formatiert werden. In der rechten Hälfte des
          Editors wird eine strukturelle Vorschau der Darstellung angezeigt. Die effektive
          Darstellung in der GIBZ App ist auf die Gestaltung der mobilen Anwendung ausgerichtet und
          weicht im Detail von der untenstehenden Vorschau ab.
        </p>
      </v-col>
      <v-col cols="12">
        <MarkdownEditor v-model="model.information.content" />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <h2 class="text-h4">Ressourcen</h2>
        <p class="text-body-1">
          Sie können dem Informations-Block verschiedene Ressourcen hinzufügen. Diese Ressourcen
          werden für die Station zusammen mit dem Informations-Block dargestellt.
        </p>
      </v-col>
    </v-row>

    <v-form v-model="resourceFormValidationModel">
      <v-row>
        <v-col>
          <v-text-field
            v-model="resourceTitle"
            label="Titel"
            prepend-inner-icon="mdi-tag"
            required
            :rules="[(v: string) => !!v || 'Die Bezeichnung der Ressource darf nicht leer sein']"
          />
        </v-col>
        <v-col>
          <v-text-field
            v-model="resourceUrl"
            label="URL"
            prepend-inner-icon="mdi-link"
            required
            :rules="[(v: string) => !!v || 'Die URL der Ressource darf nicht leer sein']"
          />
        </v-col>
      </v-row>
    </v-form>

    <v-row
      align="center"
      justify="center"
    >
      <v-menu>
        <template #activator="{ props }">
          <v-btn
            variant="text"
            v-bind="props"
            :disabled="!resourceFormValidationModel"
            >Ressource hinzufügen als...</v-btn
          >
        </template>
        <v-list>
          <v-list-item
            title="Link"
            subtitle="Ein Link wird in der GIBZ App unterhalb der Informationen aufgeführt"
            prepend-icon="mdi-web"
            @click="addAttachment(RallyAttachmentType.link)"
          />
          <v-list-item
            title="Video"
            subtitle="Ein Video wird in der GIBZ App im Kopfbereich der Station eingebettet"
            prepend-icon="mdi-video"
            @click="addAttachment(RallyAttachmentType.video)"
          />
        </v-list>
      </v-menu>
    </v-row>

    <v-row>
      <v-col>
        <v-list>
          <v-list-item
            v-for="(resource, index) in model.information.attachments"
            :key="index"
            :value="resource"
            :title="resource.title"
            :subtitle="resource.url"
          >
            <template #prepend>
              <v-icon :icon="resourceIcon(resource)" />
            </template>

            <template #append>
              <v-spacer />

              <v-btn
                icon
                size="x-small"
                class="mx-2"
                @click="showAttachment(resource)"
              >
                <v-icon size="small">mdi-open-in-new</v-icon>
              </v-btn>
              <v-btn
                icon
                size="x-small"
                @click="removeAttachment(resource)"
              >
                <v-icon size="small">mdi-delete</v-icon>
              </v-btn>
            </template>
          </v-list-item>
        </v-list>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, type PropType } from 'vue';
import type { Stage } from '@/models/rally/stage';
import { RallyAttachment, RallyAttachmentType } from '@/models/rally/rallyAttachment';
import MarkdownEditor from '../MarkdownEditor.vue';

const model = defineModel({ type: Object as PropType<Stage>, required: true });

const resourceFormValidationModel = ref(true);
const resourceTitle = ref('');
const resourceUrl = ref('');

function addAttachment(type: RallyAttachmentType): void {
  const newAttachment = new RallyAttachment('', resourceTitle.value, resourceUrl.value, type);
  model.value.information.attachments.push(newAttachment);
  resourceTitle.value = '';
  resourceUrl.value = '';
}

function removeAttachment(resource: RallyAttachment) {
  const index = model.value.information.attachments.indexOf(resource);
  if (index > -1) {
    model.value.information.attachments.splice(index, 1);
  }
}

function showAttachment(resource: RallyAttachment) {
  window.open(resource.url, '_blank');
}

function resourceIcon(resource: RallyAttachment): string {
  switch (resource.type) {
    case RallyAttachmentType.link:
      return 'mdi-web';
    case RallyAttachmentType.video:
      return 'mdi-video';
  }
  return 'mdi-web';
}
</script>
