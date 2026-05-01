import type { FitnessAttempt } from '@/models/sportsTest/fitnessAttempt';

export const mockAttempts: FitnessAttempt[] = [
  // ── INFA3a ────────────────────────────────────────────────────
  // Max Mustermann (male)
  { id: '1',  studentName: 'Max Mustermann',   gender: 'male', discipline: '12-Minutenlauf',  value: 40,    unit: 'Runden', date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '2',  studentName: 'Max Mustermann',   gender: 'male', discipline: 'Standweitsprung',  value: 235,   unit: 'cm',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '3',  studentName: 'Max Mustermann',   gender: 'male', discipline: 'Rumpfkraft',       value: 120,   unit: 'Anz.',   date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '4',  studentName: 'Max Mustermann',   gender: 'male', discipline: 'Einbeinstand',     value: 45,    unit: 'Sek',    date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '5',  studentName: 'Max Mustermann',   gender: 'male', discipline: 'Shuttle-Run',      value: 10300, unit: 'ms',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '6',  studentName: 'Max Mustermann',   gender: 'male', discipline: 'Medizinballstoss', value: 650,   unit: 'cm',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },

  // Tim Weber (male)
  { id: '7',  studentName: 'Tim Weber',        gender: 'male', discipline: '12-Minutenlauf',  value: 44,    unit: 'Runden', date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '8',  studentName: 'Tim Weber',        gender: 'male', discipline: 'Standweitsprung',  value: 270,   unit: 'cm',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '9',  studentName: 'Tim Weber',        gender: 'male', discipline: 'Rumpfkraft',       value: 230,   unit: 'Anz.',   date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '10', studentName: 'Tim Weber',        gender: 'male', discipline: 'Einbeinstand',     value: 71,    unit: 'Sek',    date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '11', studentName: 'Tim Weber',        gender: 'male', discipline: 'Shuttle-Run',      value: 9320,  unit: 'ms',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '12', studentName: 'Tim Weber',        gender: 'male', discipline: 'Medizinballstoss', value: 790,   unit: 'cm',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },

  // Laura Schmidt (female)
  { id: '13', studentName: 'Laura Schmidt',    gender: 'female', discipline: '12-Minutenlauf',  value: 34,    unit: 'Runden', date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '14', studentName: 'Laura Schmidt',    gender: 'female', discipline: 'Standweitsprung',  value: 175,   unit: 'cm',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '15', studentName: 'Laura Schmidt',    gender: 'female', discipline: 'Rumpfkraft',       value: 144,   unit: 'Anz.',   date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '16', studentName: 'Laura Schmidt',    gender: 'female', discipline: 'Einbeinstand',     value: 51,    unit: 'Sek',    date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '17', studentName: 'Laura Schmidt',    gender: 'female', discipline: 'Shuttle-Run',      value: 10760, unit: 'ms',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '18', studentName: 'Laura Schmidt',    gender: 'female', discipline: 'Medizinballstoss', value: 481,   unit: 'cm',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },

  // Jonas Becker (male)
  { id: '19', studentName: 'Jonas Becker',     gender: 'male', discipline: '12-Minutenlauf',  value: 37,    unit: 'Runden', date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '20', studentName: 'Jonas Becker',     gender: 'male', discipline: 'Standweitsprung',  value: 210,   unit: 'cm',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '21', studentName: 'Jonas Becker',     gender: 'male', discipline: 'Rumpfkraft',       value: 70,    unit: 'Anz.',   date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '22', studentName: 'Jonas Becker',     gender: 'male', discipline: 'Einbeinstand',     value: 35,    unit: 'Sek',    date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '23', studentName: 'Jonas Becker',     gender: 'male', discipline: 'Shuttle-Run',      value: 11000, unit: 'ms',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '24', studentName: 'Jonas Becker',     gender: 'male', discipline: 'Medizinballstoss', value: 550,   unit: 'cm',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },

  // Anna Fischer (female)
  { id: '25', studentName: 'Anna Fischer',     gender: 'female', discipline: '12-Minutenlauf',  value: 30,    unit: 'Runden', date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '26', studentName: 'Anna Fischer',     gender: 'female', discipline: 'Standweitsprung',  value: 147,   unit: 'cm',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '27', studentName: 'Anna Fischer',     gender: 'female', discipline: 'Rumpfkraft',       value: 63,    unit: 'Anz.',   date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '28', studentName: 'Anna Fischer',     gender: 'female', discipline: 'Einbeinstand',     value: 35,    unit: 'Sek',    date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '29', studentName: 'Anna Fischer',     gender: 'female', discipline: 'Shuttle-Run',      value: 11860, unit: 'ms',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },
  { id: '30', studentName: 'Anna Fischer',     gender: 'female', discipline: 'Medizinballstoss', value: 393,   unit: 'cm',     date: '2025-09-15', schoolYear: '2025/2026', classOrProfession: 'INFA3a' },

  // ── INFA3b ────────────────────────────────────────────────────
  // Stefan Müller (male)
  { id: '31', studentName: 'Stefan Müller',    gender: 'male', discipline: '12-Minutenlauf',  value: 42,    unit: 'Runden', date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '32', studentName: 'Stefan Müller',    gender: 'male', discipline: 'Standweitsprung',  value: 260,   unit: 'cm',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '33', studentName: 'Stefan Müller',    gender: 'male', discipline: 'Rumpfkraft',       value: 190,   unit: 'Anz.',   date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '34', studentName: 'Stefan Müller',    gender: 'male', discipline: 'Einbeinstand',     value: 58,    unit: 'Sek',    date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '35', studentName: 'Stefan Müller',    gender: 'male', discipline: 'Shuttle-Run',      value: 9600,  unit: 'ms',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '36', studentName: 'Stefan Müller',    gender: 'male', discipline: 'Medizinballstoss', value: 750,   unit: 'cm',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },

  // Daniel Keller (male)
  { id: '37', studentName: 'Daniel Keller',    gender: 'male', discipline: '12-Minutenlauf',  value: 36,    unit: 'Runden', date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '38', studentName: 'Daniel Keller',    gender: 'male', discipline: 'Standweitsprung',  value: 205,   unit: 'cm',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '39', studentName: 'Daniel Keller',    gender: 'male', discipline: 'Rumpfkraft',       value: 60,    unit: 'Anz.',   date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '40', studentName: 'Daniel Keller',    gender: 'male', discipline: 'Einbeinstand',     value: 33,    unit: 'Sek',    date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '41', studentName: 'Daniel Keller',    gender: 'male', discipline: 'Shuttle-Run',      value: 11140, unit: 'ms',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '42', studentName: 'Daniel Keller',    gender: 'male', discipline: 'Medizinballstoss', value: 530,   unit: 'cm',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },

  // Sophie Zimmermann (female)
  { id: '43', studentName: 'Sophie Zimmermann', gender: 'female', discipline: '12-Minutenlauf',  value: 38,    unit: 'Runden', date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '44', studentName: 'Sophie Zimmermann', gender: 'female', discipline: 'Standweitsprung',  value: 189,   unit: 'cm',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '45', studentName: 'Sophie Zimmermann', gender: 'female', discipline: 'Rumpfkraft',       value: 207,   unit: 'Anz.',   date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '46', studentName: 'Sophie Zimmermann', gender: 'female', discipline: 'Einbeinstand',     value: 71,    unit: 'Sek',    date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '47', studentName: 'Sophie Zimmermann', gender: 'female', discipline: 'Shuttle-Run',      value: 10220, unit: 'ms',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '48', studentName: 'Sophie Zimmermann', gender: 'female', discipline: 'Medizinballstoss', value: 525,   unit: 'cm',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },

  // Patrick Huber (male)
  { id: '49', studentName: 'Patrick Huber',    gender: 'male', discipline: '12-Minutenlauf',  value: 34,    unit: 'Runden', date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '50', studentName: 'Patrick Huber',    gender: 'male', discipline: 'Standweitsprung',  value: 195,   unit: 'cm',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '51', studentName: 'Patrick Huber',    gender: 'male', discipline: 'Rumpfkraft',       value: 40,    unit: 'Anz.',   date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '52', studentName: 'Patrick Huber',    gender: 'male', discipline: 'Einbeinstand',     value: 29,    unit: 'Sek',    date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '53', studentName: 'Patrick Huber',    gender: 'male', discipline: 'Shuttle-Run',      value: 11420, unit: 'ms',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '54', studentName: 'Patrick Huber',    gender: 'male', discipline: 'Medizinballstoss', value: 490,   unit: 'cm',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },

  // Sarah Meyer (female) - some missing results
  { id: '55', studentName: 'Sarah Meyer',     gender: 'female', discipline: '12-Minutenlauf',  value: 28,    unit: 'Runden', date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '56', studentName: 'Sarah Meyer',     gender: 'female', discipline: 'Standweitsprung',  value: 140,   unit: 'cm',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '57', studentName: 'Sarah Meyer',     gender: 'female', discipline: 'Rumpfkraft',       value: 45,    unit: 'Anz.',   date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },
  { id: '58', studentName: 'Sarah Meyer',     gender: 'female', discipline: 'Einbeinstand',     value: 0,     unit: 'Sek',    date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b', annotation: 'verletzt' },
  { id: '59', studentName: 'Sarah Meyer',     gender: 'female', discipline: 'Shuttle-Run',      value: 0,     unit: 'ms',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b', annotation: 'verletzt' },
  { id: '60', studentName: 'Sarah Meyer',     gender: 'female', discipline: 'Medizinballstoss', value: 360,   unit: 'cm',     date: '2025-09-22', schoolYear: '2025/2026', classOrProfession: 'INFA3b' },

  // ── INFA3f ────────────────────────────────────────────────────
  // Michael Berger (male) - top performer
  { id: '61', studentName: 'Michael Berger',   gender: 'male', discipline: '12-Minutenlauf',  value: 45,    unit: 'Runden', date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '62', studentName: 'Michael Berger',   gender: 'male', discipline: 'Standweitsprung',  value: 285,   unit: 'cm',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '63', studentName: 'Michael Berger',   gender: 'male', discipline: 'Rumpfkraft',       value: 290,   unit: 'Anz.',   date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '64', studentName: 'Michael Berger',   gender: 'male', discipline: 'Einbeinstand',     value: 100,   unit: 'Sek',    date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '65', studentName: 'Michael Berger',   gender: 'male', discipline: 'Shuttle-Run',      value: 8900,  unit: 'ms',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '66', studentName: 'Michael Berger',   gender: 'male', discipline: 'Medizinballstoss', value: 850,   unit: 'cm',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },

  // Jessica Weber (female)
  { id: '67', studentName: 'Jessica Weber',    gender: 'female', discipline: '12-Minutenlauf',  value: 36,    unit: 'Runden', date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '68', studentName: 'Jessica Weber',    gender: 'female', discipline: 'Standweitsprung',  value: 182,   unit: 'cm',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '69', studentName: 'Jessica Weber',    gender: 'female', discipline: 'Rumpfkraft',       value: 171,   unit: 'Anz.',   date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '70', studentName: 'Jessica Weber',    gender: 'female', discipline: 'Einbeinstand',     value: 58,    unit: 'Sek',    date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '71', studentName: 'Jessica Weber',    gender: 'female', discipline: 'Shuttle-Run',      value: 10480, unit: 'ms',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '72', studentName: 'Jessica Weber',    gender: 'female', discipline: 'Medizinballstoss', value: 503,   unit: 'cm',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },

  // Marco Steiner (male)
  { id: '73', studentName: 'Marco Steiner',    gender: 'male', discipline: '12-Minutenlauf',  value: 38,    unit: 'Runden', date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '74', studentName: 'Marco Steiner',    gender: 'male', discipline: 'Standweitsprung',  value: 225,   unit: 'cm',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '75', studentName: 'Marco Steiner',    gender: 'male', discipline: 'Rumpfkraft',       value: 100,   unit: 'Anz.',   date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '76', studentName: 'Marco Steiner',    gender: 'male', discipline: 'Einbeinstand',     value: 41,    unit: 'Sek',    date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '77', studentName: 'Marco Steiner',    gender: 'male', discipline: 'Shuttle-Run',      value: 10580, unit: 'ms',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '78', studentName: 'Marco Steiner',    gender: 'male', discipline: 'Medizinballstoss', value: 610,   unit: 'cm',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },

  // Nina Keller (female) - some dispensiert
  { id: '79', studentName: 'Nina Keller',      gender: 'female', discipline: '12-Minutenlauf',  value: 0,     unit: 'Runden', date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f', annotation: 'dispensiert' },
  { id: '80', studentName: 'Nina Keller',      gender: 'female', discipline: 'Standweitsprung',  value: 154,   unit: 'cm',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '81', studentName: 'Nina Keller',      gender: 'female', discipline: 'Rumpfkraft',       value: 90,    unit: 'Anz.',   date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '82', studentName: 'Nina Keller',      gender: 'female', discipline: 'Einbeinstand',     value: 41,    unit: 'Sek',    date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },
  { id: '83', studentName: 'Nina Keller',      gender: 'female', discipline: 'Shuttle-Run',      value: 0,     unit: 'ms',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f', annotation: 'dispensiert' },
  { id: '84', studentName: 'Nina Keller',      gender: 'female', discipline: 'Medizinballstoss', value: 415,   unit: 'cm',     date: '2025-10-01', schoolYear: '2025/2026', classOrProfession: 'INFA3f' },

  // ── Historical data (previous year) ───────────────────────────
  // Michael Berger previous year
  { id: '85', studentName: 'Michael Berger',   gender: 'male', discipline: '12-Minutenlauf',  value: 42,    unit: 'Runden', date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2f' },
  { id: '86', studentName: 'Michael Berger',   gender: 'male', discipline: 'Standweitsprung',  value: 265,   unit: 'cm',     date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2f' },
  { id: '87', studentName: 'Michael Berger',   gender: 'male', discipline: 'Rumpfkraft',       value: 210,   unit: 'Anz.',   date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2f' },
  { id: '88', studentName: 'Michael Berger',   gender: 'male', discipline: 'Einbeinstand',     value: 64,    unit: 'Sek',    date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2f' },
  { id: '89', studentName: 'Michael Berger',   gender: 'male', discipline: 'Shuttle-Run',      value: 9460,  unit: 'ms',     date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2f' },
  { id: '90', studentName: 'Michael Berger',   gender: 'male', discipline: 'Medizinballstoss', value: 770,   unit: 'cm',     date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2f' },

  // Tim Weber previous year
  { id: '91', studentName: 'Tim Weber',        gender: 'male', discipline: '12-Minutenlauf',  value: 41,    unit: 'Runden', date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2a' },
  { id: '92', studentName: 'Tim Weber',        gender: 'male', discipline: 'Standweitsprung',  value: 250,   unit: 'cm',     date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2a' },
  { id: '93', studentName: 'Tim Weber',        gender: 'male', discipline: 'Rumpfkraft',       value: 175,   unit: 'Anz.',   date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2a' },
  { id: '94', studentName: 'Tim Weber',        gender: 'male', discipline: 'Einbeinstand',     value: 54,    unit: 'Sek',    date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2a' },
  { id: '95', studentName: 'Tim Weber',        gender: 'male', discipline: 'Shuttle-Run',      value: 9740,  unit: 'ms',     date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2a' },
  { id: '96', studentName: 'Tim Weber',        gender: 'male', discipline: 'Medizinballstoss', value: 730,   unit: 'cm',     date: '2024-09-20', schoolYear: '2024/2025', classOrProfession: 'INFA2a' },
];
