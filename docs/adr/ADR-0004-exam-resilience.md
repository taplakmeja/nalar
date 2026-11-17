# ADR-0004: Exam resilience — server-authoritative with client recovery

Status: Accepted · Date: 2026-11-17

## Context

Scoring and answer validation are server-side only (engineering principle, never
trust the client). Today the exam runner (`SessionRunnerPage.tsx`) keeps progress
in memory: `totalQuestions`/`deadlineUtc` arrive via `location.state`, marks and
answered-ids live in `useState`. A refresh — trivially triggered in a standalone
PWA — loses everything and shows "Sesi tidak ditemukan". A failed answer
submission is silently swallowed (`try/finally` with no `catch`). Mobile networks
in the target market are lossy; some interruption during a 20-minute session is
the norm, not the exception.

Full offline exam-taking was considered (queue answers in IndexedDB, replay
later) and rejected: server-authoritative scoring means an answer that never
reached the server has no score, and replaying stale answers against server-side
session state creates conflict semantics the backend does not model.

## Decision

**Server-authoritative with client-side recovery.** The client makes the session
survivable, the server remains the only source of truth:

1. **Persist session metadata** (`sessionId`, `totalQuestions`, `deadlineUtc`,
   answered ids, marks, current order) in `sessionStorage` under a
   `psikotes.session.<id>` key, written on every state change. On mount, if
   `location.state` is absent, restore from `sessionStorage`; the server remains
   the authority on what is already answered (`GetSessionQuestion` reflects it).
2. **Visible failure + bounded retry** for `submitAnswer`: on failure show an
   inline Indonesian error ("Jawaban gagal terkirim — mencoba lagi…") and retry
   with exponential backoff (max 3 attempts). Only after final failure keep the
   selection and let the user retry manually. Never clear the selection on error.
3. **Guarded exits**: `beforeunload` while a timed session is active and unfinished
   (browser prompts); in-app back navigation shows a confirmation dialog. Auto-
   submit on deadline is unchanged.
4. **Deadline from server, not client**: prefer the deadline returned by the
   API over `location.state`; the wall clock of the server session is the truth.

## Consequences

- Refresh, tab restore, and network blips no longer destroy progress or lie about
  submission.
- No offline answer queue, no conflict resolution, no changes to backend scoring
  trust model.
- `sessionStorage` is per-tab and cleared when the exam ends — no stale PII.
- iOS ITP can evict storage after 7 days of inactivity — irrelevant for an active
  session, acceptable.
