<template>
  <v-row>
    <v-col>
      <h1 class="mt-5 text-h2">{{ title }}</h1>
    </v-col>
  </v-row>
</template>

<script setup lang="ts">
defineProps({
  title: { type: String, required: true },
});
</script>
