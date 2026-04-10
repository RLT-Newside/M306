import type { Assignment } from '@/models/rally/assignment';
import type { Location } from '@/models/rally/location';
import type {
  RallyDeletionImpact,
  StageActivityUsage,
  StageUsage,
} from '@/models/rally/management';
import type { Constraint, Rally, RallyStage } from '@/models/rally/rally';
import type { Stage } from '@/models/rally/stage';
import type { StageActivity } from '@/models/rally/stageActivity';
import { gibzRallyAlova } from '../alova';

export const getRallies = () => gibzRallyAlova.Get<Rally[]>('rally');

export const getRally = (rallyId: string) => gibzRallyAlova.Get<Rally>(`rally/${rallyId}`);

export const createRally = (rally: Partial<Rally>) =>
  gibzRallyAlova.Post<Rally>('rally', JSON.stringify(rally));

export const deleteRally = (rallyId: string) => gibzRallyAlova.Delete<void>(`rally/${rallyId}`);

export const getRallyDeletionImpact = (rallyId: string) =>
  gibzRallyAlova.Get<RallyDeletionImpact>(`rally/${rallyId}/deletion-impact`);

export const getStages = () => gibzRallyAlova.Get<Stage[]>('stage');

export const getStage = (stageId: string) => gibzRallyAlova.Get<Stage>(`stage/${stageId}`);

export const createStage = (stage: Partial<Stage>) =>
  gibzRallyAlova.Post<Stage>('stage', JSON.stringify(stage));

export const updateStage = (stageId: string, stage: Partial<Stage>) =>
  gibzRallyAlova.Put<Stage>(`stage/${stageId}`, JSON.stringify(stage));

export const deleteStage = (stageId: string) => gibzRallyAlova.Delete<void>(`stage/${stageId}`);

export const getStageUsage = (stageId: string) =>
  gibzRallyAlova.Get<StageUsage>(`stage/${stageId}/usage`);

export const getRallyStage = (rallyId: string, rallyStageId: string) =>
  gibzRallyAlova.Get<RallyStage>(`rally/${rallyId}/stage/${rallyStageId}`);

export const addRallyStage = (rallyId: string, rallyStage: Partial<RallyStage>) =>
  gibzRallyAlova.Post<RallyStage>(`rally/${rallyId}/stage`, JSON.stringify(rallyStage));

export const deleteRallyStage = (rallyId: string, rallyStageId: string) =>
  gibzRallyAlova.Delete<void>(`rally/${rallyId}/rally-stage/${rallyStageId}`);

export const createConstraint = (
  rallyId: string,
  rallyStageId: string,
  constraint: Partial<Constraint>
) =>
  gibzRallyAlova.Post<Constraint>(
    `rally/${rallyId}/stage/${rallyStageId}/constraint`,
    JSON.stringify(constraint)
  );

export const deleteConstraint = (rallyId: string, rallyStageId: string, constraintId: string) =>
  gibzRallyAlova.Delete<void>(`rally/${rallyId}/stage/${rallyStageId}/constraint/${constraintId}`);

export const createAssignment = (rallyId: string, assignment: Partial<Assignment>) =>
  gibzRallyAlova.Post<Assignment>(`rally/${rallyId}/assignment`, JSON.stringify(assignment));

export const deleteAssignment = (rallyId: string, assignmentId: string) =>
  gibzRallyAlova.Delete<void>(`rally/${rallyId}/assignment/${assignmentId}`);

export const getStageActivities = (stageId: string) =>
  gibzRallyAlova.Get<StageActivity[]>(`stage/${stageId}/activity`);

export const getStageActivity = (stageId: string, stageActivityId: string) =>
  gibzRallyAlova.Get<StageActivity>(`stage/${stageId}/activity/${stageActivityId}`);

export const createStageActivity = (stageId: string, stageActivity: Partial<StageActivity>) =>
  gibzRallyAlova.Post<StageActivity>(`stage/${stageId}/activity`, JSON.stringify(stageActivity));

export const updateStageActivity = (
  stageId: string,
  activityId: string,
  stageActivity: Partial<StageActivity>
) =>
  gibzRallyAlova.Put<StageActivity>(
    `stage/${stageId}/activity/${activityId}`,
    JSON.stringify(stageActivity)
  );

export const removeStageActivity = (stageId: string, stageActivityId: string) =>
  gibzRallyAlova.Delete<void>(`stage/${stageId}/activity/${stageActivityId}`);

export const getStageActivityUsage = (stageId: string, stageActivityId: string) =>
  gibzRallyAlova.Get<StageActivityUsage>(`stage/${stageId}/activity/${stageActivityId}/usage`);

export const createLocation = (stageId: string, location: Partial<Location>) =>
  gibzRallyAlova.Post<Location>(`stage/${stageId}/location`, JSON.stringify(location));

export const removeLocation = (stageId: string, locationId: string) =>
  gibzRallyAlova.Delete<void>(`stage/${stageId}/location/${locationId}`);
