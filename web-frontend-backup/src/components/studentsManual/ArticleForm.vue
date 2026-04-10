<template>
  <v-container>
    <v-row>
      <v-col>
        <v-text-field
          v-model="modelValue.title"
          label="Titel des Artikels"
          @update:model-value="updateTitle"
        ></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <p class="text-body-1">
          Im linken Teil des nachfolgenden Eingabefeldes kann der Inhalt des Artikels mit
          <em>Markdown</em> erfasst werden. Im rechten Bereich wird eine formatierte Vorschau des
          Artikels angezeigt.
        </p>
        <br />
        <p class="text-body-2">
          <strong>Hinweis: </strong>Die Formatierung entspricht nur <em>strukturell</em> der
          Darstellung des Artikels in der GIBZ App. Bei Schriftart und -grösse sowie Farben werden
          spezifische Anpassungen an das Erscheinungsbild der GIBZ App automatisch vorgenommen.
        </p>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <MarkdownEditor
          v-model="modelValue.content"
          placeholder="Inhalt des Artikels..."
          @update:model-value="updateContent"
        />
      </v-col>
    </v-row>
  </v-container>

  <v-container>
    <v-row>
      <v-col>
        <h2 class="text-h4">Ressourcen</h2>
        <p class="text-body-1">
          Dem Artikel können verschiedene Ressourcen hinzugefügt werden. Diese Ressourcen werden im
          Handbuch zusammen mit dem eigentlichen Artikel dargestellt.
        </p>
      </v-col>
    </v-row>

    <v-row align="center">
      <v-col cols="12">
        <v-text-field
          v-model="resourceUrl"
          label="URL"
          prepend-inner-icon="mdi-link"
          hide-details="auto"
        />
      </v-col>

      <v-col cols="7">
        <v-text-field
          v-model="resourceTitle"
          label="Titel"
          prepend-inner-icon="mdi-tag"
          hide-details="auto"
        />
      </v-col>

      <v-col cols="3">
        <v-btn-toggle
          v-model="resourceType"
          mandatory
          variant="outlined"
        >
          <v-btn value="link">
            <v-icon start>mdi-web</v-icon>
            Link
          </v-btn>
          <v-btn value="video">
            <v-icon start>mdi-video</v-icon>
            Video
          </v-btn>
          <v-btn value="pdf">
            <v-icon start>mdi-file-document-outline</v-icon>
            PDF
          </v-btn>
        </v-btn-toggle>
      </v-col>

      <v-col cols="2">
        <v-btn
          prepend-icon="mdi-plus"
          @click="addAttachment"
          >hinzufügen</v-btn
        >
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import MarkdownEditor from '../MarkdownEditor.vue';
import { Article } from '@/models/studentsManual/article';
import { Attachment, AttachmentType } from '@/models/studentsManual/articleResource';

const props = defineProps({
  modelValue: { type: Article, required: true },
});

const emit = defineEmits<{
  'update:modelValue': [value: Article];
  'attachment:add': [value: Attachment];
}>();

const resourceTitle = ref('');
const resourceType = ref('link');
const resourceUrl = ref('');

function updateTitle(newTitle: string) {
  emit('update:modelValue', { ...props.modelValue, title: newTitle });
}

function updateContent(newContent: string) {
  emit('update:modelValue', { ...props.modelValue, content: newContent });
}

function addAttachment() {
  const newAttachment = new Attachment(
    '',
    resourceTitle.value,
    resourceUrl.value,
    AttachmentType[resourceType.value as keyof typeof AttachmentType]
  );

  emit('attachment:add', newAttachment);

  resourceTitle.value = '';
  resourceUrl.value = '';
}
</script>
