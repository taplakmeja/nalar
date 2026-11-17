# ADR-0003: Safe-area strategy for iOS standalone PWA

Status: Accepted · Date: 2026-11-17

## Context

`viewport-fit=cover` is already set (`frontend/index.html`), which unlocks
`env(safe-area-inset-*)` but also lets fixed elements extend under the notch,
Dynamic Island, and home indicator. On iOS standalone PWAs, `dvh`/`svh` resolve
inside the safe area, so a bottom-fixed nav padded with `env(safe-area-inset-bottom)`
can leave a white gap below it, and unpadded fixed elements sit under system
gestures. Known iOS-specific behavior (WebKit home-indicator zone ≈ 34 px).

## Decision

1. Keep `viewport-fit=cover`.
2. Apply insets **inside fixed elements, never on `body`**:
   - header: `padding-top: env(safe-area-inset-top)` — background extends behind
     the status bar.
   - bottom nav: `padding-bottom: env(safe-area-inset-bottom)` with the nav's own
     background color, so the background color fills the home-indicator zone
     (match colors instead of fighting the constraint).
   - main content: `padding-bottom: calc(nav-height + env(safe-area-inset-bottom))`
     so content always clears the nav.
3. Use `min-h-svh` (already in place) for the shell; do not attempt to make any
   unit equal the physical screen height in standalone mode.
4. In landscape, add `env(safe-area-inset-left/right)` padding to the content
   container so text clears the notch/Dynamic Island.

## Consequences

- No white gaps, no obstructed controls, on notched and Dynamic Island devices.
- Desktop browsers resolve `env()` to 0 px — the same CSS is safe everywhere.
- Requires a visual QA pass on a real iOS device (or DevTools device emulation
  with safe-area simulation) before each release; recorded in the roadmap.
