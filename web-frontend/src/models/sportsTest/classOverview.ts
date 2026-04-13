export interface CohortInfo {
  id: string;
  profession: string;
  baccalaureate: boolean;
  schoolYear: number;
  firstSchoolYear: number;
  classNameVocationalEducation: string;
  classNameBaccalaureate: string | null;
}

export interface StudentDisciplineResult {
  result: number | null;
  points: number;
  momentUtc: string | null;
  annotation: string | null;
}

export interface ClassOverviewStudent {
  userId: string;
  firstName: string;
  lastName: string;
  gender: string;
  disciplines: Record<string, StudentDisciplineResult>;
  totalPoints: number | null;
  averagePoints: number | null;
  rating: string | null;
}

export interface ClassDisciplineAverage {
  averageResult: number | null;
  averagePoints: number | null;
}

export interface ClassOverview {
  cohort: CohortInfo;
  students: ClassOverviewStudent[];
  classAverages: Record<string, ClassDisciplineAverage>;
}
