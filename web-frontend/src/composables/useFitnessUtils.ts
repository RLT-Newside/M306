import type { Discipline, Unit } from '@/models/sportsTest/fitnessAttempt';

export const DISCIPLINE_CONFIG: Record<Discipline, Unit> = {
  '12-Minutenlauf': 'Runden',
  'Standweitsprung': 'cm',
  'Sit-ups': 'Anzahl',
  'Liegestütze': 'Anzahl',
  'Rumpfbeuge': 'cm',
  'Ballwurf': 'm',
};

export const ALL_DISCIPLINES = Object.keys(DISCIPLINE_CONFIG).sort() as Discipline[];

export function unitForDiscipline(discipline: Discipline): Unit {
  return DISCIPLINE_CONFIG[discipline];
}

export function computeNote(points: number): string {
  return (1 + (points / 100) * 5).toFixed(1);
}

export function formatDate(iso: string): string {
  const [y, m, d] = iso.split('-');
  return `${d}.${m}.${y}`;
}

export function schoolYearFromDate(iso: string): string {
  const date = new Date(iso);
  const year = date.getFullYear();
  const month = date.getMonth();
  return month >= 7 ? `${year}/${year + 1}` : `${year - 1}/${year}`;
}
