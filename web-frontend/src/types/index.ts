export interface Discipline {
  id: number
  name: string
  unit: string
  sort_order: number
  higher_is_better: number
}

export interface ClassInfo {
  id: number
  name: string
  school_year: string
  profession: string
  student_count?: number
}

export interface Student {
  id: number
  first_name: string
  last_name: string
  gender: 'm' | 'f'
  class_id: number
  class_name?: string
  school_year?: string
  profession?: string
}

export interface FitnessResult {
  id: number
  student_id: number
  discipline_id: number
  attempt_number: number
  value: number
  is_best: number
  date: string
  school_year: string
}

export interface Annotation {
  id: number
  student_id: number
  discipline_id: number
  school_year: string
  note: string
}

export interface DisciplineData {
  bestResult: FitnessResult | null
  attempts: FitnessResult[]
  annotation: Annotation | null
  points: number | null
  missing: boolean
}

export interface StudentResult extends Student {
  disciplines: Record<number, DisciplineData>
  totalPoints: number
  disciplineCount: number
  avgPoints: number
  grade: number | null
}

export interface ClassAverage {
  avgValue: number | null
  avgPoints: number | null
  count: number
}

export interface ClassOverviewResponse {
  classInfo: ClassInfo
  disciplines: Discipline[]
  students: StudentResult[]
  classAverages: Record<number, ClassAverage>
}

export interface LeaderboardRecord {
  rank: number
  id: number
  value: number
  date: string
  school_year: string
  attempt_number: number
  student_id: number
  first_name: string
  last_name: string
  gender: string
  class_name: string
  profession: string
}

export interface LeaderboardEntry {
  discipline: Discipline
  records: Record<string, LeaderboardRecord[]>
}

export interface LeaderboardResponse {
  disciplines: Discipline[]
  leaderboard: Record<number, LeaderboardEntry>
}
