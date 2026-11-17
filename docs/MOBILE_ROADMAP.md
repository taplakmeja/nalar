# Mobile Experience — Implementation Roadmap

Status: Approved plan · Last updated: 2026-11-17

Implements the requirements in [MOBILE_EXPERIENCE.md](MOBILE_EXPERIENCE.md).
Decisions are recorded in [docs/adr/](adr/README.md). Phases are ordered by
severity of the gaps they close; each phase ships independently and must not
regress the 139 backend + 15 frontend tests, lint, or build.

---

## M1 — Foundation: touch & safe areas (closes G1, G2, G13, G14)

**Tasks**

1. Add safe-area utilities (Tailwind plugin or CSS vars): header top inset, bottom
   nav bottom inset with matching background, content clear-calc, landscape
   left/right insets — per [ADR-0003](adr/ADR-0003-safe-area-strategy.md).
2. Button size audit: introduce/adjust `h-11` variant; update exam option rows,
   primary actions, bottom-nav items (`min-h-11`, `text-sm` labels), review-dialog
   grid cells — per [ADR-0002](adr/ADR-0002-touch-targets-44px.md).
3. Global touch polish: `touch-action: manipulation` on interactive containers,
   tap-highlight policy, bump `text-xs` labels to `text-sm` in nav.
4. iOS PWA metadata: `apple-mobile-web-app-status-bar-style`,
   `display_override: ["standalone"]` in manifest.

**Acceptance criteria**

- [ ] Automated component test asserts primary controls render ≥ 44 px hit area.
- [ ] No horizontal overflow at 320 px on all 9 routes (Playwright or manual matrix
      recorded in the phase report).
- [ ] iOS standalone (device emulation w/ safe areas + real device if available):
      no content under notch/home indicator; no white gap below bottom nav.
- [ ] All existing tests, lint, build green.

## M2 — Exam resilience (closes G3, G4, G11; part of NFR-M4)

**Tasks**

1. Session-metadata + progress persistence in `sessionStorage`, restore-on-mount
   fallback — per [ADR-0004](adr/ADR-0004-exam-resilience.md).
2. `submitAnswer` failure UX: inline error, bounded exponential retry, selection
   never cleared on error.
3. Exit guards: `beforeunload` during timed sessions, confirmation dialog on
   in-app back navigation.
4. Deadline reconciliation with server-provided deadline.

**Acceptance criteria**

- [ ] Refresh mid-session restores question position, marks, and answered state.
- [ ] Simulated network failure on submit shows the retry UI and recovers without
      data loss (component test with mocked fetch).
- [ ] Closing/backing out mid-exam prompts the user; deadline auto-submit still
      fires.
- [ ] All existing tests, lint, build green.

## M3 — Kraepelin grid & forms (closes G5, G8; FR-M6, FR-M9)

**Tasks**

1. Extend the session API (backend) to serve Kraepelin grid rows from the existing
   seeded generator; server-side validation of submitted cell values.
2. Dedicated Kraepelin runner view (lazy chunk) per
   [ADR-0006](adr/ADR-0006-kraepelin-mobile-grid.md): auto-advance grid, single
   numeric input, per-column countdown, ≥ 44 px cells.
3. Mobile-tune all inputs: `inputMode`/`enterKeyHint` audit; replace
   `type="number"` on question count with `inputMode="numeric"` steppers.

**Acceptance criteria**

- [ ] A Kraepelin session on a 360 px viewport: continuous entry at ≥ 2 cells/sec
      with no double-tap-zoom interference.
- [ ] Cell values validated server-side; client cannot fabricate results.
- [ ] Engine + integration tests cover grid generation and submission; all green.

## M4 — PWA lifecycle, install, loading & CI budgets (closes G6, G7, G9, G10, G12)

**Tasks**

1. SW update flow per [ADR-0005](adr/ADR-0005-service-worker-update-flow.md):
   waiting-worker toast, user-confirmed activation, chunk-error self-heal.
2. Install UX: Android `beforeinstallprompt` handler; iOS Add-to-Home-Screen
   instruction sheet (Safari, not installed, Indonesian).
3. Skeleton components for dashboard/runner/result loading states (CLS-safe).
4. Admin pages responsive pass at 320–430 px (stacked layouts, no tables overflow).
5. CI: Lighthouse mobile job per [ADR-0007](adr/ADR-0007-mobile-performance-budgets.md)
   + post-build chunk-size budget script.

**Acceptance criteria**

- [ ] Deploy simulation: stale tab shows the update toast; activation reloads
      cleanly; chunk-error tab self-heals once.
- [ ] Android install prompt fires; iOS instruction sheet shows on eligible
      Safari only.
- [ ] Lighthouse mobile ≥ 90 perf & a11y on dashboard + exam runner; budgets pass
      in CI.
- [ ] No layout overflow on admin pages at 320 px.

---

## Coverage matrix

| Gap | Phase | Requirement | ADR |
|---|---|---|---|
| G1 safe areas | M1 | FR-M2 | 0003 |
| G2 touch targets | M1 | FR-M1 | 0002 |
| G3 refresh state loss | M2 | FR-M3 | 0004 |
| G4 silent submit failure | M2 | FR-M4 | 0004 |
| G5 Kraepelin UI | M3 | FR-M6 | 0006 |
| G6 SW update UX | M4 | FR-M7 | 0005 |
| G7 install guidance | M4 | FR-M8 | — |
| G8 numeric inputs | M3 | FR-M9 | — |
| G9 skeletons | M4 | FR-M10 | — |
| G10 admin responsiveness | M4 | — | — |
| G11 exit guards | M2 | FR-M5 | 0004 |
| G12 perf CI | M4 | NFR-M1, NFR-M6 | 0007 |
| G13 touch polish | M1 | NFR-M3 | 0002 |
| G14 iOS metadata | M1 | — | 0003 |

## Out of scope (v1)

- Push notifications (requires VAPID backend + iOS 16.4+ standalone; separate ADR
  if pursued).
- Tablet 2-pane layouts, offline answer queues, background sync.
- RUM/analytics-based budgets (lab budgets only until real field data exists).
