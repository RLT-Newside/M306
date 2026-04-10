<template>
  <v-dialog
    :model-value="modelValue"
    max-width="620"
    @update:model-value="(value) => emit('update:modelValue', value)"
  >
    <v-card :title="title">
      <v-card-text>
        <slot name="message">
          <p>{{ message }}</p>
        </slot>
        <slot />
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn
          variant="text"
          @click="emit('update:modelValue', false)"
          >{{ cancelText }}</v-btn
        >
        <v-btn
          :color="confirmColor"
          :disabled="confirmDisabled"
          @click="emit('confirm')"
          >{{ confirmText }}</v-btn
        >
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    modelValue: boolean;
    title: string;
    message: string;
    confirmText?: string;
    cancelText?: string;
    confirmColor?: string;
    confirmDisabled?: boolean;
  }>(),
  {
    confirmText: 'Löschen',
    cancelText: 'Abbrechen',
    confirmColor: 'error',
    confirmDisabled: false,
  }
);

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'confirm'): void;
}>();
</script>
