<template>
  <MdEditor
    :model-value="modelValue"
    language="en-US"
    :toolbars="[
      'bold',
      'italic',
      '-',
      'title',
      'quote',
      '-',
      'unorderedList',
      'orderedList',
      'link',
      '-',
      'revoke',
      'next',
      '=',
      'preview',
      'fullscreen',
      'pageFullscreen',
    ]"
    :show-code-row-number="showCodeRowNumber"
    :placeholder="placeholder"
    :preview-theme="previewTheme"
    :tab-width="tabWidth"
    @on-change="onChange"
  />
</template>

<script setup lang="ts">
import { MdEditor } from 'md-editor-v3';
import 'md-editor-v3/lib/style.css';

defineProps({
  modelValue: { type: String, required: true },
  showCodeRowNumber: { type: Boolean, required: false, default: true },
  placeholder: { type: String, required: false, default: '' },
  previewTheme: { type: String, required: false, default: 'default' },
  tabWidth: { type: Number, required: false, default: 4 },
});

const emit = defineEmits<{
  'update:modelValue': [value: string];
}>();

function onChange(value: string) {
  emit('update:modelValue', value);
}
</script>
