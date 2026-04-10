<template>
  <v-container>
    <PageTitle title="Handbuch für Lernende" />
    <v-row>
      <v-col>
        <v-text-field
          v-model="searchTerm"
          variant="outlined"
          prepend-inner-icon="mdi-magnify"
          :clearable="true"
        >
        </v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-expansion-panels>
          <v-expansion-panel
            v-for="(article, index) in filteredArticles"
            :key="index"
          >
            <v-expansion-panel-title>{{ article.title }}</v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-row>
                <v-col cols="12">
                  <MdPreview :model-value="article.content" />
                </v-col>
              </v-row>
              <v-row justify="center">
                <v-col cols="auto">
                  <v-btn
                    prepend-icon="mdi-pencil"
                    :to="{ name: 'Edit article', params: { articleId: article.id } }"
                    >Artikel bearbeiten</v-btn
                  >
                </v-col>
                <v-col cols="auto">
                  <v-btn
                    prepend-icon="mdi-delete-outline"
                    @click="deleteArticle(article.id)"
                    >Artikel löschen</v-btn
                  >
                </v-col>
              </v-row>
            </v-expansion-panel-text>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-col>
    </v-row>
    <v-row justify="center">
      <v-col cols="auto">
        <v-btn
          prepend-icon="mdi-plus"
          :to="{ name: 'New article' }"
          color="primary"
        >
          Neuen Artikel erstellen
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { useRequest } from 'alova/client';
import { computed, ref } from 'vue';
import PageTitle from '@/components/PageTitle.vue';
import { MdPreview } from 'md-editor-v3';
import {
  getArticles,
  deleteArticle as deleteArticleRequest,
} from '@/api/alovaMethods/studentsManual';

const searchTerm = ref('');

const filteredArticles = computed(() => {
  return articles.value?.filter((article) => {
    return (
      article.title.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      article.content.toLowerCase().includes(searchTerm.value.toLowerCase())
    );
  });
});

const { data: articles, send: loadArticles } = useRequest(() => getArticles());

const { send: deleteArticle, onComplete: onCompleteDeleteArticle } = useRequest(
  (articleId) => deleteArticleRequest(articleId),
  { immediate: false }
);

onCompleteDeleteArticle(() => loadArticles());
</script>

<style scoped lang="scss">
.markdown-rendering {
  ul,
  li {
    list-style-position: inside;
    margin-left: 11px;
    padding-right: 0;
  }

  p {
    margin: 10px 0;
  }
}
</style>
