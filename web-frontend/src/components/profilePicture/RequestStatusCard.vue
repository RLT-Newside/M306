<template>
  <v-card
    class="status-card"
    elevation="0"
    rounded="xl"
  >
    <v-card-text class="pa-8">
      <div class="text-center mb-6">
        <v-avatar
          :color="statusColor"
          size="80"
          class="mb-4"
        >
          <v-icon
            size="50"
            color="white"
          >
            {{ statusIcon }}
          </v-icon>
        </v-avatar>
        <h2 class="text-h5 font-weight-bold mb-2">
          {{ title }}
        </h2>
      </div>

      <v-divider class="my-6" />

      <div class="text-body-1">
        <slot />
      </div>

      <v-divider class="my-6" />

      <v-alert
        type="info"
        variant="tonal"
        density="compact"
        class="text-body-2"
      >
        <template #prepend>
          <v-icon>mdi-information-outline</v-icon>
        </template>
        Bei Fragen wenden Sie sich bitte an Ihre Lehrperson oder das Sekretariat.
      </v-alert>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    title: string;
    statusColor: string;
    statusIcon: string;
  }>(),
  {}
);
</script>

<style scoped lang="scss">
.status-card {
  background: linear-gradient(135deg, #ffffff 0%, #f8f9fa 100%);
  border: 2px solid rgba(0, 0, 0, 0.06);
  animation: fadeInUp 0.5s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
