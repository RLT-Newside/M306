<template>
  <v-form
    ref="assignmentForm"
    v-model="isValidForm"
    @submit.prevent="submitAssignment"
  >
    <v-container>
      <v-row>
        <v-col>
          <v-text-field
            v-model="audienceTitle"
            label="Neue Kohorte hinzufügen"
          >
            <template #append-inner>
              <v-icon
                class="clickable-icon"
                @click="submitAssignment"
                >mdi-send</v-icon
              >
            </template>
          </v-text-field>
        </v-col>
      </v-row>
    </v-container>
  </v-form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Assignment, Audience } from '../../models/rally/assignment';
import { useRequest } from 'alova/client';
import { createAssignment } from '@/api/alovaMethods/gibzRally';
import { useNotifications } from '@/composables/useNotifications';

const props = defineProps({
  rallyId: { type: String, required: true },
});

const emit = defineEmits<{
  (e: 'addedNewAssignment'): void;
}>();

const { showSuccess, showError } = useNotifications();

const { send: createNewAssignment } = useRequest(
  (assignment: Partial<Assignment>) => createAssignment(props.rallyId, assignment),
  { immediate: false }
);

const audienceTitle = ref('');

const assignmentForm = ref<HTMLFormElement | null>(null);
const isValidForm = ref(true);

async function submitAssignment() {
  const validationResult = await assignmentForm.value?.validate();
  if (validationResult.valid) {
    try {
      await createNewAssignment({ audience: new Audience('', audienceTitle.value, []) });
      showSuccess('Zuweisung wurde erstellt.');
      audienceTitle.value = '';
      emit('addedNewAssignment');
    } catch (error) {
      showError(error, {
        notFound: 'Die Zuweisung konnte nicht erstellt werden, da die Rallye nicht existiert.',
        forbidden: 'Keine Berechtigung zum Erstellen der Zuweisung.',
        conflict: 'Diese Kohorte ist bereits der Rallye zugewiesen.',
        fallback: 'Zuweisung konnte nicht erstellt werden.',
      });
    }
  }
}
</script>

<style lang="scss">
.clickable-icon {
  pointer-events: auto;
  cursor: pointer;
}
</style>
