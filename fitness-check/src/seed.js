const { getDb, initializeDatabase } = require('./database');

function seedDatabase() {
  const db = initializeDatabase();

  const count = db.prepare('SELECT COUNT(*) as c FROM disciplines').get();
  if (count.c > 0) {
    console.log('Database already seeded.');
    return;
  }

  console.log('Seeding database...');

  // Disciplines
  const insertDiscipline = db.prepare(
    'INSERT INTO disciplines (name, unit, sort_order, higher_is_better) VALUES (?, ?, ?, ?)'
  );
  const disciplines = [
    ['12-Minutenlauf', 'Runden', 1, 1],
    ['Liegestütze', 'Anzahl', 2, 1],
    ['Weitsprung', 'cm', 3, 1],
    ['Sit-ups', 'Anzahl', 4, 1],
    ['Rumpfbeugen', 'cm', 5, 1],
    ['Shuttle Run', 'Sekunden', 6, 0],
  ];
  for (const d of disciplines) {
    insertDiscipline.run(...d);
  }

  // Scoring tables (gender-specific)
  const insertScoring = db.prepare(
    'INSERT INTO scoring_tables (discipline_id, gender, min_value, max_value, points) VALUES (?, ?, ?, ?, ?)'
  );

  // 12-Minutenlauf (rounds) - Male
  const scoringData = [
    // discipline_id, gender, min, max, points
    // 1: 12-Minutenlauf - Male
    [1, 'm', 0, 21, 1], [1, 'm', 22, 26, 2], [1, 'm', 27, 30, 3],
    [1, 'm', 31, 34, 4], [1, 'm', 35, 39, 5], [1, 'm', 40, 999, 6],
    // 1: 12-Minutenlauf - Female
    [1, 'f', 0, 17, 1], [1, 'f', 18, 21, 2], [1, 'f', 22, 25, 3],
    [1, 'f', 26, 29, 4], [1, 'f', 30, 34, 5], [1, 'f', 35, 999, 6],

    // 2: Liegestütze - Male
    [2, 'm', 0, 10, 1], [2, 'm', 11, 17, 2], [2, 'm', 18, 24, 3],
    [2, 'm', 25, 31, 4], [2, 'm', 32, 38, 5], [2, 'm', 39, 999, 6],
    // 2: Liegestütze - Female
    [2, 'f', 0, 6, 1], [2, 'f', 7, 12, 2], [2, 'f', 13, 18, 3],
    [2, 'f', 19, 24, 4], [2, 'f', 25, 31, 5], [2, 'f', 32, 999, 6],

    // 3: Weitsprung (cm) - Male
    [3, 'm', 0, 160, 1], [3, 'm', 161, 185, 2], [3, 'm', 186, 210, 3],
    [3, 'm', 211, 230, 4], [3, 'm', 231, 250, 5], [3, 'm', 251, 999, 6],
    // 3: Weitsprung - Female
    [3, 'f', 0, 130, 1], [3, 'f', 131, 155, 2], [3, 'f', 156, 175, 3],
    [3, 'f', 176, 195, 4], [3, 'f', 196, 215, 5], [3, 'f', 216, 999, 6],

    // 4: Sit-ups - Male
    [4, 'm', 0, 15, 1], [4, 'm', 16, 22, 2], [4, 'm', 23, 28, 3],
    [4, 'm', 29, 34, 4], [4, 'm', 35, 40, 5], [4, 'm', 41, 999, 6],
    // 4: Sit-ups - Female
    [4, 'f', 0, 12, 1], [4, 'f', 13, 18, 2], [4, 'f', 19, 24, 3],
    [4, 'f', 25, 30, 4], [4, 'f', 31, 36, 5], [4, 'f', 37, 999, 6],

    // 5: Rumpfbeugen (cm) - Male
    [5, 'm', -20, 0, 1], [5, 'm', 1, 5, 2], [5, 'm', 6, 10, 3],
    [5, 'm', 11, 15, 4], [5, 'm', 16, 20, 5], [5, 'm', 21, 999, 6],
    // 5: Rumpfbeugen - Female
    [5, 'f', -20, 2, 1], [5, 'f', 3, 7, 2], [5, 'f', 8, 12, 3],
    [5, 'f', 13, 17, 4], [5, 'f', 18, 22, 5], [5, 'f', 23, 999, 6],

    // 6: Shuttle Run (seconds, lower is better) - Male
    [6, 'm', 0, 10.5, 6], [6, 'm', 10.6, 11.2, 5], [6, 'm', 11.3, 12.0, 4],
    [6, 'm', 12.1, 12.8, 3], [6, 'm', 12.9, 13.5, 2], [6, 'm', 13.6, 999, 1],
    // 6: Shuttle Run - Female
    [6, 'f', 0, 11.5, 6], [6, 'f', 11.6, 12.2, 5], [6, 'f', 12.3, 13.0, 4],
    [6, 'f', 13.1, 13.8, 3], [6, 'f', 13.9, 14.5, 2], [6, 'f', 14.6, 999, 1],
  ];

  const insertScoringTx = db.transaction(() => {
    for (const s of scoringData) {
      insertScoring.run(...s);
    }
  });
  insertScoringTx();

  // Classes (multiple school years)
  const insertClass = db.prepare(
    'INSERT INTO classes (name, school_year, profession) VALUES (?, ?, ?)'
  );
  const classes = [
    // Current year
    ['INFA3a', '2025/2026', 'Informatiker/in EFZ Applikationsentwicklung'],
    ['INFA3b', '2025/2026', 'Informatiker/in EFZ Applikationsentwicklung'],
    ['INFA3f', '2025/2026', 'Informatiker/in EFZ Applikationsentwicklung'],
    ['KE2a', '2025/2026', 'Kauffrau/Kaufmann EFZ'],
    ['ELT2a', '2025/2026', 'Elektroniker/in EFZ'],
    // Previous years
    ['INFA3a', '2024/2025', 'Informatiker/in EFZ Applikationsentwicklung'],
    ['INFA3b', '2024/2025', 'Informatiker/in EFZ Applikationsentwicklung'],
    ['KE2a', '2024/2025', 'Kauffrau/Kaufmann EFZ'],
    ['INFA3a', '2023/2024', 'Informatiker/in EFZ Applikationsentwicklung'],
    ['KE2a', '2023/2024', 'Kauffrau/Kaufmann EFZ'],
  ];
  for (const c of classes) {
    insertClass.run(...c);
  }

  // Students
  const insertStudent = db.prepare(
    'INSERT INTO students (first_name, last_name, gender, class_id) VALUES (?, ?, ?, ?)'
  );

  const studentData = [
    // INFA3a 2025/2026 (class_id=1)
    ['Luca', 'Müller', 'm', 1], ['Noah', 'Meier', 'm', 1],
    ['Finn', 'Schneider', 'm', 1], ['Leon', 'Fischer', 'm', 1],
    ['Elias', 'Weber', 'm', 1], ['Laura', 'Brunner', 'f', 1],
    ['Anna', 'Baumann', 'f', 1], ['Mia', 'Koch', 'f', 1],
    // INFA3b 2025/2026 (class_id=2)
    ['Jan', 'Huber', 'm', 2], ['Tim', 'Schmid', 'm', 2],
    ['Nico', 'Keller', 'm', 2], ['Sara', 'Wagner', 'f', 2],
    ['Lisa', 'Gerber', 'f', 2], ['Emma', 'Fuchs', 'f', 2],
    ['David', 'Steiner', 'm', 2], ['Marco', 'Berger', 'm', 2],
    // INFA3f 2025/2026 (class_id=3)
    ['Robin', 'Zimmermann', 'm', 3], ['Alex', 'Moser', 'm', 3],
    ['Sven', 'Graf', 'm', 3], ['Julia', 'Frei', 'f', 3],
    ['Nina', 'Roth', 'f', 3], ['Lea', 'Bühler', 'f', 3],
    ['Patrick', 'Wyss', 'm', 3], ['Fabian', 'Studer', 'm', 3],
    // KE2a 2025/2026 (class_id=4)
    ['Chiara', 'Brunner', 'f', 4], ['Sofia', 'Hartmann', 'f', 4],
    ['Lara', 'Lehmann', 'f', 4], ['Ben', 'Sutter', 'm', 4],
    ['Nils', 'Ammann', 'm', 4], ['Yannick', 'Blaser', 'm', 4],
    // ELT2a 2025/2026 (class_id=5)
    ['Lukas', 'Meier', 'm', 5], ['Jonas', 'Widmer', 'm', 5],
    ['Samuel', 'Peter', 'm', 5], ['Florian', 'Hofer', 'm', 5],
    ['Selina', 'Brun', 'f', 5], ['Nadine', 'Lüthi', 'f', 5],
    // Previous years students
    // INFA3a 2024/2025 (class_id=6)
    ['Rafael', 'Künzli', 'm', 6], ['Dominik', 'Vogt', 'm', 6],
    ['Simon', 'Bachmann', 'm', 6], ['Tanja', 'Aebi', 'f', 6],
    ['Sabrina', 'Bieri', 'f', 6],
    // INFA3b 2024/2025 (class_id=7)
    ['Michael', 'Stöckli', 'm', 7], ['Thomas', 'Zaugg', 'm', 7],
    ['Sandra', 'Lanz', 'f', 7], ['Corina', 'Wälti', 'f', 7],
    // KE2a 2024/2025 (class_id=8)
    ['Elena', 'Moor', 'f', 8], ['Maria', 'Baumgartner', 'f', 8],
    ['Kevin', 'Flückiger', 'm', 8], ['Peter', 'Zürcher', 'm', 8],
    // INFA3a 2023/2024 (class_id=9)
    ['Daniel', 'Rohrer', 'm', 9], ['Stefan', 'Bosshard', 'm', 9],
    ['Andrea', 'Spörri', 'f', 9],
    // KE2a 2023/2024 (class_id=10)
    ['Hannah', 'Stalder', 'f', 10], ['Melanie', 'Grob', 'f', 10],
    ['Oliver', 'Käser', 'm', 10],
  ];

  const studentIds = [];
  for (const s of studentData) {
    const info = insertStudent.run(...s);
    studentIds.push(info.lastInsertRowid);
  }

  // Fitness results with seed data
  const insertResult = db.prepare(
    'INSERT INTO fitness_results (student_id, discipline_id, attempt_number, value, is_best, date, school_year) VALUES (?, ?, ?, ?, ?, ?, ?)'
  );

  const rng = createSeededRng(42);

  function randomInRange(min, max) {
    return Math.round((min + rng() * (max - min)) * 10) / 10;
  }

  // Ranges for generating realistic data per discipline and gender
  const valueRanges = {
    1: { m: [20, 45], f: [16, 38] },   // 12-min run (rounds)
    2: { m: [8, 42], f: [5, 35] },      // Push-ups
    3: { m: [150, 270], f: [120, 230] }, // Long jump (cm)
    4: { m: [12, 45], f: [10, 40] },     // Sit-ups
    5: { m: [-5, 25], f: [0, 28] },      // Trunk flexion (cm)
    6: { m: [9.5, 14.0], f: [10.5, 15.0] }, // Shuttle run (seconds)
  };

  const schoolYearByClass = {};
  for (const c of classes) {
    schoolYearByClass[c[0] + '_' + c[1]] = c[1];
  }

  // Map student index to class info for school year lookup
  const classSchoolYears = [
    '2025/2026', '2025/2026', '2025/2026', '2025/2026', '2025/2026',
    '2024/2025', '2024/2025', '2024/2025', '2023/2024', '2023/2024',
  ];

  const dates2025 = ['2025-09-15', '2025-09-22', '2025-10-06'];
  const dates2024 = ['2024-09-16', '2024-09-23', '2024-10-07'];
  const dates2023 = ['2023-09-18', '2023-09-25', '2023-10-09'];

  const insertResultTx = db.transaction(() => {
    for (let studentIdx = 0; studentIdx < studentData.length; studentIdx++) {
      const s = studentData[studentIdx];
      const studentId = studentIdx + 1;
      const gender = s[2];
      const classId = s[3];

      // Determine school year and dates
      let schoolYear, testDates;
      if (classId <= 5) {
        schoolYear = '2025/2026';
        testDates = dates2025;
      } else if (classId <= 8) {
        schoolYear = '2024/2025';
        testDates = dates2024;
      } else {
        schoolYear = '2023/2024';
        testDates = dates2023;
      }

      for (let discId = 1; discId <= 6; discId++) {
        // Some students might be missing results (for quality management testing)
        if (rng() < 0.08) {
          continue; // Skip - missing result
        }

        const range = valueRanges[discId][gender];
        const numAttempts = discId === 1 ? 1 : (rng() < 0.3 ? 1 : (rng() < 0.5 ? 2 : 3));

        let bestValue = null;
        let bestAttempt = 0;
        const higherIsBetter = discId !== 6;
        const attempts = [];

        for (let attempt = 1; attempt <= numAttempts; attempt++) {
          const value = randomInRange(range[0], range[1]);
          const dateIdx = Math.min(attempt - 1, testDates.length - 1);
          attempts.push({ attempt, value, date: testDates[dateIdx] });

          if (bestValue === null ||
              (higherIsBetter && value > bestValue) ||
              (!higherIsBetter && value < bestValue)) {
            bestValue = value;
            bestAttempt = attempt;
          }
        }

        for (const a of attempts) {
          insertResult.run(
            studentId, discId, a.attempt, a.value,
            a.attempt === bestAttempt ? 1 : 0,
            a.date, schoolYear
          );
        }
      }
    }
  });
  insertResultTx();

  // Add some annotations for missing results
  const insertAnnotation = db.prepare(
    'INSERT OR IGNORE INTO annotations (student_id, discipline_id, school_year, note) VALUES (?, ?, ?, ?)'
  );

  const annotationNotes = ['verletzt', 'dispensiert', 'krank', 'abwesend'];

  // Find students with missing disciplines and add annotations for some
  const allStudents = db.prepare('SELECT id, class_id FROM students').all();
  for (const student of allStudents) {
    let schoolYear;
    if (student.class_id <= 5) schoolYear = '2025/2026';
    else if (student.class_id <= 8) schoolYear = '2024/2025';
    else schoolYear = '2023/2024';

    for (let discId = 1; discId <= 6; discId++) {
      const hasResult = db.prepare(
        'SELECT COUNT(*) as c FROM fitness_results WHERE student_id = ? AND discipline_id = ? AND school_year = ?'
      ).get(student.id, discId, schoolYear);

      if (hasResult.c === 0 && rng() < 0.7) {
        const note = annotationNotes[Math.floor(rng() * annotationNotes.length)];
        insertAnnotation.run(student.id, discId, schoolYear, note);
      }
    }
  }

  console.log('Database seeded successfully.');
}

// Simple seeded random number generator
function createSeededRng(seed) {
  let s = seed;
  return function() {
    s = (s * 1664525 + 1013904223) & 0xffffffff;
    return (s >>> 0) / 0xffffffff;
  };
}

module.exports = { seedDatabase };
