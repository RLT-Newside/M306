export interface StageUsage {
  isUsed: boolean;
  rallyCount?: number;
  rallyTitles?: string[];
}

export interface StageUsageState {
  isUsed: boolean | null;
  rallyCount: number | null;
  rallyTitles: string[];
}

export interface StageActivityUsage {
  isUsed: boolean;
  rallyCount?: number;
}

export interface StageActivityUsageState {
  isUsed: boolean | null;
  rallyCount: number | null;
}

export interface RallyDeletionImpact {
  assignmentCount: number;
  rallyStageCount: number;
  stageActivityResultCount?: number;
}

export interface RallyDeletionImpactState {
  assignmentCount: number;
  rallyStageCount: number;
  stageActivityResultCount: number | null;
}
