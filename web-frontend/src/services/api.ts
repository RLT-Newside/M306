import type {
  ClassInfo,
  ClassOverviewResponse,
  FitnessResult,
  Annotation,
  LeaderboardResponse,
} from '../types'

const API_BASE = '/api'

async function fetchJson<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(url, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  })
  if (!res.ok) {
    const error = await res.json().catch(() => ({ error: res.statusText }))
    throw new Error(error.error || res.statusText)
  }
  return res.json()
}

export const api = {
  // Classes
  getClasses(): Promise<ClassInfo[]> {
    return fetchJson(`${API_BASE}/classes`)
  },

  getClassOverview(classId: number): Promise<ClassOverviewResponse> {
    return fetchJson(`${API_BASE}/classes/${classId}`)
  },

  // Results
  createResult(data: {
    student_id: number
    discipline_id: number
    value: number
    date: string
    school_year: string
  }): Promise<FitnessResult> {
    return fetchJson(`${API_BASE}/results`, {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  updateResult(id: number, data: { value?: number; date?: string }): Promise<FitnessResult> {
    return fetchJson(`${API_BASE}/results/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  deleteResult(id: number): Promise<{ success: boolean }> {
    return fetchJson(`${API_BASE}/results/${id}`, {
      method: 'DELETE',
    })
  },

  // Annotations
  createAnnotation(data: {
    student_id: number
    discipline_id: number
    school_year: string
    note: string
  }): Promise<Annotation> {
    return fetchJson(`${API_BASE}/annotations`, {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  deleteAnnotation(id: number): Promise<{ success: boolean }> {
    return fetchJson(`${API_BASE}/annotations/${id}`, {
      method: 'DELETE',
    })
  },

  // Leaderboard
  getLeaderboard(params?: { gender?: string; discipline_id?: number }): Promise<LeaderboardResponse> {
    const query = new URLSearchParams()
    if (params?.gender) query.set('gender', params.gender)
    if (params?.discipline_id) query.set('discipline_id', String(params.discipline_id))
    const qs = query.toString()
    return fetchJson(`${API_BASE}/leaderboard${qs ? '?' + qs : ''}`)
  },
}
