<template>
  <v-card
    elevation="2"
    rounded="xl"
    class="pa-4 qr-card mx-auto"
    :max-width="maxWidth"
  >
    <div class="text-h6 font-weight-bold mb-4">{{ title }}</div>
    <div class="qr-container mb-4">
      <a
        :href="link"
        target="_blank"
        class="qr-link"
      >
        <div :class="large ? 'qr-wrapper-large' : 'qr-wrapper'">
          <VueQrcode
            :value="link"
            :options="{ width: qrSize }"
          />
        </div>
      </a>
    </div>
    <p
      v-if="description"
      class="text-caption text-medium-emphasis mb-4"
    >
      <!-- eslint-disable-next-line vue/no-v-html -->
      <span v-html="description" />
    </p>
    <v-btn
      v-if="buttonText"
      :href="link"
      target="_blank"
      color="primary"
      size="large"
      rounded="pill"
      class="store-button"
    >
      <v-icon
        v-if="buttonIcon"
        start
        size="24"
      >
        {{ buttonIcon }}
      </v-icon>
      {{ buttonText }}
    </v-btn>
  </v-card>
</template>

<script setup lang="ts">
import VueQrcode from '@chenfengyuan/vue-qrcode';
import { computed } from 'vue';

const props = withDefaults(
  defineProps<{
    title: string;
    link: string;
    description?: string;
    buttonText?: string;
    buttonIcon?: string;
    large?: boolean;
    maxWidth?: string | number;
  }>(),
  {
    description: undefined,
    buttonText: undefined,
    buttonIcon: undefined,
    large: false,
    maxWidth: 400,
  }
);

const qrSize = computed(() => (props.large ? 240 : 200));
</script>

<style scoped lang="scss">
.qr-card {
  background: linear-gradient(135deg, #ffffff 0%, #f8f9fa 100%);
  border: 2px solid rgba(25, 118, 210, 0.1);
  transition: all 0.3s ease;

  &:hover {
    transform: scale(1.02);
    box-shadow: 0 12px 28px rgba(0, 0, 0, 0.12) !important;
  }

  .qr-container {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
  }

  .qr-wrapper,
  .qr-wrapper-large {
    padding: 16px;
    background: white;
    border-radius: 12px;
    display: inline-block;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
    transition: all 0.3s ease;

    &:hover {
      transform: scale(1.02);
      box-shadow: 0 6px 16px rgba(0, 0, 0, 0.12);
    }

    :deep(canvas) {
      display: block;
      max-width: 100%;
      height: auto;
    }
  }

  .qr-wrapper-large {
    padding: 20px;
  }

  .qr-link {
    text-decoration: none;
    display: inline-block;
  }
}

.store-button {
  font-weight: 600;
  letter-spacing: 0.5px;
  text-transform: none;
  box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
  transition: all 0.3s ease;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 16px rgba(25, 118, 210, 0.4);
  }
}
</style>
