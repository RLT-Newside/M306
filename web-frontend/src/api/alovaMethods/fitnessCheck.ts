import type { Leaderboard } from '@/models/sportsTest/leaderboard';
import type { ClassOverview, CohortInfo } from '@/models/sportsTest/classOverview';
import type { YearOverview } from '@/models/sportsTest/myResults';
import { fitnessCheckAlova } from '../alova';

// ── Leaderboard ──────────────────────────────────────────────────────────────

export const getLeaderboard = () => fitnessCheckAlova.Get<Leaderboard>('leaderboard');

// ── Class overview ───────────────────────────────────────────────────────────

export const getCohorts = () => fitnessCheckAlova.Get<CohortInfo[]>('classOverview/cohorts');

export const getClassOverview = (cohortId: string) =>
  fitnessCheckAlova.Get<ClassOverview>(`classOverview/${cohortId}`);

export const setResult = (
  cohortId: string,
  userId: string,
  discipline: string,
  body: { result: number; gender: string }
) =>
  fitnessCheckAlova.Put(
    `classOverview/${cohortId}/students/${userId}/disciplines/${discipline}`,
    JSON.stringify(body)
  );

export const deleteResult = (cohortId: string, userId: string, discipline: string) =>
  fitnessCheckAlova.Delete(
    `classOverview/${cohortId}/students/${userId}/disciplines/${discipline}`
  );

export const setAnnotation = (
  cohortId: string,
  userId: string,
  discipline: string,
  annotation: string
) =>
  fitnessCheckAlova.Put(
    `classOverview/${cohortId}/students/${userId}/disciplines/${discipline}/annotation`,
    JSON.stringify({ annotation })
  );

// ── Student overview ─────────────────────────────────────────────────────────

export const getOverview = () => fitnessCheckAlova.Get<YearOverview[]>('overview');

/** Fetches remaining-attempt info for a single discipline. The discipline key
 *  maps to a camelCase URL segment (e.g. "CoreStrength" → "coreStrength"). */
export const getDisciplineInfo = (disciplineKey: string) => {
  const route = disciplineKey.charAt(0).toLowerCase() + disciplineKey.slice(1);
  return fitnessCheckAlova.Get<{ remainingAttempts: number; maxAllowedAttempts: number }>(route);
};

// ── Student attempt submission ───────────────────────────────────────────────

export const submitCoreStrength = (resultInSeconds: number) =>
  fitnessCheckAlova.Post('coreStrength', JSON.stringify({ resultInSeconds }));

export const submitMedicineBallPush = (resultInCentimeters: number) =>
  fitnessCheckAlova.Post('medicineBallPush', JSON.stringify({ resultInCentimeters }));

export const submitStandingLongJump = (resultInCentimeters: number) =>
  fitnessCheckAlova.Post('standingLongJump', JSON.stringify({ resultInCentimeters }));

export const submitShuttleRun = (resultInMilliseconds: number) =>
  fitnessCheckAlova.Post('shuttleRun', JSON.stringify({ resultInMilliseconds }));

export const submitTwelveMinutesRun = (resultInRounds: number) =>
  fitnessCheckAlova.Post('twelveMinutesRun', JSON.stringify({ resultInRounds }));

export const submitOneLegStand = (resultInSeconds: number, foot: 'Left' | 'Right') =>
  fitnessCheckAlova.Post('oneLegStand', JSON.stringify({ resultInSeconds, foot }));
