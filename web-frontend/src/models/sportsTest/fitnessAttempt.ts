export interface FitnessAttempt {
  id: string;
  studentName: string;
  gender: 'male' | 'female';
  discipline: string;
  value: number;
  unit: string;
  date: string;
  schoolYear: string;
  classOrProfession: string;
  annotation?: string;
}
