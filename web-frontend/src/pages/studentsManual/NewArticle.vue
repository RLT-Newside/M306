<template>
  <v-container>
    <v-row>
      <v-col>
        <h1 class="text-h3">Neuen Artikel erfassen</h1>
      </v-col>
    </v-row>
  </v-container>

  <ArticleForm
    :model-value="article"
    @attachment:add="addAttachment"
  />

  <AttachmentList
    :attachments="article.attachments"
    @remove="removeAttachment"
  />

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
          :loading="loading"
          @click="submitArticle"
          >Speichern</v-btn
        >
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRequest } from 'alova/client';
import ArticleForm from '@/components/studentsManual/ArticleForm.vue';
import AttachmentList from '@/components/studentsManual/AttachmentList.vue';
import { Article } from '@/models/studentsManual/article';
import type { Attachment } from '@/models/studentsManual/articleResource';
import { createArticle } from '@/api/alovaMethods/studentsManual';
import router from '@/plugins/router';

const article = ref(new Article('', '', '', 0, [], []));

function addAttachment(newAttachment: Attachment) {
  article.value.attachments.push(newAttachment);
}

function removeAttachment(attachment: Attachment) {
  const index = article.value.attachments.indexOf(attachment);
  if (index >= 0) {
    article.value.attachments.splice(index, 1);
  }
}

function submitArticle(): void {
  send();
}

const { loading, send, onComplete } = useRequest(() => createArticle(article.value), {
  immediate: false,
});

onComplete(() => {
  router.push({ name: 'Students manual' });
});
</script>
