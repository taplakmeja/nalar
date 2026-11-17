# ADR-0006: Dedicated Kraepelin grid UI on mobile

Status: Accepted · Date: 2026-11-17

## Context

Kraepelin/Pauli is a continuous arithmetic test: rows of single digits, the user
writes row sums left-to-right under time pressure, one column per ~minute. The
current runner renders every session as MCQ, so selecting "Kraepelin" produces a
question shape that does not match the exercise (known limitation since Phase 8).
On mobile this is worse: the exercise needs fast numeric entry — dozens of inputs
per minute — which the MCQ flow cannot express.

## Decision

Build a dedicated Kraepelin runner view, separate from the MCQ flow:

1. **Grid**: server-generated digit rows rendered as a horizontally scrollable
   grid; the active cell is auto-advanced after each entry (no Enter key needed).
2. **Input**: a single always-focused input with `inputMode="numeric"` and
   `enterKeyHint="next"` driving the cell cursor — one soft keyboard for the whole
   session, no per-cell focus churn.
3. **Touch**: cells ≥ 44×44 px; `touch-action: manipulation` on the grid to
   eliminate double-tap-zoom delay; rapid consecutive entry is the core loop.
4. **Timing**: per-column countdown from the server-side session deadline;
   auto-submit on expiry, consistent with the MCQ runner.
5. **Scoring**: unchanged — the backend validates submitted cell values
   server-side (existing Kraepelin session model); the client only captures
   entries.
6. **Regeneration**: rows are produced by the existing Kraepelin generator
   (seeded, deterministic); the client requests rows via the session API — no
   client-side question generation.

## Consequences

- One additional route-level chunk (~small); MCQ runner untouched.
- Kraepelin becomes the most touch-intensive flow — it sets the floor for
  [touch targets](ADR-0002-touch-targets-44px.md) and `touch-action` policy.
- Backend may need a small extension to expose grid rows for the session (tracked
  in the roadmap phase; the generator already produces the data).
