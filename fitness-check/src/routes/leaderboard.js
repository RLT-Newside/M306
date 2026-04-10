const express = require('express');
const { getDb } = require('../database');

const router = express.Router();

// GET /api/leaderboard - Global leaderboard (gender-specific)
// Returns the best attempt per discipline per gender, with date, school year, class/profession
router.get('/', (req, res) => {
  const db = getDb();
  const { gender, discipline_id } = req.query;

  const disciplines = db.prepare('SELECT * FROM disciplines ORDER BY sort_order').all();

  const leaderboard = {};

  for (const disc of disciplines) {
    if (discipline_id && disc.id !== parseInt(discipline_id)) continue;

    const genders = gender ? [gender] : ['m', 'f'];

    leaderboard[disc.id] = {
      discipline: disc,
      records: {},
    };

    for (const g of genders) {
      // Get the single best result across all students for this discipline and gender
      const order = disc.higher_is_better ? 'DESC' : 'ASC';

      const topResults = db.prepare(`
        SELECT
          fr.id,
          fr.value,
          fr.date,
          fr.school_year,
          fr.attempt_number,
          s.id as student_id,
          s.first_name,
          s.last_name,
          s.gender,
          c.name as class_name,
          c.profession
        FROM fitness_results fr
        JOIN students s ON s.id = fr.student_id
        JOIN classes c ON c.id = s.class_id
        WHERE fr.discipline_id = ?
          AND s.gender = ?
          AND fr.is_best = 1
        ORDER BY fr.value ${order}
        LIMIT 10
      `).all(disc.id, g);

      leaderboard[disc.id].records[g] = topResults.map((r, idx) => ({
        rank: idx + 1,
        ...r,
      }));
    }
  }

  res.json({
    disciplines,
    leaderboard,
  });
});

// GET /api/leaderboard/discipline/:id - Leaderboard for a specific discipline
router.get('/discipline/:id', (req, res) => {
  const db = getDb();
  const disc = db.prepare('SELECT * FROM disciplines WHERE id = ?').get(req.params.id);
  if (!disc) return res.status(404).json({ error: 'Discipline not found' });

  const gender = req.query.gender;
  const genders = gender ? [gender] : ['m', 'f'];
  const limit = parseInt(req.query.limit) || 20;
  const order = disc.higher_is_better ? 'DESC' : 'ASC';

  const records = {};

  for (const g of genders) {
    records[g] = db.prepare(`
      SELECT
        fr.id,
        fr.value,
        fr.date,
        fr.school_year,
        fr.attempt_number,
        s.id as student_id,
        s.first_name,
        s.last_name,
        s.gender,
        c.name as class_name,
        c.profession
      FROM fitness_results fr
      JOIN students s ON s.id = fr.student_id
      JOIN classes c ON c.id = s.class_id
      WHERE fr.discipline_id = ?
        AND s.gender = ?
        AND fr.is_best = 1
      ORDER BY fr.value ${order}
      LIMIT ?
    `).all(disc.id, g, limit).map((r, idx) => ({
      rank: idx + 1,
      ...r,
    }));
  }

  res.json({
    discipline: disc,
    records,
  });
});

module.exports = router;
