export interface LeaderboardEntry {
  result: number;
  momentUtc: string;
  schoolYear: number;
  profession: string;
  className: string;
}

export interface LeaderboardDiscipline {
  discipline: string;
  male: LeaderboardEntry | null;
  female: LeaderboardEntry | null;
}

export interface Leaderboard {
  disciplines: LeaderboardDiscipline[];
}
