# ADR-0007: Mobile performance budgets enforced in CI

Status: Accepted · Date: 2026-11-17

## Context

Route-level code splitting brought the main chunk to 319 kB (102 kB gzip), but
nothing prevents regression, and no mobile-profile measurements exist. Target
users are on mid-range Android devices with throttled connections; the exam
runner is timing-sensitive, so INP and long tasks matter directly.

## Decision

Enforce budgets in CI (Lighthouse mobile profile, Moto G-class emulation,
throttled 4G) on two routes — dashboard and exam runner:

| Metric | Budget |
|---|---|
| LCP | ≤ 2.5 s |
| INP (TBT proxy in lab runs) | ≤ 200 ms INP / ≤ 200 ms TBT |
| CLS | ≤ 0.1 |
| Main JS chunk | ≤ 350 kB raw / 120 kB gzip (vite build output check) |
| Lighthouse performance & accessibility | ≥ 90 each |

Implementation: a CI job runs `npm run build` + Lighthouse CI against the
production build served locally; the chunk-size check runs as a script after
`vite build` (fails the build when exceeded). Budgets are versioned in this ADR;
changes require a new ADR.

## Consequences

- Regressions are caught before merge, not by users.
- Lighthouse lab numbers are proxies for field data (no RUM yet); thresholds are
  set with headroom for lab variance.
- Adding a heavy dependency now visibly costs CI time, keeping the dependency
  discipline honest.
