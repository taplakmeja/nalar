# Phase 5 Report — Analytics & Adaptive Difficulty

**Status:** completed 2025-11-16

## Scope delivered

### Analytics (`backend/src/Application/UseCases/Analytics`)
- `GetDashboardQuery` — all metrics computed server-side from session data:
  - Sessions completed, total answered, overall accuracy, average answer time
    (per-session duration / answered count)
  - Performance per category (accuracy + trailing consecutive correct/incorrect)
  - Performance per difficulty level
  - Recent session history (last 10)
  - Recommendations: weakest category (< 60% accuracy, ≥ 5 attempts), 3-in-a-row
    wrong (suggest lowering difficulty), strongest category (≥ 85%, suggest raising)
  - Readiness estimate: overall accuracy → Siap (≥ 80) / Hampir Siap (≥ 60) /
    Belum Siap; below 10 answered questions → "Data Belum Cukup". Always labelled
    **indicative, never a pass guarantee** (server + UI disclaimer).
- `ITrainingStatsService` — per-category and per-generator stats with trailing
  consecutive-run computation, shared by dashboard and session start.
- Web: `AnalyticsController` → `GET /api/analytics/dashboard` `[Authorize]`.

### Adaptive difficulty
- `IAdaptiveDifficultyService` / `AdaptiveDifficultyService`:
  - 3 consecutive incorrect → difficulty −1
  - 4 consecutive correct → +1
  - 3 consecutive correct with ≥ 85% accuracy → +1
  - ≥ 5 answered with < 50% accuracy → −1
  - clamped to 0–4
- `StartSessionCommand.UseAdaptiveDifficulty` flag (UI toggle) and
  WeaknessTraining always adaptive; per-category stats drive the recommendation.
- Weakness Training with `Mixed` scope auto-selects the user's weakest category
  (lowest accuracy with ≥ 3 attempts; fallback least-attempted; no data → Mixed).

### Frontend
- `DashboardPage` rewritten as a live learning dashboard: stat cards, readiness
  card with indicative disclaimer, per-category accuracy bars (color-coded),
  per-difficulty table, recommendations, recent-session history linking to
  result pages.
- `ModeSelectPage`: adaptive-difficulty switch.

## Acceptance criteria
- [x] All analytics computed server-side from session data
- [x] Readiness labelled indicative with explicit disclaimer (API + UI)
- [x] Adaptive difficulty reacts to consecutive results and per-category stats
- [x] Unit tests cover all adaptation rules; integration tests cover dashboard
      (empty + after activity), weakness auto-selection, and auth

## Verification
- `dotnet test` — 109 tests green (44 Application, 49 Engine, 16 Integration)
- `npm run build` + `npm test` + `npm run lint` — green
