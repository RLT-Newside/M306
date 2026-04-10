const Database = require('better-sqlite3');
const path = require('path');

const DB_PATH = path.join(__dirname, 'fitness-check.db');

let db;

function getDb() {
  if (!db) {
    db = new Database(DB_PATH);
    db.pragma('journal_mode = WAL');
    db.pragma('foreign_keys = ON');
  }
  return db;
}

function initializeDatabase() {
  const db = getDb();

  db.exec(`
    CREATE TABLE IF NOT EXISTS disciplines (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      name TEXT NOT NULL,
      unit TEXT NOT NULL,
      sort_order INTEGER NOT NULL,
      higher_is_better INTEGER NOT NULL DEFAULT 1
    );

    CREATE TABLE IF NOT EXISTS classes (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      name TEXT NOT NULL,
      school_year TEXT NOT NULL,
      profession TEXT NOT NULL,
      UNIQUE(name, school_year)
    );

    CREATE TABLE IF NOT EXISTS students (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      first_name TEXT NOT NULL,
      last_name TEXT NOT NULL,
      gender TEXT NOT NULL CHECK(gender IN ('m', 'f')),
      class_id INTEGER NOT NULL,
      FOREIGN KEY (class_id) REFERENCES classes(id)
    );

    CREATE TABLE IF NOT EXISTS fitness_results (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      student_id INTEGER NOT NULL,
      discipline_id INTEGER NOT NULL,
      attempt_number INTEGER NOT NULL,
      value REAL NOT NULL,
      is_best INTEGER NOT NULL DEFAULT 0,
      date TEXT NOT NULL,
      school_year TEXT NOT NULL,
      FOREIGN KEY (student_id) REFERENCES students(id),
      FOREIGN KEY (discipline_id) REFERENCES disciplines(id)
    );

    CREATE TABLE IF NOT EXISTS annotations (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      student_id INTEGER NOT NULL,
      discipline_id INTEGER NOT NULL,
      school_year TEXT NOT NULL,
      note TEXT NOT NULL,
      FOREIGN KEY (student_id) REFERENCES students(id),
      FOREIGN KEY (discipline_id) REFERENCES disciplines(id),
      UNIQUE(student_id, discipline_id, school_year)
    );

    CREATE TABLE IF NOT EXISTS scoring_tables (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      discipline_id INTEGER NOT NULL,
      gender TEXT NOT NULL CHECK(gender IN ('m', 'f')),
      min_value REAL NOT NULL,
      max_value REAL NOT NULL,
      points INTEGER NOT NULL,
      FOREIGN KEY (discipline_id) REFERENCES disciplines(id)
    );
  `);

  return db;
}

module.exports = { getDb, initializeDatabase };
