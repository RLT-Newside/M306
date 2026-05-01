import calculationTable from '@/data/calculationTable.json';

export type DisciplineKey =
  | 'TwelveMinutesRun'
  | 'StandingLongJump'
  | 'CoreStrength'
  | 'OneLegStand'
  | 'ShuttleRun'
  | 'MedicineBallPush';

export interface DisciplineConfig {
  name: string;
  key: DisciplineKey;
  unit: string;
  lowerIsBetter: boolean;
}

export const DISCIPLINE_CONFIG: DisciplineConfig[] = [
  { name: '12-Minutenlauf', key: 'TwelveMinutesRun', unit: 'Runden', lowerIsBetter: false },
  { name: 'Standweitsprung', key: 'StandingLongJump', unit: 'cm', lowerIsBetter: false },
  { name: 'Rumpfkraft', key: 'CoreStrength', unit: 'Anz.', lowerIsBetter: false },
  { name: 'Einbeinstand', key: 'OneLegStand', unit: 'Sek', lowerIsBetter: false },
  { name: 'Shuttle-Run', key: 'ShuttleRun', unit: 'ms', lowerIsBetter: true },
  { name: 'Medizinballstoss', key: 'MedicineBallPush', unit: 'cm', lowerIsBetter: false },
];

export const DISCIPLINES = DISCIPLINE_CONFIG.map((d) => d.name);

export function getDisciplineConfig(name: string): DisciplineConfig | undefined {
  return DISCIPLINE_CONFIG.find((d) => d.name === name);
}

export function unitForDiscipline(name: string): string {
  return getDisciplineConfig(name)?.unit ?? '';
}

interface TableRow {
  Points: number;
  Gender: string;
  Grade: string;
  MedicineBallPush: number;
  StandingLongJump: number;
  CoreStrength: number;
  OneLegStand: number;
  ShuttleRun: number;
  TwelveMinutesRun: number;
}

const table = calculationTable as TableRow[];

export function calculatePoints(
  discipline: string,
  gender: 'male' | 'female',
  rawValue: number
): number {
  const config = getDisciplineConfig(discipline);
  if (!config) return 0;

  const genderKey = gender === 'male' ? 'm' : 'f';
  const rows = table.filter((r) => r.Gender === genderKey && r.Points > 0);

  let bestPoints = 0;
  for (const row of rows) {
    const threshold = row[config.key] as number;
    if (config.lowerIsBetter) {
      if (rawValue <= threshold && row.Points > bestPoints) {
        bestPoints = row.Points;
      }
    } else {
      if (rawValue >= threshold && row.Points > bestPoints) {
        bestPoints = row.Points;
      }
    }
  }
  return bestPoints;
}

export function getGrade(points: number): string {
  if (points >= 22) return 'UED';
  if (points >= 16) return 'UEE';
  if (points >= 7) return 'EE';
  if (points >= 1) return 'TE';
  return 'TE';
}

export function getGradeLabel(grade: string): string {
  switch (grade) {
    case 'TE': return 'Teilerfüllt';
    case 'EE': return 'Erfüllt';
    case 'UEE': return 'Übertroffen';
    case 'UED': return 'Deutlich übertroffen';
    default: return '';
  }
}

export function computeSwissNote(avgPoints: number): string {
  const note = 1 + (avgPoints / 25) * 5;
  return Math.min(6, Math.max(1, note)).toFixed(1);
}
