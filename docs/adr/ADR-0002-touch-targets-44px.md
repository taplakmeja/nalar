# ADR-0002: 44 px minimum touch targets

Status: Accepted · Date: 2026-11-17

## Context

The default shadcn button is `h-9` (36 px) and several controls (bottom-nav items,
review-dialog grid buttons) render smaller. WCAG 2.2 AA only requires 24×24 CSS px
(SC 2.5.8), but Apple HIG specifies 44×44 pt and Material Design 48×48 dp as the
size at which fingertip mis-taps become rare. A training app is used fast and
repetitively — mis-taps on answer options are a correctness problem, not just a
comfort problem.

## Decision

All interactive targets (buttons, nav items, option rows, inputs, Kraepelin cells)
must have a hit area of at least **44×44 CSS px**, with ≥ 8 px spacing between
adjacent targets. Where the visual design needs a smaller element, the hit area is
extended with padding or a pseudo-element, never by scaling the visible glyph.

Implementation notes:

- Add a `mobile` (or adjust existing) button size variant with `h-11` (44 px) and
  use it for exam options and primary actions.
- Bottom-nav items get `min-h-11` and larger labels (`text-xs` → `text-sm`).
- The review-dialog grid uses ≥ 40 px cells with 8 px gaps (dense but compliant
  via spacing), or 44 px where space allows.

## Consequences

- Slightly taller layouts on answer lists — acceptable; 5 options × 44 px still
  fits comfortably in the exam viewport.
- WCAG 2.5.8 AA is exceeded, and 2.5.5 AAA (44 px) is met by default.
- A lint/test guard (target-size audit in component tests) prevents regression.
