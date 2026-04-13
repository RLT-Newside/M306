import type { CohortInfo } from './classOverview';

export interface BestAttemptResult {
  userId: string;
  cohortId: string;
  discipline: string;
  result: number;
  points: number;
  momentUtc: string;
  leftFootResult: number | null;
  rightFootResult: number | null;
}

export interface YearOverview {
  schoolYear: number;
  bestAttempts: BestAttemptResult[];
  totalPoints: number | null;
  averagePoints: number | null;
  rating: string | null;
  cohort: CohortInfo | null;
}
