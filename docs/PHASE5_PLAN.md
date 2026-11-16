# Phase 5 Plan — Analytics & Adaptive Difficulty

## Scope
Performance analytics, weakness identification, recommendations, indicative
readiness estimation, and adaptive difficulty.

## Tasks
- Application queries: dashboard (accuracy, avg time, per category/difficulty,
  history), recommendations (recurring mistakes, weakest categories),
  readiness estimate (indicative only, profile-based).
- `AdaptiveDifficultyService`: accuracy + consecutive correct/incorrect +
  per-generator performance → next difficulty.
- Weakness Training mode auto-selects weakest category + adaptive difficulty.
- Web: analytics endpoints; frontend dashboard with progress + recommendations.

## Acceptance Criteria
1. All analytics computed server-side from session data.
2. Readiness is labelled indicative, never a pass guarantee.
3. Adaptive difficulty reacts to consecutive results and per-category stats.
4. Integration tests cover dashboard + readiness; unit tests cover adaptation.
