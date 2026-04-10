<template>
  <v-list-item class="detail-list-item">
    <template #prepend>
      <v-avatar
        :color="avatarColor"
        size="32"
      >
        <v-icon
          size="18"
          :color="iconColor"
        >
          {{ icon }}
        </v-icon>
      </v-avatar>
    </template>
    <v-list-item-title class="text-subtitle-2 text-medium-emphasis">
      {{ label }}
    </v-list-item-title>
    <v-list-item-subtitle class="font-weight-medium mt-1">
      <slot />
    </v-list-item-subtitle>
  </v-list-item>
</template>

<script setup lang="ts">
interface Props {
  icon: string;
  label: string;
  avatarColor?: string;
  iconColor?: string;
}

withDefaults(defineProps<Props>(), {
  avatarColor: 'rgba(25, 118, 210, 0.12)',
  iconColor: 'primary',
});
</script>
