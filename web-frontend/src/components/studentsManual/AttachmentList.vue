<template>
  <v-container>
    <v-row>
      <v-col>
        <v-list>
          <v-list-item
            v-for="(resource, index) in attachments"
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
                <v-icon size="small">mdi-close</v-icon>
              </v-btn>
            </template>
          </v-list-item>
        </v-list>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { AttachmentType, type Attachment } from '@/models/studentsManual/articleResource';

const props = defineProps({
  attachments: { type: Array<Attachment>, required: true },
});

const emit = defineEmits<{
  remove: [value: Attachment];
}>();

function removeAttachment(attachment: Attachment) {
  emit('remove', attachment);
}

function showAttachment(resource: Attachment) {
  window.open(resource.url, '_blank');
}

function resourceIcon(resource: Attachment): string {
  switch (resource.type) {
    case AttachmentType.link:
      return 'mdi-web';
    case AttachmentType.video:
      return 'mdi-video';
    case AttachmentType.pdf:
      return 'mdi-file-document-outline';
    default:
      return 'mdi-web';
  }
}
</script>
