<template>
  <v-card
    :title="stageInformation.title"
    class="rally-section-card"
  >
    <v-card-text>
      <MdPreview :model-value="stageInformation.content" />
      <v-list v-if="stageInformation.attachments.length > 0">
        <v-list-item
          v-for="attachment in stageInformation.attachments"
          :key="attachment.id"
          :title="attachment.title"
          :subtitle="attachment.url"
          :prepend-icon="attachment.type === RallyAttachmentType.video ? 'mdi-web' : 'mdi-video'"
        >
          <template #append>
            <v-btn
              class="rally-list-icon-btn"
              icon
              variant="text"
              @click="openResource(attachment)"
            >
              <v-icon>mdi-open-in-new</v-icon>
            </v-btn>
          </template>
        </v-list-item>
      </v-list>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { MdPreview } from 'md-editor-v3';
import { StageInformation } from '@/models/rally/stage';
import { RallyAttachment, RallyAttachmentType } from '@/models/rally/rallyAttachment';
import type { PropType } from 'vue';

const props = defineProps({
  stageInformation: { type: Object as PropType<StageInformation>, required: true },
});

function openResource(attachment: RallyAttachment): void {
  window.open(attachment.url, '_blank');
}
</script>
