<template>
  <v-timeline-item
    dot-color="primary"
    size="small"
    :icon="`mdi-numeric-${index + 1}-circle`"
  >
    <v-card
      elevation="0"
      class="timeline-card"
    >
      <v-card-text class="py-3">
        <div class="text-body-1 font-weight-medium">{{ text }}</div>
      </v-card-text>
    </v-card>
  </v-timeline-item>
</template>

<script setup lang="ts">
defineProps<{
  text: string;
  index: number;
}>();
</script>

<style scoped lang="scss">
.timeline-card {
  background: linear-gradient(135deg, #ffffff 0%, #fafbfc 100%);
  border: 2px solid rgba(25, 118, 210, 0.08);
  border-radius: 12px;
  transition: all 0.3s ease;

  &:hover {
    border-color: rgba(25, 118, 210, 0.2);
    transform: scale(1.02);
  }
}
</style>
