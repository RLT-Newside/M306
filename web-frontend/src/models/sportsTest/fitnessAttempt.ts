export type Discipline =
  | '12-Minutenlauf'
  | 'Standweitsprung'
  | 'Sit-ups'
  | 'Liegestütze'
  | 'Rumpfbeuge'
  | 'Ballwurf';

export type Unit = 'Runden' | 'cm' | 'Anzahl' | 'm';

export interface FitnessAttempt {
  id: string;
  studentName: string;
  gender: 'male' | 'female';
  discipline: Discipline;
  value: number;
  unit: Unit;
  date: string;
  schoolYear: string;
  classOrProfession: string;
  points: number;
}
