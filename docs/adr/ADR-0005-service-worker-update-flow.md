# ADR-0005: Service worker update flow with user-confirmed activation

Status: Accepted · Date: 2026-11-17

## Context

The current `sw.js` calls `skipWaiting()` + `clients.claim()`: a deployed new
version takes over all open tabs immediately. Tabs that were loaded before the
deploy keep running old lazy chunks while `index.html` and the lazy-chunk
filenames have changed — navigating to a not-yet-loaded route after a deploy can
throw a chunk-load error. There is also no user-visible signal that an update
exists.

## Decision

1. Remove `skipWaiting()` from install; the new SW waits until activated.
2. The page listens for `waiting`/`updatefound` on the registration and shows a
   non-blocking toast: "Versi baru tersedia — Muat ulang" (Indonesian).
3. On user confirmation, the page posts `SKIP_WAITING` to the SW, which relays
   activation; the page reloads on `controllerchange`.
4. Chunk-load errors on lazy routes are caught globally and trigger one
   automatic reload (guarded against loops via a `sessionStorage` flag) — this
   heals stale tabs that missed the toast.
5. The API network-first strategy and app-shell precache are unchanged.

## Consequences

- No stale-chunk mismatch after deploys; users consciously accept updates.
- Updates apply at the next interaction instead of instantly — acceptable for a
  training app (no security-critical hotfix path; a forced variant can be added
  behind a flag if ever needed).
- Slightly more SW logic in `public/sw.js` (still hand-rolled, no workbox
  dependency added).
