<template>
  <v-container>
    <v-row>
      <v-col cols="12">
        <h1 class="text-h3">Globale Bestenliste</h1>
      </v-col>
      <v-col cols="12">
        <p class="text-body-1">
          Für jede Disziplin wird hier das beste je erzielte Ergebnis aller Lernenden aufgeführt –
          separat für Männer und Frauen. Angegeben sind der absolute Wert, das Datum, das Schuljahr
          sowie die Klasse und der Beruf des Rekordhalters bzw. der Rekordhalterin.
        </p>
      </v-col>
    </v-row>

    <v-row v-if="loading">
      <v-col class="text-center">
        <v-progress-circular indeterminate color="primary" />
      </v-col>
    </v-row>

    <v-row v-else-if="error">
      <v-col>
        <v-alert type="error" variant="tonal">
          Die Bestenliste konnte nicht geladen werden. Bitte versuche es später erneut.
        </v-alert>
      </v-col>
    </v-row>

    <template v-else-if="leaderboard">
      <!-- Gender toggle -->
      <v-row>
        <v-col cols="12" sm="auto">
          <v-btn-toggle
            v-model="selectedGender"
            mandatory
            color="primary"
            variant="outlined"
            divided
          >
            <v-btn value="male" prepend-icon="mdi-gender-male">Männer</v-btn>
            <v-btn value="female" prepend-icon="mdi-gender-female">Frauen</v-btn>
          </v-btn-toggle>
        </v-col>
      </v-row>

      <v-row>
        <v-col>
          <v-card class="rally-table-surface">
            <v-card-text class="pa-0">
              <v-table class="rally-table" fixed-header>
                <thead>
                  <tr>
                    <th class="text-left">Disziplin</th>
                    <th class="text-left">Bestes Ergebnis</th>
                    <th class="text-left">Datum</th>
                    <th class="text-left">Schuljahr</th>
                    <th class="text-left">Klasse</th>
                    <th class="text-left">Beruf</th>
                  </tr>
                </thead>
                <tbody>
                  <tr
                    v-for="discipline in leaderboard.disciplines"
                    :key="discipline.discipline"
                  >
                    <td>
                      <span class="font-weight-medium">
                        {{ disciplineLabel(discipline.discipline) }}
                      </span>
                    </td>
                    <template v-if="entryForGender(discipline)">
                      <td>
                        <span class="font-weight-bold text-primary">
                          {{ formatResult(discipline.discipline, entryForGender(discipline)!.result) }}
                        </span>
                      </td>
                      <td>{{ formatDate(entryForGender(discipline)!.momentUtc) }}</td>
                      <td>{{ formatSchoolYear(entryForGender(discipline)!.schoolYear) }}</td>
                      <td>{{ entryForGender(discipline)!.className }}</td>
                      <td>{{ entryForGender(discipline)!.profession }}</td>
                    </template>
                    <template v-else>
                      <td colspan="5">
                        <span class="text-medium-emphasis text-caption">Keine Daten vorhanden</span>
                      </td>
                    </template>
                  </tr>
                </tbody>
              </v-table>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </template>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRequest } from 'alova/client';
import { getLeaderboard } from '@/api/alovaMethods/fitnessCheck';
import type { LeaderboardDiscipline, LeaderboardEntry } from '@/models/sportsTest/leaderboard';

const selectedGender = ref<'male' | 'female'>('male');

const { data: leaderboard, loading, error } = useRequest(getLeaderboard());

function entryForGender(discipline: LeaderboardDiscipline): LeaderboardEntry | null {
  return selectedGender.value === 'male' ? discipline.male : discipline.female;
}

const DISCIPLINE_LABELS: Record<string, string> = {
  CoreStrength: 'Rumpfkraft',
  MedicineBallPush: 'Medizinballstoss',
  StandingLongJump: 'Standweitsprung',
  ShuttleRun: 'Pendellauf',
  TwelveMinutesRun: '12-Minutenlauf',
  OneLegStand: 'Einbeinstand',
};

function disciplineLabel(discipline: string): string {
  return DISCIPLINE_LABELS[discipline] ?? discipline;
}

function formatResult(discipline: string, result: number): string {
  switch (discipline) {
    case 'CoreStrength':
      return `${result} Sek.`;
    case 'MedicineBallPush':
    case 'StandingLongJump':
      return `${result} cm`;
    case 'ShuttleRun':
      // result is in milliseconds – display as seconds with two decimal places
      return `${(result / 1000).toFixed(2)} Sek.`;
    case 'TwelveMinutesRun':
      return `${result} Runden`;
    case 'OneLegStand':
      return `${result} Sek.`;
    default:
      return String(result);
  }
}

function formatDate(momentUtc: string): string {
  return new Date(momentUtc).toLocaleDateString('de-CH', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });
}

function formatSchoolYear(schoolYear: number): string {
  return `${schoolYear}/${String(schoolYear + 1).slice(-2)}`;
}
</script>

<style lang="scss"></style>
