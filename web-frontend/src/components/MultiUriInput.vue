<template>
  <v-text-field
    ref="uriInputElement"
    v-model="uriInput"
    :label="props.label"
    type="url"
    :rules="[rules.url]"
    @keydown.enter="addUri"
  />

  <v-chip-group column>
    <v-chip
      v-for="uri in uris"
      :key="uri"
      closable
      @click:close="() => removeUri(uri)"
    >
      {{ uri }}
    </v-chip>
  </v-chip-group>
</template>

<script setup lang="ts">
import { reactive, ref, watch } from 'vue';

const props = defineProps({
  modelValue: { type: Array<string>, required: true },
  label: { type: String, required: true },
  clearAfterSubmit: { type: Boolean, default: false },
});

const emit = defineEmits(['update:modelValue']);

const rules = {
  url: (value: string) => {
    // More permissive URL pattern that supports:
    // - Standard domains (example.com)
    // - Localhost (localhost, localhost:3000)
    // - IP addresses (127.0.0.1, 192.168.1.1)
    // - Ports (:3000, :8080)
    // - Paths and query parameters
    const urlPattern =
      /^https?:\/\/(?:(?:localhost|(?:\d{1,3}\.){3}\d{1,3})|(?:www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}(?:\.[a-zA-Z0-9()]{1,6})?)(?::\d{1,5})?(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$/;
    return urlPattern.test(value) || 'Invalid URL pattern';
  },
};

const uriInputElement = ref<HTMLFormElement | null>(null);

const uriInput = ref('');
const uris = reactive<string[]>([...props.modelValue]);

// Watch for external changes to modelValue and sync internal state
watch(
  () => props.modelValue,
  (newValue) => {
    // Clear and repopulate the array to maintain reactivity
    uris.splice(0, uris.length, ...newValue);
  }
);

function addUri() {
  if (uriInput.value.length > 0) {
    uriInputElement.value?.validate().then((validationErrors: Array<string>) => {
      if (validationErrors.length === 0) {
        uris.push(uriInput.value);
        emit('update:modelValue', uris);

        if (props.clearAfterSubmit) {
          uriInput.value = '';
          uriInputElement.value?.reset();
        }
      }
    });
  }
}

function removeUri(uri: string) {
  const index = uris.indexOf(uri);
  if (index > -1) {
    uris.splice(index, 1);
  }
  uriInputElement.value?.reset();
  emit('update:modelValue', uris);
}
</script>
