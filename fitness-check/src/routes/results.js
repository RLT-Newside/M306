const express = require('express');
const { getDb } = require('../database');

const router = express.Router();

// POST /api/results - Create a new result
router.post('/', (req, res) => {
  const db = getDb();
  const { student_id, discipline_id, value, date, school_year } = req.body;

  if (!student_id || !discipline_id || value === undefined || !date || !school_year) {
    return res.status(400).json({ error: 'Missing required fields' });
  }

  const discipline = db.prepare('SELECT * FROM disciplines WHERE id = ?').get(discipline_id);
  if (!discipline) return res.status(404).json({ error: 'Discipline not found' });

  // Determine next attempt number
  const lastAttempt = db.prepare(`
    SELECT MAX(attempt_number) as max_attempt FROM fitness_results
    WHERE student_id = ? AND discipline_id = ? AND school_year = ?
  `).get(student_id, discipline_id, school_year);

  const attemptNumber = (lastAttempt.max_attempt || 0) + 1;

  // Insert the result
  const result = db.prepare(`
    INSERT INTO fitness_results (student_id, discipline_id, attempt_number, value, is_best, date, school_year)
    VALUES (?, ?, ?, ?, 0, ?, ?)
  `).run(student_id, discipline_id, attemptNumber, value, date, school_year);

  // Recalculate best attempt
  recalculateBest(db, student_id, discipline_id, school_year, discipline.higher_is_better);

  const newResult = db.prepare('SELECT * FROM fitness_results WHERE id = ?').get(result.lastInsertRowid);
  res.status(201).json(newResult);
});

// PUT /api/results/:id - Update a result
router.put('/:id', (req, res) => {
  const db = getDb();
  const { value, date } = req.body;
  const resultId = req.params.id;

  const existing = db.prepare('SELECT * FROM fitness_results WHERE id = ?').get(resultId);
  if (!existing) return res.status(404).json({ error: 'Result not found' });

  const discipline = db.prepare('SELECT * FROM disciplines WHERE id = ?').get(existing.discipline_id);

  const updates = {};
  if (value !== undefined) updates.value = value;
  if (date !== undefined) updates.date = date;

  if (Object.keys(updates).length === 0) {
    return res.status(400).json({ error: 'No fields to update' });
  }

  const setClauses = Object.keys(updates).map(k => `${k} = ?`).join(', ');
  const values = Object.values(updates);

  db.prepare(`UPDATE fitness_results SET ${setClauses} WHERE id = ?`).run(...values, resultId);

  // Recalculate best
  recalculateBest(db, existing.student_id, existing.discipline_id, existing.school_year, discipline.higher_is_better);

  const updated = db.prepare('SELECT * FROM fitness_results WHERE id = ?').get(resultId);
  res.json(updated);
});

// DELETE /api/results/:id - Delete a result
router.delete('/:id', (req, res) => {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM fitness_results WHERE id = ?').get(req.params.id);
  if (!existing) return res.status(404).json({ error: 'Result not found' });

  const discipline = db.prepare('SELECT * FROM disciplines WHERE id = ?').get(existing.discipline_id);

  db.prepare('DELETE FROM fitness_results WHERE id = ?').run(req.params.id);

  // Renumber remaining attempts
  const remaining = db.prepare(`
    SELECT id FROM fitness_results
    WHERE student_id = ? AND discipline_id = ? AND school_year = ?
    ORDER BY attempt_number
  `).all(existing.student_id, existing.discipline_id, existing.school_year);

  const updateAttempt = db.prepare('UPDATE fitness_results SET attempt_number = ? WHERE id = ?');
  remaining.forEach((r, idx) => {
    updateAttempt.run(idx + 1, r.id);
  });

  // Recalculate best
  recalculateBest(db, existing.student_id, existing.discipline_id, existing.school_year, discipline.higher_is_better);

  res.json({ success: true });
});

function recalculateBest(db, studentId, disciplineId, schoolYear, higherIsBetter) {
  // Reset all
  db.prepare(`
    UPDATE fitness_results SET is_best = 0
    WHERE student_id = ? AND discipline_id = ? AND school_year = ?
  `).run(studentId, disciplineId, schoolYear);

  // Find best
  const order = higherIsBetter ? 'DESC' : 'ASC';
  const best = db.prepare(`
    SELECT id FROM fitness_results
    WHERE student_id = ? AND discipline_id = ? AND school_year = ?
    ORDER BY value ${order}
    LIMIT 1
  `).get(studentId, disciplineId, schoolYear);

  if (best) {
    db.prepare('UPDATE fitness_results SET is_best = 1 WHERE id = ?').run(best.id);
  }
}

module.exports = router;
