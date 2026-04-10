<template>
  <v-dialog
    v-model="showDialog"
    width="auto"
  >
    <v-form @submit.prevent="submitStageActivity">
      <v-card
        width="750px"
        max-height="750px"
      >
        <v-card-title>
          <p class="text-h5">{{ localStageActivity?.title }}</p>
        </v-card-title>
        <v-divider />
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="localStageActivity.title"
                  label="Titel"
                  required
                  :rules="[(v: string) => !!v || 'Der Titel der Aktivität darf nicht leer sein.']"
                />
              </v-col>

              <v-col cols="12">
                <MarkdownEditor v-model="localStageActivity.task" />
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script setup lang="ts">
import { StageActivity, StageActivityType } from '@/models/rally/stageActivity';
import { computed, ref, watch, type PropType } from 'vue';
import MarkdownEditor from '../MarkdownEditor.vue';

const props = defineProps({
  value: { type: Boolean, required: false, default: false },
  stageActivity: { type: Object as PropType<StageActivity>, required: false, default: null },
});

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
}>();

const localStageActivity = ref<StageActivity>(
  props.stageActivity
    ? { ...props.stageActivity }
    : { id: '', title: '', task: '', maxPoints: 0, type: StageActivityType.textInput, answers: [] }
);

watch(
  () => props.stageActivity,
  (newValue) => {
    if (newValue) {
      localStageActivity.value = { ...newValue };
    }
  }
);

const showDialog = computed({
  get() {
    return props.value;
  },
  set(value) {
    emit('update:modelValue', value);
  },
});

function submitStageActivity() {}
</script>
