<template>
  <v-container>
    <v-row>
      <v-col>
        <v-text-field
          v-model="model.title"
          label="Titel"
          required
          :rules="[(v: string) => !!v || 'Der Titel der Aktivität darf nicht leer sein.']"
        />
      </v-col>

      <v-col cols="12">
        <p class="text-body-1">
          Die Beschreibung der Aktivität kann mit
          <span class="font-italic">Markdown</span> formatiert werden. In der rechten Spalte wird
          eine mögliche Vorschau der Darstellung angezeigt. Die effektive Darstellung in der GIBZ
          App entspricht strukturell der Vorschau, weicht im Detail jedoch von der Vorschau ab.
        </p>
      </v-col>
      <v-col cols="12">
        <MarkdownEditor
          v-model="model.task"
          placeholder="Beschreibung der Aktivität..."
        />
      </v-col>

      <v-col cols="12">
        <v-text-field
          v-model="model.maxPoints"
          label="Maximale Punktezahl"
          required
          type="number"
          :rules="[
            (v: number) => v > -1 || 'Die maximale Punktezahl darf nicht negativ sein.',
            (v: string) => !!v || 'Die maximale Punktezahl darf nicht leer sein',
          ]"
        />
      </v-col>

      <v-col>
        <v-select
          v-model="model.type"
          label="Fragetyp"
          :items="typeOptions"
          :disabled="!!model.id"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <span class="text-h6">Antworten</span>
      </v-col>

      <v-col cols="12">
        <p
          v-if="model.type === StageActivityType.singleChoice"
          class="text-body-1"
        >
          Es können beliebig viele Antworten erfasst und als korrekt markiert werden. Die
          Teilnehmer/-innen können bei der Bearbeitung der Aktivität
          <span class="font-weight-bold">eine Antwort</span> auswählen. Sie erhalten genau dann die
          maximale Punktezahl, wenn die ausgewählte Antwort eine korrekte Antwort ist.
        </p>
        <p
          v-if="model.type === StageActivityType.multipleChoice"
          class="text-body-1"
        >
          Es können beliebig viele Antworten erfasst und als korrekt markiert werden. Die
          Teilnehmer/-innen können bei der Bearbeitung der Aktivität
          <span class="font-weight-bold">mehrere Antworten</span> auswählen. Sie erhalten genau dann
          die maximale Punktezahl, wenn alle korrekten Antworten ausgewählt sind. Für fehlende,
          korrekte Antworten oder ausgewählte, inkorrekte Antworten werden Punkte anteilsmässig
          abgezogen.
        </p>
        <p
          v-if="model.type === StageActivityType.textInput"
          class="text-body-1"
        >
          Es können beliebig viele Antworten erfasst werden. Die Teilnehmer/-innen können bei der
          Bearbeitung der Aktivität
          <span class="font-weight-bold">eine Freitext-Antwort</span> eintippen. Sie erhalten genau
          dann die maximale Punktezahl, wenn die eingetippte Antwort einer der korrekten Antworten
          entspricht.
        </p>
        <p
          v-if="model.type === StageActivityType.qrCode"
          class="text-body-1"
        >
          Beim Aktivitätstyp QR-Code müssen keine Antworten erfasst werden. Bei der Bearbeitung der
          Aktivität scannen die Teilnehmer/-innen mit Ihrem Smartphone einen QR-Code ein. In diesem
          QR-Code ist der prozentuale Anteil der Maximalpunktezahl im JSON-Format mit dem Schlüssel
          <code>percentage</code> codiert.
        </p>
      </v-col>

      <v-col
        v-if="model.type !== StageActivityType.qrCode"
        cols="12"
      >
        <v-list
          select-strategy="leaf"
          density="compact"
        >
          <v-list-item
            v-for="(answer, index) in model.answers"
            :key="index"
          >
            <template #prepend>
              <v-list-item-action start>
                <v-checkbox
                  :key="index"
                  v-model="answer.isCorrect"
                  hide-details
                  :disabled="!!answer.id"
                />
              </v-list-item-action>
            </template>
            <v-list-item-title>{{ answer.answerText }}</v-list-item-title>
            <template #append>
              <v-icon @click="removeAnswer(index)">mdi-delete</v-icon>
            </template>
          </v-list-item>
        </v-list>
      </v-col>

      <v-col
        v-if="model.type !== StageActivityType.qrCode"
        cols="12"
      >
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
</template>

<script setup lang="ts">
import { ref, type PropType } from 'vue';
import MarkdownEditor from '../MarkdownEditor.vue';
import {
  StageActivity,
  StageActivityAnswer,
  StageActivityType,
} from '@/models/rally/stageActivity';

const model = defineModel({ type: Object as PropType<StageActivity>, required: true });

const typeOptions: any[] = [
  { title: 'Single Choice', value: StageActivityType.singleChoice },
  { title: 'Multiple Choice', value: StageActivityType.multipleChoice },
  { title: 'Texteingabe', value: StageActivityType.textInput },
  { title: 'QR Code', value: StageActivityType.qrCode },
];

const newAnswer = ref<StageActivityAnswer>(new StageActivityAnswer('', '', false));

function submitAnswerForm(): void {
  model.value.answers.push(newAnswer.value);
  newAnswer.value = new StageActivityAnswer('', '', false);
}

function removeAnswer(answerIndex: number): void {
  model.value.answers.splice(answerIndex, 1);
}
</script>
