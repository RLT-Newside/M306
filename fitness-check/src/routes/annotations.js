const express = require('express');
const { getDb } = require('../database');

const router = express.Router();

// POST /api/annotations - Create or update an annotation
router.post('/', (req, res) => {
  const db = getDb();
  const { student_id, discipline_id, school_year, note } = req.body;

  if (!student_id || !discipline_id || !school_year || !note) {
    return res.status(400).json({ error: 'Missing required fields' });
  }

  const existing = db.prepare(`
    SELECT * FROM annotations
    WHERE student_id = ? AND discipline_id = ? AND school_year = ?
  `).get(student_id, discipline_id, school_year);

  if (existing) {
    db.prepare('UPDATE annotations SET note = ? WHERE id = ?').run(note, existing.id);
    const updated = db.prepare('SELECT * FROM annotations WHERE id = ?').get(existing.id);
    return res.json(updated);
  }

  const result = db.prepare(`
    INSERT INTO annotations (student_id, discipline_id, school_year, note)
    VALUES (?, ?, ?, ?)
  `).run(student_id, discipline_id, school_year, note);

  const newAnnotation = db.prepare('SELECT * FROM annotations WHERE id = ?').get(result.lastInsertRowid);
  res.status(201).json(newAnnotation);
});

// DELETE /api/annotations/:id - Delete an annotation
router.delete('/:id', (req, res) => {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM annotations WHERE id = ?').get(req.params.id);
  if (!existing) return res.status(404).json({ error: 'Annotation not found' });

  db.prepare('DELETE FROM annotations WHERE id = ?').run(req.params.id);
  res.json({ success: true });
});

module.exports = router;
