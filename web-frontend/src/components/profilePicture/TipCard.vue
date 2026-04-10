<template>
  <v-card
    class="tip-card mb-3"
    elevation="0"
    rounded="lg"
  >
    <v-card-text class="d-flex align-start pa-4">
      <v-icon
        color="primary"
        size="28"
        class="mr-3 mt-1"
      >
        mdi-check-circle
      </v-icon>
      <div>
        <div class="text-body-1 font-weight-bold mb-1">
          {{ title }}
        </div>
        <div class="text-body-2 text-medium-emphasis">
          {{ subtitle }}
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
defineProps<{
  title: string;
  subtitle: string;
}>();
</script>

<style scoped lang="scss">
.tip-card {
  background: linear-gradient(135deg, #ffffff 0%, #f0f4f8 100%);
  border: 2px solid rgba(25, 118, 210, 0.1);
  transition: all 0.3s ease;

  &:hover {
    border-color: rgba(25, 118, 210, 0.3);
    transform: scale(1.02);
    box-shadow: 0 4px 12px rgba(25, 118, 210, 0.1);
  }
}
</style>
