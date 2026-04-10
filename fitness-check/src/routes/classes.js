const express = require('express');
const { getDb } = require('../database');
const { getPointsForValue, calculateGrade } = require('../scoring');

const router = express.Router();

// GET /api/classes - List all classes
router.get('/', (req, res) => {
  const db = getDb();
  const classes = db.prepare(`
    SELECT c.*, COUNT(s.id) as student_count
    FROM classes c
    LEFT JOIN students s ON s.class_id = c.id
    GROUP BY c.id
    ORDER BY c.school_year DESC, c.name
  `).all();
  res.json(classes);
});

// GET /api/classes/:id - Get class overview with all results
router.get('/:id', (req, res) => {
  const db = getDb();
  const classInfo = db.prepare('SELECT * FROM classes WHERE id = ?').get(req.params.id);
  if (!classInfo) return res.status(404).json({ error: 'Class not found' });

  const disciplines = db.prepare('SELECT * FROM disciplines ORDER BY sort_order').all();

  const students = db.prepare(`
    SELECT s.* FROM students s
    WHERE s.class_id = ?
    ORDER BY s.last_name, s.first_name
  `).all(req.params.id);

  const results = [];
  const classAverages = {};

  for (const disc of disciplines) {
    classAverages[disc.id] = { totalValue: 0, totalPoints: 0, count: 0 };
  }

  for (const student of students) {
    const studentResult = {
      ...student,
      disciplines: {},
      totalPoints: 0,
      disciplineCount: 0,
      avgPoints: 0,
      grade: null,
    };

    for (const disc of disciplines) {
      // Get best result for this discipline
      const bestResult = db.prepare(`
        SELECT * FROM fitness_results
        WHERE student_id = ? AND discipline_id = ? AND school_year = ? AND is_best = 1
        ORDER BY value ${disc.higher_is_better ? 'DESC' : 'ASC'}
        LIMIT 1
      `).get(student.id, disc.id, classInfo.school_year);

      // Get all attempts
      const attempts = db.prepare(`
        SELECT * FROM fitness_results
        WHERE student_id = ? AND discipline_id = ? AND school_year = ?
        ORDER BY attempt_number
      `).all(student.id, disc.id, classInfo.school_year);

      // Get annotation if exists
      const annotation = db.prepare(`
        SELECT * FROM annotations
        WHERE student_id = ? AND discipline_id = ? AND school_year = ?
      `).get(student.id, disc.id, classInfo.school_year);

      const points = bestResult ? getPointsForValue(disc.id, student.gender, bestResult.value) : null;

      studentResult.disciplines[disc.id] = {
        bestResult,
        attempts,
        annotation,
        points,
        missing: !bestResult && !annotation,
      };

      if (points !== null) {
        studentResult.totalPoints += points;
        studentResult.disciplineCount++;
        classAverages[disc.id].totalValue += bestResult.value;
        classAverages[disc.id].totalPoints += points;
        classAverages[disc.id].count++;
      }
    }

    if (studentResult.disciplineCount > 0) {
      studentResult.avgPoints = Math.round((studentResult.totalPoints / studentResult.disciplineCount) * 100) / 100;
      studentResult.grade = calculateGrade(studentResult.avgPoints);
    }

    results.push(studentResult);
  }

  // Calculate class averages
  const classAvg = {};
  for (const disc of disciplines) {
    const avg = classAverages[disc.id];
    classAvg[disc.id] = {
      avgValue: avg.count > 0 ? Math.round((avg.totalValue / avg.count) * 100) / 100 : null,
      avgPoints: avg.count > 0 ? Math.round((avg.totalPoints / avg.count) * 100) / 100 : null,
      count: avg.count,
    };
  }

  res.json({
    classInfo,
    disciplines,
    students: results,
    classAverages: classAvg,
  });
});

module.exports = router;
