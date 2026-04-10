const express = require('express');
const { getDb } = require('../database');
const { getPointsForValue, calculateGrade } = require('../scoring');

const router = express.Router();

// GET /api/students/:id - Get student details with all results
router.get('/:id', (req, res) => {
  const db = getDb();
  const student = db.prepare(`
    SELECT s.*, c.name as class_name, c.school_year, c.profession
    FROM students s
    JOIN classes c ON c.id = s.class_id
    WHERE s.id = ?
  `).get(req.params.id);

  if (!student) return res.status(404).json({ error: 'Student not found' });

  const disciplines = db.prepare('SELECT * FROM disciplines ORDER BY sort_order').all();

  const disciplineResults = {};
  for (const disc of disciplines) {
    const attempts = db.prepare(`
      SELECT * FROM fitness_results
      WHERE student_id = ? AND discipline_id = ?
      ORDER BY school_year DESC, attempt_number
    `).all(student.id, disc.id);

    const annotation = db.prepare(`
      SELECT * FROM annotations
      WHERE student_id = ? AND discipline_id = ? AND school_year = ?
    `).get(student.id, disc.id, student.school_year);

    const bestResult = attempts.find(a => a.is_best && a.school_year === student.school_year);
    const points = bestResult ? getPointsForValue(disc.id, student.gender, bestResult.value) : null;

    disciplineResults[disc.id] = {
      attempts,
      annotation,
      bestResult,
      points,
    };
  }

  res.json({
    student,
    disciplines,
    results: disciplineResults,
  });
});

// GET /api/disciplines - List all disciplines
router.get('/', (req, res) => {
  const db = getDb();
  const disciplines = db.prepare('SELECT * FROM disciplines ORDER BY sort_order').all();
  res.json(disciplines);
});

module.exports = router;
