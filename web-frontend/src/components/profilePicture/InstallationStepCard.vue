<template>
  <v-list-item
    class="installation-step pa-4 mb-3"
    rounded="lg"
  >
    <template #prepend>
      <v-avatar
        :color="avatarColor"
        size="60"
        :class="[
          'mr-4',
          'step-avatar',
          (isFirstIcon || isLastIcon) && iosStyle ? 'ios-rounded-icon' : '',
        ]"
      >
        <v-icon
          v-if="icon"
          size="35"
          color="white"
        >
          {{ icon }}
        </v-icon>
        <v-img
          v-else-if="image"
          :class="[
            needsPadding ? 'icon-with-padding' : '',
            isFirstIcon && !iosStyle ? 'android-first-icon' : '',
          ]"
          :src="image"
        />
      </v-avatar>
    </template>
    <v-list-item-title class="text-wrap text-body-1 font-weight-medium mb-1">
      {{ title }}
    </v-list-item-title>
    <v-list-item-subtitle class="text-wrap">
      {{ subtitle }}
    </v-list-item-subtitle>
  </v-list-item>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    title: string;
    subtitle: string;
    avatarColor: string;
    icon?: string;
    image?: string;
    needsPadding?: boolean;
    isFirstIcon?: boolean;
    isLastIcon?: boolean;
    iosStyle?: boolean;
  }>(),
  {
    icon: undefined,
    image: undefined,
    needsPadding: false,
    isFirstIcon: false,
    isLastIcon: false,
    iosStyle: false,
  }
);
</script>

<style scoped lang="scss">
.installation-step {
  background: linear-gradient(135deg, #ffffff 0%, #fafbfc 100%);
  border: 2px solid rgba(25, 118, 210, 0.08);
  transition: all 0.3s ease;
  cursor: pointer;

  &:hover {
    transform: scale(1.02);
    border-color: rgba(25, 118, 210, 0.2);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  }

  :deep(.v-avatar) {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }

  :deep(.ios-rounded-icon) {
    border-radius: 13px !important;
  }

  :deep(.icon-with-padding) {
    padding: 8px;
  }

  :deep(.android-first-icon) {
    img {
      padding: 10px;
      padding-right: 5px;
      object-fit: contain;
    }
  }
}
</style>
