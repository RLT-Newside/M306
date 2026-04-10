<template>
  <v-dialog
    v-model="showDialog"
    :retain-focus="false"
    persistent
    scrollable
    width="auto"
  >
    <v-form
      ref="stageActivityForm"
      v-model="isValidForm"
      @submit.prevent="submitStageActivity"
    >
      <v-card
        width="750px"
        max-height="750px"
      >
        <v-card-title>
          <span class="text-h5">Neue Aktivität</span>
        </v-card-title>
        <v-divider />
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="stageActivity.title"
                  label="Titel"
                  required
                  :rules="[(v: string) => !!v || 'Der Titel der Aktivität darf nicht leer sein.']"
                />
              </v-col>

              <v-col cols="12">
                <MarkdownEditor
                  v-model="stageActivity.task"
                  placeholder="Beschreibung der Aktivität..."
                />
              </v-col>

              <v-col cols="12">
                <v-text-field
                  v-model="stageActivity.maxPoints"
                  label="Maximale Punktezahl"
                  required
                  type="number"
                  :rules="[
                    (v: number) => v > -1 || 'Die maximale Punktezahl darf nicht negativ sein.',
                  ]"
                />
              </v-col>

              <v-col>
                <v-select
                  v-model="stageActivity.type"
                  label="Fragetyp"
                  :items="typeOptions"
                />
              </v-col>
            </v-row>
            <v-row v-if="stageActivity.type !== StageActivityType.qrCode">
              <v-col cols="12">
                <span class="text-h6">Antworten</span>
              </v-col>

              <v-col cols="12">
                <v-list
                  select-strategy="leaf"
                  density="compact"
                >
                  <v-list-item
                    v-for="(answer, index) in stageActivity.answers"
                    :key="index"
                  >
                    <template #prepend>
                      <v-list-item-action start>
                        <v-checkbox
                          :key="index"
                          v-model="answer.isCorrect"
                          hide-details
                        />
                      </v-list-item-action>
                    </template>
                    <v-list-item-title>{{ answer.answerText }}</v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-col>

              <v-col cols="12">
                <v-form
                  ref="answerForm"
                  @submit.prevent="submitAnswerForm"
                >
                  <v-text-field
                    v-model="newAnswer.answerText"
                    label="Neue Antwort"
                  >
                    <template #prepend>
                      <v-checkbox
                        v-model="newAnswer.isCorrect"
                        hide-details
                      />
                    </template>
                    <template #append-inner>
                      <v-icon
                        class="clickable-icon"
                        @click="submitAnswerForm"
                        >mdi-send</v-icon
                      >
                    </template>
                  </v-text-field>
                </v-form>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
        <v-divider />
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            variant="text"
            @click="cancel"
          >
            Abbrechen
          </v-btn>
          <v-btn
            type="submit"
            color="primary"
            variant="text"
            @click="submitStageActivity"
          >
            Speichern
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import type { PropType } from 'vue';
import MarkdownEditor from '../MarkdownEditor.vue';
import {
  StageActivity,
  StageActivityAnswer,
  StageActivityType,
} from '../../models/rally/stageActivity';

const props = defineProps({
  value: { type: Boolean, required: false, default: false },
  initialStageActivity: {
    type: Object as PropType<StageActivity>,
    required: false,
    default: new StageActivity('', '', '', 0, StageActivityType.singleChoice, []),
  },
});

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'saveStageActivity', location: StageActivity): void;
}>();

const showDialog = computed({
  get() {
    return props.value;
  },
  set(value) {
    emit('update:modelValue', value);
  },
});

watch(
  () => props.initialStageActivity,
  (newInitialStageActivity: StageActivity) => {
    stageActivity.value = newInitialStageActivity;
  }
);

const isValidForm = ref(true);
const stageActivityForm = ref<HTMLFormElement | null>(null);

const stageActivity = ref<StageActivity>(props.initialStageActivity);
const newAnswer = ref<StageActivityAnswer>(new StageActivityAnswer('', '', false));

const typeOptions: Array<{ title: string; value: StageActivityType }> = [
  { title: 'Single Choice', value: StageActivityType.singleChoice },
  { title: 'Multiple Choice', value: StageActivityType.multipleChoice },
  { title: 'Texteingabe', value: StageActivityType.textInput },
  { title: 'QR Code', value: StageActivityType.qrCode },
];

function cancel() {
  clearData();
  emit('update:modelValue', false);
}

function clearData() {
  stageActivity.value = new StageActivity('', '', '', 0, StageActivityType.singleChoice, []);
}

async function submitStageActivity() {
  if (stageActivity.value.type === StageActivityType.qrCode) {
    stageActivity.value.answers = [];
  }
  const validationResult = await stageActivityForm.value?.validate();
  if (validationResult.valid) {
    emit('saveStageActivity', stageActivity.value);
    clearData();
  }
}

function submitAnswerForm() {
  stageActivity.value.answers.push(newAnswer.value);
  newAnswer.value = new StageActivityAnswer('', '', false);
}
</script>

<style lang="scss">
.clickable-icon {
  pointer-events: auto;
  cursor: pointer;
}

.v-input__prepend {
  padding-top: 0;
}
</style>
