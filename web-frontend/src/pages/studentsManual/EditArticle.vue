<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Artikel bearbeiten</h1>
      </v-col>
    </v-row>
  </v-container>

  <template v-if="article">
    <ArticleForm
      :model-value="article"
      @attachment:add="addAttachment"
    />
    <AttachmentList
      :attachments="article.attachments"
      @remove="removeAttachment"
    />
  </template>

  <v-container>
    <v-row
      align="center"
      justify="center"
    >
      <v-col cols="auto">
        <v-btn :to="{ name: 'Students manual' }">Abbrechen</v-btn>
      </v-col>
      <v-col cols="auto">
        <v-btn
          color="primary"
          :loading="articleUpdatePending"
          @click="submitArticle"
          >Speichern</v-btn
        >
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { useRequest } from 'alova/client';
import router from '@/plugins/router';
import ArticleForm from '@/components/studentsManual/ArticleForm.vue';
import AttachmentList from '@/components/studentsManual/AttachmentList.vue';
import {
  getArticle,
  removeAttachment as attachmentRemovalRequest,
  addAttachment as attachmentAddingRequest,
  updateArticle as articleUpdateRequest,
} from '@/api/alovaMethods/studentsManual';
import type { Attachment } from '@/models/studentsManual/articleResource';

const props = defineProps({
  articleId: { type: String, required: true },
});

const attachmentsToAdd = reactive<Array<Attachment>>([]);
const attachmentsToRemove = reactive<Array<Attachment>>([]);

const {
  loading: articleLoading,
  data: article,
  send: requestArticle,
} = useRequest(getArticle(props.articleId), { immediate: false });

const {
  loading: articleUpdatePending,
  send: updateArticle,
  onSuccess: successfullyUpdatedArticle,
} = useRequest((articleToUpdate) => articleUpdateRequest(props.articleId, articleToUpdate), {
  immediate: false,
});

const { loading: attachmentAddingLoading, send: sendAttachmentAdding } = useRequest(
  (newAttachment: Attachment) => attachmentAddingRequest(props.articleId, newAttachment),
  { immediate: false }
);

const { loading: attachmentRemovalLoading, send: sendAttachmentRemoval } = useRequest(
  (attachmentId: string) => attachmentRemovalRequest(props.articleId, attachmentId),
  { immediate: false }
);

onMounted(() => {
  requestArticle();
});

function addAttachment(newAttachment: Attachment) {
  attachmentsToAdd.push(newAttachment);
  article.value.attachments.push(newAttachment);
}

function removeAttachment(attachment: Attachment) {
  if (attachmentsToAdd.includes(attachment)) {
    const index = attachmentsToAdd.indexOf(attachment);
    attachmentsToAdd.splice(index, 1);
  } else {
    attachmentsToRemove.push(attachment);
  }

  if (article.value.attachments.includes(attachment)) {
    const index = article.value.attachments.indexOf(attachment);
    article.value.attachments.splice(index, 1);
  }
}

function submitArticle(): void {
  // Remove attachmentsToRemove
  attachmentsToRemove.forEach((attachment) => {
    sendAttachmentRemoval(attachment.id);
  });

  // Add attachmentsToAdd
  attachmentsToAdd.forEach((attachment) => {
    sendAttachmentAdding(attachment);
  });

  // Update article
  updateArticle(article.value);
}

successfullyUpdatedArticle(() => {
  router.push({ name: 'Students manual' });
});
</script>
