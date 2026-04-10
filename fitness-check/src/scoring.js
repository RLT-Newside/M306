const { getDb } = require('./database');

function getPointsForValue(disciplineId, gender, value) {
  const db = getDb();
  const row = db.prepare(`
    SELECT points FROM scoring_tables
    WHERE discipline_id = ? AND gender = ? AND min_value <= ? AND max_value >= ?
    ORDER BY points DESC LIMIT 1
  `).get(disciplineId, gender, value, value);
  return row ? row.points : 0;
}

function calculateGrade(avgPoints) {
  if (avgPoints >= 5.25) return 6.0;
  if (avgPoints >= 4.75) return 5.5;
  if (avgPoints >= 4.25) return 5.0;
  if (avgPoints >= 3.75) return 4.5;
  if (avgPoints >= 3.25) return 4.0;
  if (avgPoints >= 2.75) return 3.5;
  if (avgPoints >= 2.25) return 3.0;
  if (avgPoints >= 1.75) return 2.5;
  if (avgPoints >= 1.25) return 2.0;
  return 1.5;
}

module.exports = { getPointsForValue, calculateGrade };
