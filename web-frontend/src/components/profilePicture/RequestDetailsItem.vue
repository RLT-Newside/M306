<template>
  <v-col
    :cols="labelCols"
    class="py-1"
  >
    <p class="text-body-1">{{ label }}</p>
  </v-col>
  <v-col
    :cols="valueCols"
    class="py-1"
  >
    <p
      class="text-body-1 font-weight-bold"
      :class="{ clipboard: enableClipboard }"
      v-on="enableClipboard ? { click: copyToClipboard } : {}"
    >
      {{ value }}
    </p>
    <v-btn
      v-if="enableClipboard"
      variant="tonal"
      size="x-small"
      icon="mdi-content-copy"
      class="ml-1"
      @click="copyToClipboard"
    ></v-btn>
  </v-col>
</template>

<script setup lang="ts">
const props = defineProps({
  label: { type: String, required: true },
  value: { type: String, required: true },
  labelCols: { type: Number, required: false, default: 3 },
  valueCols: { type: Number, required: false, default: 3 },
  enableClipboard: { type: Boolean, required: false, default: false },
});

function copyToClipboard() {
  navigator.clipboard.writeText(props.value);
}
</script>

<style>
.clipboard {
  display: inline;
  cursor: pointer;
}
</style>
